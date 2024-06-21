using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Karting.WebcamFeed
{
    public class KartingWebcamFeedController : MonoBehaviour
    {
        // For camera streaming
        private WebCamTexture webcamTexture;
        private Color32[] frame;
        public RawImage rawImage;
        // rawimage to display the preprocessed image
        public RawImage preprocessedImage;
        private bool nextFrameReady = true;
        // get socket from SocketClient
        private GlobalAssets.Socket.SocketUDP socketClient;

        // fps text
        public TMP_Text fpsText;
        private ProjectController projectController;


        public int pythonPredictedClass { get; private set; } = -1;
        public string UnityPredictedClass { get; private set; } = "";
        void Start()
        {
            // get socket from SocketClient
            socketClient = GlobalAssets.Socket.SocketUDP.Instance;
            projectController = ProjectController.Instance;
            // Start the webcam
            webcamTexture = new WebCamTexture
            {
                // reduce the resolution of the webcam
                requestedWidth = 320,
                requestedHeight = 180
            };
            // Start Camera from Unity
            rawImage.texture = webcamTexture;
            rawImage.material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
        // Update is called once per frame
        void Update()
        {
            // ----------- Send the frame from Unity to Python server -----------
            SendFrameFromUnityCamera();
            // --------- Receive the response from the server ---------
            if (socketClient.isDataAvailable())
            {
                Dictionary<string, string> response = socketClient.ReceiveDictMessage();
                // Debug.Log("----------------------");
                // Debug.Log("Received:");
                // foreach (KeyValuePair<string, string> kvp in response)
                // {
                //     string v = kvp.Value ?? "null";
                //     Debug.Log(kvp.Key + ": " + v);
                // }
                // Debug.Log("----------------------");
                SetFPSText(response["FPS"]);
                if (response["event"] == "predict_hand_pose")
                {
                    pythonPredictedClass = int.Parse(response["prediction"]);
                    UnityPredictedClass = PythonToUnityClassName(response["prediction"]);
                    // Debug.Log("Python Prediction: " + pythonPredictedClass);
                    // Debug.Log("Unity Prediction: " + UnityPredictedClass);
                }
                else if (response["event"] == "preprocess_hand_pose")
                {
                    string image = response["preprocessed_image"];
                    byte[] imageBytes = Convert.FromBase64String(image);
                    Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
                    texture.LoadImage(imageBytes);
                    if (preprocessedImage != null)
                    {
                        preprocessedImage.texture = texture;
                        preprocessedImage.material.mainTexture = texture;
                    }
                }

                nextFrameReady = true;
            }
        }
        string PythonToUnityClassName(string message)
        {
            if (projectController.PythonClassesToUnityClassesMap.ContainsKey(message))
            {
                return projectController.PythonClassesToUnityClassesMap[message];
            }
            return message;
        }
        void SetFPSText(string fps)
        {
            if (fpsText != null)
            {
                fpsText.text = "FPS: " + fps;
            }
        }

        void SendFrameFromUnityCamera()
        {
            if (nextFrameReady)
            {
                if (webcamTexture.isPlaying)
                {
                    // Send the frame to the server
                    // Debug.Log("Width: " + webcamTexture.width + " Height: " + webcamTexture.height);
                    frame = webcamTexture.GetPixels32();
                    byte[] frameBytes = Color32ArrayToByteArrayWithoutAlpha(frame);
                    // Encode the byte array to a Base64 string
                    string frameBase64 = Convert.ToBase64String(frameBytes);
                    // Construct dictionary to send to server
                    Dictionary<string, string> message = new Dictionary<string, string>
                    {
                        { "frame", frameBase64 },
                        { "width", webcamTexture.width.ToString() },
                        { "height", webcamTexture.height.ToString() },
                        // { "event", "predict_frame" }
                        { "event", "predict_hand_pose" }
                    };
                    socketClient.SendMessage(message);
                    nextFrameReady = false;
                }
            }
        }

        void OnDestroy()
        {
            // Stop the webcam
            if (webcamTexture != null)
            {
                webcamTexture.Stop();
            }
        }
        private byte[] Color32ArrayToByteArrayWithoutAlpha(Color32[] colors)
        {
            if (colors == null || colors.Length == 0)
                return null;
            byte[] bytes = new byte[colors.Length * 3];
            for (int i = 0; i < colors.Length; i++)
            {
                bytes[i * 3] = colors[i].r;
                bytes[i * 3 + 1] = colors[i].g;
                bytes[i * 3 + 2] = colors[i].b;
            }
            return bytes;
        }
    }
}
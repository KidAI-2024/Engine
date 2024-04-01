using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace GlobalAssets.WebcamFeed
{
    public class WebcamFeedController : MonoBehaviour
    {
        // For camera streaming
        private WebCamTexture webcamTexture;
        private Color32[] frame;
        public RawImage rawImage;
        // rawimage to display the preprocessed image
        public RawImage preprocessedImage;
        private bool nextFrameReady = true;
        // get socket from SocketClient
        private Socket.SocketUDP socketClient;
        // fps text
        public TMP_Text fpsText;
        void Start()
        {
            // get socket from SocketClient
            socketClient = Socket.SocketUDP.Instance;
            // Start the webcam
            webcamTexture = new WebCamTexture
            {
                // reduce the resolution of the webcam
                requestedWidth = 320,
                requestedHeight = 180
            };
            // rawImage.texture = webcamTexture;
            // rawImage.material.mainTexture = webcamTexture;
            // // set height and width of rawImage
            // // rawImage.rectTransform.sizeDelta = new Vector2(webcamTexture.width, webcamTexture.height);
            // webcamTexture.Play();
            SendEvent_StartFeedHandPose();
        }
        // Update is called once per frame
        void Update()
        {
            // ----------- Send the frame to the server -----------
            // if (Input.GetKeyDown(KeyCode.Space))
            // if (nextFrameReady)
            // {
            //     if (webcamTexture.isPlaying)
            //     {
            //         // Send the frame to the server
            //         // Debug.Log("Width: " + webcamTexture.width + " Height: " + webcamTexture.height);
            //         frame = webcamTexture.GetPixels32();
            //         byte[] frameBytes = Color32ArrayToByteArrayWithoutAlpha(frame);
            //         // Encode the byte array to a Base64 string
            //         string frameBase64 = Convert.ToBase64String(frameBytes);
            //         // Construct dictionary to send to server
            //         Dictionary<string, string> message = new Dictionary<string, string>
            //         {
            //             { "frame", frameBase64 },
            //             { "width", webcamTexture.width.ToString() },
            //             { "height", webcamTexture.height.ToString() },
            //             // { "event", "predict_frame" }
            //             { "event", "preprocess_hand_pose" }
            //         };
            //         socketClient.SendMessage(message);
            //         nextFrameReady = false;
            //     }
            // }
            // get feed request
            if (nextFrameReady)
            {

                SendEvent_GetFeedFrameHandPose();
                nextFrameReady = false;
            }
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
                if (fpsText != null)
                {
                    fpsText.text = "FPS: " + response["FPS"];
                }
                if (response["event"] == "predict_frame")
                {
                    Debug.Log("Prediction: " + response["prediction"]);
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
                else if (response["event"] == "get_feed_frame_handpose")
                {
                    if (response["frame"] != null)
                    {
                        string image = response["frame"];
                        byte[] imageBytes = Convert.FromBase64String(image);
                        Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
                        texture.LoadImage(imageBytes);
                        if (preprocessedImage != null)
                        {
                            preprocessedImage.texture = texture;
                            preprocessedImage.material.mainTexture = texture;
                        }
                    }
                }
                else if (response["event"] == "start_feed_hand_pose")
                {
                    Debug.Log("Response of: " + "start_feed_hand_pose");
                    Debug.Log("Result: " + response["message"]);
                }
                else if (response["event"] == "stop_feed_hand_pose")
                {
                    Debug.Log("Response of: " + "stop_feed_hand_pose");
                    Debug.Log("Result: " + response["message"]);
                }
                nextFrameReady = true;
            }
        }
        void SendEvent_StartFeedHandPose()
        {
            Dictionary<string, string> message = new Dictionary<string, string>
                {
                    { "event", "start_feed_hand_pose" }
                };
            socketClient.SendMessage(message);
        }
        void SendEvent_GetFeedFrameHandPose()
        {
            Dictionary<string, string> message = new Dictionary<string, string>
                {
                    { "event", "get_feed_frame_handpose" }
                };
            socketClient.SendMessage(message);
        }
        void SendEvent_StopFeedHandPose()
        {
            Dictionary<string, string> message = new Dictionary<string, string>
                {
                    { "event", "stop_feed_hand_pose" }
                };
            socketClient.SendMessage(message);
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
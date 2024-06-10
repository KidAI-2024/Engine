using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlobalAssets.UI
{
    public class CameraPreprocessing : MonoBehaviour
    {
        private WebCamTexture webcamTexture;
        private Color[] UnProcessedImage;
        public RawImage rawImage;
        // For camera stream preprocessing
        private bool nextFrameReady = true;
        private Color32[] frame;

        private Socket.SocketUDP socketClient;
        private bool isRunning = true;
        // Start is called before the first frame update
        void Start()
        {
            socketClient = Socket.SocketUDP.Instance;
            webcamTexture = new WebCamTexture
            {
                requestedWidth = 320,
                requestedHeight = 180
            };
            rawImage.texture = webcamTexture;
            rawImage.material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
        // Update is called once per frame
        void Update()
        {
            PreprocessFrames();
        }

        void PreprocessFrames()
        {
            SendFrameFromUnityCamera();
            CheckForServerResponse();
        }
        void CheckForServerResponse()
        {
            try
            {
                if (socketClient.isDataAvailable())
                {
                    Dictionary<string, string> response = socketClient.ReceiveDictMessage();
                    if (response["event"] == "preprocess_body_pose")
                    {
                        string image = response["preprocessed_image"];
                        byte[] imageBytes = Convert.FromBase64String(image);
                        Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
                        texture.LoadImage(imageBytes);
                        if (rawImage != null)
                        {
                            rawImage.texture = texture;
                            rawImage.material.mainTexture = texture;
                        }
                    }
                    nextFrameReady = true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while checking server response: " + ex.Message);
                nextFrameReady = true;
            }
        }
        void SendFrameFromUnityCamera()
        {
            if (nextFrameReady)
            {
                if (webcamTexture.isPlaying)
                {
                    UnProcessedImage = webcamTexture.GetPixels();
                    // Send the frame to the server
                    frame = webcamTexture.GetPixels32();
                    byte[] frameBytes = Color32ArrayToByteArrayWithoutAlpha(frame);
                    // Encode the byte array to a Base64 string
                    string frameBase64 = Convert.ToBase64String(frameBytes);
                    // Construct dictionary to send to server
                    Dictionary<string, string> message = new Dictionary<string, string>
                    {
                        { "frame", frameBase64 },
                        { "width", "320" },
                        { "height", "180" },
                        // { "event", "predict_frame" }
                        { "event", "preprocess_body_pose" }
                    };
                    socketClient.SendMessage(message);
                    nextFrameReady = false;
                }
            }
        }

        public Color[] GetCameraFrame()
        {
            return UnProcessedImage;
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

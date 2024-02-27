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
        private bool nextFrameReady = true;
        // get socket from SocketClient
        private Socket.SocketUDP socketClient;
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
            rawImage.texture = webcamTexture;
            rawImage.material.mainTexture = webcamTexture;
            // set height and width of rawImage
            // rawImage.rectTransform.sizeDelta = new Vector2(webcamTexture.width, webcamTexture.height);
            webcamTexture.Play();
        }
        // Update is called once per frame
        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.Space))
            if (nextFrameReady)
            {
                if (webcamTexture.isPlaying)
                {
                    // Send the frame to the server
                    Debug.Log("Width: " + webcamTexture.width + " Height: " + webcamTexture.height);
                    frame = webcamTexture.GetPixels32();
                    byte[] frameBytes = Color32ArrayToByteArrayWithoutAlpha(frame);

                    socketClient.SendMessage(frameBytes);
                    // SendFrame(frame);
                    nextFrameReady = false;
                }
            }
            // Receive the response from the server
            if (socketClient.isDataAvailable())
            {
                byte[] data = socketClient.ReceiveData();
                string message = System.Text.Encoding.UTF8.GetString(data);
                Debug.Log("Received: " + message);
                nextFrameReady = true;
            }
        }

        void OnDestroy()
        {
            // Stop the webcam
            webcamTexture.Stop();
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
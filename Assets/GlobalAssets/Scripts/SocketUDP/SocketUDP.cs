using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.UI;
namespace GlobalAssets.SocketUDP
{

    public class SocketUDP : MonoBehaviour
    {
        // For camera streaming
        private WebCamTexture webcamTexture;
        private Color32[] frame;
        public RawImage rawImage;

        private UdpClient udp;
        private IPEndPoint remoteEP;
        private bool nextFrameReady = true;
        private String host = "localhost";
        private int port = 5065;

        void Start()
        {
            udp = new UdpClient();
            remoteEP = new IPEndPoint(IPAddress.Any, 0);
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
                    SendFrame(frame);
                    nextFrameReady = false;
                }
            }
            // Receive the response from the server
            if (udp.Available > 0)
            {
                byte[] data = ReceiveData();
                string message = System.Text.Encoding.UTF8.GetString(data);
                Debug.Log("Received: " + message);
                nextFrameReady = true;
            }
        }
        private void SendFrame(Color32[] frame)
        {
            byte[] frameBytes = Color32ArrayToByteArrayWithoutAlpha(frame);
            int chunkSize = 60000; // Size of each chunk in bytes
            for (int i = 0; i < frameBytes.Length; i += chunkSize)
            {
                byte[] chunk = new byte[Mathf.Min(chunkSize, frameBytes.Length - i)];
                Array.Copy(frameBytes, i, chunk, 0, chunk.Length);
                udp.Send(chunk, chunk.Length, host, port);
            }
        }
        private byte[] ReceiveData()
        {
            byte[] data = udp.Receive(ref remoteEP);
            return data;
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
        void OnDestroy()
        {
            webcamTexture.Stop();
        }
        void OnApplicationQuit()
        {
            udp.Close();
        }
    }
}
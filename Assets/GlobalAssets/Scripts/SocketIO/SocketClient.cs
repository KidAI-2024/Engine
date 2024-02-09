using SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient.Newtonsoft.Json;
using System.Runtime.InteropServices;
using UnityEngine.UI;

namespace GlobalAssets.SocketIO
{
    public class SocketClient : MonoBehaviour
    {
        public RawImage rawImage;
        public static SocketClient Instance { get; private set; }

        private SocketIOUnity socket;
        private WebCamTexture webcamTexture;
        private Color32[] frame;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            var uri = new Uri("http://localhost:5000");
            socket = new SocketIOUnity(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string> { { "token", "UNITY" } },
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });

            socket.JsonSerializer = new NewtonsoftJsonSerializer();

            socket.OnConnected += (sender, e) =>
            {
                Debug.Log("socket.OnConnected");
            };
            socket.On("result", (response) =>
            {
                // Handle the result here
                Debug.Log("Received result from server:");
                Debug.Log(response.GetValue<string>());
            });

            socket.Connect();
            // Start the webcam
            webcamTexture = new WebCamTexture
            {
                // reduce the resolution of the webcam
                requestedWidth = 320,
                requestedHeight = 240
            };
            rawImage.texture = webcamTexture;
            rawImage.material.mainTexture = webcamTexture;
            // set height and width of rawImage
            // rawImage.rectTransform.sizeDelta = new Vector2(webcamTexture.width, webcamTexture.height);
            webcamTexture.Play();
        }
        void Start()
        {

        }

        void Update()
        {
            // // Emit a frame to the server
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // list of 1,2,3
                //     List<int> frameBytes = new List<int> { 1, 2, 3 };
                //     var frameData = new Dictionary<string, object>
                // {
                //     { "width", 10 },
                //     { "height", 20 },
                //     { "frame", frameBytes }
                // };
                //     socket.Emit("frame", frameData);
                if (webcamTexture.isPlaying)
                {

                    Debug.Log("Width: " + webcamTexture.width + " Height: " + webcamTexture.height);
                    // Capture the current frame
                    frame = webcamTexture.GetPixels32();


                    // Create a new texture with lower resolution
                    // int newWidth = webcamTexture.width / 2;
                    // int newHeight = webcamTexture.height / 2;
                    int newWidth = webcamTexture.width;
                    int newHeight = webcamTexture.height;
                    // Texture2D lowResTexture = new Texture2D(newWidth, newHeight);

                    // Draw the original image onto the new texture
                    // Graphics.ConvertTexture(webcamTexture, lowResTexture);

                    // Get the pixel data from the new texture
                    // Color32[] lowResFrame = lowResTexture.GetPixels32();


                    // Convert the frame to a byte array
                    // byte[] frameBytes = Color32ArrayToByteArray(frame);
                    byte[] frameBytes = Color32ArrayToByteArrayWithoutAlpha(frame);
                    Debug.Log("Size of frameBytes: " + frameBytes.Length);

                    // List<int> frameBytes = new List<int> { 1, 2, 3 };

                    var frameData = new Dictionary<string, object>
            {
                { "width", newWidth },
                { "height", newHeight },
                { "frame", frameBytes }
            };
                    Debug.Log("frameData: " + frameBytes);
                    // Emit the frame to the server
                    socket.Emit("frame", frameData);
                }
            }
        }

        void OnDestroy()
        {
            socket.Disconnect();
        }
        // Helper method to convert a Color32 array to a byte array
        // private byte[] Color32ArrayToByteArray(Color32[] colors)
        // {
        //     if (colors == null || colors.Length == 0)
        //         return null;

        //     byte[] bytes = new byte[colors.Length * 4];
        //     for (int i = 0; i < colors.Length; i++)
        //     {
        //         bytes[i * 4] = colors[i].r;
        //         bytes[i * 4 + 1] = colors[i].g;
        //         bytes[i * 4 + 2] = colors[i].b;
        //         bytes[i * 4 + 3] = colors[i].a;
        //     }

        //     return bytes;
        // }
        // Helper method to convert a Color32 array to a byte array without alpha component
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
        // Helper method to convert a Color32 array to a byte array
        private byte[] Color32ArrayToByteArray(Color32[] colors)
        {
            if (colors == null || colors.Length == 0)
                return null;

            int lengthOfColor32 = Marshal.SizeOf(typeof(Color32));
            int length = lengthOfColor32 * colors.Length;
            byte[] bytes = new byte[length];

            GCHandle handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
                IntPtr ptr = handle.AddrOfPinnedObject();
                Marshal.Copy(ptr, bytes, 0, length);
            }
            finally
            {
                if (handle != default(GCHandle))
                    handle.Free();
            }

            return bytes;

        }
    }

}
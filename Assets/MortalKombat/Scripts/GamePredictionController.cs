using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePredictionController : MonoBehaviour
{
    public RawImage webcamDisplay;
    public TextMeshProUGUI predictionText;

    private bool nextFrameReady = true;
    private Color32[] frame;
    private GlobalAssets.Socket.SocketUDP socketClient;
    private WebCamTexture webcamTexture;

    void Start()
    {
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
        // Check if the device supports webcam
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No webcam found!");
            return;
        }
        // Get the default webcam and start streaming
        webcamTexture = new WebCamTexture
        {
            // reduce the resolution of the webcam
            requestedWidth = 320,
            requestedHeight = 180
        };
        webcamDisplay.texture = webcamTexture;
        webcamDisplay.material.mainTexture = webcamTexture;
        webcamTexture.Play();
        predictionText.text = "";
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
                byte[] frameBytes = Color32ArrayToByteArrayWithoutAlpha(frame);
                // Encode the byte array to a Base64 string
                string frameBase64 = Convert.ToBase64String(frameBytes);
                // Construct dictionary to send to server
                Dictionary<string, string> message = new Dictionary<string, string>
                {
                    { "frame", frameBase64 },
                    { "width", webcamTexture.width.ToString() },
                    { "height", webcamTexture.height.ToString() },
                    { "event", "predict_frame" }
                };
                socketClient.SendMessage(message);
                nextFrameReady = false;
            }
        }
        // Receive the response from the server
        if (socketClient.isDataAvailable())
        {
            string message = socketClient.ReceiveMessage();
            // predictionText.text = message;
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
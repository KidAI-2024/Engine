using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PredictionController : MonoBehaviour
{
    public string PredictEventName = "";
    public RawImage webcamDisplay;
    public TextMeshProUGUI predictionText;
    public GameObject classesContainer;
    public GameObject predictButton;

    private bool nextFrameReady = true;
    private bool startPrediction = false;
    private Color32[] frame;
    private Dictionary<string, string> classMap = new Dictionary<string, string>();
    private GlobalAssets.Socket.SocketUDP socketClient;
    private WebCamTexture webcamTexture;
    private bool togglePredicting = false;
    private ProjectController projectController;

    void Start()
    {
        predictionText.text = "Predict";
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
        projectController = ProjectController.Instance;
        // Get the default webcam and start streaming
        webcamTexture = new WebCamTexture
        {
            // reduce the resolution of the webcam
            requestedWidth = 320,
            requestedHeight = 180
        };
    }
    public void StartPrediction()
    {
        togglePredicting = !togglePredicting;
        if (togglePredicting)
        {
            predictButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
            startPrediction = true;
            // Check if the device supports webcam
            if (WebCamTexture.devices.Length == 0)
            {
                Debug.LogError("No webcam found!");
                return;
            }
            webcamDisplay.texture = webcamTexture;
            webcamDisplay.material.mainTexture = webcamTexture;
            webcamTexture.Play();
        }
        else
        {
            predictButton.GetComponentInChildren<TextMeshProUGUI>().text = "Predict";
            startPrediction = false;
            webcamTexture.Stop();
            webcamDisplay.texture = null;
            webcamDisplay.material.mainTexture = null;
            predictionText.text = "Predict";
        }
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
                    { "event", PredictEventName }
                };
                socketClient.SendMessage(message);
                nextFrameReady = false;
            }
        }
        // Receive the response from the server
        if (socketClient.isDataAvailable())
        {
            Dictionary<string, string> response = socketClient.ReceiveDictMessage();
            string pred = response["prediction"];
            Debug.Log("Received: " + pred);
            predictionText.text = MapToClassName(pred);
            nextFrameReady = true;
            // Invoke("ResetNextFrameReady", 1.0f);
        }
    }
    // This function maps the prediction to the class name text
    string MapToClassName(string message)
    {
        if (projectController.PythonClassesToUnityClassesMap.ContainsKey(message))
        {
            return projectController.PythonClassesToUnityClassesMap[message];
        }
        return message;
    }
    void ResetNextFrameReady(){
        nextFrameReady = true;
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
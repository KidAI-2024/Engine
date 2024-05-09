using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PredictionController : MonoBehaviour
{
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

    void Start()
    {
        predictionText.text = "Predict";
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
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
            CreateClassMap();
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
            predictionText.text = MapToClassName(message);
            Invoke("ResetNextFrameReady", 1.0f);
        }
    }
    // This function creates a map of class names to class indices
    void CreateClassMap()
    {
        // Get the classes container to map the prediction to the class name
        if (classesContainer == null)
        {
            Debug.LogError("Please assign the target GameObject!");
            return;
        }
        int j = 0;
        Transform targetTransform = classesContainer.transform;
        foreach (Transform child in targetTransform)
        {
            classMap.Add(j.ToString(), child.GetComponentInChildren<TMP_InputField>().text);
            j++;
        }
    }
    // This function maps the prediction to the class name text
    string MapToClassName(string message)
    {
        if (classMap.ContainsKey(message))
        {
            return classMap[message];
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
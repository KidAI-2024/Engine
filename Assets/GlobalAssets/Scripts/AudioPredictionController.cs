using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class AudioPredictionController : MonoBehaviour
{
    public string PredictEventName = "";
    public string LoadModelEventName = "";
    public TextMeshProUGUI predictionText;
    public GameObject predictButton;
    public int sampleRate = 44100; // Audio sample rate
    public float chunkDuration = 1.0f; // Duration of each audio chunk in seconds
    public GameObject classesContainer;
    public GameObject TrainingButton;
    private AudioClip audioClip; // Recorded audio clip
    private bool startPrediction = false; // Flag to start/stop prediction
    private GlobalAssets.Socket.SocketUDP socketClient; // Socket client for communication
    private bool togglePredicting = false; // Toggle predicting flag
    private ProjectController projectController; // Reference to project controller
    private int lastSamplePosition = 0; // To keep track of the recording position

        void Start()
        {
            Debug.Log("In the audio pred class");
            predictionText.text = "Predict";
            socketClient = GlobalAssets.Socket.SocketUDP.Instance;
            projectController = ProjectController.Instance;

            LoadModelToML();
            predictButton.GetComponent<Button>().interactable = true;
        }

    public void StartPrediction()
    {
        Debug.Log("In start");
        togglePredicting = !togglePredicting;
        if (togglePredicting)
        {
            Debug.Log("In if");
            predictButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
            startPrediction = true;
            // Check if the device supports microphone
            Dictionary<string, string> message = new Dictionary<string, string>
        {
            
            { "event", PredictEventName }
        };

            // Send the message to the server
            socketClient.SendMessage(message);
            predictionText.text = "Recording...";
        }
        else
        {
            Debug.Log("In else");
            predictButton.GetComponentInChildren<TextMeshProUGUI>().text = "Predict";
            startPrediction = false;
            Dictionary<string, string> message = new Dictionary<string, string>
        {
           
            { "event", "stop_prediction" }
        };

            // Send the message to the server
            socketClient.SendMessage(message);
            predictionText.text = "Predict";
        }
    }

    void LoadModelToML()
    {
        Debug.Log("LOADING MODEL >>>>>");
        // Construct dictionary to send to server
        Dictionary<string, string> message = new Dictionary<string, string>
        {
            { "path",  Path.Combine(projectController.directoryPath, projectController.projectName) },
            { "saved_model_name", projectController.savedModelFileName },
            { "event", LoadModelEventName }
        };
        socketClient.SendMessage(message);
    }

    //void Update()
    //{
    //    if (startPrediction && Microphone.IsRecording(null))
    //    {
    //        Debug.Log("IN UPDATE 1");
    //        // Get the current position of the recording
    //        int currentPosition = Microphone.GetPosition(null);
    //        Debug.Log($"Current Position: {currentPosition}, Last Sample Position: {lastSamplePosition}");

    //        // Check if we have recorded enough samples for a chunk
    //        if (currentPosition > lastSamplePosition + sampleRate * chunkDuration)
    //        {
    //            Debug.Log("In current pos");

    //            // Process and send the audio chunk for prediction
    //            ProcessAudioForPrediction(audioClip);

    //            // Update the last sample position
    //            lastSamplePosition += Mathf.FloorToInt(sampleRate * chunkDuration);
    //        }
    //    }

    //    // Receive the response from the server
    //    if (socketClient.isDataAvailable())
    //    {
    //        Debug.Log("THERE IS DATA");
    //        Dictionary<string, string> response = socketClient.ReceiveDictMessage();
    //        if (response["event"] == PredictEventName)
    //        {
    //            string pred = response["prediction"];
    //            Debug.Log("Received: " + pred);
    //            predictionText.text = MapToClassName(pred);
    //        }
    //        else if (response["event"] == LoadModelEventName)
    //        {
    //            if (response["status"] == "success") // Model loaded successfully
    //            {
    //                CreateClassMap();
    //                predictButton.GetComponent<Button>().interactable = true;
    //            }
    //            else // Model loading failed.. lock the predict button
    //            {
    //                predictButton.GetComponent<Button>().interactable = true;
    //            }
    //        }
    //    }
    //}

    void Update()
    {
        bool trainingInProgress = TrainingButton.GetComponent<StartTraining>().trainingInProgress;
        //bool trainingInProgress = true;
        //Debug.Log("Is training in progress "+ trainingInProgress);
        if (!trainingInProgress)
        {
            // Receive the response from the server (Python)
            if (socketClient.isDataAvailable())
            {
                Dictionary<string, string> response = socketClient.ReceiveDictMessage();
                //foreach (KeyValuePair<string, string> kvp in response)
                //{

                //    Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");

                //}
                if (response["event"] == PredictEventName)
                {
                    string pred = response["prediction"];
                    Debug.Log("Received Prediction: " + MapToClassName(pred)+" "+pred);

                    predictionText.text = MapToClassName(pred);
                }
                else if (response["event"] == LoadModelEventName)
                {
                    if (response["status"] == "success") // Model loaded successfully
                    {
                        Debug.Log("Model loaded successfully.");
                        // Optionally enable predict button or other UI updates
                        CreateClassMap();
                        predictButton.GetComponent<Button>().interactable = true;
                    }
                    else // Model loading failed
                    {
                        // Optionally disable predict button or handle failure
                        predictButton.GetComponent<Button>().interactable = false;
                    }
                }
            }
        }
    }

    //void ProcessAudioForPrediction(AudioClip audioClip)
    //{
    //    // Convert the AudioClip to a WAV byte array
    //    byte[] audioBytes = WavUtility.FromAudioClipStream(audioClip);

    //    // Encode the byte array to a Base64 string
    //    string audioBase64 = Convert.ToBase64String(audioBytes);

    //    // Construct dictionary to send to server
    //    Dictionary<string, string> message = new Dictionary<string, string>
    //    {
    //        { "audio", audioBase64 },
    //        { "sample_rate", sampleRate.ToString() },
    //        { "event", PredictEventName }
    //    };

    //    // Send the message to the server
    //    socketClient.SendMessage(message);
    //}

    void CreateClassMap()
    {
        projectController.PythonClassesToUnityClassesMap.Clear();
        int j = 0;
        foreach (string className in projectController.classes)
        {
            projectController.PythonClassesToUnityClassesMap.Add(j.ToString(), className);
            Debug.Log("Class: " + j.ToString() + " Name: " + className);
           
            j++;
        }
    }
    string MapToClassName(string message)
    {
        if (projectController.PythonClassesToUnityClassesMap.ContainsKey(message))
        {
            return projectController.PythonClassesToUnityClassesMap[message];
        }
        return message;
    }
    public void stopping()
    {
        Dictionary<string, string> message = new Dictionary<string, string>
        {

            { "event", "stop_prediction" }
        };

        // Send the message to the server
        socketClient.SendMessage(message);
    }
    void OnDestroy()
    {
        //// Ensure microphone is stopped when the script is destroyed
        //if (Microphone.IsRecording(null))
        //{
        //    Microphone.End(null);
        //}
    }
}
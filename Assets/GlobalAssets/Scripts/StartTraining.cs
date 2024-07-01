using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;
using GlobalAssets.UI;
using UnityEngine.SceneManagement;

public class StartTraining : MonoBehaviour
{
    public string trainingEvent;
    public GameObject saveProjectButton;
    public bool trainingInProgress = false;
    public GameObject warningPanel;
    private GlobalAssets.Socket.SocketUDP socketClient;
    private ProjectController projectController;
    private bool isTrainingFinished = false;
    private bool isTrainingStarted = false;
    private GameObject TrainingButton;

    void Start()
    {
        projectController = ProjectController.Instance;
        TrainingButton = this.gameObject;
        // get socket from SocketClient
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
    }
    void Update()
    {
        trainingInProgress = !isTrainingFinished && isTrainingStarted;
        if (trainingInProgress)
        {
          
            if (socketClient.isDataAvailable())
            {
                Dictionary<string, string> response = socketClient.ReceiveDictMessage();
                Debug.Log(response);
                foreach (KeyValuePair<string, string> kvp in response)
                {
                   
                        Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
                    
                }
                // Debug.Log("Received: " + response["status"]);
                if (response["status"] == "success")
                {
                    Debug.Log("In Train");
                    projectController.isTrained = true;
                    projectController.savedModelFileName = response["saved_model_name"];
                    projectController.Save();
                    isTrainingFinished = true;
                    isTrainingStarted = false;
                    TrainingButton.transform.GetChild(0).gameObject.SetActive(true);
                    TrainingButton.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }
    public void StartSocketTraining()
    {
        TrainingButton.transform.GetChild(0).gameObject.SetActive(false);
        TrainingButton.transform.GetChild(1).gameObject.SetActive(true);
        isTrainingStarted = true;
        isTrainingFinished=false;
        CreateClassMap();
        if (SceneManager.GetActiveScene().name == "Audio")
        {
            saveProjectButton.GetComponent<SaveAudioProject>().Save();
        }
        else
        {
            saveProjectButton.GetComponent<SaveProject>().Save();
        }
       
        if (!Validate()) return;
        SocketTrain();
    }
    // This function creates a map of class names to class indices
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
    private bool Validate()
    {
        // check if number of classes is greater than 0
        if (projectController.numberOfClasses < 1)
        {
            DisplayWarning("Add at least one class", "OK");
            return false;
        }

        // check empty class names
        foreach (string className in projectController.classes)
        {
            if (className == "")
            {
                DisplayWarning("Class name cannot be empty", "OK");
                return false;
            }
        }

        // check if number if image a class is less than 10
        foreach (string className in projectController.classes)
        {
            if (projectController.imagesPerClass[className] < 1)
            {
                DisplayWarning("Add at least 10 images to each class", "OK");
                return false;
            }
        }
        // check number of images in each class is greater than 0
        foreach (string className in projectController.classes)
        {
            if (projectController.imagesPerClass[className] < 1)
            {
                DisplayWarning("Add images to class " + className, "OK");
                return false;
            }
        }
        return true;
    }

    private void DisplayWarning(string message, string buttonText)
    {
        // enable warning panel
        warningPanel.SetActive(true);
        // warning message
        warningPanel.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
        // warning button text
        warningPanel.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonText;
    }
    private void SocketTrain()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        Dictionary<string, string> message = new Dictionary<string, string>
        {
            { "path",  "Projects/"+ projectController.projectName },
            { "event", trainingEvent },
            {"num_classes", projectController.numberOfClasses.ToString() },
            {"epochs", projectController.epochs.ToString()},
            {"max_lr", projectController.learningRate.ToString()}

        };
        socketClient.SendMessage(message);
        Debug.Log("Training Started");
    }
}
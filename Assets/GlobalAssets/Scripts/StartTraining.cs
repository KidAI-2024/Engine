using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;
using GlobalAssets.UI;
using GlobalAssets.Socket;
using System;
using UnityEngine.SceneManagement;


public enum DisplayMessageType
{
    Success,
    Error,
    Warning
}
public class StartTraining : MonoBehaviour
{
    public string trainingEvent;
    public bool trainingInProgress = false;
    public GameObject saveProjectButton;
    public GameObject warningPanel;
    public GameObject feebackPanel;
    public GameObject predictButton;
    public GameObject uploadButton;

    private GlobalAssets.Socket.SocketUDP socketClient;
    private ProjectController projectController;
    private bool isTrainingFinished = false;
    private bool isTrainingStarted = false;
    private GameObject TrainingButton;
    private GameObject GraphImage;

    public Sprite warningIcon;
    public Sprite errorIcon;
    public Sprite successIcon;

    void Start()
    {
        projectController = ProjectController.Instance;
        TrainingButton = this.gameObject;
        // get the graph image container
        if (feebackPanel.transform.childCount > 0)
            GraphImage = feebackPanel.transform.GetChild(0).gameObject;
        // get socket from SocketClient
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
    }
    void Update()
    {
        // Check available messages incoming from the server 
        // only if the training is in progress (started and not finished yet)
        trainingInProgress = !isTrainingFinished && isTrainingStarted;
        if (trainingInProgress)
        {
            if (socketClient.isDataAvailable())
            {
                Debug.Log("Training data Available");
                Dictionary<string, string> response = socketClient.ReceiveDictMessage();
                // If the training is completed successfully
                if (response["status"] == "success")
                {
                    projectController.isTrained = true;
                    projectController.savedModelFileName = response["saved_model_name"];
                    projectController.Save();
                    isTrainingFinished = true;
                    isTrainingStarted = false;
                    TrainingButton.transform.GetChild(0).gameObject.SetActive(true);
                    TrainingButton.transform.GetChild(1).gameObject.SetActive(false);
                    if (response.ContainsKey("feature_importance_graph"))
                    {
                        string graph = response["feature_importance_graph"];
                        byte[] imageBytes = Convert.FromBase64String(graph);
                        Texture2D texture = new Texture2D(1000, 600);
                        texture.LoadImage(imageBytes);
                        GraphImage.GetComponent<RawImage>().texture = texture;
                    }
                    // unlock the predict button
                    predictButton.GetComponent<Button>().interactable = true;
                    uploadButton.GetComponent<Button>().interactable = true;
                    DisplayWarning("Training completed successfully", "OK", DisplayMessageType.Success);
                }
                else if (response["status"] == "failed") // training failed for a server side error
                {
                    if (response.ContainsKey("error"))
                        DisplayWarning(response["error"], "OK");
                    else
                        DisplayWarning("Training failed", "OK");
                    TrainingButton.transform.GetChild(0).gameObject.SetActive(true);
                    TrainingButton.transform.GetChild(1).gameObject.SetActive(false);
                    isTrainingFinished = true;
                    isTrainingStarted = false;
                }
            }
        }
    }
    // save the project before training
    // Type Audio save audio project (.wav) have different save function
    public void StartSocketTraining()
    {
        if (SceneManager.GetActiveScene().name == "Audio")
        {
            saveProjectButton.GetComponent<SaveAudioProject>().Save();
        }
        else
        {
            saveProjectButton.GetComponent<SaveProject>().Save();
        }
        if (!Validate()) return; // validate the project before training
        // set the training in progress flags
        isTrainingFinished = false;
        isTrainingStarted = true;
        CreateClassMap();
        TrainingButton.transform.GetChild(0).gameObject.SetActive(false);
        TrainingButton.transform.GetChild(1).gameObject.SetActive(true);
        SocketTrain();
    }
    // This function creates a map of class names to class indices
    // because the server expects the class indices to be in the form of strings
    // example { "0": "class1", "1": "class2", "2": "class3" }
    // to be able to map classes from the server to the classes in the project
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
    // Client side validation of the project before training
    private bool Validate()
    {
        // check if any 2 classes have same name
        for (int i = 0; i < projectController.classes.Count; i++)
        {
            for (int j = i + 1; j < projectController.classes.Count; j++)
            {
                if (projectController.classes[i] == projectController.classes[j])
                {
                    DisplayWarning("Two classes cannot have the same name", "OK");
                    return false;
                }
            }
        }
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
                DisplayWarning("Add at least 10 images to each class", "OK", DisplayMessageType.Warning);
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

    // Display a warning message to the user
    private void DisplayWarning(string message, string buttonText, DisplayMessageType messageType = DisplayMessageType.Error)
    {
        // enable warning panel
        warningPanel.SetActive(true);
        // image 
        switch (messageType)
        {
            case DisplayMessageType.Error:
                warningPanel.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = errorIcon;
                break;
            case DisplayMessageType.Warning:
                warningPanel.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = warningIcon;
                break;
            case DisplayMessageType.Success:
                warningPanel.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = successIcon;
                break;
        }
        // warning message
        warningPanel.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
        // warning button text
        warningPanel.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonText;
    }

    // Send a message to the server to start training
    private void SocketTrain()
    {    
        // Absolute path to the project directory
        string projectPath = Path.Combine(projectController.directoryPath, projectController.projectName);
        Debug.Log("Training Path: " + projectPath);
        Dictionary<string, string> message = new Dictionary<string, string>
        {
            { "path",  projectPath },
            { "model", projectController.model },
            { "feature_extraction_type", projectController.featureExtractionType },
            { "features", string.Join(",", projectController.features) },
            {"num_classes", projectController.numberOfClasses.ToString() },
            {"epochs", projectController.epochs.ToString()},
            {"max_lr", projectController.learningRate.ToString()},
            {"model_category", projectController.modelCategory.ToString()},
            {"classical_model_type", projectController.classicalModelType.ToString()},
            { "feature_extraction_type_img", projectController.featureExtractionTypeImg.ToString()},
            { "event", trainingEvent }
        };
        socketClient.SendMessage(message);
        Debug.Log("Training Started");
    }
}
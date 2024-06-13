using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;
using GlobalAssets.UI;

public class StartTraining : MonoBehaviour
{
    public GameObject saveProjectButton;
    public GameObject warningPanel;
    private GlobalAssets.Socket.SocketUDP socketClient;
    private ProjectController projectController;

    void Start()
    {
        projectController = ProjectController.Instance;
        // get socket from SocketClient
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
    }
    void Update()
    {
        // Receive the response from the server
        if (socketClient.isDataAvailable())
        {
            Dictionary<string, string> response = socketClient.ReceiveDictMessage();
            Debug.Log("Received: " + response["status"]);
            if (response["status"] == "success")
            {
                projectController.isTrained = true;
                projectController.savedModelFileName = response["saved_model_name"];
                projectController.Save();
            }
        }
    }
    public void StartSocketTraining(){
        saveProjectButton.GetComponent<SaveProject>().Save();
        if (!Validate()) return;
        SocketTrain();
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
            if (projectController.imagesPerClass[className] < 10)
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
            { "event", "start_body_pose_train" }
        };
        socketClient.SendMessage(message);
        Debug.Log("Training Started");
    } 
}
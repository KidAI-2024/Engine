using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class StartTraining : MonoBehaviour
{
    public GameObject targetGameObject; // The GameObject whose children contain ScrollView components
    public string folderPath = "Projects"; // Folder path to save the images
    public GameObject warningPanel;
    private string savePath;
    private string projectName;
    private List<ClassData> ImagesData = new List<ClassData>();
    private GlobalAssets.Socket.SocketUDP socketClient;
    private ProjectController projectController;

    private struct ClassData
    {
        public string className;
        public List<byte[]> images;
    }

    void Start()
    {
        projectController = ProjectController.Instance;
        projectName = projectController.projectName; 
        // get socket from SocketClient
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
    }

    public void StartSocketTraining(){
        GetImages();
        if (!Validate()) return;
        SaveImagesToPath();
        SocketTrain();
    }
    private bool Validate()
    {
        // check if number of classes is greater than 0
        if (ImagesData.Count < 1)
        {
            DisplayWarning("Add classes to start training", "OK");
            return false;
        }
        // check empty class names
        for (int i = 0; i < ImagesData.Count; i++)
        {
            if (ImagesData[i].className == "")
            {
                DisplayWarning("Class " + (i+1) +" name cannot be empty", "OK");
                return false;
            }
        }
        // check number of images in each class is greater than 0
        for (int i = 0; i < ImagesData.Count; i++)
        {
            if (ImagesData[i].images.Count == 0)
            {
                DisplayWarning("Add images to the class " + ImagesData[i].className, "OK");
                return false;
            }
        }
        // check if number if image a class is less than 10
        for (int i = 0; i < ImagesData.Count; i++)
        {
            if (ImagesData[i].images.Count < 10)
            {
                DisplayWarning("Add more than 10 images to the class " + ImagesData[i].className, "OK");
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
            { "path", savePath },
            { "event", "start_body_pose_train" }
        };
        socketClient.SendMessage(message);
        Debug.Log("Training Started");
    } 
    void Update()
    {
        // Receive the response from the server
        if (socketClient.isDataAvailable())
        {
            string message = socketClient.ReceiveMessage();
            Debug.Log("Received: " + message);
        }
    }
    private void SaveImagesToPath()
    {
        // Define the base folder path
        string basePath = folderPath;
        savePath = basePath + "/" + projectName; // Project One be dynamic
        // Save ImagesData to basePath/Project1/ImagesData.className/images[i].png
        for (int i = 0; i < ImagesData.Count; i++)
        {
            // Construct the folder path for the current class
            string classFolderPath = Path.Combine(basePath, projectName, i + "_" + ImagesData[i].className);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(classFolderPath))
            {
                Directory.CreateDirectory(classFolderPath);
            }

            // Save each image
            for (int j = 0; j < ImagesData[i].images.Count; j++)
            {
                // Construct the file path for the current image
                string imagePath = Path.Combine(classFolderPath, i + "_" + ImagesData[i].className + "_" + j + ".png");

                // Write the image bytes to the file
                File.WriteAllBytes(imagePath, ImagesData[i].images[j]);
            }
            Debug.Log("Saved Class : " + ImagesData[i].className + " images to " + classFolderPath);
        }
    }

    private void GetImages()
    {
        ImagesData.Clear();

        // Check if the targetGameObject is provided
        if (targetGameObject == null)
        {
            Debug.LogError("Please assign the target GameObject!");
            return;
        }
        int j = 0;
        // Get the transform of the targetGameObject
        Transform targetTransform = targetGameObject.transform;
        projectController.classes.Clear(); // clear the classes list
        // Iterate through all direct children of the targetGameObject
        foreach (Transform child in targetTransform)
        {
            // from each child, find the input field and get its value
            TMP_InputField className = child.GetChild(0).GetChild(1).GetComponentInChildren<TMP_InputField>();
            if (className != null)
            {
                // Create a new ClassData object and add it to the list
                string classNameText = className.text;
                projectController.classes.Add(classNameText); // add the class to the project controller
                ImagesData.Add(new ClassData { className = classNameText, images = new List<byte[]>() });
                
                // from each child, find "Content" object and get the images inside it
                Transform content = child.GetComponentInChildren<Mask>().transform;
                if (content != null)
                {
                    // Get all the Raw Images inside the Content object
                    RawImage[] images = content.GetComponentsInChildren<RawImage>(true);
                    // Loop through each image and save it
                    for (int i = 0; i < images.Length; i++)
                    {
                        // Get the texture from the Raw Image
                        Texture2D texture = images[i].texture as Texture2D;
                        // Convert the texture to a byte array
                        byte[] bytes = texture.EncodeToPNG();
                        // Add the byte array to the list of images
                        ImagesData[j].images.Add(bytes);
                    }
                    // Debug.Log("ImagesData['"+ImagesData[j].className+"'].images.Count: " + ImagesData[j].images.Count);
                }
                j++;
            }
        }
    }
}
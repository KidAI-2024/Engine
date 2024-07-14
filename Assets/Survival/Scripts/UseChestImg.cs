/*
    Script Name: UseChestImg
    Purpose: This script facilitates interaction with a chest GameObject in a game environment,
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Linq;
using TMPro;

namespace Survival
{
    public class UseChestImg : MonoBehaviour
    {
        // Public variables
        public GameObject handUI; // UI element for indicating interaction with the chest
        public Text predictionResult; // Text UI element to display prediction result
        public GameObject rawImageToPredict; // GameObject containing RawImage for displaying images to predict
        public int pythonPredictedClass { get; private set; } = -1; // Predicted class ID from Python model
        public string UnityPredictedClass { get; private set; } = ""; // Predicted class name mapped to Unity
        // Private variables
        private GameObject chestObj; // Reference to the chest GameObject
        private ProjectController projectController; // Reference to the project controller for managing project-related data
        private string projectPath = ""; // Path to the project directory
        private System.Random random = new System.Random(); // Random number generator
        private bool canReach; // Flag to track if the player is within reach of the chest
        private int numberOfClasses; // Number of classes (categories) in the project
        private Color32[] frame; // Array of colors representing the current frame

        private GlobalAssets.Socket.SocketUDP socketClient; // Socket client for sending and receiving messages
        private bool predictDone = false; // Flag to track if prediction process is completed
        private bool reqSent = false; // Flag to track if prediction request has been sent

        private int randomClass = 0; // Index of the randomly chosen class

        // Coroutine to load a random image from a specific class folder
        IEnumerator LoadRandomImage(int randomClass, System.Action<Texture2D> callback)
        {
            // Get the class name from the classes list
            string className = projectController.classes[randomClass];
            // Get the path of the class folder
            string classPath = projectPath + "\\" + "test" + "\\" + randomClass.ToString() + '_' + className;
            Debug.Log(classPath);
            // Get all image files from the specified folder
            List<string> imageFiles = Directory.GetFiles(classPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg")).ToList();

            if (imageFiles.Count == 0)
            {
                Debug.LogError("No image files found in the specified folder.");
                yield break;
            }

            // Select a random file
            string randomImagePath = imageFiles[random.Next(0, imageFiles.Count)];

            // Load the image into a texture
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file:///" + randomImagePath))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to load image: " + uwr.error);
                }
                else
                {
                    // Assign the texture to the RawImage
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                    Canvas canvasComponent = rawImageToPredict.GetComponentInChildren<Canvas>();
                    if (canvasComponent != null)
                    {
                        // Get the RawImage component that is a child of the Canvas
                        RawImage rawImageComponent = canvasComponent.GetComponentInChildren<RawImage>();
                        if (rawImageComponent != null)
                        {
                            // Assign the texture to the RawImage component
                            rawImageComponent.texture = texture;
                            // Call the callback function with the loaded texture
                            callback(texture);
                        }
                        else
                        {
                            Debug.LogError("RawImage component not found in children of Canvas.");
                        }
                    }
                }
            }
        }

        // Called when another collider enters the trigger zone of this GameObject
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Reach")
            {
                canReach = true;
                handUI.SetActive(true); // Activate the hand UI for interaction
            }
        }

        // Called when another collider exits the trigger zone of this GameObject
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Reach")
            {
                canReach = false;
                handUI.SetActive(false); // Deactivate the hand UI
            }
        }

        // Converts a class name received from Python to the corresponding Unity class name
        string PythonToUnityClassName(string message)
        {
            if (projectController.PythonClassesToUnityClassesMap.ContainsKey(message))
            {
                return projectController.PythonClassesToUnityClassesMap[message];
            }
            return message;
        }

        // Converts an array of Color32 to a byte array without alpha channel
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

        // Initialization method called once when the script starts
        void Start()
        {
            chestObj = this.gameObject; // Assign the current GameObject (chest) to chestObj
            handUI.SetActive(false); // Deactivate the hand UI initially
            rawImageToPredict.SetActive(false); // Deactivate the image to predict initially

            // Get references and initialize variables
            socketClient = GlobalAssets.Socket.SocketUDP.Instance; // Get socket client instance
            projectController = ProjectController.Instance; // Get project controller instance
            projectPath = projectController.directoryPath + "\\" + projectController.projectName; // Construct project path
            Debug.Log("Project path: " + projectPath);
            numberOfClasses = projectController.numberOfClasses; // Get number of classes in the project
        }

        // Update is called once per frame
        void Update()
        {
            // Check if the player is in reach and presses the interact button
            if (canReach && Input.GetButtonDown("Interact"))
            {
                handUI.SetActive(false); // Deactivate the hand UI
                canReach = false; // Player can no longer interact with the chest until the next trigger enter

                // Generate a random class index between 0 and numberOfClasses
                randomClass = random.Next(0, numberOfClasses);

                // Load a random image from the selected class folder asynchronously
                StartCoroutine(LoadRandomImage(randomClass, texture =>
                {
                    // Show the image to predict
                    rawImageToPredict.SetActive(true);
                    chestObj.GetComponent<Animator>().SetBool("open", true); // Open the chest animation
                    chestObj.GetComponent<BoxCollider>().enabled = false; // Disable the chest collider to prevent further interactions

                    // Prepare frame data to send to the server
                    frame = texture.GetPixels32();
                    byte[] frameBytes = Color32ArrayToByteArrayWithoutAlpha(frame);
                    string frameBase64 = Convert.ToBase64String(frameBytes);

                    // Construct message to send to the server
                    Dictionary<string, string> message = new Dictionary<string, string>
                    {
                        { "frame", frameBase64 },
                        { "width", texture.width.ToString() },
                        { "height", texture.height.ToString() },
                        { "event", "predict_image_classifier" }
                    };

                    // Send prediction request to the server via socket
                    socketClient.SendMessage(message);
                    Debug.Log("Prediction request sent");
                    reqSent = true;
                }));
            }

            // Check if data is available from the socket and prediction process is ongoing
            if (socketClient.isDataAvailable() && !predictDone && reqSent)
            {
                // Receive and process prediction result from the server
                Dictionary<string, string> response = socketClient.ReceiveDictMessage();

                if (response["event"] == "predict_image_classifier")
                {
                    Debug.Log("predict_image_classifier");
                    pythonPredictedClass = int.Parse(response["prediction"]); // Get predicted class ID from Python
                    UnityPredictedClass = PythonToUnityClassName(response["prediction"]); // Map Python class name to Unity class name

                    Debug.Log("Python Prediction: " + pythonPredictedClass);
                    Debug.Log("Unity Prediction: " + UnityPredictedClass);
                    Debug.Log("Random Class: " + randomClass);

                    // Display prediction result based on correctness
                    if (pythonPredictedClass == randomClass)
                    {
                        predictionResult.text = "Correctly Classified, Class: " + UnityPredictedClass;
                        PlayerController.Instance.numOfCorrectClassifiedImgs += 1; // Increment correct predictions count
                        Debug.Log("Number of correct predictions: " + PlayerController.Instance.numOfCorrectClassifiedImgs);
                    }
                    else
                    {
                        predictionResult.text = "Misclassified, Class: " + UnityPredictedClass;
                    }

                    predictDone = true;

                    // Activate animation if available
                    Canvas canvasComponent = rawImageToPredict.GetComponentInChildren<Canvas>();
                    if (canvasComponent != null)
                    {
                        Animator animator = canvasComponent.GetComponent<Animator>();
                        if (animator != null)
                        {
                            animator.SetTrigger("StartAnimation"); // Trigger animation
                            Debug.Log("Animator activated");
                        }
                        else
                        {
                            Debug.LogError("Animator component not found on the target GameObject.");
                        }
                    }
                }
            }
        }
    }
}

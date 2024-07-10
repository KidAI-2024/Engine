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
        private GameObject OB; //this is the chest
        public GameObject handUI;
        // public RawImage rawImageToPredict;
        public GameObject rawImageToPredict;
        private ProjectController projectController;
        private string projectPath = "";
        private System.Random random = new System.Random();
        private bool inReach;
        // get the number of classes in the project folder
        private int numberOfClasses;
        public int pythonPredictedClass { get; private set; } = -1;
        public string UnityPredictedClass { get; private set; } = "";
        private Color32[] frame;

        // get socket from SocketClient
        private GlobalAssets.Socket.SocketUDP socketClient;
        public Text predictionResult;
        private bool predictDone = false;
        private bool reqSent = false;

        private int randomClass = 0;
        IEnumerator LoadRandomImage(int randomClass, System.Action<Texture2D> callback)
        {
            // get the class name from the classes list
            string className = projectController.classes[randomClass];
            // get the path of the class folder
            string classPath = projectPath + "\\" + randomClass.ToString() + '_' + className;
            Debug.Log(classPath);
            // Get all image files from the specified folder
            List<string> imageFiles = Directory.GetFiles(classPath, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg")).ToList();

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
                            // Call the call back function
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
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Reach")
            {
                inReach = true;
                handUI.SetActive(true);
            }

        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Reach")
            {
                inReach = false;
                handUI.SetActive(false);
            }
        }
        string PythonToUnityClassName(string message)
        {
            if (projectController.PythonClassesToUnityClassesMap.ContainsKey(message))
            {
                return projectController.PythonClassesToUnityClassesMap[message];
            }
            return message;
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
        void Start()
        {

            OB = this.gameObject;

            handUI.SetActive(false);

            // rawImageToPredict.gameObject.SetActive(false);
            rawImageToPredict.SetActive(false);
            // get socket from SocketClient
            socketClient = GlobalAssets.Socket.SocketUDP.Instance;
            projectController = ProjectController.Instance;
            projectPath = projectController.directoryPath + "\\" + projectController.projectName;
            Debug.Log("project path" + projectPath);
            numberOfClasses = projectController.numberOfClasses;

        }

        void Update()
        {
            if (inReach && Input.GetButtonDown("Interact"))
            {

                handUI.SetActive(false);
                inReach = false;
                // choose random number between 0 and numberOfClasses
                // int randomClassIndex = Random.Range(0, numberOfClasses);
                // Generate a random number between 0 (inclusive) and numberOfClasses (exclusive)
                randomClass = random.Next(0, numberOfClasses);
                // load the image from the disk
                StartCoroutine(LoadRandomImage(randomClass, texture =>
                {

                    // show the image
                    rawImageToPredict.SetActive(true);
                    OB.GetComponent<Animator>().SetBool("open", true);
                    OB.GetComponent<BoxCollider>().enabled = false;
                    // send request
                    frame = texture.GetPixels32();
                    // Send the frame to the server
                    byte[] frameBytes = Color32ArrayToByteArrayWithoutAlpha(frame);
                    // Encode the byte array to a Base64 string
                    string frameBase64 = Convert.ToBase64String(frameBytes);
                    // Construct dictionary to send to server
                    Dictionary<string, string> message = new Dictionary<string, string>
                    {
                            { "frame", frameBase64 },
                            { "width", texture.width.ToString() },
                            { "height", texture.height.ToString() },
                            { "event", "predict_image_classifier" }
                    };
                    socketClient.SendMessage(message);
                    Debug.Log("Req sent");
                    reqSent = true;


                }));



            }
            if (socketClient.isDataAvailable() && !predictDone && reqSent)
            {
                Debug.Log("isDataAvailable");
                Dictionary<string, string> response = socketClient.ReceiveDictMessage();

                if (response["event"] == "predict_image_classifier")
                {
                    Debug.Log("predict_image_classifier");
                    pythonPredictedClass = int.Parse(response["prediction"]);
                    UnityPredictedClass = PythonToUnityClassName(response["prediction"]);
                    Debug.Log("Python Prediction: " + pythonPredictedClass);
                    Debug.Log("Unity Prediction: " + UnityPredictedClass);
                    Debug.Log("randomClass: " + randomClass);
                    if (pythonPredictedClass == randomClass)
                    {
                        predictionResult.text = "Correctly Classified, Class: " + UnityPredictedClass;
                        // Access and modify the global variable
                        PlayerController.Instance.numOfCorrectClassifiedImgs += 1;
                        Debug.Log("PlayerController.Instance.numOfCorrectClassifiedImgs: " + PlayerController.Instance.numOfCorrectClassifiedImgs);
                    }
                    else
                    {
                        predictionResult.text = "MisClassified, Class: " + UnityPredictedClass;
                    }
                    //hide the image
                    // rawImageToPredict.SetActive(false);
                    predictDone = true;
                    Canvas canvasComponent = rawImageToPredict.GetComponentInChildren<Canvas>();
                    if (canvasComponent != null)
                    {
                        // Get the Animator component from the target GameObject
                        Animator animator = canvasComponent.GetComponent<Animator>();
                        if (animator != null)
                        {
                            // Enable the Animator
                            // animator.enabled = true;
                            animator.SetTrigger("StartAnimation");

                            Debug.Log("animator is activated");
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
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace MortalKombat
{
    public class GamePredictionController : MonoBehaviour
    {
        public RawImage webcamDisplay;
        public TextMeshProUGUI predictionText;
        public int framePredicitonRate = 5; // predict every 5 frames

        private GameObject player1;
        private GameObject player2;
        private bool nextFrameReady = true;
        private Color32[] frame;
        private GlobalAssets.Socket.SocketUDP socketClient;
        private WebCamTexture webcamTexture;
        private int frameCounter = 0;
        private ProjectController projectController;
        private GameManager gameManager;

        void Start()
        {
            gameManager = GameManager.Instance;
            socketClient = GlobalAssets.Socket.SocketUDP.Instance;
            projectController = ProjectController.Instance;
            // search game object called PLayer1
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
            
            player1 = GameObject.Find("Player1");
            // player2 = GameObject.Find("Player2");
        }
        void Update()
        {
            frameCounter++;
            if (gameManager.predictIsOn && nextFrameReady && frameCounter % framePredicitonRate == 0)
            {
                frameCounter = 0;
                if (webcamTexture.isPlaying)
                {
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
                        { "event", "predict_body_pose" }
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
                string unityPredictedClass = MapToClassName(pred); // ML code returns 1,2,3 we want "class x", "class y", "class z"
                player1.GetComponent<Player1Controller>().prediction = unityPredictedClass;
                predictionText.text = unityPredictedClass;
                nextFrameReady = true;
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
}
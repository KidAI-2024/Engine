using UnityEngine;
using UnityEngine.UI;
using System.IO.Pipes;
using System;

namespace GlobalAssets.LocalNamedPipes
{
    public class LocalNamedPipesClient : MonoBehaviour
    {
        // For camera streaming
        private WebCamTexture webcamTexture;
        private Color32[] frame;
        public RawImage rawImage;
        // public string message = "Hello, World!";
        private NamedPipeClientStream clientStream;
        private string pipeName = "\\\\.\\pipe\\unity"; // Replace with your pipe name

        // Start is called before the first frame update
        void Start()
        {
            try
            {
                clientStream = new NamedPipeClientStream(pipeName);
                clientStream.Connect(1000); // Timeout in milliseconds
                Debug.Log("Connected to server.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error connecting to server: {e.Message}");
            }
            // Start the webcam
            webcamTexture = new WebCamTexture
            {
                // reduce the resolution of the webcam
                requestedWidth = 160,
                requestedHeight = 120
            };
            rawImage.texture = webcamTexture;
            rawImage.material.mainTexture = webcamTexture;
            // set height and width of rawImage
            // rawImage.rectTransform.sizeDelta = new Vector2(webcamTexture.width, webcamTexture.height);
            webcamTexture.Play();

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

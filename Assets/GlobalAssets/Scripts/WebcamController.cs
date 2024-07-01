using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

public class WebcamController : MonoBehaviour
{
    public bool EnablePreprocessing = false;
    public string PreprocessingEventName = "";
    public RawImage webcamDisplay;
    public GameObject imagePrefab; // Prefab of RawImage to instantiate
    public Transform imageContainer; // Parent transform for instantiated RawImages
    public Transform finalImagesContainer; // Parent transform for instantiated RawImages
    public Button captureButton;
    public Button autoCaptureButton;
    public GameObject autoCaptureGO;
    public TextMeshProUGUI numCapturedText;

    private int autoCaptureTime = 1;
    private WebCamTexture webcamTexture;
    private TMP_Dropdown autoCaptureDDL;
    public GameObject EmptyImage;
    private GameObject EmptyImageCaptureImages;
    private GameObject numberOFImagesOutside;
    public List<Texture2D> capturedImages = new List<Texture2D>();
    public bool isPreprossingInProgress = false;

    private Color32[] NonProcessedFrame;
    private Color32[] frame;
    private bool nextFrameReady = true;
    private GlobalAssets.Socket.SocketUDP socketClient;



    void Start()
    {
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
        // EmptyImage = finalImagesContainer.parent.parent.gameObject.transform.GetChild(2).gameObject;
        // EmptyImageCaptureImages = imageContainer.parent.parent.gameObject.transform.GetChild(2).gameObject;
        // Add a listener to the capture button
        autoCaptureDDL = autoCaptureGO.GetComponent<TMP_Dropdown>();
        captureButton.onClick.AddListener(CapturePhoto);
        autoCaptureButton.onClick.AddListener(AutoCapture);
        autoCaptureDDL.onValueChanged.AddListener(delegate {
            AutoCaptureTimeValueChanged(autoCaptureDDL.options[autoCaptureDDL.value].text);
        });
        if(PreprocessingEventName == "" && EnablePreprocessing == true)
        {
            Debug.LogError("PreprocessingEventName is not set to gameobject TrainingCanvas > ClassesContainer > CameraPanel");
        }
    }
    void Update()
    {
        numCapturedText.text = "Captured: " + capturedImages.Count + " images";
        EmptyImageCaptureImages.SetActive(capturedImages.Count == 0);
        
        if (EnablePreprocessing && isPreprossingInProgress)
        {
            SendFrameFromUnityCamera();
            ReceivePreprocessedImage();
        }
    }
    void SendFrameFromUnityCamera()
    {
        if (nextFrameReady)
        {
            if (webcamTexture.isPlaying)
            {
                // Send the frame to the server
                // Debug.Log("Width: " + webcamTexture.width + " Height: " + webcamTexture.height);
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
                    // { "event", "predict_frame" }
                    { "event", PreprocessingEventName }
                };
                socketClient.SendMessage(message);
                nextFrameReady = false;
            }
        }
    }
    void ReceivePreprocessedImage()
    {
        if (socketClient.isDataAvailable())
        {
            Dictionary<string, string> response = socketClient.ReceiveDictMessage();
            if (response["event"] == PreprocessingEventName)
            {
                string image = response["preprocessed_image"];
                byte[] imageBytes = Convert.FromBase64String(image);
                Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
                texture.LoadImage(imageBytes);
                if (webcamDisplay != null)
                {
                    webcamDisplay.texture = texture;
                    webcamDisplay.material.mainTexture = texture;
                }
            }
            nextFrameReady = true;
        }
    }
    public void OpenCamera(GameObject outsideImagesContainer)
    {   
        isPreprossingInProgress = true;
        // get the gameobject of the button that opens the camera
        finalImagesContainer = outsideImagesContainer.transform;
        EmptyImage = finalImagesContainer.parent.parent.gameObject.transform.GetChild(2).gameObject;
        EmptyImageCaptureImages = imageContainer.parent.parent.gameObject.transform.GetChild(2).gameObject;
        // Check if the device supports webcam
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No webcam found!");
            return;
        }

        // Get the default webcam and start streaming
        // webcamTexture = new WebCamTexture();
        webcamTexture = new WebCamTexture
        {
            // reduce the resolution of the webcam
            requestedWidth = 320,
            requestedHeight = 180
        };
        webcamDisplay.texture = webcamTexture;
        webcamTexture.Play();
    }
    public void AutoCaptureTimeValueChanged(string value)
    {
        autoCaptureTime = int.Parse(value);
    }

    public void AutoCapture()
    {
        // change autoCaptureButton text mesh pro to stop
        autoCaptureButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
        InvokeRepeating("CapturePhoto", autoCaptureTime, autoCaptureTime);
        autoCaptureButton.onClick.RemoveAllListeners();
        autoCaptureButton.onClick.AddListener(StopAutoCapture);
    }
    public void StopAutoCapture()
    {
        // change autoCaptureButton text mesh pro to stop
        CancelInvoke("CapturePhoto");
        autoCaptureButton.GetComponentInChildren<TextMeshProUGUI>().text = "Auto capture";
        autoCaptureButton.onClick.RemoveAllListeners();
        autoCaptureButton.onClick.AddListener(AutoCapture);
    }
    public void CapturePhoto()
    {
        // Create a texture with the same dimensions as the webcam feed
        Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
        // Read pixels from the webcam feed into the texture
        photo.SetPixels(webcamTexture.GetPixels());
        // Apply changes
        photo.Apply();

        // Instantiate a new RawImage and set its texture
        GameObject newImageObject = Instantiate(imagePrefab, imageContainer);
        RawImage newImage = newImageObject.GetComponent<RawImage>();
        newImage.texture = photo;

        // Set the position of the new RawImage to stack horizontally in a grid
        float spacingX = 70f; // Adjust the horizontal spacing between images
        float spacingY = 70f; // Adjust the vertical spacing between rows
        int maxColumns = 3; // Number of columns in the grid
        int row = capturedImages.Count / maxColumns; // Calculate the row index
        int col = capturedImages.Count % maxColumns; // Calculate the column index

        // Add the captured photo to the list
        capturedImages.Add(photo);

        Vector3 newPosition = new Vector3(col * spacingX + 15, -row * spacingY - 10, 0);
        newImageObject.transform.localPosition = newPosition;
        newImageObject.GetComponent<RemoveImage>().ImageIndex = capturedImages.Count - 1;
        
        // get the imageContainer and increase its height
        if (col == 0 && capturedImages.Count > 8)
        {
            imageContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                imageContainer.GetComponent<RectTransform>().sizeDelta.x,
                imageContainer.GetComponent<RectTransform>().sizeDelta.y + 70
            );
        }
    }

    public void CloseCamera()
    {
        isPreprossingInProgress = false;
        nextFrameReady = true;
        // stop the InvokeRepeating
        CancelInvoke("CapturePhoto");

        // clear finalImagesContainer content
        foreach (Transform child in finalImagesContainer)
        {
            Destroy(child.gameObject);
        }
        // Stop webcam feed when the object is destroyed
        if (webcamTexture != null)
            webcamTexture.Stop();

        int i = 0;
        // add all the captured images to the finalImagesContainer
        foreach (Texture2D image in capturedImages)
        {
            GameObject newImageObject = Instantiate(imagePrefab, finalImagesContainer);
            RawImage newImage = newImageObject.GetComponent<RawImage>();
            newImage.texture = image;
            
            // Set the position of the new RawImage to stack horizontally in a grid
            float spacingX = 70f; // Adjust the horizontal spacing between images
            float spacingY = 70f; // Adjust the vertical spacing between rows
            int maxColumns = 3; // Number of columns in the grid
            int row = i / maxColumns; // Calculate the row index
            int col = i % maxColumns; // Calculate the column index


            Vector3 newPosition = new Vector3(col * spacingX + 5, -row * spacingY - 5, 0);
            newImageObject.transform.localPosition = newPosition;
            
            // get the imageContainer and increase its height
            if (col == 0 && capturedImages.Count > 8)
            {
                finalImagesContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                finalImagesContainer.GetComponent<RectTransform>().sizeDelta.x, 
                finalImagesContainer.GetComponent<RectTransform>().sizeDelta.y + 70);
            }
            i++;
        }
        EmptyImage.SetActive(capturedImages.Count == 0);
        //numberOFImagesOutside.GetComponent<TextMeshProUGUI>().text = capturedImages.Count > 1? capturedImages.Count + " IMAGES": capturedImages.Count + " IMAGE";
    }

    // This function is called by the popup activate to show the captured images of the class clicked
    public void InstantiateCapturedImages()
    {
        int i = 0;
        foreach (Texture2D image in capturedImages)
        {
            GameObject newImageObject = Instantiate(imagePrefab, imageContainer);
            RawImage newImage = newImageObject.GetComponent<RawImage>();
            newImage.texture = image;
            
            // Set the position of the new RawImage to stack horizontally in a grid
            float spacingX = 70f; // Adjust the horizontal spacing between images
            float spacingY = 70f; // Adjust the vertical spacing between rows
            int maxColumns = 3; // Number of columns in the grid
            int row = i / maxColumns; // Calculate the row index
            int col = i % maxColumns; // Calculate the column index

            Vector3 newPosition = new Vector3(col * spacingX + 15, -row * spacingY - 10, 0);
            newImageObject.transform.localPosition = newPosition;
            newImageObject.GetComponent<RemoveImage>().ImageIndex = i;
            
            // get the imageContainer and increase its height
            if (col == 0 && capturedImages.Count > 8)
            {
                imageContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    imageContainer.GetComponent<RectTransform>().sizeDelta.x,
                    imageContainer.GetComponent<RectTransform>().sizeDelta.y + 70
                );
            }
            i++;
        }
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

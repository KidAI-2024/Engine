using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WebcamController : MonoBehaviour
{
    public RawImage webcamDisplay;
    public GameObject imagePrefab; // Prefab of RawImage to instantiate
    public Transform imageContainer; // Parent transform for instantiated RawImages
    public Transform finalImagesContainer; // Parent transform for instantiated RawImages
    public Button captureButton;

    private WebCamTexture webcamTexture;
    public List<Texture2D> capturedImages = new List<Texture2D>();


    void Start()
    {
        // Add a listener to the capture button
        captureButton.onClick.AddListener(CapturePhoto);
    }

    public void OpenCamera()
    {   
        // Check if the device supports webcam
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No webcam found!");
            return;
        }

        // Get the default webcam and start streaming
        webcamTexture = new WebCamTexture();
        webcamDisplay.texture = webcamTexture;
        webcamTexture.Play();
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

        Vector3 newPosition = new Vector3(col * spacingX, -row * spacingY, 0);
        newImageObject.transform.localPosition = newPosition;
        newImageObject.GetComponent<RemoveImage>().ImageIndex = capturedImages.Count - 1;
        
        // get the imageContainer and increase its height
        if (col == 0 && capturedImages.Count > 8)
        {
            imageContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(imageContainer.GetComponent<RectTransform>().sizeDelta.x, imageContainer.GetComponent<RectTransform>().sizeDelta.y + 70);
        }
    }

    public void CloseCamera()
    {
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


            Vector3 newPosition = new Vector3(col * spacingX, -row * spacingY, 0);
            newImageObject.transform.localPosition = newPosition;
            
            // get the imageContainer and increase its height
            if (col == 0 && capturedImages.Count > 8)
            {
                finalImagesContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(finalImagesContainer.GetComponent<RectTransform>().sizeDelta.x, finalImagesContainer.GetComponent<RectTransform>().sizeDelta.y + 70);
            }
            i++;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using SFB; // Import the namespace for NativeFilePicker

public class UploadImageController : MonoBehaviour
{
    public GameObject imagePrefab; // Prefab of RawImage to instantiate
    public Transform finalImagesContainer; // Parent transform for instantiated RawImages
    
    private List<Texture2D> capturedImages = new List<Texture2D>();

    // This function should open the file explorer to select an image
    public void SelectImagesFromFileExplorer()
    {   
        
    }

    public void OnSelectImageFinish()
    {
        int i = 0;
        // add all the captured images to the finalImagesContainer
        foreach (Texture2D image in capturedImages)
        {
            GameObject newImageObject = Instantiate(imagePrefab, finalImagesContainer);
            RawImage newImage = newImageObject.GetComponent<RawImage>();
            newImage.texture = image;
            // reduce its size and set its position as in the imageContainer
            // newImageObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

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

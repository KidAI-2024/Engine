using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class StartTraining : MonoBehaviour
{
    public GameObject targetGameObject; // The GameObject whose children contain ScrollView components
    public string folderPath = "Training"; // Folder path to save the images
    private string savePath;
    
    public void ExtractAndSaveImages()
    {
        // Check if the targetGameObject is provided
        if (targetGameObject == null)
        {
            Debug.LogError("Please assign the target GameObject!");
            return;
        }
        // Create the base folder if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Get all children of the targetGameObject
        Transform[] children = targetGameObject.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            savePath = "";
            savePath = folderPath;
            // from each child, find the input field and get its value
            TMP_InputField inputField = child.GetComponent<TMP_InputField>();
            
            if (inputField != null)
            {
                // Get the text from the input field
                savePath += "/" + inputField.text;
            }
            
            // Create the folder if it doesn't exist
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            // from each child, find "Content" object and get the images inside it
            Transform content = child.Find("Content");
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
                    
                    // Construct the file path for saving the image
                    Debug.Log("File path: " + savePath + "/img_" + i + ".png");
                    
                    // Save the image to the folder
                    File.WriteAllBytes(savePath + "/img_" + i + ".png", bytes);
                }
            }
        }
    }
}
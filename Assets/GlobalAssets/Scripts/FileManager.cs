using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class FileManager : MonoBehaviour
{
    // Start is called before the first frame update
    string path;
    public GameObject imagePrefab; // Prefab of RawImage to instantiate
    List<Texture2D> capturedImages = new List<Texture2D>();
    public Transform finalImagesContainer; // Parent transform for instantiated RawImages

    public void OpenFileExplorer(GameObject outsideImagesContainer)
    {
        finalImagesContainer = outsideImagesContainer.transform;
        // Debug.Log("finalImagesContainer: " + finalImagesContainer);

        // path = EditorUtility.OpenFilePanel("Select images", "", "png,jpg,jpeg");
        path = EditorUtility.OpenFolderPanel("Select images", "./", "");
        Debug.Log("path: " + path);

        string[] files = Directory.GetFiles(path);

        foreach (string file in files)
        {
            if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".PNG") || file.EndsWith(".JPG") || file.EndsWith(".JPEG"))
            {

                // File.Copy(file, EditorApplication.currentScene);
                // Debug.Log("file:" + path + "/" + Path.GetFileName(file));

                GetImg(path + "/" + Path.GetFileName(file));
            }
        }
        // empty the list of captured images
        capturedImages.Clear();
    }

    public void GetImg(string imgPath)
    {
        if (imgPath != null)
        {
            UpdateImage(imgPath);
        }
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
    public void UpdateImage(string imgPath)
    {
        WWW www = new WWW("file://" + imgPath);
        // add the texture to the list of captured images
        capturedImages.Add(www.texture);
        // print the length of the list of captured images
        // Debug.Log(capturedImages.Count);
        OnSelectImageFinish();

    }
}

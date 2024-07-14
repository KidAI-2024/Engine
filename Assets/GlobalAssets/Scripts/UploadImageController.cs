// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEditor;
// using SFB; // Import the namespace for NativeFilePicker


// public class UploadImageController : MonoBehaviour
// {
//     public GameObject imagePrefab; // Prefab of RawImage to instantiate
//     public Transform finalImagesContainer; // Parent transform for instantiated RawImages

//     private List<Texture2D> capturedImages = new List<Texture2D>();
//     // This method opens the file explorer and returns the selected file paths
//     public void OpenFileExplorer()
//     {
//         // Set the file extension filter
//         ExtensionFilter[] extensions = new[]
//         {
//             new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
//             new ExtensionFilter("All Files", "*"),
//         };

//         // Open the file explorer with the specified filter and allow multiple selection
//         string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);

//         if (paths.Length > 0)
//         {
//             foreach (string path in paths)
//             {
//                 Debug.Log("Selected file: " + path);
//                 StartCoroutine(LoadImage(path));
//                 // Check if all images have been loaded
//                 if (capturedImages.Count == paths.Length)
//                 {
//                     OnSelectImageFinish();
//                 }
//             }
//         }
//     }

//     // Coroutine to load and display the image
//     private IEnumerator LoadImage(string path)
//     {
//         WWW www = new WWW("file:///" + path);
//         yield return www;

//         Texture2D texture = www.texture;

//         // Instantiate a new RawImage and set its texture
//         // GameObject newImageObject = Instantiate(imagePrefab, imageContainer);
//         // RawImage newImage = newImageObject.GetComponent<RawImage>();
//         // newImage.texture = texture;

//         // // Set the position of the new RawImage (customize as needed)
//         // float spacingX = 70f; // Adjust the horizontal spacing between images
//         // float spacingY = 70f; // Adjust the vertical spacing between rows
//         // int maxColumns = 3; // Number of columns in the grid
//         // int row = imageContainer.childCount / maxColumns; // Calculate the row index
//         // int col = imageContainer.childCount % maxColumns; // Calculate the column index

//         // Vector3 newPosition = new Vector3(col * spacingX + 15, -row * spacingY - 10, 0);
//         // newImageObject.transform.localPosition = newPosition;
//         // add the loaded image to the list of captured images
//         capturedImages.Add(texture);


//     }
//     public void OnSelectImageFinish()
//     {
//         int i = 0;
//         // add all the captured images to the finalImagesContainer
//         foreach (Texture2D image in capturedImages)
//         {
//             GameObject newImageObject = Instantiate(imagePrefab, finalImagesContainer);
//             RawImage newImage = newImageObject.GetComponent<RawImage>();
//             newImage.texture = image;
//             // reduce its size and set its position as in the imageContainer
//             // newImageObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

//             // Set the position of the new RawImage to stack horizontally in a grid
//             float spacingX = 70f; // Adjust the horizontal spacing between images
//             float spacingY = 70f; // Adjust the vertical spacing between rows
//             int maxColumns = 3; // Number of columns in the grid
//             int row = i / maxColumns; // Calculate the row index
//             int col = i % maxColumns; // Calculate the column index

//             Vector3 newPosition = new Vector3(col * spacingX, -row * spacingY, 0);
//             newImageObject.transform.localPosition = newPosition;

//             // get the imageContainer and increase its height
//             if (col == 0 && capturedImages.Count > 8)
//             {
//                 finalImagesContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(finalImagesContainer.GetComponent<RectTransform>().sizeDelta.x, finalImagesContainer.GetComponent<RectTransform>().sizeDelta.y + 70);
//             }
//             i++;
//         }
//     }
//     // This function should open the file explorer to select an image
//     // public void SelectImagesFromFileExplorer()
//     // {
//     // var bp = new BrowserProperties();
//     // bp.filter = "Image files (*.png, *.jpg)|*.png;*.jpg";
//     // bp.filterIndex = 0;
//     // bp.defaultExt = "png";
//     // bp.title = "Select Image";
//     // bp.directory = Application.dataPath;
//     // bp.defaultFileName = "image.png";
//     // // bp.multiSelect = true;
//     // // bp.browserType = BrowserType.Open;
//     // // bp.browserSize = new Vector2(800, 600);
//     // // bp.browserPosition = new Vector2(200, 200);
//     // // bp.showMinimiseButton = true;
//     // new FileBrowser().OpenFileBrowser(bp, (path) =>
//     // {
//     //     if (path.Length == 0)
//     //     {
//     //         Debug.Log("No file selected");
//     //         return;
//     //     }
//     //     StartCoroutine(LoadImage(path));
//     // });
//     // }
//     /*IEnumerable LoadImage(string path)
//     {
//         // var url = "file://" + path;
//         // var www = new WWW(url);
//         // yield return www;
//         // if (string.IsNullOrEmpty(www.error))
//         // {
//         //     Texture2D texture = new Texture2D(2, 2);
//         //     www.LoadImageIntoTexture(texture);
//         //     capturedImages.Add(texture);
//         //     OnSelectImageFinish();
//         // }
//         // else
//         // {
//         //     Debug.Log("Error loading image: " + www.error);
//         // }
//         //using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
//         //{
//         //    yield return www.SendWebRequest();
//         //    if (www.isNetworkError || www.isHttpError)
//         //    {
//         //        Debug.Log(www.error);
//         //    }
//         //    else
//         //    {
//         //        Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
//         //        capturedImages.Add(texture);
//         //        OnSelectImageFinish();
//         //    }
//         //}
//     }
//     */

// }

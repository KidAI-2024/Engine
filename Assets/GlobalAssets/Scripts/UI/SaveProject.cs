using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;


namespace GlobalAssets.UI
{
    public class SaveProject : MonoBehaviour
    {
        
        public GameObject targetGameObject; // The GameObject whose children contain ScrollView components
        public GameObject AddNewClassButton; // The button that adds a new class
        public GameObject CameraController; // The CameraController GameObject (will be used to instantiate the classBoxes)
        public string folderPath = "Projects"; // Folder path to save the images
        private string savePath;
        private List<ClassData> ImagesData = new List<ClassData>();
        ProjectController projectController;
        // for loading images
        List<Texture2D> capturedImages = new List<Texture2D>();
        public GameObject imagePrefab;
        GameObject EmptyImage;
        Transform finalImagesContainer;

        private struct ClassData
        {
            public string className;
            public List<byte[]> images;
        }

        // Start is called before the first frame update
        void Start()
        {
            projectController = ProjectController.Instance;
            Load();
            this.gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Save());
        }
        private void Load()
        {
            // If number of classes > 2 instantiate new classBoxes using AddNewClassButton.GetComponent<AddNewClass>().InstantiateNewClass();
            // Loop over classes in the projectController.classes list folders and load the images
            // Modify the classBox's input field to have the class name
            // Load the images into the classBox   
            for (int i = 0; i < projectController.classes.Count; i++)
            {
                capturedImages.Clear();
                if (i > 1)
                    AddNewClassButton.GetComponent<AddNewClass>().InstantiateNewClass();

                Transform classBox = targetGameObject.transform.GetChild(i);
                TMP_InputField className = classBox.GetChild(0).GetChild(1).GetComponentInChildren<TMP_InputField>();
                className.text = projectController.classes[i];
                finalImagesContainer = classBox.GetChild(0).GetChild(5).GetChild(0).GetChild(0).gameObject.transform; // ImagesContainer
                EmptyImage = finalImagesContainer.parent.parent.transform.GetChild(2).gameObject;
                string classFolderPath = Path.Combine(projectController.directoryPath, projectController.projectName, i + "_" + projectController.classes[i]);
                if (Directory.Exists(classFolderPath))
                {
                    string[] imagePaths = Directory.GetFiles(classFolderPath);

                    foreach (string imagePath in imagePaths)
                    {
                        byte[] bytes = File.ReadAllBytes(imagePath);
                        Texture2D texture = new Texture2D(320,180);
                        texture.LoadImage(bytes);
                        capturedImages.Add(texture);
                    }
                    LoadImages();
                }
            }
        }
        public void LoadImages()
        {
            finalImagesContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                finalImagesContainer.GetComponent<RectTransform>().sizeDelta.x,
                137.5f
            );
            foreach (Transform child in finalImagesContainer)
            {
                Destroy(child.gameObject);
            }
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
                newImageObject.GetComponent<RemoveImage>().ImageIndex = i;
                newImageObject.GetComponent<RemoveImage>().capturedImages = capturedImages;
                newImageObject.GetComponent<RemoveImage>().isLoad = true;
                // get the imageContainer and increase its height
                if (col == 0 && i > 5)
                {
                    finalImagesContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                        finalImagesContainer.GetComponent<RectTransform>().sizeDelta.x, 
                        finalImagesContainer.GetComponent<RectTransform>().sizeDelta.y + 70
                    );
                }
                i++;
            }
            EmptyImage.SetActive(i == 0);
        }

        public void Save()
        {
            //if()schene name
            GetImages();
            SaveImagesToPath();
            projectController.Save();

        }
        private void SaveImagesToPath()
        {
            // Define the base folder path
            savePath = Path.Combine(projectController.directoryPath, projectController.projectName); // Project One be dynamic
            // Save ImagesData to basePath/Project1/ImagesData.className/images[i].png
            for (int i = 0; i < ImagesData.Count; i++)
            {
                // Construct the folder path for the current class
                string classFolderPath = Path.Combine(savePath, i + "_" + ImagesData[i].className);
                
                // delete directory that starts with i_
                string[] directories = Directory.GetDirectories(savePath);
                foreach (string directory in directories)
                {
                    if (directory.Contains(i + "_"))
                    {
                        Directory.Delete(directory, true);
                    }
                }
                
                // Create the directory if it doesn't exist
                if (!Directory.Exists(classFolderPath))
                {
                    Directory.CreateDirectory(classFolderPath);
                }

                // Save each image
                for (int j = 0; j < ImagesData[i].images.Count; j++)
                {

                    // Construct the file path for the current image
                    string imagePath = Path.Combine(classFolderPath, i + "_" + ImagesData[i].className + "_" + j + ".png");

                    // Write the image bytes to the file
                    File.WriteAllBytes(imagePath, ImagesData[i].images[j]);
                }
            }
        }

        private void GetImages()
        {

            // Check if the targetGameObject is provided
            if (targetGameObject == null)
            {
                Debug.LogError("Please assign the target GameObject!");
                return;
            }
            int j = 0;
            ImagesData.Clear();
            projectController.classes.Clear(); // clear the classes list
            projectController.imagesPerClass.Clear(); // clear the images per class dictionary
            // Get the transform of the targetGameObject
            Transform targetTransform = targetGameObject.transform;
            // Iterate through all direct children of the targetGameObject
            foreach (Transform child in targetTransform)
            {
                // from each child, find the input field and get its value
                TMP_InputField className = child.GetChild(0).GetChild(1).GetComponentInChildren<TMP_InputField>();
                if (className != null)
                {
                    // Create a new ClassData object and add it to the list
                    string classNameText = className.text;
                    projectController.classes.Add(classNameText); // add the class to the project controller
                    ImagesData.Add(new ClassData { className = classNameText, images = new List<byte[]>() });
                    
                    // from each child, find "Content" object and get the images inside it
                    Transform content = child.GetComponentInChildren<Mask>().transform;
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
                            // Add the byte array to the list of images
                            ImagesData[j].images.Add(bytes);
                        }
                        projectController.imagesPerClass.Add(classNameText, ImagesData[j].images.Count);
                    }
                    j++;
                }
            }
        }
    }
}
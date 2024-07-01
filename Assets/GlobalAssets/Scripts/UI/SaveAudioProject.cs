using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace GlobalAssets.UI
{
    public class SaveAudioProject : MonoBehaviour
    {
        public GameObject targetGameObject; // The GameObject whose children contain ScrollView components
        public GameObject AddNewClassButton; // The button that adds a new class
        public GameObject CameraController; // The CameraController GameObject (will be used to instantiate the classBoxes)
        public string folderPath = "Projects"; // Folder path to save the audios
        private string savePath;
        private List<ClassData> AudiosData = new List<ClassData>();
        ProjectController projectController;
        // for loading audios
        List<AudioClip> capturedAudios = new List<AudioClip>();
        public GameObject audioPrefab;
        GameObject EmptyAudio;
        Transform finalAudiosContainer;

        private struct ClassData
        {
            public string className;
            public List<AudioClip> audios;
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
            // Loop over classes in the projectController.classes list folders and load the audios
            // Modify the classBox's input field to have the class name
            // Load the audios into the classBox   

            for (int i = 0; i < projectController.classes.Count; i++)
            {
                capturedAudios.Clear();
                if (i > 1)
                    AddNewClassButton.GetComponent<AddAudioClass>().InstantiateNewClass();
                    //AddNewClassButton.GetComponent<?AddAudioClass>().InstantiateNewClass();

                Transform classBox = targetGameObject.transform.GetChild(i);
                TMP_InputField className = classBox.GetChild(0).GetChild(1).GetComponentInChildren<TMP_InputField>();
                className.text = projectController.classes[i];
                finalAudiosContainer = classBox.GetChild(0).GetChild(4).GetChild(0).GetChild(0).gameObject.transform; // AudiosContainer
                EmptyAudio = finalAudiosContainer.parent.parent.transform.GetChild(2).gameObject;
                string classFolderPath =  Path.Combine(projectController.directoryPath, projectController.projectName, i + "_" + projectController.classes[i]);
                if (Directory.Exists(classFolderPath))
                {
                    string[] audioPaths = Directory.GetFiles(classFolderPath);

                    foreach (string audioPath in audioPaths)
                    {
                        byte[] bytes = File.ReadAllBytes(audioPath);
                        AudioClip audioClip = WavUtility.ToAudioClip(bytes, audioPath); // Assuming you have a utility to convert bytes to AudioClip
                        capturedAudios.Add(audioClip);
                    }
                    LoadAudios();
                }
            }
        }

        public void LoadAudios()
        {
            foreach (Transform child in finalAudiosContainer)
            {
                Destroy(child.gameObject);
            }
            int i = 0;
            // add all the captured audios to the finalAudiosContainer
            foreach (AudioClip audio in capturedAudios)
            {
                GameObject newAudioObject = Instantiate(audioPrefab, finalAudiosContainer);
                AudioSource newAudioSource = newAudioObject.GetComponent<AudioSource>();
                newAudioSource.clip = audio;

                // Set the position of the new AudioSource to stack horizontally in a grid
                float spacingX = 70f; // Adjust the horizontal spacing between audios
                float spacingY = 70f; // Adjust the vertical spacing between rows
                int maxColumns = 3; // Number of columns in the grid
                int row = i / maxColumns; // Calculate the row index
                int col = i % maxColumns; // Calculate the column index

                Vector3 newPosition = new Vector3(col * spacingX + 5, -row * spacingY - 5, 0);
                newAudioObject.transform.localPosition = newPosition;
                newAudioObject.GetComponent<RemoveAudios>().ImageIndex = i;
                newAudioObject.GetComponent<RemoveAudios>().capturedAudios = capturedAudios;
                newAudioObject.GetComponent<RemoveAudios>().isLoad = true;
                // get the audioContainer and increase its height
                if (col == 0 && capturedAudios.Count > 8)
                {
                    finalAudiosContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                        finalAudiosContainer.GetComponent<RectTransform>().sizeDelta.x,
                        finalAudiosContainer.GetComponent<RectTransform>().sizeDelta.y + 70
                    );
                }
                i++;
            }
            EmptyAudio.SetActive(capturedAudios.Count == 0);
        }

        public void Save()
        {
            GetAudios();
            SaveAudiosToPath();
            projectController.Save();
        }

        private void SaveAudiosToPath()
        {
            // Define the base folder path
            savePath = Path.Combine(projectController.directoryPath, projectController.projectName); // Project One be dynamic
            // Save AudiosData to basePath/Project1/AudiosData.className/audios[i].wav
            for (int i = 0; i < AudiosData.Count; i++)
            {
                // Construct the folder path for the current class
                string classFolderPath = Path.Combine(savePath, i + "_" + AudiosData[i].className);
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

                // Save each audio
                for (int j = 0; j < AudiosData[i].audios.Count; j++)
                {
                    // Construct the file path for the current audio
                    string audioPath = Path.Combine(classFolderPath, i + "_" + AudiosData[i].className + "_" + j + ".wav");
                    // Convert the AudioClip to a byte array
                    byte[] bytes = WavUtility.FromAudioClipStream(AudiosData[i].audios[j]);

                    // Write the audio bytes to the file
                    File.WriteAllBytes(audioPath, bytes);
                }
            }
        }

        private void GetAudios()
        {
            // Check if the targetGameObject is provided
            if (targetGameObject == null)
            {
                Debug.LogError("Please assign the target GameObject!");
                return;
            }
            int j = 0;
            AudiosData.Clear();
            projectController.classes.Clear(); // clear the classes list
            projectController.imagesPerClass.Clear(); // clear the audios per class dictionary
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
                    AudiosData.Add(new ClassData { className = classNameText, audios = new List<AudioClip>() });

                    // from each child, find "Content" object and get the audios inside it
                    Transform content = child.GetComponentInChildren<Mask>().transform;
                    if (content != null)
                    {
                        // Get all the AudioSource components inside the Content object
                        AudioSource[] audios = content.GetComponentsInChildren<AudioSource>(true);
                        // Loop through each audio and save it
                        for (int i = 0; i < audios.Length; i++)
                        {
                            // Get the AudioClip from the AudioSource
                            AudioClip audioClip = audios[i].clip;
                            // Add the AudioClip to the list of audios
                            AudiosData[j].audios.Add(audioClip);
                        }
                        projectController.imagesPerClass.Add(classNameText, AudiosData[j].audios.Count);
                    }
                    j++;
                }
            }
        }
    }
}

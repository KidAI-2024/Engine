using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ProjectController : MonoBehaviour
{
    // Singleton pattern
    public static ProjectController Instance { get; private set; }

    // Project information
    public string projectName;
    public string createdAt;
    public string projectType;
    public string sceneName;
    public int numberOfClasses { get { return classes.Count; } }
    public bool isTrained = false;
    public bool isCreated = false;
    public string savedModelFileName;
    public List<string> classes = new List<string>();
    public Dictionary<string, int> imagesPerClass = new Dictionary<string, int>();  // Class : ImageCount
    public Dictionary<string, string> classesToControlsMap = new Dictionary<string, string>(); // Class : ControlName
    public Dictionary<string, string> ControlsToclassesMap = new Dictionary<string, string>(); // ControlName : Class
    public Dictionary<string, string> PythonClassesToUnityClassesMap = new Dictionary<string, string>(); // Python Predicted Class : Unity Class
    public List<string> features = new List<string>();
    public string model = "";
    public string featureExtractionType = "";

    public string directoryPath;

    public int epochs = 5;
    public float learningRate = 0.01f;
    public int modelCategory = 0; // 0 for classical, 1 for resnet, 2 for CNN
    // Ensure only one instance of ProjectController exists
    public int classicalModelType = 2;
    /*
    if classical model:
    0: SVM
    1: Logistic Regression
    2: Random Forest
    */
    public int featureExtractionTypeImg = 1;
    /*
    this is for classical model:
    0: SIFT
    1: HOG
    2: LBP
    */
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            directoryPath = Path.Combine(Application.persistentDataPath, "Projects");
            directoryPath = Path.GetFullPath(directoryPath);
        }
        else
        {
            Destroy(gameObject);
        }
        DummyFill();
    }
    private void DummyFill()
    {
        projectName = "DummyProject";
        createdAt = DateTime.Now.ToString();
        projectType = "Image Classification";
        sceneName = "ImageClassification";
        classes.Add("Class1");
        classes.Add("Class2");
        classes.Add("Class3");
        classes.Add("Class4");
        imagesPerClass.Add("Class1", 10);
        imagesPerClass.Add("Class2", 20);
        imagesPerClass.Add("Class3", 30);
        imagesPerClass.Add("Class4", 20);
        classesToControlsMap.Add("Class1", "Right");
        classesToControlsMap.Add("Class2", "Left");
        classesToControlsMap.Add("Class3", "Primary");
        classesToControlsMap.Add("Class4", "Secondary");
    }
    public void Save()
    {
        // save the Instance data to a file in the directory Engine/Projects/{projectName}
        string json = JsonUtility.ToJson(Instance);
        string projectDirectory = Path.Combine(directoryPath, projectName);
        if (!Directory.Exists(projectDirectory))
        {
            Directory.CreateDirectory(projectDirectory);
        }

        string path = Path.Combine(projectDirectory, "project.json");
        System.IO.File.WriteAllText(path, json);
        Debug.Log("Saved project data to: " + path);
    }

    // this function returns only projectName, createdAt, projectType
    public void Load(string prjctName)
    {
        // load the Instance data from a file in the directory Projects/{projectName}
        string path = Path.Combine(directoryPath, prjctName, "project.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, Instance);
            isCreated = false;
            Debug.Log("Loaded project data from: " + path);
        }
        else
        {
            Debug.LogError("File not found: " + path);
        }
    }

    public void Reset()
    {
        projectName = "";
        createdAt = "";
        projectType = "";
        sceneName = "";
        isTrained = false;
        savedModelFileName = "";
        model = "";
        featureExtractionType = "";
        features.Clear();
        classes.Clear();
        imagesPerClass.Clear();
        classesToControlsMap.Clear();
        ControlsToclassesMap.Clear();
        PythonClassesToUnityClassesMap.Clear();
    }
}

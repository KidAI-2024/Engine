using System;
using System.Collections.Generic;
using UnityEngine;

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
    public string savedModelFileName;
    public List<string> classes = new List<string>();
    public Dictionary<string, int> imagesPerClass = new Dictionary<string, int>();  // Class : ImageCount
    public Dictionary<string, string> classesToControlsMap = new Dictionary<string, string>(); // Class : ControlName
    public Dictionary<string, string> ControlsToclassesMap = new Dictionary<string, string>(); // ControlName : Class

    // Ensure only one instance of ProjectController exists
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        // save the Instance data to a file in the directory Engine/Projects/{projectName}
        string json = JsonUtility.ToJson(Instance);
        string path = Application.dataPath.Replace("/Assets", "/Projects") + "/" + projectName + "/project.json";
        System.IO.File.WriteAllText(path, json);
        Debug.Log("Saved project data to: " + path);
    }

    // this function returns only projectName, createdAt, projectType
    public void Load(string prjctName)
    {
        // load the Instance data from a file in the directory Projects/{projectName}
        string path = Application.dataPath.Replace("/Assets", "/Projects") + "/" + prjctName + "/project.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, Instance);
            Debug.Log("Loaded project data from: " + path);
        }
    }
}

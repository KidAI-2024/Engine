using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;


public class ProjectName : MonoBehaviour
{
    public GameObject ProjectNameTextField;
    public GameObject ErrorMessageTextField;
    public GameObject ProjectListPanel;    
    public GameObject ProjectBtnPrefab;
    public string projectType;
    public string nextSceneName;
    
    [System.Serializable]
    public class ProjectData
    {
        public string projectName;
        public string createdAt;
        public string projectType;
        public string sceneName;
        public int numberOfClasses;
        public bool isTrained;
        public string savedModelFileName;
        public List<string> classes;
        public Dictionary<string, int> imagesPerClass;
        public Dictionary<string, string> classesToControlsMap; // Class : ControlName
    }

    private ProjectController projectController;
    void Start()
    {
        projectController = ProjectController.Instance;
        LoadProjectsList();
    }
    void LoadProjectsList()
    {
        string projectsPath = Application.dataPath.Replace("/Assets", "/Projects") + "/";
        string[] projectFolders = System.IO.Directory.GetDirectories(projectsPath);
        foreach (string projectFolder in projectFolders)
        {
            // inside each folder read file project.json that contains projectName, createdAt, projectType
            string projectJsonPath = projectFolder + "/project.json";
            if (System.IO.File.Exists(projectJsonPath))
            {
                string json = System.IO.File.ReadAllText(projectJsonPath);
                // read json as dictionary
                ProjectData projectData = JsonUtility.FromJson<ProjectData>(json);
                InstentiateProjectBtn(projectData.projectName, projectData.projectType, projectData.createdAt, projectData.sceneName);
            }
        }
    }
    public void SetProjectType(string type)
    {
        projectType = type;
    }
    public void SetNextScene(string scene)
    {
        nextSceneName = scene;
    }
    public void CreateProject()
    {
        string projectName = ProjectNameTextField.GetComponentInChildren<TMP_InputField>().text;
        TextMeshProUGUI ErrorMessageText = ErrorMessageTextField.GetComponentInChildren<TextMeshProUGUI>();
        if(projectName == "")
        {
            ErrorMessageText.text = "*Project must have a name";
            Debug.Log("*Project must have a name");
            return;
        }
        ErrorMessageText.text = "";
        string createdAt = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        InstentiateProjectBtn(projectName, projectType, createdAt, nextSceneName);
        // Create project folder
        string basePath = Application.dataPath.Replace("/Assets", "/Projects");
        string classFolderPath = Path.Combine(basePath, projectName);
        if (!Directory.Exists(classFolderPath))
        {
            Directory.CreateDirectory(classFolderPath);
        }
        projectController.projectName = projectName;
        projectController.createdAt = createdAt;
        projectController.projectType = projectType;
        projectController.sceneName = nextSceneName;
        projectController.Save();


        SceneManager.LoadScene(nextSceneName);
    }

    void InstentiateProjectBtn(string ProjectName, string ProjectType, string CreatedAt, string nextSceneName)
    {
        GameObject newProjectBtn = Instantiate(ProjectBtnPrefab, ProjectListPanel.transform);
        // newProjectBtn.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = ProjectName;
        // newProjectBtn.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = ProjectName;
        // newProjectBtn.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
        TMP_InputField projectNameInputField = newProjectBtn.transform.GetChild(0).GetChild(0).GetComponent<TMP_InputField>();
        projectNameInputField.text = ProjectName;
        newProjectBtn.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = ProjectType;
        newProjectBtn.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = CreatedAt;
        newProjectBtn.GetComponentInChildren<Button>().onClick.AddListener(() => LoadProject (projectNameInputField.text, nextSceneName));

        // add listener to the project name input field to rename the project
        projectNameInputField.onEndEdit.AddListener((string newProjectName) => RenameProject(ProjectName, newProjectName));
        

        int projectCount = ProjectListPanel.transform.childCount;
        RectTransform newProjectBtnRectTransform = newProjectBtn.GetComponent<RectTransform>();
        float buttonHeight = newProjectBtnRectTransform.rect.height;
        float offsetY = -65 * (projectCount - 1) - 10;
        newProjectBtnRectTransform.anchoredPosition = new Vector2(0, offsetY);
    }
    public void RenameProject(string oldProjectName, string newProjectName)
    {
        Debug.Log("Rename project from " + oldProjectName + " to " + newProjectName);
        string basePath = Application.dataPath.Replace("/Assets", "/Projects");
        string oldProjectFolderPath = Path.Combine(basePath, oldProjectName);
        string newProjectFolderPath = Path.Combine(basePath, newProjectName);
        if (!Directory.Exists(newProjectFolderPath))
        {
            Directory.Move(oldProjectFolderPath, newProjectFolderPath);
            string projectJsonPath = newProjectFolderPath + "/project.json";
            if (System.IO.File.Exists(projectJsonPath))
            {
                string json = System.IO.File.ReadAllText(projectJsonPath);
                ProjectData projectData = JsonUtility.FromJson<ProjectData>(json);
                projectData.projectName = newProjectName;
                json = JsonUtility.ToJson(projectData);
                System.IO.File.WriteAllText(projectJsonPath, json);
            }
        }
        else
        {
            Debug.Log("Project already exists");
        }   
    }
    void LoadProject(string projectName, string sceneName)
    {
        projectController.Load(projectName);
        SceneManager.LoadScene(sceneName);
    }
}

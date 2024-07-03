using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System;

public class ProjectName : MonoBehaviour
{
    public GameObject ProjectNameTextField;
    public GameObject ErrorMessageTextField;
    public GameObject ProjectListPanel;    
    public GameObject ProjectBtnPrefab;
    public GameObject warningPanel;
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

    public GameObject LoadingPanel;
    private ProjectController projectController;
    private List<ProjectData> projectDataList = new List<ProjectData>();
    
    private GlobalAssets.Socket.SocketUDP socketClient;
    void Start()
    {
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
        PingServer();
        projectController = ProjectController.Instance;
        projectController.Reset();
        LoadProjectsList();
    }
    void Update()
    {
        try
        {
            if (socketClient.isDataAvailable())
            {
                Debug.Log("Server Alive");
                Dictionary<string, string> response = socketClient.ReceiveDictMessage();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Server is not available");
            warningPanel.SetActive(true);
        }
    }

    void PingServer()
    {
        socketClient.SendMessage(new Dictionary<string, string> { { "event", "ping" } });
    }
    // sort by createdAt
    void LoadProjectsList()
    {
        string projectsPath = projectController.directoryPath;
        string[] projectFolders = Directory.GetDirectories(projectsPath);

        foreach (string projectFolder in projectFolders)
        {
            string projectJsonPath = projectFolder + "/project.json";
            if (File.Exists(projectJsonPath))
            {
                string json = File.ReadAllText(projectJsonPath);
                ProjectData projectData = JsonUtility.FromJson<ProjectData>(json);
                projectDataList.Add(projectData);
            }
        }

        projectDataList.Sort((x, y) => DateTime.Parse(y.createdAt).CompareTo(DateTime.Parse(x.createdAt)));
        int i = 0;
        foreach (ProjectData projectData in projectDataList)
        {
            InstantiateProjectBtn(projectData.projectName, projectData.projectType, projectData.createdAt, projectData.sceneName);
            if(i >= 4)
            {
                RectTransform ProjectListPanelRectTransform = ProjectListPanel.GetComponent<RectTransform>();
                ProjectListPanelRectTransform.sizeDelta = new Vector2(ProjectListPanelRectTransform.sizeDelta.x, ProjectListPanelRectTransform.sizeDelta.y + 65);
            }
            i++;
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
        projectController.Reset();
        string projectName = ProjectNameTextField.GetComponentInChildren<TMP_InputField>().text;
        TextMeshProUGUI ErrorMessageText = ErrorMessageTextField.GetComponentInChildren<TextMeshProUGUI>();
        if(projectName == "")
        {
            ErrorMessageText.text = "*Project must have a name";
            Debug.Log("*Project must have a name");
            return;
        }
        LoadingPanel.SetActive(true);
        ErrorMessageText.text = "";
        string createdAt = System.DateTime.Now.ToString("yyyy-MM-dd");
        
        InstantiateProjectBtn(projectName, projectType, createdAt, nextSceneName);
        // Create project folder
        string classFolderPath = Path.Combine(projectController.directoryPath, projectName);
        if (!Directory.Exists(classFolderPath))
        {
            Directory.CreateDirectory(classFolderPath);
        }
        projectController.projectName = projectName;
        projectController.createdAt = createdAt;
        projectController.projectType = projectType;
        projectController.sceneName = nextSceneName;
        projectController.isCreated = true;
        projectController.Save();

        SceneManager.LoadScene(nextSceneName);
    }

    void InstantiateProjectBtn(string ProjectName, string ProjectType, string CreatedAt, string nextSceneName)
    {
        GameObject newProjectBtn = Instantiate(ProjectBtnPrefab, ProjectListPanel.transform);
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
        string oldProjectFolderPath = Path.Combine(projectController.directoryPath, oldProjectName);
        string newProjectFolderPath = Path.Combine(projectController.directoryPath, newProjectName);
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
        LoadingPanel.SetActive(true);
        projectController.Load(projectName);
        SceneManager.LoadScene(sceneName);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class ProjectName : MonoBehaviour
{
    public GameObject ProjectNameTextField;
    public GameObject ErrorMessageTextField;
    public GameObject ProjectListPanel;    
    public GameObject ProjectBtnPrefab;
    public string projectType;
    public string nextSceneName;
    

    private ProjectController projectController;
    void Start()
    {
        projectController = ProjectController.Instance;
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
        // Pass it to the next screen (save to shared pref)
        projectController.projectName = projectName;

        // create new button prefab instance and add it to ProjectListPanel children
        GameObject newProjectBtn = Instantiate(ProjectBtnPrefab, ProjectListPanel.transform);
        
        // newProjectBtn has 3 children of btn type, 
        // the first one is the project name
        // second one is project type (get from the button that invoked this function) 
        // the third one is the date
        // each button has a child that has text component
        newProjectBtn.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = projectName;
        newProjectBtn.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = projectType;
        newProjectBtn.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = System.DateTime.Now.ToString("dd/MM/yyyy");

        // Add a listener to the button
        // newProjectBtn.GetComponentInChildren<Button>().onClick.AddListener(() => LoadProject(projectName));

        // Change its position (add -70 in the y axes) depending on the count of the projects till now
        int projectCount = ProjectListPanel.transform.childCount;
        // newProjectBtn.transform.localPosition = new Vector3(0, -70 * projectCount, 0);
        RectTransform newProjectBtnRectTransform = newProjectBtn.GetComponent<RectTransform>();
        float buttonHeight = newProjectBtnRectTransform.rect.height;
        float offsetY = -65 * (projectCount - 1) - 10;
        newProjectBtnRectTransform.anchoredPosition = new Vector2(0, offsetY);

        SceneManager.LoadScene(nextSceneName);
    }
}

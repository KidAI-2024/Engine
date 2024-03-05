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
        PlayerPrefs.SetString("ProjectName", projectName);

        // create new button prefab instance and add it to ProjectListPanel children
        GameObject newProjectBtn = Instantiate(ProjectBtnPrefab, ProjectListPanel.transform);
        
        // newProjectBtn has 3 children of text type, 
        // the first one is the project name 
        // second one is project type (get from the button that invoked this function) 
        // the third one is the date
        newProjectBtn.GetComponentInChildren<TextMeshProUGUI>().text = projectName;
        newProjectBtn.GetComponentsInChildren<TextMeshProUGUI>()[1].text = projectType; 
        newProjectBtn.GetComponentsInChildren<TextMeshProUGUI>()[2].text = System.DateTime.Now.ToString("dd/MM/yyyy");

        // Add a listener to the button
        // newProjectBtn.GetComponentInChildren<Button>().onClick.AddListener(() => LoadProject(projectName));

        // Change its position (add -70 in the y axes) depending on the count of the projects till now
        int projectCount = ProjectListPanel.transform.childCount;
        newProjectBtn.transform.localPosition = new Vector3(0, -70 * projectCount, 0);
        SceneManager.LoadScene(nextSceneName);
    }
}

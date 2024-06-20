using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassesMapping : MonoBehaviour
{
    public GameObject classesContainer;
    public GameObject classBoxPrefab;
    public GameObject ControlsPanel;
    
    private ProjectController projectController;
    private float verticalOffset = 75;
    private List<string> controls = new List<string>();

    void Start()
    {
        projectController = ProjectController.Instance;
        FillControlsList();
        RenderClasses();
    }
    void FillControlsList()
    {
        // loop over the controls panel children and get the color of each button
        for (int i = 0; i < ControlsPanel.transform.childCount; i++)
        {
            GameObject control = ControlsPanel.transform.GetChild(i).gameObject;
            controls.Add(control.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text);
        }
    }
    void RenderClasses()
    {
        for (int i = 0; i < projectController.numberOfClasses; i++)
        {
            GameObject classBox = Instantiate(classBoxPrefab, classesContainer.transform);
            // set text mesh pro child of the button 
            classBox.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = projectController.classes[i];
            // set the position of the class box
            classBox.transform.localPosition = new Vector3(classBox.transform.localPosition.x, classBox.transform.localPosition.y - verticalOffset * i, 0);
            // get the list of buttons from child(1)
            GameObject buttonsContainer = classBox.transform.gameObject;
            // loop over the buttons and add listener to each one and set the button name to the control name
            for (int j = 0; j < controls.Count; j++)
            {
                GameObject button = buttonsContainer.transform.GetChild(1).GetChild(j).gameObject;
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => AssignClassToControl());
                button.name = controls[j];
            }
        }
    }

    void AssignClassToControl()
    {
        // when the button is clicked change the color of the parent classBox
        UnityEngine.UI.Button button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>();
        GameObject classBox = button.transform.parent.parent.gameObject;
        // set the color of the classBox to the color of the button
        Color controlColor = button.GetComponent<UnityEngine.UI.Image>().color; // get the color of the button clicked
        classBox.GetComponent<UnityEngine.UI.Image>().color = controlColor;
        string className = classBox.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        
        // class : control
        projectController.classesToControlsMap[className] = button.name;

        // if the color is already assigned to another class, assign another color (from the existing colors) the other class
        foreach (Transform child in classesContainer.transform)
        {
            if (child.gameObject != classBox && child.name != "Title")
            {
                if (child.GetComponent<UnityEngine.UI.Image>().color == controlColor)
                {
                    child.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
                    string otherClassName = child.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
                    projectController.classesToControlsMap[otherClassName] = "";
                }
            }
        }

        // PrintProjectControllerInfo();
    }
    void PrintProjectControllerInfo()
    {
        Debug.Log("Project Name: " + projectController.projectName);
        foreach (string className in projectController.classes)
        {
            Debug.Log("Class: " + className);
        }
        foreach (KeyValuePair<string, string> kvp in projectController.classesToControlsMap)
        {
            Debug.Log("Class: " + kvp.Key + " Controlled by : " + kvp.Value);
        }
    }
    void InverseClassToCtrlMapping()
    {
        foreach (var item in projectController.classesToControlsMap)
        {
            projectController.ControlsToclassesMap[item.Value] = item.Key;
            Debug.Log("Control: " + item.Value + " Class: " + item.Key);
        }
    }
    void OnDestroy()
    {
        InverseClassToCtrlMapping();
        projectController.Save();
    }
}

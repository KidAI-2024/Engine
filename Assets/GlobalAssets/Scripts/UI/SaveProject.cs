using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveProject : MonoBehaviour
{
    public GameObject classesContainer;
    ProjectController projectController;
    // Start is called before the first frame update
    void Start()
    {
        projectController = ProjectController.Instance;
        this.gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Save());
    }
    void Save()
    {

    }
}

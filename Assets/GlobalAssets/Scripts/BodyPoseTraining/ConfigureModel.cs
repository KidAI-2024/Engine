using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfigureModel : MonoBehaviour
{
    public GameObject features;

    public GameObject mediaPipeToggle;
    public GameObject classicalToggle;
    public GameObject featureExtractionTypeNotes;

    public GameObject SelectModelTogglesParent;

    private string mediapipeNote = "High performance, Accurate feature extraction, and Real-time processing.";
    private string classicalNote = "Traditional training mode, Time-consuming process.";
    private ProjectController ProjectController;
    private GameObject mediapipeSkeleton;
    private GameObject classicalWarning;

    void Start()
    {
        foreach (Transform child in SelectModelTogglesParent.transform)
        {
            child.GetComponent<Toggle>().onValueChanged.AddListener((value) => ManageToggles(child.GetComponent<Toggle>()));
        }

        mediaPipeToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => MediaPipeToggle(value));
        classicalToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => ClassicalToggle(value));

        mediapipeSkeleton = features.transform.parent.gameObject;
        classicalWarning = features.transform.parent.parent.GetChild(2).gameObject;

        ProjectController = ProjectController.Instance;
        LoadFeatures();
        LoadModel();
        LoadTrainingMode();
        Save();
    }
    public void Save()
    {
        if (ProjectController == null)
        {
            return;
        }
        ProjectController.features = GetFeaturesList();
        ProjectController.model = GetModel();
        ProjectController.featureExtractionType = GetTrainingMode();
        ProjectController.Save();
    }
    void ManageToggles(Toggle clickedToggle)
    {
        if (!clickedToggle.isOn)
        {
            // Prevent unchecking the last toggle
            int checkedCount = 0;
            foreach (Transform child in SelectModelTogglesParent.transform)
            {
                Toggle toggle = child.GetComponent<Toggle>();
                if (toggle.isOn)
                {
                    checkedCount++;
                }
            }
            if (checkedCount == 0)
            {
                // Re-check the toggle to ensure at least one is always checked
                clickedToggle.isOn = true;
                return;
            }
        }
    
        // Toggle off all other models
        foreach (Transform child in SelectModelTogglesParent.transform)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != clickedToggle)
            {
                toggle.isOn = false;
            }
        }
    }


    List<string> GetFeaturesList()
    {
        List<string> featuresList = new List<string>();
        // loop on features children (each child is a toggle) if on then add the child.name to the list
        foreach (Transform child in features.transform)
        {
            if (child.GetComponent<Toggle>().isOn)
            {
                featuresList.Add(child.name);
            }
        }
        return featuresList;
    }
    string GetModel()
    {
        string model = "SVM";
        foreach (Transform child in SelectModelTogglesParent.transform)
        {
            if (child.GetComponent<Toggle>().isOn)
            {
                model = child.name;
                break;
            }
        }
        return model;
    }
    string GetTrainingMode()
    {
        string model = "Classical";
        if (mediaPipeToggle.GetComponent<Toggle>().isOn)
        {
            model = "mediapipe";
            mediapipeSkeleton.SetActive(true);
            classicalWarning.SetActive(false);
        }
        else
        {
            model = "Classical";
            mediapipeSkeleton.SetActive(false);
            classicalWarning.SetActive(true);
        }
        return model;
    }
    void MediaPipeToggle(bool value)
    {
        if (value)
        {
            classicalToggle.GetComponent<Toggle>().isOn = false;
            featureExtractionTypeNotes.GetComponent<TextMeshProUGUI>().text = mediapipeNote;
            mediapipeSkeleton.SetActive(true);
            classicalWarning.SetActive(false);
        }
    }
    void ClassicalToggle(bool value)
    {
        if (value)
        {
            mediaPipeToggle.GetComponent<Toggle>().isOn = false;
            featureExtractionTypeNotes.GetComponent<TextMeshProUGUI>().text = classicalNote;
            mediapipeSkeleton.SetActive(false);
            classicalWarning.SetActive(true);
        }
    }
    void LoadFeatures()
    {
        if (ProjectController.isCreated)
        {
            ProjectController.isCreated = false;
            return;
        }
        // loop on features children (each child is a toggle) if the child.name is in the ProjectController.features then set the toggle to on
        foreach (Transform child in features.transform)
        {
            if (ProjectController.features.Contains(child.name))
            {
                child.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                child.GetComponent<Toggle>().isOn = false;
            }
        }
    }
    void LoadModel()
    {
        foreach (Transform child in SelectModelTogglesParent.transform)
        {
            if (child.name == ProjectController.model || ProjectController.model == "")
            {
                child.GetComponent<Toggle>().isOn = true;
            }
            else
            {
                child.GetComponent<Toggle>().isOn = false;
            }
        }
    }
    void LoadTrainingMode()
    {
        if (ProjectController.featureExtractionType == "mediapipe" || ProjectController.featureExtractionType == "")
        {
            mediaPipeToggle.GetComponent<Toggle>().isOn = true;
            featureExtractionTypeNotes.GetComponent<TextMeshProUGUI>().text = mediapipeNote;
        }
        else
        {
            classicalToggle.GetComponent<Toggle>().isOn = true;
            featureExtractionTypeNotes.GetComponent<TextMeshProUGUI>().text = classicalNote;
        }
    }
}

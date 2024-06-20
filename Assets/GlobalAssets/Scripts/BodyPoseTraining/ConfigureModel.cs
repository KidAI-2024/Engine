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

    public GameObject SVMtoggle;
    public GameObject NeuralNetworktoggle;

    private string mediapipeNote = "High performance, Accurate feature extraction, and Real-time processing.";
    private string classicalNote = "Traditional training mode, Time-consuming process.";
    private ProjectController ProjectController;

    void Start()
    {
        SVMtoggle.GetComponent<Toggle>().onValueChanged.AddListener((value) =>  NeuralNetworktoggle.GetComponent<Toggle>().isOn = !value);
        NeuralNetworktoggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => SVMtoggle.GetComponent<Toggle>().isOn = !value);

        mediaPipeToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => MediaPipeToggle(value));
        classicalToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => ClassicalToggle(value));

        ProjectController = ProjectController.Instance;
        LoadFeatures();
        LoadModel();
        LoadTrainingMode();
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
        Debug.Log("Features List: " + string.Join(", ", featuresList));
        return featuresList;
    }
    string GetModel()
    {
        string model = "NeuralNetwork";
        if (SVMtoggle.GetComponent<Toggle>().isOn)
        {
            model = "SVM";
        }
        else
        {
            model = "NeuralNetwork";
        }
        Debug.Log("Model: " + model);
        return model;
    }
    string GetTrainingMode()
    {
        string model = "Classical";
        if (mediaPipeToggle.GetComponent<Toggle>().isOn)
        {
            model = "mediapipe";
        }
        else
        {
            model = "Classical";
        }
        Debug.Log("Training Type: " + model);
        return model;
    }
    void MediaPipeToggle(bool value)
    {
        if (value)
        {
            classicalToggle.GetComponent<Toggle>().isOn = false;
            featureExtractionTypeNotes.GetComponent<TextMeshProUGUI>().text = mediapipeNote;
        }
    }
    void ClassicalToggle(bool value)
    {
        if (value)
        {
            mediaPipeToggle.GetComponent<Toggle>().isOn = false;
            featureExtractionTypeNotes.GetComponent<TextMeshProUGUI>().text = classicalNote;
        }
    }
    void LoadFeatures()
    {
        // loop on features children (each child is a toggle) if the child.name is in the ProjectController.features then set the toggle to on
        foreach (Transform child in features.transform)
        {
            if (ProjectController.features.Contains(child.name))
            {
                child.GetComponent<Toggle>().isOn = true;
            }
        }
    }
    void LoadModel()
    {
        if (ProjectController.model == "SVM")
        {
            SVMtoggle.GetComponent<Toggle>().isOn = true;
        }
        else
        {
            NeuralNetworktoggle.GetComponent<Toggle>().isOn = true;
        }
    }
    void LoadTrainingMode()
    {
        if (ProjectController.featureExtractionType == "mediapipe")
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

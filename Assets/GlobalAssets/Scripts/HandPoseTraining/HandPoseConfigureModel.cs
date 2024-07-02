using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GlobalAssets.HandPoseTraining
{
    public class HandPoseConfigureModel : MonoBehaviour
    {
        public GameObject features;
        //public GameObject models;
        //public GameObject featureExtarction;

        public GameObject mediaPipeToggle;
        //public GameObject classicalToggle;
        public GameObject featureExtractionTypeNotes;

        public GameObject SVMtoggle;
        public GameObject NeuralNetworktoggle;

        private string mediapipeNote = "High performance, Accurate feature extraction, and Real-time processing.";
        //private string classicalNote = "Traditional training mode, Time-consuming process.";
        private ProjectController ProjectController;
        private GameObject mediapipeSkeleton;
        //private GameObject classicalWarning;

        void Start()
        {
            if (features == null)
            {
                Debug.LogError("Features GameObject is not set in the HandPoseConfigureModel script.");
            }
            if (mediaPipeToggle == null)
            {
                Debug.LogError("MediaPipeToggle GameObject is not set in the HandPoseConfigureModel script.");
            }
            
            //if (SVMtoggle != null)
            //{

            //    SVMtoggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => NeuralNetworktoggle.GetComponent<Toggle>().isOn = !value);
            //}
            //if (NeuralNetworktoggle != null)
            //{
            //    NeuralNetworktoggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => SVMtoggle.GetComponent<Toggle>().isOn = !value);
            //}
            if (mediaPipeToggle != null)
            {
                featureExtractionTypeNotes.GetComponent<TextMeshProUGUI>().text = mediapipeNote;

                mediaPipeToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => MediaPipeToggle(value));
            }
            //classicalToggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => ClassicalToggle(value));

            //classicalWarning = features.transform.parent.parent.GetChild(2).gameObject;

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
            ProjectController.featureExtractionType = GetFeatureExtractionMethod();
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
        //public void SetModel(string name)
        //{
        //    // loop on features children (each child is a toggle) and set all to off
        //    foreach (Transform child in models.transform)
        //    {
        //        if (child.name == name)
        //        {
        //            child.GetComponent<Toggle>().isOn = true;
        //        }
        //        else
        //        {
        //            child.GetComponent<Toggle>().isOn = false;
        //        }
        //    }
        //}
        //string GetModel()
        //{
        //    for (int i = 0; i < models.transform.childCount; i++)
        //    {
        //        if (models.transform.GetChild(i).GetComponent<Toggle>().isOn)
        //        {
        //            Debug.Log("Model: " + models.transform.GetChild(i).name);
        //            return models.transform.GetChild(i).name;
        //        }
        //    }
        //    return "";
        //}
        string GetModel()
        {
            string model = "SVM";
            if (SVMtoggle.GetComponent<Toggle>().isOn)
            {
                model = "SVM";
            }
            else
            {
                model = "";
            }
            Debug.Log("Model: " + model);
            return model;
        }
        string GetFeatureExtractionMethod()
        {
            string featureExtraction = "mediapipe";
            if (mediaPipeToggle.GetComponent<Toggle>().isOn)
            {
                featureExtraction = "mediapipe";
                //mediapipeSkeleton.SetActive(true);
                //classicalWarning.SetActive(false);
            }
            else
            {
                //model = "Classical";
                //mediapipeSkeleton.SetActive(false);
                ////classicalWarning.SetActive(true);
            }
            Debug.Log("Feature Extraction: " + featureExtraction);
            return featureExtraction;
        }
        void MediaPipeToggle(bool value)
        {
            if (value)
            {
                //classicalToggle.GetComponent<Toggle>().isOn = false;
                featureExtractionTypeNotes.GetComponent<TextMeshProUGUI>().text = mediapipeNote;
                //mediapipeSkeleton.SetActive(true);
                //classicalWarning.SetActive(false);
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
                    child.GetComponent<ToggleColorChanger>().UpdateColor(true);
                }
                else
                {
                    child.GetComponent<Toggle>().isOn = false; 
                    child.GetComponent<ToggleColorChanger>().UpdateColor(false);
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
                //NeuralNetworktoggle.GetComponent<Toggle>().isOn = true;
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
                //classicalToggle.GetComponent<Toggle>().isOn = true;
                //featureExtractionTypeNotes.GetComponent<TextMeshProUGUI>().text = classicalNote;
            }
        }
    }
}
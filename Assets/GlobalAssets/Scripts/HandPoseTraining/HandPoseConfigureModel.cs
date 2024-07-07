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
        public GameObject models;
        public GameObject featureExtarction;
        public TMP_Text featureExtractionDescription;
        public TMP_Text modelDescription;
        private ProjectController ProjectController;
        [System.Serializable]
        public class ToggleDescription
        {
            public Toggle button;
            public string description;
        }

        public List<ToggleDescription> featureExtractionButtonsWithDescriptions;
        public List<ToggleDescription> modelButtonsWithDescriptions;

        private string selectedModelDescription;
        private string selectedFeatureExtractionDescription;

        void Start()
        {
            if (features == null)
            {
                Debug.LogError("Features GameObject is not set in the HandPoseConfigureModel script.");
            }
         
            if (featureExtractionDescription == null)
            {
                Debug.LogError("FeatureExtractionTypeNotes GameObject is not set in the HandPoseConfigureModel script.");
            }
            ProjectController = ProjectController.Instance;
            LoadFeatures();
            LoadModel();
            LoadFeatureExtractionMode();

            if (models == null)
            {
                Debug.LogError("Models GameObject is not set in the HandPoseConfigureModel script.");
            }
            else
            {
                AddChildrenTogglesListeners(models);
            }
            if (featureExtarction == null)
            {
                Debug.LogError("FeatureExtarction GameObject is not set in the HandPoseConfigureModel script.");
            }
            else
            {
                AddChildrenTogglesListeners(featureExtarction);
            }
            
           

            
        }
        void AddChildrenTogglesListeners(GameObject parentObj)
        {
            foreach (Transform child in parentObj.transform)
            {
                Toggle toggle = child.GetComponent<Toggle>();

                if (toggle != null)
                {
                    toggle.onValueChanged.RemoveAllListeners(); // Remove any existing listeners
                    toggle.onValueChanged.AddListener((value) => { 
                        OnToggleClicked(toggle, parentObj);
                        //set description to the selected model or feature extraction
                        if (parentObj == models)
                        {
                            foreach (ToggleDescription buttonDescription in modelButtonsWithDescriptions)
                            {
                                if (buttonDescription.button.name == toggle.name)
                                {
                                    selectedModelDescription = buttonDescription.description;
                                    modelDescription.text = selectedModelDescription;
                                    break;
                                }
                            }
                        }
                        else if (parentObj == featureExtarction)
                        {
                            foreach (ToggleDescription buttonDescription in featureExtractionButtonsWithDescriptions)
                            {
                                if (buttonDescription.button.name == toggle.name)
                                {
                                    selectedFeatureExtractionDescription = buttonDescription.description;
                                    featureExtractionDescription.text = selectedFeatureExtractionDescription;
                                    break;
                                }
                            }
                        }

                    });
                }
            }
        }

        void OnToggleClicked(Toggle clickedToggle, GameObject parentObj)
        {
            if (!clickedToggle.isOn)
            {
                // Prevent unchecking the last toggle
                int checkedCount = 0;
                foreach (Transform child in parentObj.transform)
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
            foreach (Transform child in parentObj.transform)
            {
                Toggle toggle = child.GetComponent<Toggle>();
                if (toggle != clickedToggle)
                {
                    toggle.isOn = false;
                }
            }
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
       
        string GetModel()
        {
            string model = "";
            //loop on all models, get the one with on toggle
            foreach (Transform child in models.transform)
            {
                if (child.GetComponent<Toggle>().isOn)
                {
                    model = child.name;
                    break;
                }
            }
            return model;
        }
        string GetFeatureExtractionMethod()
        {
            string featureExtraction = "";
            foreach (Transform child in featureExtarction.transform)
            {
                if (child.GetComponent<Toggle>().isOn)
                {
                    featureExtraction = child.name;
                    break;
                }
            }
            Debug.Log("Feature Extraction: " + featureExtraction);
            return featureExtraction;
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
            Debug.Log("Loading Model: " + ProjectController.model);
            for (int i = 0; i < models.transform.childCount; i++)
            {
                if (models.transform.GetChild(i).name == ProjectController.model)
                {
                    models.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                }
                else
                {
                    models.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                }
            }
            //set description to the selected model
            //loop on modelButtonsWithDescriptions list, if the button name is the same as the selected model then set the description to the button description
            foreach (ToggleDescription buttonDescription in modelButtonsWithDescriptions)
            {
                if (buttonDescription.button.name == ProjectController.model)
                {
                    selectedModelDescription = buttonDescription.description;
                    modelDescription.text = selectedModelDescription;
                    break;
                }
            }
            //adjust the scrollbar to the selected model


        }
        void LoadFeatureExtractionMode()
        {
            Debug.Log("Loading Feature Extraction: " + ProjectController.featureExtractionType);
            for (int i = 0; i < featureExtarction.transform.childCount; i++)
            {
                if (featureExtarction.transform.GetChild(i).name == ProjectController.featureExtractionType)
                {
                    featureExtarction.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                }
                else
                {
                    featureExtarction.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                }
            }
            //set description to the selected feature extraction
            //loop on featureExtractionButtonsWithDescriptions list, if the button name is the same as the selected feature extraction then set the description to the button description
            foreach (ToggleDescription buttonDescription in featureExtractionButtonsWithDescriptions)
            {
                if (buttonDescription.button.name == ProjectController.featureExtractionType)
                {
                    selectedFeatureExtractionDescription = buttonDescription.description;
                    featureExtractionDescription.text = selectedFeatureExtractionDescription;
                    break;
                }
            }

        }


    }
}
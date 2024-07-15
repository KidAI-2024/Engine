using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlobalAssets.HandPoseTraining
{
    public class InitTrainingFeatures : MonoBehaviour
    {
        public GameObject features;
        private ProjectController ProjectController;
        void Awake()
        {
            ProjectController = ProjectController.Instance;
            if (ProjectController.isCreated)
            {
                ProjectController.features = GetAllFeaturesList();
                ProjectController.featureExtractionType = "mediapipe";
                ProjectController.model = "SVM";
                ProjectController.isCreated = false;
                ProjectController.Save();
            }
        }

        List<string> GetAllFeaturesList()
        {
            List<string> featuresList = new List<string>();
            // loop on features children (each child is a toggle) if on then add the child.name to the list
            foreach (Transform child in features.transform)
            {
                    featuresList.Add(child.name);
            }
            Debug.Log("Features List: " + string.Join(", ", featuresList));
            return featuresList;
        }
    }
}

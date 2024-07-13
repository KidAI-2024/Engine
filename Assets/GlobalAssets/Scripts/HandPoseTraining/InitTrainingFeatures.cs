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
        // Start is called before the first frame update
        void Start()
        {
            ProjectController = ProjectController.Instance;
            if (ProjectController.features.Count == 0)
            {
                ProjectController.features = GetFeaturesList();
            }
            if (ProjectController.featureExtractionType == "")
            {
                ProjectController.featureExtractionType = "mediapipe";
            }
            if (ProjectController.model == "")
            {
                ProjectController.model = "SVM";
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
            Debug.Log("Features List: " + string.Join(", ", featuresList));
            return featuresList;
        }
    }
}

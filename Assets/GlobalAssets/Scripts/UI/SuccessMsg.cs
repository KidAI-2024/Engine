using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GlobalAssets.UI
{
    public class SuccessMsg : MonoBehaviour
    {
        // public GameObject successPanel;
        public Text successText;
        public Text errorText;
        void Start()
        {
            successText.text = "";
        }
        // public GameObject successIcon;
        public void ShowSaveSuccessMessageEpoch()
        {
            if (errorText.text == "")
            {
                successText.text = "No.Epochs Saved Successfully!!!";
                // successPanel.SetActive(true);
                // successIcon.SetActive(true);
            }
            else
            {
                successText.text = "";
            }
        }
        // success message for saving learning rate
        public void ShowSaveSuccessMessageLR()
        {
            if (errorText.text == "")
            {
                successText.text = "Learning Rate Saved Successfully!!!";
            }
            else
            {
                successText.text = "";
            }
        }
        // success message for model category
        public void ShowSaveSuccessMessageModelCategory()
        {
            if (errorText.text == "")
            {
                successText.text = "Model Category Saved Successfully!!!";
            }
            else
            {
                successText.text = "";
            }
        }
        // success meesage for Feature extraction method
        public void ShowSaveSuccessMessageFeatureExtractionMethod()
        {
            if (errorText.text == "")
            {
                successText.text = "Feature Extraction Method Saved Successfully!!!";
            }
            else
            {
                successText.text = "";
            }
        }
        // success message for classical model type
        public void ShowSaveSuccessMessageClassicalModelType()
        {
            if (errorText.text == "")
            {
                successText.text = "Classical Model Type Saved Successfully!!!";
            }
            else
            {
                successText.text = "";
            }
        }
    }
}
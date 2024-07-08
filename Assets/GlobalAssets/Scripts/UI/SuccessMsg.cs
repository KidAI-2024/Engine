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
        // public GameObject successIcon;
        public void ShowSaveSuccessMessage()
        {
            if (errorText.text == "")
            {
                successText.text = "Data Saved Successfully!!!";
                // successPanel.SetActive(true);
                // successIcon.SetActive(true);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalAssets.UI
{
    public class AddNewClass : MonoBehaviour
    {
        public GameObject ClassPrefab; 
        public GameObject ClassesContainer;
        public GameObject CameraPanel;
        // add listener to the button
        public void Start()
        {
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(InstantiateNewClass);
        }
        public void InstantiateNewClass()
        {
            GameObject newClass = Instantiate(ClassPrefab, ClassesContainer.transform);
            RectTransform lastClassRect = ClassesContainer.transform.GetChild(ClassesContainer.transform.childCount - 2).GetComponent<RectTransform>();
            RectTransform newClassRect = newClass.GetComponent<RectTransform>();
            
            // Position the new class below the last class
            newClassRect.anchorMin = new Vector2(0.5f, 1);
            newClassRect.anchorMax = new Vector2(0.5f, 1);
            newClassRect.pivot = new Vector2(0.5f, 1);
            newClassRect.sizeDelta = new Vector2(98,196);
            newClassRect.localPosition = new Vector3(lastClassRect.localPosition.x, lastClassRect.localPosition.y - 155, lastClassRect.localPosition.z);
            

            // increase the size of the ClassesContainer
            RectTransform contRect = ClassesContainer.transform.GetComponent<RectTransform>();
            contRect.sizeDelta = new Vector2(contRect.sizeDelta.x,contRect.sizeDelta.y + 155); 

            // Add necessary objects to scripts
            GameObject classBox = newClass.transform.GetChild(0).gameObject;
            Popup popup = classBox.GetComponent<Popup>();
            if (popup != null)
            {
                popup.dialog = CameraPanel;
            }
            else
            {
                Debug.LogError("Popup component is missing on the classBox.");
            }


            // Add Listener to the OpenCameraButton on click to call OpenCamera(outsideImagesContainer) that is in WebcamController of the CameraPanel
            UnityEngine.UI.Button btn = classBox.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Button>();
            GameObject outsideImagesContainer = classBox.transform.GetChild(4).GetChild(0).GetChild(0).gameObject;
            btn.onClick.AddListener(() => CameraPanel.GetComponent<WebcamController>().OpenCamera(outsideImagesContainer));

            // auto scroll to the end of the ClassesContainer.parent.parent that is a ScrollRect
            ClassesContainer.transform.parent.parent.GetComponent<UnityEngine.UI.ScrollRect>().verticalNormalizedPosition = 0;
            
        }
    }
}

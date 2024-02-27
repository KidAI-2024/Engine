using UnityEngine;
namespace GlobalAssets.UI
{
    public class Popup : MonoBehaviour
    {
        public GameObject dialog; // Reference to your dialog GameObject

        public Canvas captureImageCanvas; // Reference to your camera panel GameObject

        void Start()
        {
            // Disable the dialog initially
            dialog.SetActive(false);
        }

        public void OpenDialog()
        {
            // Enable the dialog when the button is clicked
            dialog.SetActive(true);

            if(captureImageCanvas != null)
            {
                // get the script component of the canvas called AlwaysOnTopUI and manipulate the property sortingOrder make it 11
                captureImageCanvas.GetComponent<AlwaysOnTopUI>().MakeOnTop();
            }
        }

        public void CloseDialog()
        {
            // Disable the dialog when the button is clicked
            dialog.SetActive(false);

            if(captureImageCanvas != null)
            {
                // get the script component of the canvas called AlwaysOnTopUI and manipulate the property sortingOrder make it 11
                captureImageCanvas.GetComponent<AlwaysOnTopUI>().ResetOrder();
            }
        }   
    }
}

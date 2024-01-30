using UnityEngine;

public class Popup : MonoBehaviour
{
    public GameObject dialog; // Reference to your dialog GameObject

    void Start()
    {
        // Disable the dialog initially
        dialog.SetActive(false);
    }

    public void OpenDialog()
    {
        // Enable the dialog when the button is clicked
        dialog.SetActive(true);
    }

    public void CloseDialog()
    {
        // Disable the dialog when the button is clicked
        dialog.SetActive(false);
    }   
}

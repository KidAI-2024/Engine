using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GlobalAssets.UI
{
    public class Popup : MonoBehaviour
    {
        public GameObject dialog; // Reference to your dialog GameObject
        public bool IscaptureCameraPanel = false;
        public Canvas captureImageCanvas; // Reference to your camera panel GameObject
        public bool isAudio = false;
        void Start()
        {
            // Disable the dialog initially
            dialog.SetActive(false);
        }

        public void OpenDialog()
        {
            // Enable the dialog when the button is clicked
            dialog.SetActive(true);


            // this if condition is only for openining the camera panel of the training scene
            // to choose list of images of same class
            if (captureImageCanvas != null&&!isAudio)
            {
                // get the last child of this.gameObject and get the first child of that child and get the first child of that child and get the first child of that child
                GameObject content = this.gameObject.transform.GetChild(this.gameObject.transform.childCount - 1).GetChild(0).GetChild(0).gameObject;

                // capturedImages = the raw images children of the content object
                List<Texture2D> capturedImages = new List<Texture2D>();
                for (int i = 0; i < content.transform.childCount; i++)
                {
                    capturedImages.Add(content.transform.GetChild(i).GetComponent<RawImage>().texture as Texture2D);
                }
                    dialog.GetComponent<WebcamController>().capturedImages = new List<Texture2D>(capturedImages);
                    dialog.GetComponent<WebcamController>().InstantiateCapturedImages();     
            }
            else if (isAudio && captureImageCanvas != null)
            {
                Debug.Log("IN here");
                GameObject content = this.gameObject.transform.GetChild(this.gameObject.transform.childCount - 1).GetChild(0).GetChild(0).gameObject;
                Debug.Log("Object name: " + gameObject.name);
                // Create a new list to hold the captured audio clips
                List<AudioClip> capturedAudios = new List<AudioClip>();

                // Loop through each child of the content object
                for (int i = 0; i < content.transform.childCount; i++)
                {
                    Debug.Log("In loopppppp");
                    Transform child = content.transform.GetChild(i);

                    // Log the name of the child GameObject
                    Debug.Log("Child GameObject name: " + child.gameObject.name);
                    // Attempt to get the AudioSource component from each child, if it exists
                    AudioSource audioSource = content.transform.GetChild(i).GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        Debug.Log("Not Null");
                        capturedAudios.Add(audioSource.clip);
                    }
                }
                Debug.Log("Number of captured audios: " + capturedAudios.Count);


                // Set the capturedAudios list of the MicController component
                dialog.GetComponent<MicController>().capturedAudios = new List<AudioClip>(capturedAudios);

                // Instantiate the captured audios in the UI
                dialog.GetComponent<MicController>().InstantiateCapturedAudios();
            }

        }

        public void CloseDialog()
        {
            // Disable the dialog when the button is clicked
            dialog.SetActive(false);

            if(IscaptureCameraPanel)
            {
                // Clear all images inside the content object (6th child of the dialog object)
                Transform content = dialog.transform.GetChild(5).GetChild(0).GetChild(0);
                for (int i = 0; i < content.childCount; i++)
                {
                    Destroy(content.GetChild(i).gameObject);
                }
            }
        }   
    }
}

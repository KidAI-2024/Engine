/*
Purpose of the Script
The Door script handles the interaction with a door in the game. It manages UI elements to indicate when the player can interact with the door, 
checks if the player meets the conditions to proceed (having correctly classified a certain number of images), 
triggers animations, and transitions to the next scene. 
The script ensures a smooth and interactive experience for the player when interacting with doors.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Imports the SceneManager for scene management

namespace Survival // Defines the namespace "Survival" to organize the code
{
    public class Door : MonoBehaviour // Defines a public class named "Door" that inherits from MonoBehaviour
    {
        // Public variables accessible in the Unity Editor
        public GameObject handImg; // UI element indicating interaction is possible
        public GameObject noKeyText; // UI text to display interaction information
        public GameObject exitText; // UI text to display exit information
        public GameObject key; // UI element for the inventory key
        public GameObject fadeEffectFx; // UI element for fade effect
        public string nextScene; // Name of the next scene to load
        // Private variables
        private bool canReach; // Boolean to check if the player is within reach of the door

        void Start()
        {
            // Initialize all UI elements to inactive
            handImg.SetActive(false);
            noKeyText.SetActive(false);
            key.SetActive(false);
            fadeEffectFx.SetActive(false);
            exitText.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            // Check if the colliding object has the tag "Reach"
            if (other.gameObject.tag == "Reach")
            {
                canReach = true; // Player is within reach
                handImg.SetActive(true); // Activate the hand UI
                exitText.SetActive(true); // Show the exit text
            }
        }

        void OnTriggerExit(Collider other)
        {
            // Check if the exiting object has the tag "Reach"
            if (other.gameObject.tag == "Reach")
            {
                canReach = false; // Player is no longer within reach
                handImg.SetActive(false); // Deactivate the hand UI
                noKeyText.SetActive(false); // Deactivate the interaction text
                exitText.SetActive(false); // Deactivate the exit text
            }
        }
        // with key
        // if (canReach && Input.GetButtonDown("Interact") && !key.activeInHierarchy)
        // if (canReach && Input.GetButtonDown("Interact") && key.activeInHierarchy)

        void Update()
        {
            // Check if the player is within reach and presses the "Interact" button and has less than 5 correctly classified images
            if (canReach && Input.GetButtonDown("Interact") && PlayerController.Instance.numOfCorrectClassifiedImgs < 5)
            {
                handImg.SetActive(true); // Activate the hand UI
                noKeyText.SetActive(true); // Show the interaction text
            }

            // Check if the player is within reach and presses the "Interact" button and has 5 or more correctly classified images
            if (canReach && Input.GetButtonDown("Interact") && PlayerController.Instance.numOfCorrectClassifiedImgs >= 5)
            {
                handImg.SetActive(false); // Deactivate the hand UI
                noKeyText.SetActive(false); // Deactivate the interaction text
                fadeEffectFx.SetActive(true); // Activate the fade effect

                // Get the Animator component from the GameObject
                Animator animator = this.gameObject.GetComponent<Animator>();
                if (animator != null)
                {
                    // Set the "StartAnimation" trigger in the Animator
                    animator.SetTrigger("StartAnimation");
                    Debug.Log("Animator is activated");
                }
                else
                {
                    Debug.LogError("Animator component not found on the target GameObject.");
                }
                StartCoroutine(ending()); // Start the coroutine to end the game
            }
        }

        IEnumerator ending()
        {
            yield return new WaitForSeconds(1.0f); // Wait for 1 second
            SceneManager.LoadScene(nextScene); // Load the specified next scene
        }
    }
}

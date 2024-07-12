using UnityEngine;
using UnityEngine.SceneManagement;

namespace Survival
{
    public class KillPlayer : MonoBehaviour
    {
        // Public variables
        public string nextSceneName; // Name of the next scene to load after the player is "killed".
        public float delay = 0.5f; // Delay in seconds before loading the next scene to allow for any fadeout or other effects.
        public GameObject fadeout; // Reference to a fadeout GameObject to trigger a fadeout effect.

        // Private variables
        private bool playerInsideTrigger = false; // Flag to check if the player is inside the trigger zone.

        // Method called when another collider enters the trigger collider attached to the GameObject.
        private void OnTriggerEnter(Collider other)
        {
            // Check if the collider that entered the trigger has the tag "Player".
            if (other.CompareTag("Player"))
            {
                // Set the flag to true indicating the player is inside the trigger.
                playerInsideTrigger = true;

                // Activate the fadeout effect.
                fadeout.SetActive(true);

                // Invoke the LoadNextScene method after the specified delay.
                Invoke("LoadNextScene", delay);
            }
        }

        // Method to load the next scene.
        private void LoadNextScene()
        {
            // Check if the player is still inside the trigger before loading the next scene.
            if (playerInsideTrigger)
            {
                // Load the scene with the specified name.
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}

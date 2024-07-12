/*
This script effectively manages background music, 
ensuring it persists across scenes and providing simple control to play and stop the music.
*/
using UnityEngine;

namespace Survival
{
    // This class controls the background music in the game
    public class MusicControl : MonoBehaviour
    {
        // Private variables
        private AudioSource audioSource; // Reference to the AudioSource component

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // Ensure this GameObject persists across different scenes
            DontDestroyOnLoad(transform.gameObject);
            // Get the AudioSource component attached to the GameObject
            audioSource = GetComponent<AudioSource>();
        }

        // Method to play the music if it's not already playing
        public void PlayMusic()
        {
            // Check if the audio is already playing to avoid overlapping
            if (audioSource.isPlaying) return;
            // Play the audio
            audioSource.Play();
        }

        // Method to stop the music
        public void StopMusic()
        {
            // Stop the audio
            audioSource.Stop();
        }
    }
}

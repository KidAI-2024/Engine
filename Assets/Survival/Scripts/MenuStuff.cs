/*
Purpose of the Script
The MenuStuff script is responsible for handling various menu actions, such as loading scenes and quitting the game. 
It ensures that the appropriate music control actions are taken when switching scenes and prevents duplication of the PlayerController instance between scenes.
*/
//This script is attached to "Canvas: in MainMenuScene
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Imports the SceneManager for scene management

namespace Survival // Defines the namespace "Survival" to organize the code
{
    public class MenuStuff : MonoBehaviour // Defines a public class named "MenuStuff" that inherits from MonoBehaviour
    {
        // Public variables accessible in the Unity Editor
        public string nextSceneName; // Name of the next scene to load

        void Start()
        {
            // Finds the GameObject with the tag "music" and gets its MusicControl component to play music
            GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().PlayMusic();
        }

        public void B_LoadScene()
        {
            // Checks if the next scene to load is "SampleScene"
            if (nextSceneName == "SampleScene")
            {
                try
                {
                    // Attempts to stop the music if the tag "music" GameObject has a MusicControl component
                    GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().StopMusic();
                }
                catch (System.Exception e)
                {
                    // Logs any exception that occurs during the process
                    Debug.Log(e);
                }
            }
            // Loads the specified next scene
            SceneManager.LoadScene(nextSceneName);
            // Destroys the PlayerController instance to prevent duplication between scenes
            Destroy(PlayerController.Instance.gameObject);
        }

        public void LoadScene(string sceneName)
        {
            // Checks if the specified scene name is "SampleScene"
            if (sceneName == "SampleScene")
            {
                try
                {
                    // Attempts to stop the music if the tag "music" GameObject has a MusicControl component
                    GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().StopMusic();
                }
                catch (System.Exception e)
                {
                    // Logs any exception that occurs during the process
                    Debug.Log(e);
                }
            }
            // Loads the specified scene
            SceneManager.LoadScene(sceneName);
            // Destroys the PlayerController instance to prevent duplication between scenes
            Destroy(PlayerController.Instance.gameObject);
        }

        public void B_QuitGame()
        {
            try
            {
                // Attempts to stop the music if the tag "music" GameObject has a MusicControl component
                GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().StopMusic();
            }
            catch (System.Exception e)
            {
                // Logs any exception that occurs during the process
                Debug.Log(e);
            }
            // Loads the "Lobby" scene, which is likely the main menu or exit scene
            SceneManager.LoadScene("Lobby");
            // Destroys the PlayerController instance to prevent duplication between scenes
            Destroy(PlayerController.Instance.gameObject);
        }
    }
}

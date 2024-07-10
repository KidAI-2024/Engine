using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Survival
{
    public class MenuStuff : MonoBehaviour
    {
        public string nextSceneName; // Name of the next scene to load
        void Start()
        {
            GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().PlayMusic();
        }
        public void B_LoadScene()
        {
            if (nextSceneName == "SampleScene")
            {
                try
                {
                    GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().StopMusic();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
            }
            SceneManager.LoadScene(nextSceneName);
            Destroy(PlayerController.Instance.gameObject);

            // Get the current active scene
            // Scene currentScene = SceneManager.GetActiveScene();
            // // Destroy the current scene
            // SceneManager.UnloadSceneAsync(currentScene);
            // StartCoroutine(LoadSceneAndUnloadCurrent(nextSceneName));

        }

        public void LoadScene(string sceneName)
        {
            if (sceneName == "SampleScene")
            {
                try
                {
                    GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().StopMusic();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
            }
            // Get the current active scene
            // Scene currentScene = SceneManager.GetActiveScene();
            // // Destroy the current scene
            // SceneManager.UnloadSceneAsync(currentScene);
            SceneManager.LoadScene(sceneName);
            Destroy(PlayerController.Instance.gameObject);

            // StartCoroutine(LoadSceneAndUnloadCurrent(sceneName));

        }
        private IEnumerator LoadSceneAndUnloadCurrent(string sceneName)
        {
            // Get the current active scene
            Scene currentScene = SceneManager.GetActiveScene();

            // Load the new scene asynchronously
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // Wait until the new scene is fully loaded
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Set the newly loaded scene as the active scene
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(newScene);

            // Unload the previous scene asynchronously
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentScene);

            // Wait until the previous scene is fully unloaded
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
        }
        public void B_QuitGame()
        {
            // Application.Quit();
            try
            {
                GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().StopMusic();
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
            SceneManager.LoadScene("Lobby");
            Destroy(PlayerController.Instance.gameObject);
            // // Get the current active scene
            // Scene currentScene = SceneManager.GetActiveScene();
            // // Destroy the current scene
            // SceneManager.UnloadSceneAsync(currentScene);
            // StartCoroutine(LoadSceneAndUnloadCurrent("Lobby"));

        }
    }
}
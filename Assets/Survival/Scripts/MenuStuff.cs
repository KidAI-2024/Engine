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
            SceneManager.LoadScene(nextSceneName);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);

        }
        public void B_QuitGame()
        {
            // Application.Quit();
            SceneManager.LoadScene("Lobby");

        }
    }
}
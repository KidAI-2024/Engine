using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Karting.UI
{

    public class ChooseMode : MonoBehaviour
    {
        public Button TimedButton;
        public String nextSceneName = "KartingChooseTrack";
        void Start()
        {
            Game.GameManager gameManager = FindObjectOfType<Game.GameManager>();
            TimedButton.onClick.AddListener(() => gameManager.SelectGameMode_Timed());
            TimedButton.onClick.AddListener(() => SceneManager.LoadScene(nextSceneName));
        }
    }
}

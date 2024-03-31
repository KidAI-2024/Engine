using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Karting.UI
{

    public class ChooseTrack : MonoBehaviour
    {
        public Button forestButton;
        public String nextSceneName = "KartingChooseCar";
        void Start()
        {
            Game.GameManager gameManager = FindObjectOfType<Game.GameManager>();
            forestButton.onClick.AddListener(() => gameManager.SelectRaceTrack_Forest());
            forestButton.onClick.AddListener(() => SceneManager.LoadScene(nextSceneName));
        }
    }
}

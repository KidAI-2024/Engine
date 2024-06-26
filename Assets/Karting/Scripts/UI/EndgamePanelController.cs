using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Karting.UI
{

    public class EndgamePanelController : MonoBehaviour
    {
        public GameObject endgamePanel;
        public TMPro.TMP_Text minutesText;
        public TMPro.TMP_Text secondsText;
        public TMPro.TMP_Text millisecondsText;
        public GameObject gamePlayManagerObj;
        Karting.Game.GameplayManager gamePlayManager;
        public Button exitButton;

        // Start is called before the first frame update
        void Start()
        {
            endgamePanel.SetActive(false);
            gamePlayManager = gamePlayManagerObj.GetComponent<Karting.Game.GameplayManager>();
            if (exitButton != null)
            {
                // change scene to kartingintro
                exitButton.onClick.AddListener(() => SceneManager.LoadScene("KartingIntro"));
            }
            else
            {
                Debug.Log("Exit Button is null in EndgamePanelController");
            }
        }
        void Update()
        {
            if (gamePlayManager.win)
            {
                Debug.Log("Game Over");
                // Set the time text
                int minutes = Mathf.FloorToInt(gamePlayManager.time / 60);
                int seconds = Mathf.FloorToInt(gamePlayManager.time % 60);
                int milliseconds = Mathf.FloorToInt((gamePlayManager.time * 1000) % 1000);

                minutesText.text = minutes.ToString("00");
                secondsText.text = seconds.ToString("00");
                millisecondsText.text = (milliseconds / 100).ToString(); // Display single digit milliseconds
                                                                         // Show the endgame panel
                endgamePanel.SetActive(true);
            }
            else
            {
                endgamePanel.SetActive(false);
            }
        }
    }

}
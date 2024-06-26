using UnityEngine;
using TMPro;
namespace Karting.UI
{
    public class Timer : MonoBehaviour
    {
        public TMP_Text minutesText;
        public TMP_Text secondsText;
        public TMP_Text millisecondsText;

        private float startTime;
        private bool timerRunning;
        public GameObject gamePlayManagerObj;
        Karting.Game.GameplayManager gamePlayManager;

        void Start()
        {
            timerRunning = false;
            StartStopwatch();
            gamePlayManager = gamePlayManagerObj.GetComponent<Karting.Game.GameplayManager>();
        }

        void Update()
        {
            if (timerRunning)
            {
                float elapsedTime = Time.time - startTime;
                UpdateTimer(elapsedTime);
                gamePlayManager.time = elapsedTime;
                // Debug.Log("Time: " + gamePlayManager.time);
                if (gamePlayManager.win)
                {
                    StopStopwatch();
                }
            }
        }

        public void StartStopwatch()
        {
            startTime = Time.time; // Start time from current time
            timerRunning = true;
        }

        public void StopStopwatch()
        {
            timerRunning = false;
        }

        public void ResetStopwatch()
        {
            startTime = Time.time; // Reset start time to current time
            UpdateTimer(0); // Update timer text to show 00:00:0
        }

        void UpdateTimer(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);

            minutesText.text = minutes.ToString("00");
            secondsText.text = seconds.ToString("00");
            millisecondsText.text = (milliseconds / 100).ToString(); // Display single digit milliseconds
        }
    }
}

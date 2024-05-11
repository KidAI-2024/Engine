using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace MortalKombat
{
    public class UI : MonoBehaviour
    {
        // get game manager static instance
        public static GameManager gameManager;

        private static bool isInputEnabled = false;
        public bool EnableTimer;

        public Slider Player1HealthSlider;
        public Slider Player2HealthSlider; 
        public GameObject Player1Score;
        public GameObject Player2Score;
        Player1Controller player1;
        Player1Controller player2;

        public TextMeshProUGUI countdownText;
        private int seconds = 90;

        public TextMeshProUGUI preGameCountdownText;
        private int preGameSeconds = 5;
        
        public GameObject GameOverText;
        public GameObject RoundOverText;
        
        public AudioSource audioSource;
        public AudioClip roundStartSound;
        public AudioClip round1Sound;
        public AudioClip round2Sound;
        public AudioClip round3Sound;
        
        // Start with Round 1
        public static int Round = 1;
        public static int RoundScore = 0; // Round score is calculated +1 for each round win for player 1 and -1 for each round win for player 2

        private bool flag;
        // Property to get the input status
        public static bool IsInputEnabled
        {
            get { return isInputEnabled; }
        }

        void Start()
        {
            gameManager = GameManager.Instance;
            gameManager.InstantiateCharacters();
            // Load the Round value from Player Preferences
            Round = gameManager.Round;
            RoundScore = gameManager.RoundScore;
            PlayerPrefs.DeleteAll();
            flag = true;

            player1 = GameObject.Find("Player1").GetComponent<Player1Controller>();
            player2 = GameObject.Find("Player2").GetComponent<Player1Controller>();
            Player1HealthSlider.maxValue = player1.health;
            Player2HealthSlider.maxValue = player2.health;
            RoundOverText.gameObject.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                Player1Score.transform.GetChild(i).gameObject.SetActive(false);
                Player2Score.transform.GetChild(i).gameObject.SetActive(false);
            }
            if (EnableTimer)
            {
                isInputEnabled = false;

                countdownText.gameObject.SetActive(false);
                // Start the pre-game countdown
                InvokeRepeating("UpdatePreGameCountdown", 1f, 1f);
            }
            else{
                
                preGameCountdownText.gameObject.SetActive(false);
                isInputEnabled = true;
            }
            PlayRoundSound(Round);
        }
        void Update()
        {
            Player1HealthSlider.value = player1.health;
            Player2HealthSlider.value = player2.health;
            // Round over
            if ((player1.health <= 0 || player2.health <= 0) && flag)
            {
                flag = false;
                isInputEnabled = false;
                countdownText.gameObject.SetActive(false);
                preGameCountdownText.gameObject.SetActive(false);
                
                // Increment the Round variable
                Round++;
                RoundScore = player1.health <= 0 ? RoundScore - 1 : RoundScore + 1;
                UpdateRoundScoreUI();
                if (Round > 3 || RoundScore > 1 ||  RoundScore < -1) // 3 rounds finished = game over
                {
                    PlayerPrefs.DeleteAll();
                    GameOverText.GetComponentInChildren<TextMeshProUGUI>(true).text = RoundScore > 0 ? "Player 1 Wins!" : "Player 2 Wins!"; // +ve means player 1 wins, -ve means player 2 wins
                    GameOverText.SetActive(true);
                }
                else{
                    // Save the Round value to Player Preferences
                    gameManager.Round = Round;
                    gameManager.RoundScore = RoundScore;
                    RoundOverText.GetComponentInChildren<TextMeshProUGUI>(true).text = player1.health <= 0 ? "Player 2 Wins!" : "Player 1 Wins!";
                    RoundOverText.SetActive(true);
                    Invoke("RestartGame", 8);   
                }
            }
        }
        void UpdatePreGameCountdown()
        {
            preGameSeconds--;

            if (preGameSeconds < 0)
            {
                preGameSeconds = 0;
                // Start the main game countdown when the pre-game countdown reaches zero
                StartMainCountdown();
            }

            UpdatePreGameCountdownDisplay();
        }

        void UpdatePreGameCountdownDisplay()
        { 
            if(preGameSeconds > 3){
                preGameCountdownText.color = Color.red;
                preGameCountdownText.text = "Round " + Round ;
            }
            // else{
            //     // color = #FA5801
            //     preGameCountdownText.color = new Color(0.9803922f, 0.345098f, 0.003921569f);
            //     preGameCountdownText.text = preGameSeconds.ToString();
            // }

            // Disable the pre-game countdown text when the countdown is complete
            if (preGameSeconds == 3)
            {
                preGameCountdownText.color = Color.red;
                preGameCountdownText.text = "Fight! ";
                // disable pregame countdown text after 1 second
                Invoke("DisablePreGameCountdownText", 1);
                // countdownText.gameObject.SetActive(true);
                isInputEnabled = true;
            }
        }
        void DisablePreGameCountdownText()
        {
            preGameCountdownText.gameObject.SetActive(false);
        }
        void StartMainCountdown()
        {
            // Cancel the pre-game countdown
            CancelInvoke("UpdatePreGameCountdown");

            // Start the main game countdown
            InvokeRepeating("UpdateTimer", 1f, 1f);
        }

        void UpdateTimer()
        {
            seconds--;

            if (seconds < 0)
            {
                seconds = 0;
                // You can add additional logic here when the main countdown reaches zero
            }
            countdownText.text = seconds.ToString();
        }

        void RestartGame()
        {
            // Reload the scene to restart the game with a new round
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        void UpdateRoundScoreUI()
        {
            if (RoundScore > 0)
            {
                Player1Score.transform.GetChild(RoundScore - 1).gameObject.SetActive(true);
            }
            if (RoundScore < 0)
            {
                Player2Score.transform.GetChild(-RoundScore -1).gameObject.SetActive(true);
            }
        }
        void PlayRoundSound(int Round)
        {
            audioSource.PlayOneShot(roundStartSound);
            switch (Round)
            {
                case 1:
                    audioSource.PlayOneShot(round1Sound);
                    break;
                case 2:
                    audioSource.PlayOneShot(round2Sound);
                    break;
                case 3:
                    audioSource.PlayOneShot(round3Sound);
                    break;
            }
        }
    }
}
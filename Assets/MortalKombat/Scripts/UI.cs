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

        public TextMeshProUGUI countdownTillBacktoChooseCharacterText;
        public GameObject predictionPanel;
        public GameObject settingsPanel;
        public Slider Player1HealthSlider;
        public Slider Player2HealthSlider; 
        public GameObject Player1Score;
        public GameObject Player2Score;
        public GameObject player1text;
        public GameObject player2text;

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
        public int backAfter = 5;
        
        // Start with Round 1
        public static int Round = 1;
        public static int RoundScore = 0; // Round score is calculated +1 for each round win for player 1 and -1 for each round win for player 2
        public static int Player1ScoreValue = 0;
        public static int Player2ScoreValue = 0;
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
            player1text.GetComponent<TextMeshProUGUI>().text = gameManager.player1Name;
            player2text.GetComponent<TextMeshProUGUI>().text = gameManager.player2Name;
            // Debug.Log("Player 1: " + gameManager.player1Name + " Player 2: " + gameManager.player2Name);
            // Load the Round value from Player Preferences
            Round = gameManager.Round;
            RoundScore = gameManager.RoundScore;
            Player1ScoreValue = gameManager.Player1ScoreValue;
            Player2ScoreValue = gameManager.Player2ScoreValue;
            
            Debug.Log("Round: " + Round + " RoundScore: " + RoundScore);
            flag = true;

            player1 = GameObject.Find("Player1").GetComponent<Player1Controller>();
            player2 = GameObject.Find("Player2").GetComponent<Player1Controller>();
            Player1HealthSlider.maxValue = player1.health;
            Player2HealthSlider.maxValue = player2.health;


            RoundOverText.gameObject.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                // if is enables then bring the animation to its end else disable it
                string animeName = "";
                if(i < Player1ScoreValue || i < Player2ScoreValue)
                {
                    if (i < Player1ScoreValue)
                    {
                        Player1Score.transform.GetChild(i).gameObject.SetActive(true);
                        // skip the default animation and go to the end
                        animeName = i > 0 ? "star_intro" + (i+1) : "star_intro";
                        Animator animator = Player1Score.transform.GetChild(i).GetComponent<Animator>();
                        animator.Play(animeName, 0, 1);
                    }
                    if (i < Player2ScoreValue)
                    {
                        Player2Score.transform.GetChild(i).gameObject.SetActive(true);
                        // skip the default animation and go to the end
                        animeName = "star_intro" + (i+4);
                        Debug.Log(animeName);
                        Animator animator = Player2Score.transform.GetChild(i).GetComponent<Animator>();
                        animator.Play(animeName, 0, 1);
                    }
                }
                else
                {
                    Player1Score.transform.GetChild(i).gameObject.SetActive(false);
                    Player2Score.transform.GetChild(i).gameObject.SetActive(false);
                }

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
            }
            PlayRoundSound(Round);
        }
        void Update()
        {
            // if escape button is clicked open settings panel and pause the scene, if it is already open close it and resume the scene
            bool escapePressed = Input.GetKeyDown(KeyCode.Escape);
            if (escapePressed)
            {
                if (settingsPanel.activeSelf)
                {
                    settingsPanel.SetActive(false);
                    Time.timeScale = 1;
                }
                else
                {
                    settingsPanel.SetActive(true);
                    Time.timeScale = 0;
                }
            }
            Player1HealthSlider.value = player1.health;
            Player2HealthSlider.value = player2.health;
            // player 1 health slider color changes with the health.. when it appreaches 0.. slider color appreaches red
            if (player1.health <= player1.maxHealth * 0.3)
            {
                Player1HealthSlider.fillRect.GetComponent<Image>().color = Color.red;
            }
            // player 2 health slider color changes with the health.. when it appreaches 0.. slider color appreaches red
            if (player2.health <= player2.maxHealth * 0.3)
            {
                Player2HealthSlider.fillRect.GetComponent<Image>().color = Color.red;
            }
            // Round over
            if ((player1.health <= 0 || player2.health <= 0) && flag)
            {
                flag = false;
                isInputEnabled = false;
                countdownText.gameObject.SetActive(false);
                preGameCountdownText.gameObject.SetActive(false);
                
                // Increment the Round variable
                Round++;
                if (player1.health <= 0){
                    Player2ScoreValue++;
                }
                else{
                    Player1ScoreValue++;
                }
                RoundScore = player1.health <= 0 ? RoundScore - 1 : RoundScore + 1;
                UpdateRoundScoreUI();
                if (Player1ScoreValue > 2 ||  Player2ScoreValue > 2) // 3 rounds finished = game over
                {
                    PlayerPrefs.DeleteAll();
                    GameOverText.GetComponentInChildren<TextMeshProUGUI>(true).text = RoundScore > 0 ? "Player 1 Wins!" : "Player 2 Wins!"; // +ve means player 1 wins, -ve means player 2 wins
                    GameOverText.SetActive(true);
                    gameManager.Reset();
                    predictionPanel.SetActive(false);
                    InvokeRepeating("CountDownAfterGameOver", 1, 1);
                    // Invoke("BackToCharacterSelect", 6); 
                }
                else{
                    // Save the Round value to Player Preferences
                    gameManager.Round = Round;
                    gameManager.RoundScore = RoundScore;
                    gameManager.Player1ScoreValue = Player1ScoreValue;
                    gameManager.Player2ScoreValue = Player2ScoreValue;
                    RoundOverText.GetComponentInChildren<TextMeshProUGUI>(true).text = player1.health <= 0 ? "Player 2 Wins!" : "Player 1 Wins!";
                    RoundOverText.SetActive(true);
                    Invoke("RestartRound", 8);   
                }
            }
        }
        void CountDownAfterGameOver()
        {
            backAfter--;
            countdownTillBacktoChooseCharacterText.text = "Back to Choose Character in " + backAfter;
            if (backAfter == 0)
            {
                BackToCharacterSelect();
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
                // isInputEnabled = true;
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

        void RestartRound()
        {
            // Reload the scene to restart the game with a new round
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        public void RestartGame()
        {
            Time.timeScale = 1;
            gameManager.Reset();
            RestartRound();
        }
        public void ResetTimeScale()
        {
            Time.timeScale = 1;
        }
        void BackToCharacterSelect()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelect");
        }
        void UpdateRoundScoreUI()
        {
            if (Player1ScoreValue > 0)
            {
                Player1Score.transform.GetChild(Player1ScoreValue - 1).gameObject.SetActive(true);
            }
            if (Player2ScoreValue > 0)
            {
                Player2Score.transform.GetChild(Player2ScoreValue -1).gameObject.SetActive(true);
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
            // enable input after the audio finished
            Invoke("EnableInput", 3.0f);
        }
        void EnableInput()
        {
            isInputEnabled = true;
        }
    }
}
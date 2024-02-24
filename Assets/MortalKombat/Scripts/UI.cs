using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private static bool isInputEnabled = false;
    public bool EnableTimer;

    public Slider Player1HealthSlider;
    public Slider Player2HealthSlider; 
    Player1Controller player1;
    Player2Controller player2;

    public TextMeshProUGUI countdownText;
    private int seconds = 90;

    public TextMeshProUGUI preGameCountdownText;
    private int preGameSeconds = 6;
    
    public GameObject GameOverText;
    public GameObject RoundOverText;
    
    
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
        // Load the Round value from Player Preferences
        Round = PlayerPrefs.GetInt("Round", 1);
        RoundScore = PlayerPrefs.GetInt("RoundScore", 0);
        flag = true;

        player1 = GameObject.Find("Player1").GetComponent<Player1Controller>();
        player2 = GameObject.Find("Player2").GetComponent<Player2Controller>();
        Player1HealthSlider.maxValue = player1.health;
        Player2HealthSlider.maxValue = player2.health;
        RoundOverText.gameObject.SetActive(false);
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
            Debug.Log("Round: " + Round + " RoundScore: " + RoundScore);
            if (Round > 3 || RoundScore > 1 ||  RoundScore < -1) // 3 rounds finished = game over
            {
                PlayerPrefs.DeleteAll();
                GameOverText.GetComponentInChildren<TextMeshProUGUI>(true).text = RoundScore > 0 ? "Player 1 Wins!" : "Player 2 Wins!"; // +ve means player 1 wins, -ve means player 2 wins
                GameOverText.SetActive(true);
            }
            else{
                // Save the Round value to Player Preferences
                PlayerPrefs.SetInt("Round", Round);
                PlayerPrefs.SetInt("RoundScore", RoundScore);
                RoundOverText.GetComponentInChildren<TextMeshProUGUI>(true).text = player1.health <= 0 ? "Player 2 Wins!" : "Player 1 Wins!";
                RoundOverText.SetActive(true);
                // wait for 10 seconds then restart the game
                Invoke("RestartGame", 10f);
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
        else{
            // color = #FA5801
            preGameCountdownText.color = new Color(0.9803922f, 0.345098f, 0.003921569f);
            preGameCountdownText.text = preGameSeconds.ToString();
        }

        // Disable the pre-game countdown text when the countdown is complete
        if (preGameSeconds == 0)
        {
            preGameCountdownText.gameObject.SetActive(false);
            countdownText.gameObject.SetActive(true);
            isInputEnabled = true;
        }
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

        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        countdownText.text = seconds.ToString();
    }

    void RestartGame()
    {
        // Reload the scene to restart the game with a new round
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    void OnDestroy()
    {
        // Delete all player preferences when the script is destroyed
        PlayerPrefs.DeleteAll();
    }
}

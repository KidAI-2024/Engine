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
    private int preGameSeconds = 3;
    
    public GameObject GameOverText;
    
    // Property to get the input status
    public static bool IsInputEnabled
    {
        get { return isInputEnabled; }
    }

    void Start()
    {
        player1 = GameObject.Find("Player1").GetComponent<Player1Controller>();
        player2 = GameObject.Find("Player2").GetComponent<Player2Controller>();
        Player1HealthSlider.value = 100;
        Player2HealthSlider.value = 100;
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
        if (player1.health <= 0 || player2.health <= 0)
        {
            isInputEnabled = false;
            countdownText.gameObject.SetActive(false);
            preGameCountdownText.gameObject.SetActive(false);
            GameOverText.SetActive(true);
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
        preGameCountdownText.text = preGameSeconds.ToString();

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
}

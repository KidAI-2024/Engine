using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private static bool isInputEnabled = false;

    public TextMeshProUGUI countdownText;
    private int seconds = 90;

    public TextMeshProUGUI preGameCountdownText;
    private int preGameSeconds = 3;
    
    
    // Property to get the input status
    public static bool IsInputEnabled
    {
        get { return isInputEnabled; }
    }

    void Start()
    {
        isInputEnabled = false;

        countdownText.gameObject.SetActive(false);
        // Start the pre-game countdown
        InvokeRepeating("UpdatePreGameCountdown", 1f, 1f);
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

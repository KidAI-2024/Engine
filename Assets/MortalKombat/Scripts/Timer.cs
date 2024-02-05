using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
     public TextMeshProUGUI countdownText;
    private int seconds = 90;

    void Start()
    {
        UpdateTimerDisplay();
        InvokeRepeating("UpdateTimer", 1f, 1f);
    }

    void UpdateTimer()
    {
        seconds--;

        if (seconds < 0)
        {
            seconds = 0;
            // You can add additional logic here when the countdown reaches zero
            Debug.Log("Time's up!");
        }

        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        countdownText.text = seconds.ToString();
    }
}

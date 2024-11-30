using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField] private float timeRemaining = 10f;
    [SerializeField] private TMP_Text text;

    public delegate void TimerEnded();
    public event TimerEnded OnTimerEnd; // Event triggered when time runs out

    private bool timeIsRunning = false;

    void Start()
    {
        // Timer starts only on reset
    }

    void Update()
    {
        if (timeIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timeIsRunning = false;
                OnTimerEnd?.Invoke(); // Trigger event
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        text.text = seconds.ToString();
    }

    public void ResetTimer(float newTime)
    {
        timeRemaining = newTime;
        timeIsRunning = true;
    }

    public void StopTimer()
    {
        timeIsRunning = false;
    }
}

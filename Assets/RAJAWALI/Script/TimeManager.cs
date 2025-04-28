using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;  // For UnityEvent

public class TimeManager : MonoBehaviour
{
    public TextMeshProUGUI missionTimeText;
    private float remainingTime;

    // Game Over UnityEvent, to be assigned in the Inspector
    public UnityEvent onGameOver;

    void Start()
    {
        // Initialize the UnityEvent if it wasn't assigned in the Inspector
        if (onGameOver == null)
            onGameOver = new UnityEvent();

        remainingTime = PlayerPrefs.GetFloat("MissionTime", 0f);  // Default to 0 if no time is set

        // Start the mission countdown
        StartMission(remainingTime);
    }

    // Start the mission (from LoadoutScene)
    public void StartMission(float missionTime)
    {
        remainingTime = missionTime;
        UpdateTimeDisplay();
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimeDisplay();

            if (remainingTime <= 0)
            {
                GameOver();
            }
        }
    }

    private void UpdateTimeDisplay()
    {
        missionTimeText.text = FormatTime(remainingTime);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Game Over method that triggers the event
    private void GameOver()
    {
        if (onGameOver != null)
        {
            onGameOver.Invoke();  // Invoke the Game Over event
        }
    }
}

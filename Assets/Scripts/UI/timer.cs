using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private int startTimeInSeconds = 180;

    private int currentTime;

    private void Start()
    {
        currentTime = startTimeInSeconds;
        UpdateTimerUI();
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSecondsRealtime(1f); // 🔥 FIX

            currentTime--;
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = currentTime / 60;
        int seconds = currentTime % 60;

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
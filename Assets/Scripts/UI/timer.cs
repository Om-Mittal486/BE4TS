using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private int startTimeInSeconds = 180;

    [Header("Game Over")]
    [SerializeField] private GameObject timeOverUI;

    private int currentTime;
    private Coroutine timerCoroutine; // ✅ store coroutine

    private void Start()
    {
        StartTimer();

        if (timeOverUI != null)
            timeOverUI.SetActive(false);
    }

    // ✅ NEW: Start Timer Method
    public void StartTimer()
    {
        currentTime = startTimeInSeconds;
        UpdateTimerUI();

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSecondsRealtime(1f);

            currentTime--;
            UpdateTimerUI();
        }

        TimeOver();
    }

    private void TimeOver()
    {
        if (timeOverUI != null)
            timeOverUI.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = currentTime / 60;
        int seconds = currentTime % 60;

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
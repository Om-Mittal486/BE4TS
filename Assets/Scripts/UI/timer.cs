using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private int startTimeInSeconds = 180;

    [Header("Game Over")]
    [SerializeField] private GameObject timeOverUI;

    private float currentTime;
    private Coroutine localTimerCoroutine;

    private void OnEnable()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTimerTick += HandleSessionTimerTick;
            GameSessionManager.Instance.OnTimeOver += TimeOver;
            UpdateDisplay(GameSessionManager.Instance.TimeRemaining);
        }
    }

    private void OnDisable()
    {
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.OnTimerTick -= HandleSessionTimerTick;
            GameSessionManager.Instance.OnTimeOver -= TimeOver;
        }
    }

    private void Start()
    {
        if (timeOverUI != null)
            timeOverUI.SetActive(false);

        // If no global session exists (e.g. running scene directly in editor), fallback to local timer
        if (GameSessionManager.Instance == null)
        {
            StartTimer();
        }
        else
        {
            UpdateDisplay(GameSessionManager.Instance.TimeRemaining);
        }
    }

    private void HandleSessionTimerTick(float timeRemaining, float timeElapsed)
    {
        UpdateDisplay(timeRemaining);
    }

    // Local standalone timer fallback
    public void StartTimer()
    {
        currentTime = startTimeInSeconds;
        UpdateDisplay(currentTime);

        if (localTimerCoroutine != null)
            StopCoroutine(localTimerCoroutine);

        localTimerCoroutine = StartCoroutine(LocalTimerCoroutine());
    }

    private IEnumerator LocalTimerCoroutine()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
            currentTime--;
            UpdateDisplay(currentTime);
        }

        TimeOver();
    }

    public void TimeOver()
    {
        if (timeOverUI != null)
            timeOverUI.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateDisplay(float secondsRemaining)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(Mathf.Max(0, secondsRemaining) / 60f);
        int seconds = Mathf.FloorToInt(Mathf.Max(0, secondsRemaining) % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
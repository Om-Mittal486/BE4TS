using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    [Header("Timer Configuration")]
    [Tooltip("Total allowed time to complete all levels (in seconds). Default 3 minutes = 180s")]
    [SerializeField] private float totalAllowedTime = 180f;

    [Header("Current Session State")]
    [SerializeField] private string currentPlayerName = "Player";
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool isTimerRunning = false;
    [SerializeField] private bool isGameCompleted = false;

    // Events for UI or external scripts to subscribe to
    public event Action<float, float> OnTimerTick; // (timeRemaining, timeElapsed)
    public event Action OnTimeOver;
    public event Action<string, float, int> OnGameCompleted; // (playerName, totalTimeElapsed, rank)

    public string CurrentPlayerName => currentPlayerName;
    public float TimeRemaining => timeRemaining;
    public float TimeElapsed => Mathf.Max(0f, totalAllowedTime - timeRemaining);
    public float TotalAllowedTime => totalAllowedTime;
    public bool IsTimerRunning => isTimerRunning;
    public bool IsGameCompleted => isGameCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!isTimerRunning || isGameCompleted) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isTimerRunning = false;
            OnTimerTick?.Invoke(timeRemaining, TimeElapsed);
            TriggerTimeOver();
        }
        else
        {
            OnTimerTick?.Invoke(timeRemaining, TimeElapsed);
        }
    }

    /// <summary>
    /// Starts a new run with the entered player name, initializes the 3-minute timer, and loads the first level.
    /// </summary>
    public void StartGame(string playerName, string firstSceneName = "Mix")
    {
        currentPlayerName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
        timeRemaining = totalAllowedTime;
        isTimerRunning = true;
        isGameCompleted = false;
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(firstSceneName))
        {
            SceneManager.LoadScene(firstSceneName);
        }
    }

    /// <summary>
    /// Called when the player reaches the final success/finish trigger.
    /// Stops the timer and records the player's run to the leaderboard.
    /// </summary>
    public int CompleteGame()
    {
        if (isGameCompleted) return -1;

        isGameCompleted = true;
        isTimerRunning = false;
        float totalTimeTaken = TimeElapsed;

        // Save to leaderboard if completed within the 3 minute limit
        int achievedRank = LeaderboardManager.AddEntry(currentPlayerName, totalTimeTaken);
        Debug.Log($"[GameSessionManager] Level Completed! Player: {currentPlayerName}, Time Taken: {LeaderboardEntry.FormatTime(totalTimeTaken)}, Rank: #{achievedRank}");

        OnGameCompleted?.Invoke(currentPlayerName, totalTimeTaken, achievedRank);
        return achievedRank;
    }

    /// <summary>
    /// Handles time expiration (3-minute countdown reached 00:00).
    /// </summary>
    private void TriggerTimeOver()
    {
        Debug.Log("[GameSessionManager] 3 Minutes Timer Expired! Time Over.");
        OnTimeOver?.Invoke();
    }

    /// <summary>
    /// Pauses or resumes the timer.
    /// </summary>
    public void SetTimerPaused(bool paused)
    {
        isTimerRunning = !paused;
    }

    /// <summary>
    /// Resets the session state.
    /// </summary>
    public void ResetSession()
    {
        isTimerRunning = false;
        isGameCompleted = false;
        timeRemaining = totalAllowedTime;
    }
}


using UnityEngine;
using TMPro;

public class SuccessTrigger : MonoBehaviour
{
    [Header("Success UI")]
    [SerializeField] private GameObject successUI; // Drag your Success panel here

    [Header("Optional Result Display")]
    [SerializeField] private TextMeshProUGUI completionTimeText; // Displays "Time: 01:23.45"
    [SerializeField] private TextMeshProUGUI rankText;           // Displays "Rank: #1"

    private bool isTriggered = false;

    private void Start()
    {
        if (successUI != null)
            successUI.SetActive(false); // Hide at start
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            ActivateSuccess();
        }
    }

    private void ActivateSuccess()
    {
        int rank = -1;
        float timeTaken = 0f;

        // Complete the session and record leaderboard rank
        if (GameSessionManager.Instance != null)
        {
            timeTaken = GameSessionManager.Instance.TimeElapsed;
            rank = GameSessionManager.Instance.CompleteGame();
        }

        // Update UI text if assigned
        if (completionTimeText != null)
        {
            completionTimeText.text = $"Time: {LeaderboardEntry.FormatTime(timeTaken)}";
        }

        if (rankText != null && rank > 0)
        {
            rankText.text = $"Leaderboard Rank: #{rank}";
        }

        // Show success UI
        if (successUI != null)
            successUI.SetActive(true);

        // Pause game
        Time.timeScale = 0f;

        // Unlock & show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
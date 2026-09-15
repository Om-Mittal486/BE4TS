using UnityEngine;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeText;

    /// <summary>
    /// Sets the row information for rank, player name, and completion time.
    /// </summary>
    public void SetEntry(int rank, string playerName, string formattedTime)
    {
        if (rankText != null)
        {
            rankText.text = rank switch
            {
                1 => "<color=#FFD700>#1</color>", // Gold
                2 => "<color=#C0C0C0>#2</color>", // Silver
                3 => "<color=#CD7F32>#3</color>", // Bronze
                _ => $"#{rank}"
            };
        }

        if (nameText != null)
        {
            nameText.text = playerName;
        }

        if (timeText != null)
        {
            timeText.text = formattedTime;
        }
    }
}


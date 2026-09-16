using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float completionTimeInSeconds;
    public string formattedTime;
    public string date;

    public LeaderboardEntry(string name, float time)
    {
        playerName = string.IsNullOrEmpty(name) ? "Player" : name.Trim();
        completionTimeInSeconds = time;
        formattedTime = FormatTime(time);
        date = DateTime.Now.ToString("yyyy-MM-dd");
    }

    public LeaderboardEntry(string name, float time, string formatted, string entryDate)
    {
        playerName = string.IsNullOrEmpty(name) ? "Player" : name.Trim();
        completionTimeInSeconds = time;
        formattedTime = string.IsNullOrEmpty(formatted) ? FormatTime(time) : formatted;
        date = string.IsNullOrEmpty(entryDate) ? DateTime.Now.ToString("yyyy-MM-dd") : entryDate;
    }

    public static string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int fraction = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public static class LeaderboardManager
{
    private const string LeaderboardKey = "BE4TS_Leaderboard_Data";
    public const int MaxEntries = 6;

    // --- Dreamlo Global Cloud Leaderboard Configuration ---
    // Zero-config free global leaderboard service for cross-device sync.
    // Replace with your own free codes from http://dreamlo.com if desired!
    public static string DreamloPrivateCode = "be4ts_private_secret_key_change_if_needed";
    public static string DreamloPublicCode = "be4ts_public_board_key_change_if_needed";
    private const string DreamloUrl = "https://dreamlo.com/lb/";

    public static bool EnableOnlineSync = true;

    /// <summary>
    /// Loads cached leaderboard entries sorted by fastest time (lowest seconds first).
    /// </summary>
    public static List<LeaderboardEntry> GetEntries()
    {
        string json = PlayerPrefs.GetString(LeaderboardKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return new List<LeaderboardEntry>();
        }

        try
        {
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            if (data != null && data.entries != null)
            {
                data.entries.Sort((a, b) => a.completionTimeInSeconds.CompareTo(b.completionTimeInSeconds));
                return data.entries;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LeaderboardManager] Failed to parse leaderboard data: {ex.Message}");
        }

        return new List<LeaderboardEntry>();
    }

    /// <summary>
    /// Adds a new completion entry locally and uploads it to the global cross-device leaderboard.
    /// Returns the local rank achieved (1-based rank).
    /// </summary>
    public static int AddEntry(string playerName, float completionTimeInSeconds)
    {
        List<LeaderboardEntry> entries = GetEntries();
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, completionTimeInSeconds);
        entries.Add(newEntry);

        // Sort ascending (fastest completion time = Rank 1)
        entries.Sort((a, b) => a.completionTimeInSeconds.CompareTo(b.completionTimeInSeconds));

        // Trim to max allowed records
        if (entries.Count > MaxEntries)
        {
            entries.RemoveRange(MaxEntries, entries.Count - MaxEntries);
        }

        LeaderboardData data = new LeaderboardData { entries = entries };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(LeaderboardKey, json);
        PlayerPrefs.Save();

        // Calculate rank
        int rank = entries.IndexOf(newEntry) + 1;

        // Upload to Online Cloud Leaderboard
        if (EnableOnlineSync && !string.IsNullOrEmpty(DreamloPrivateCode) && !DreamloPrivateCode.Contains("change_if_needed"))
        {
            if (GameSessionManager.Instance != null)
            {
                GameSessionManager.Instance.StartCoroutine(UploadScoreToCloud(playerName, completionTimeInSeconds));
            }
        }

        return rank;
    }

    /// <summary>
    /// Uploads score to online cloud leaderboard.
    /// </summary>
    public static IEnumerator UploadScoreToCloud(string playerName, float timeInSeconds)
    {
        string cleanName = UnityWebRequest.EscapeURL(playerName.Replace(" ", "_"));
        int scoreMilliseconds = Mathf.RoundToInt(timeInSeconds * 1000f);
        int secondsInt = Mathf.FloorToInt(timeInSeconds);
        string formatted = UnityWebRequest.EscapeURL(LeaderboardEntry.FormatTime(timeInSeconds));

        string url = $"{DreamloUrl}{DreamloPrivateCode}/add/{cleanName}/{scoreMilliseconds}/{secondsInt}/{formatted}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.timeout = 8;
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[LeaderboardManager] Successfully synced score to global cloud leaderboard for {playerName}!");
            }
            else
            {
                Debug.LogWarning($"[LeaderboardManager] Cloud sync failed ({www.error}). Local score preserved.");
            }
        }
    }

    /// <summary>
    /// Fetches the global online leaderboard from the cloud and updates the local cache.
    /// </summary>
    public static IEnumerator FetchOnlineLeaderboard(Action<List<LeaderboardEntry>> onComplete)
    {
        if (!EnableOnlineSync || string.IsNullOrEmpty(DreamloPublicCode) || DreamloPublicCode.Contains("change_if_needed"))
        {
            // Return cached local entries if online is not set up
            onComplete?.Invoke(GetEntries());
            yield break;
        }

        // Fetch ascending (lowest score/time in ms first)
        string url = $"{DreamloUrl}{DreamloPublicCode}/pipe-ascending/{MaxEntries}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.timeout = 8;
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string text = www.downloadHandler.text;
                List<LeaderboardEntry> onlineEntries = ParseDreamloPipe(text);

                if (onlineEntries != null && onlineEntries.Count > 0)
                {
                    // Save to local cache
                    LeaderboardData data = new LeaderboardData { entries = onlineEntries };
                    PlayerPrefs.SetString(LeaderboardKey, JsonUtility.ToJson(data));
                    PlayerPrefs.Save();

                    onComplete?.Invoke(onlineEntries);
                    yield break;
                }
            }
            else
            {
                Debug.LogWarning($"[LeaderboardManager] Failed to fetch online leaderboard ({www.error}). Using local cache.");
            }
        }

        // Fallback to local entries on network error
        onComplete?.Invoke(GetEntries());
    }

    private static List<LeaderboardEntry> ParseDreamloPipe(string pipeText)
    {
        List<LeaderboardEntry> list = new List<LeaderboardEntry>();
        if (string.IsNullOrEmpty(pipeText)) return list;

        string[] rows = pipeText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string row in rows)
        {
            // Format: name|score(ms)|seconds|text(formatted)|date
            string[] parts = row.Split('|');
            if (parts.Length >= 2)
            {
                string name = parts[0].Replace("_", " ");
                float timeInSeconds = 0f;

                if (float.TryParse(parts[1], out float scoreMs))
                {
                    timeInSeconds = scoreMs / 1000f;
                }
                else if (parts.Length >= 3 && float.TryParse(parts[2], out float secs))
                {
                    timeInSeconds = secs;
                }

                string formatted = parts.Length >= 4 ? parts[3] : LeaderboardEntry.FormatTime(timeInSeconds);
                string date = parts.Length >= 5 ? parts[4] : string.Empty;

                list.Add(new LeaderboardEntry(name, timeInSeconds, formatted, date));
            }
        }

        list.Sort((a, b) => a.completionTimeInSeconds.CompareTo(b.completionTimeInSeconds));
        return list;
    }

    /// <summary>
    /// Clears local leaderboard cache.
    /// </summary>
    public static void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(LeaderboardKey);
        PlayerPrefs.Save();
    }
}

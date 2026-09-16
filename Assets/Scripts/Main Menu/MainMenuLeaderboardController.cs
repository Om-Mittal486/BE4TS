using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuLeaderboardController : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("The main menu container (holds Title, Play, LeaderBoard, Quit)")]
    [SerializeField] private GameObject mainMenuPanel;
    [Tooltip("The player name input panel popup/screen (PlayerNameInput)")]
    [SerializeField] private GameObject playerNamePanel;
    [Tooltip("The leaderboard panel popup/screen (LeaderBoardUI)")]
    [SerializeField] private GameObject leaderboardPanel;

    [Header("Player Name Input Elements")]
    [Tooltip("TMP InputField where player enters their name")]
    [SerializeField] private TMP_InputField nameInputField;
    [Tooltip("Optional text to display warnings (e.g., 'Please enter your name first!')")]
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("Leaderboard List References")]
    [Tooltip("The Transform / Content GameObject where entry rows are spawned (e.g., RowTransform)")]
    [SerializeField] private Transform leaderboardContentParent;
    [Tooltip("Prefab for a single leaderboard entry row (e.g., RowPrefab)")]
    [SerializeField] private GameObject leaderboardRowPrefab;
    [Tooltip("Optional notice / text displayed when leaderboard has no entries yet")]
    [SerializeField] private GameObject emptyLeaderboardNotice;

    [Header("Online Global Leaderboard (Cross-Device)")]
    [Tooltip("Enable online cloud sync across different devices")]
    [SerializeField] private bool enableOnlineSync = true;
    [Tooltip("Optional: Your free Dreamlo Public Code from dreamlo.com")]
    [SerializeField] private string dreamloPublicCode = "";
    [Tooltip("Optional: Your free Dreamlo Private Code from dreamlo.com")]
    [SerializeField] private string dreamloPrivateCode = "";

    [Header("Buttons (Optional UI.Button auto-binding)")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button confirmNameButton;

    [Header("Audio Feedback")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Scene Configuration")]
    [SerializeField] private string firstGameScene = "Mix";

    [Header("Leaderboard Layout Settings")]
    [Tooltip("Max number of entries to display on the leaderboard (capped at 6)")]
    [SerializeField] private int maxLeaderboardEntries = 6;
    [Tooltip("Y-position of the first leaderboard row inside RowTransform")]
    [SerializeField] private float rowStartY = 95f;
    [Tooltip("Vertical spacing between leaderboard rows")]
    [SerializeField] private float rowSpacing = 38f;

    private void Awake()
    {
        // Ensure GameSessionManager instance exists in scene
        if (GameSessionManager.Instance == null)
        {
            GameObject sessionGO = new GameObject("GameSessionManager");
            sessionGO.AddComponent<GameSessionManager>();
        }

        if (uiAudioSource == null)
            uiAudioSource = GetComponent<AudioSource>();

        // Apply online configuration if provided
        LeaderboardManager.EnableOnlineSync = enableOnlineSync;
        if (!string.IsNullOrEmpty(dreamloPublicCode)) LeaderboardManager.DreamloPublicCode = dreamloPublicCode;
        if (!string.IsNullOrEmpty(dreamloPrivateCode)) LeaderboardManager.DreamloPrivateCode = dreamloPrivateCode;

        // Auto-find panels if not set in Inspector
        if (mainMenuPanel == null)
        {
            GameObject found = GameObject.Find("Main Menu");
            if (found != null) mainMenuPanel = found;
        }

        if (playerNamePanel == null)
        {
            GameObject found = GameObject.Find("PlayerNameInput");
            if (found != null) playerNamePanel = found;
        }

        if (leaderboardPanel == null)
        {
            GameObject found = GameObject.Find("LeaderBoardUI");
            if (found != null) leaderboardPanel = found;
        }
    }

    private void Start()
    {
        // Unlock mouse cursor for typing name and UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerNamePanel != null)
            playerNamePanel.SetActive(false);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        // Auto hook UI buttons if standard UI.Button is attached
        if (playButton != null) playButton.onClick.AddListener(OpenPlayerNameInput);
        if (confirmNameButton != null) confirmNameButton.onClick.AddListener(ConfirmPlayerNameAndStartGame);
        if (leaderboardButton != null) leaderboardButton.onClick.AddListener(OpenLeaderboard);
        if (backButton != null) backButton.onClick.AddListener(CloseLeaderboard);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);

        // Support pressing Enter key in the InputField to submit
        if (nameInputField != null)
        {
            nameInputField.onSubmit.AddListener(delegate { ConfirmPlayerNameAndStartGame(); });

            string savedName = PlayerPrefs.GetString("BE4TS_LastPlayerName", string.Empty);
            if (!string.IsNullOrEmpty(savedName))
            {
                nameInputField.text = savedName;
            }
        }
    }

    public void PlaySound()
    {
        if (uiAudioSource != null && buttonClickSound != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound);
        }
        else if (buttonClickSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonClickSound, Camera.main != null ? Camera.main.transform.position : Vector3.zero);
        }
    }

    /// <summary>
    /// Step 1: Called when 'Play' is clicked. Opens the PlayerNameInput popup.
    /// </summary>
    public void OpenPlayerNameInput()
    {
        PlaySound();

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        if (playerNamePanel != null)
            playerNamePanel.SetActive(true);

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        // Focus input field
        if (nameInputField != null)
        {
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Closes the PlayerNameInput popup and returns to the Main Menu.
    /// </summary>
    public void ClosePlayerNameInput()
    {
        PlaySound();

        if (playerNamePanel != null)
            playerNamePanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        // Refresh menu button controller
        MenuButtonController mbc = FindObjectOfType<MenuButtonController>();
        if (mbc != null) mbc.RefreshMaxIndex();
    }

    /// <summary>
    /// Step 2: Called when 'Confirm' / 'Continue' is clicked.
    /// Validates player name and starts game with the 3-minute persistent timer.
    /// </summary>
    public void ConfirmPlayerNameAndStartGame()
    {
        PlaySound();

        string enteredName = nameInputField != null ? nameInputField.text.Trim() : string.Empty;

        if (string.IsNullOrWhiteSpace(enteredName))
        {
            ShowWarning("Please enter your name first!");
            if (nameInputField != null)
            {
                nameInputField.Select();
                nameInputField.ActivateInputField();
            }
            return;
        }

        if (warningText != null)
            warningText.gameObject.SetActive(false);

        // Save player name
        PlayerPrefs.SetString("BE4TS_LastPlayerName", enteredName);
        PlayerPrefs.Save();

        // Start game session with persistent 3-minute timer
        GameSessionManager.Instance.StartGame(enteredName, firstGameScene);
    }

    /// <summary>
    /// Opens the Leaderboard panel and populates ranking entries (local + online sync).
    /// </summary>
    public void OpenLeaderboard()
    {
        PlaySound();

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (playerNamePanel != null)
            playerNamePanel.SetActive(false);

        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);

            // 1. Immediately render cached/local entries (instant response)
            RenderLeaderboardRows(LeaderboardManager.GetEntries());

            // 2. Fetch latest global leaderboard in background and refresh
            if (enableOnlineSync)
            {
                StartCoroutine(LeaderboardManager.FetchOnlineLeaderboard(onlineEntries =>
                {
                    if (leaderboardPanel != null && leaderboardPanel.activeInHierarchy)
                    {
                        RenderLeaderboardRows(onlineEntries);
                    }
                }));
            }
        }
    }

    /// <summary>
    /// Closes the Leaderboard panel and reloads the Main Menu scene so 'Press X to start' appears fresh.
    /// </summary>
    public void CloseLeaderboard()
    {
        PlaySound();
        StartCoroutine(ReloadSceneAfterAudio());
    }

    private IEnumerator ReloadSceneAfterAudio()
    {
        // Small wait so the click sound plays crisply before reload
        yield return new WaitForSecondsRealtime(0.12f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void OnQuitClicked()
    {
        PlaySound();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// Populates the leaderboard rows for a list of entries (capped at 6 entries).
    /// </summary>
    public void RenderLeaderboardRows(List<LeaderboardEntry> entries)
    {
        if (leaderboardContentParent == null) return;

        // Clear existing instantiated row objects
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in leaderboardContentParent)
        {
            toDestroy.Add(child.gameObject);
        }
        for (int i = 0; i < toDestroy.Count; i++)
        {
            Destroy(toDestroy[i]);
        }

        if (entries == null || entries.Count == 0)
        {
            if (emptyLeaderboardNotice != null)
            {
                emptyLeaderboardNotice.SetActive(true);
                TextMeshProUGUI noticeTMP = emptyLeaderboardNotice.GetComponent<TextMeshProUGUI>();
                if (noticeTMP != null)
                {
                    noticeTMP.text = "No records yet.\nComplete all levels within 3 minutes to rank!";
                }
            }
            return;
        }

        if (emptyLeaderboardNotice != null)
        {
            if (emptyLeaderboardNotice.transform != leaderboardContentParent)
                emptyLeaderboardNotice.SetActive(false);
            else
            {
                TextMeshProUGUI noticeTMP = emptyLeaderboardNotice.GetComponent<TextMeshProUGUI>();
                if (noticeTMP != null) noticeTMP.text = string.Empty;
            }
        }

        int count = Mathf.Min(entries.Count, maxLeaderboardEntries);

        for (int i = 0; i < count; i++)
        {
            LeaderboardEntry entry = entries[i];
            int rank = i + 1;

            if (leaderboardRowPrefab != null)
            {
                GameObject rowGO = Instantiate(leaderboardRowPrefab, leaderboardContentParent);
                rowGO.SetActive(true);

                RectTransform rowRect = rowGO.GetComponent<RectTransform>();
                if (rowRect != null)
                {
                    rowRect.localScale = Vector3.one;
                    rowRect.localRotation = Quaternion.identity;
                    rowRect.anchoredPosition = new Vector2(0f, rowStartY - (i * rowSpacing));
                }

                // Check for LeaderboardEntryUI component
                LeaderboardEntryUI rowUI = rowGO.GetComponent<LeaderboardEntryUI>();
                if (rowUI != null)
                {
                    rowUI.SetEntry(rank, entry.playerName, entry.formattedTime);
                }
                else
                {
                    // Locate Text components smartly by name or position
                    TextMeshProUGUI[] texts = rowGO.GetComponentsInChildren<TextMeshProUGUI>(true);
                    TextMeshProUGUI rankTMP = null;
                    TextMeshProUGUI nameTMP = null;
                    TextMeshProUGUI timeTMP = null;

                    foreach (var t in texts)
                    {
                        string tName = t.gameObject.name.ToLower();
                        if (tName.Contains("rank")) rankTMP = t;
                        else if (tName.Contains("name")) nameTMP = t;
                        else if (tName.Contains("time")) timeTMP = t;

                        // Align child local Y offsets to 0 so parent row spacing controls vertical layout
                        RectTransform childRect = t.GetComponent<RectTransform>();
                        if (childRect != null)
                        {
                            Vector2 ap = childRect.anchoredPosition;
                            childRect.anchoredPosition = new Vector2(ap.x, 0f);
                        }
                    }

                    // Fallback to array index if names didn't match
                    if (rankTMP == null && texts.Length > 0) rankTMP = texts[0];
                    if (nameTMP == null && texts.Length > 1) nameTMP = texts[1];
                    if (timeTMP == null && texts.Length > 2) timeTMP = texts[2];

                    if (rankTMP != null)
                    {
                        rankTMP.text = rank switch
                        {
                            1 => "<color=#FFD700>#1</color>",
                            2 => "<color=#C0C0C0>#2</color>",
                            3 => "<color=#CD7F32>#3</color>",
                            _ => $"#{rank}"
                        };
                    }

                    if (nameTMP != null) nameTMP.text = entry.playerName;
                    if (timeTMP != null) timeTMP.text = entry.formattedTime;
                }
            }
        }
    }

    private void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.gameObject.SetActive(true);
        }
    }
}

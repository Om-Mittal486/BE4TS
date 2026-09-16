using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuAction : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Menus")]
    public GameObject mainMenu;

    [Header("Leaderboard Controller")]
    [SerializeField] private MainMenuLeaderboardController leaderboardController;

    private bool canAcceptInput = false;

    private void Awake()
    {
        if (leaderboardController == null)
        {
            leaderboardController = FindObjectOfType<MainMenuLeaderboardController>();
        }
    }

    void Start()
    {
        UnlockCursor();

        if (mainMenu != null && mainMenu.activeInHierarchy)
            StartCoroutine(EnableMenuLogicAfterDelay());
        else
            canAcceptInput = true;
    }

    void OnEnable()
    {
        UnlockCursor();

        if (mainMenu != null && mainMenu.activeInHierarchy)
            StartCoroutine(EnableMenuLogicAfterDelay());
        else
            canAcceptInput = true;
    }

    IEnumerator EnableMenuLogicAfterDelay()
    {
        canAcceptInput = false;
        yield return new WaitForSeconds(0.2f);
        canAcceptInput = true;
    }

    public void OnMenuSubmit()
    {
        if (!canAcceptInput) return;

        if (leaderboardController == null)
            leaderboardController = FindObjectOfType<MainMenuLeaderboardController>();

        string btnName = gameObject.name.Trim();

        switch (btnName)
        {
            case "Play":
                if (leaderboardController != null)
                {
                    leaderboardController.OpenPlayerNameInput();
                }
                else
                {
                    SceneManager.LoadScene("Mix");
                }
                break;

            case "Continue":
            case "Confirm":
                if (leaderboardController != null)
                {
                    leaderboardController.ConfirmPlayerNameAndStartGame();
                }
                else
                {
                    SceneManager.LoadScene("Mix");
                }
                break;

            case "LeaderBoard":
            case "Leaderboard":
                if (leaderboardController != null)
                {
                    leaderboardController.OpenLeaderboard();
                }
                break;

            case "Button":
            case "Back":
                if (leaderboardController != null)
                {
                    // Check whether to close leaderboard or name input
                    GameObject namePanel = GameObject.Find("PlayerNameInput");
                    if (namePanel != null && namePanel.activeInHierarchy)
                    {
                        leaderboardController.ClosePlayerNameInput();
                    }
                    else
                    {
                        leaderboardController.CloseLeaderboard();
                    }
                }
                break;

            case "Quit":
                if (leaderboardController != null)
                {
                    leaderboardController.OnQuitClicked();
                }
                else
                {
                    Application.Quit();
                }
                break;

            default:
                Debug.Log($"[MenuAction] Unhandled button action: '{btnName}'");
                break;
        }
    }

    // --- Mouse / Pointer Support ---
    public void OnPointerClick(PointerEventData eventData)
    {
        OnMenuSubmit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuButton menuBtn = GetComponent<MenuButton>();
        if (menuBtn != null)
        {
            menuBtn.OnPointerHoverEnter();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Optional exit behaviour
    }

    // --- Cursor Control ---
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

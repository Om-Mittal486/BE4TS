using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject timeOverUI;

    [Header("Extra Object To Disable")]
    [SerializeField] private GameObject objectToDisable; // 👈 assign in inspector

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [SerializeField] private GameTimer gameTimer; // drag your timer here

    public void ResumeGame()
    {
        // Hide UI
        if (timeOverUI != null)
            timeOverUI.SetActive(false);

        // Resume time
        Time.timeScale = 1f;

        // 🔥 Reset timer
        if (gameTimer != null)
            gameTimer.StartTimer();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 🏠 Go to Main Menu
    public void LoadMainMenu()
    {
        // 🔥 Destroy object before leaving
        if (objectToDisable != null)
            Destroy(objectToDisable);

        // Reset time
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
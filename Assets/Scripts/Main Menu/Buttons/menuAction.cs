using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuAction : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenu;

    private bool canAcceptInput = false;

    void Start()
    {
        LockCursor();

        if (mainMenu.activeInHierarchy)
            StartCoroutine(EnableMenuLogicAfterDelay());
    }

    void OnEnable()
    {
        // If main menu is re-enabled later
        if (mainMenu != null && mainMenu.activeInHierarchy)
            StartCoroutine(EnableMenuLogicAfterDelay());
    }

    IEnumerator EnableMenuLogicAfterDelay()
    {
        canAcceptInput = false;
        yield return new WaitForSeconds(2f);
        canAcceptInput = true;
    }

    void OnMenuSubmit()
    {
        // ❌ Block input if main menu inactive OR delay not finished
        if (!mainMenu.activeInHierarchy || !canAcceptInput)
            return;

        switch (gameObject.name)
        {
            case "Play":
                SceneManager.LoadScene("Mix");
                break;

            case "Quit":
                Application.Quit();
                break;
        }
    }

    // --- Cursor Control ---
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

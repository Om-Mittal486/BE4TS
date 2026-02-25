using System.Diagnostics;
using UnityEngine;

public class SuccessTrigger : MonoBehaviour
{
    [Header("Success UI")]
    [SerializeField] private GameObject successUI; // Drag your Success panel here

    private void Start()
    {
        if (successUI != null)
            successUI.SetActive(false); // Hide at start
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateSuccess();
        }
    }

    private void ActivateSuccess()
    {
    // Show success UI
        if (successUI != null)
            successUI.SetActive(true);

        // Pause game
        Time.timeScale = 0f;

        // 🔥 Unlock & show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
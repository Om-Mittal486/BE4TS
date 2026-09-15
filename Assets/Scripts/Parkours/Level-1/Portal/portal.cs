using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneName;

    private bool triggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            SceneManager.LoadScene(sceneName);
        }
    }
}

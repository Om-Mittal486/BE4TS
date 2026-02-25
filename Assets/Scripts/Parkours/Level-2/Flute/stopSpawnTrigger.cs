using UnityEngine;

public class StopNoteSpawningTrigger : MonoBehaviour
{
    [SerializeField] private MusicNoteSpawner spawner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        spawner.allowSpawning = false;

        Debug.Log("Note spawning stopped");
    }
}
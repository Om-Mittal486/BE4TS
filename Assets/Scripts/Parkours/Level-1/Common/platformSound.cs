using UnityEngine;

public class pianoSound : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip platformSound;

    private bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!hasPlayed)
        {
            audioSource.PlayOneShot(platformSound);
            hasPlayed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Reset so sound can play again when player comes back
        hasPlayed = false;
    }
}

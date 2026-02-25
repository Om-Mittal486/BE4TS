using UnityEngine;

public class PlatformBeatTrigger : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource beatAudio;

    [Header("Volume Settings")]
    [SerializeField] private float normalVolume = 0.2f;
    [SerializeField] private float boostedVolume = 0.3f;

    [Header("Smooth Speed")]
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Exit Buffer")]
    [SerializeField] private float stayThreshold = 0.05f;

    private float targetVolume;
    private bool isPlayerOnPlatform = false;
    private float stayTimer = 0f;

    private void Start()
    {
        targetVolume = normalVolume;

        if (beatAudio != null)
            beatAudio.volume = normalVolume;
    }

    private void Update()
    {
        // If player not detected recently → assume left platform
        if (stayTimer > stayThreshold)
        {
            isPlayerOnPlatform = false;
        }

        stayTimer += Time.deltaTime;

        // Set target volume
        targetVolume = isPlayerOnPlatform ? boostedVolume : normalVolume;

        // Smooth transition
        if (beatAudio != null)
        {
            beatAudio.volume = Mathf.Lerp(
                beatAudio.volume,
                targetVolume,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
            stayTimer = 0f; // Reset timer while player is inside
        }
    }
}
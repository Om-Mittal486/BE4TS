using UnityEngine;

public class GrowOnContact : MonoBehaviour
{
    [Header("Growth Settings")]
    [SerializeField] private float requiredContactTime = 0.1f;
    [SerializeField] private Vector3 maxScale = new Vector3(1.4f, 1.4f, 1f);
    [SerializeField] private float growSpeed = 5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip growSound;
    [SerializeField] [Range(0f, 1f)] private float maxVolume = 1f;
    [SerializeField] private float fadeSpeed = 5f;

    private Vector3 originalScale;
    private float contactTimer;
    private bool playerTouching;

    private float lastScaleX;

    void Start()
    {
        originalScale = transform.localScale;
        lastScaleX = transform.localScale.x;

        if (audioSource != null)
        {
            audioSource.clip = growSound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0f;
        }
    }

    void Update()
    {
        bool isGrowing = false;

        float beforeX = transform.localScale.x;

        if (playerTouching)
        {
            contactTimer += Time.deltaTime;

            if (contactTimer >= requiredContactTime)
            {
                transform.localScale = Vector3.MoveTowards(
                    transform.localScale,
                    maxScale,
                    growSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            transform.localScale = Vector3.MoveTowards(
                transform.localScale,
                originalScale,
                growSpeed * Time.deltaTime
            );
        }

        float afterX = transform.localScale.x;

        // 🔥 TRUE growth detection
        if (afterX > beforeX + 0.0001f)
            isGrowing = true;

        HandleAudio(isGrowing, afterX);
    }

    void HandleAudio(bool isGrowing, float currentScaleX)
    {
        if (audioSource == null || growSound == null)
            return;

        float progress = Mathf.InverseLerp(
            originalScale.x,
            maxScale.x,
            currentScaleX
        );

        float targetVolume = isGrowing ? progress * maxVolume : 0f;

        // 🎚 Smooth volume change
        audioSource.volume = Mathf.MoveTowards(
            audioSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );

        // ▶️ Start
        if (isGrowing && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        // ⏹ Stop completely when silent
        if (!isGrowing && audioSource.volume <= 0.01f && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerTouching = true;
        contactTimer = 0f;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerTouching = false;
        contactTimer = 0f;
    }
}
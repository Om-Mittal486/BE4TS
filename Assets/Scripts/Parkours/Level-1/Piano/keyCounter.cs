using UnityEngine;

public class PianoKeyCounter : MonoBehaviour
{
    public static PianoKeyCounter Instance;

    [Header("Key Goal")]
    [SerializeField] private int requiredKeys = 9;

    [Header("Platform Movement")]
    [SerializeField] private Transform platform;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeIntensity = 0.08f;
    [SerializeField] private float shakeDuration = 1.2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip moveLoop;
    [SerializeField][Range(0f,1f)] private float volume = 1f;

    [SerializeField] private float fadeInSpeed = 3f;   // ✅ NEW
    [SerializeField] private float fadeOutSpeed = 3f;

    private int keyCount;
    private bool platformMoving;

    private Vector3 lastPosition;
    private bool wasMovingLastFrame;

    private Coroutine fadeCoroutine; // ✅ prevent overlap

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        lastPosition = platform.position;

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.volume = 0f; // start silent for fade-in
        }
    }

    void Update()
    {
        HandleMovement();
        HandleAudio();

        lastPosition = platform.position;
    }

    void HandleMovement()
    {
        if (!platformMoving) return;

        platform.position = Vector3.MoveTowards(
            platform.position,
            targetPosition.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(platform.position, targetPosition.position) < 0.001f)
        {
            platform.position = targetPosition.position;
            platformMoving = false;
        }
    }

    void HandleAudio()
    {
        if (audioSource == null || moveLoop == null) return;

        float movement = Vector3.Distance(platform.position, lastPosition);
        bool isMoving = movement > 0.0001f;

        // ▶️ Start with fade-in
        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.clip = moveLoop;
            audioSource.volume = 0f;
            audioSource.Play();

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeIn());
        }

        // 🔉 Fade-out when stopping
        if (!isMoving && wasMovingLastFrame)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOutAndStop());
        }

        wasMovingLastFrame = isMoving;
    }

    System.Collections.IEnumerator FadeIn()
    {
        while (audioSource.volume < volume)
        {
            audioSource.volume += fadeInSpeed * Time.deltaTime;
            yield return null;
        }

        audioSource.volume = volume;
    }

    System.Collections.IEnumerator FadeOutAndStop()
    {
        while (audioSource.volume > 0.01f)
        {
            audioSource.volume -= fadeOutSpeed * Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 0f;
    }

    public void RegisterKey()
    {
        keyCount++;
        Debug.Log("Keys stepped: " + keyCount);

        if (keyCount == requiredKeys)
        {
            platformMoving = true;

            CameraFollow2D cam = FindObjectOfType<CameraFollow2D>();
            if (cam != null)
                cam.Shake(shakeIntensity, shakeDuration);
        }
    }

    public int GetCount()
    {
        return keyCount;
    }
}
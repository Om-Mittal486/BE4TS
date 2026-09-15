using UnityEngine;

public class BarAudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BarGrowth barGrowth;
    [SerializeField] private Transform player;
    [SerializeField] private Collider2D barCollider;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip growSound;
    [SerializeField] private AudioClip shrinkSound;
    [SerializeField][Range(0f, 1f)] private float maxVolume = 1f;

    [SerializeField] private float fadeSpeed = 10f;

    [Header("Release Settings")]
    [SerializeField] private float releaseFadeSpeed = 3f;
    [SerializeField] private float stopDelay = 0.1f;

    [Header("Distance Settings")]
    [SerializeField] private float maxHearingDistance = 8f;
    [SerializeField] private float minHearingDistance = 0.5f;

    private float stopTimer = 0f;

    void Start()
    {
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.volume = 0f;
        }
    }

    void Update()
    {
        if (barGrowth == null) return;

        HandleAudio();
    }

    void HandleAudio()
    {
        if (audioSource == null || player == null || barCollider == null)
            return;

        // 📏 Distance (nearest point)
        Vector2 closestPoint = barCollider.ClosestPoint(player.position);
        float distance = Vector2.Distance(closestPoint, player.position);

        if (distance == 0f)
            distance = Vector2.Distance(transform.position, player.position);

        // 🎚 Distance factor
        float distanceFactor = 0f;

        if (distance <= minHearingDistance)
            distanceFactor = 1f;
        else if (distance <= maxHearingDistance)
            distanceFactor = 1f - Mathf.InverseLerp(minHearingDistance, maxHearingDistance, distance);
        else
            distanceFactor = 0f;

        float scaleX = barGrowth.CurrentScaleX;

        // 📈 Progress
        float progress = Mathf.InverseLerp(
            barGrowth.MinScaleX,
            barGrowth.MaxScaleX,
            scaleX
        );

        var state = barGrowth.CurrentState;

        // 🔊 Play correct sound
        if (state == BarGrowth.State.Growing && growSound != null)
        {
            stopTimer = 0f;

            if (audioSource.clip != growSound)
            {
                audioSource.clip = growSound;
                audioSource.Play();
            }
        }
        else if (state == BarGrowth.State.Shrinking && shrinkSound != null)
        {
            stopTimer = 0f;

            if (audioSource.clip != shrinkSound)
            {
                audioSource.clip = shrinkSound;
                audioSource.Play();
            }
        }

        float targetVolume = progress * maxVolume * distanceFactor;

        if (state != BarGrowth.State.Idle)
        {
            // 🎚 Active phase
            audioSource.volume = Mathf.MoveTowards(
                audioSource.volume,
                targetVolume,
                fadeSpeed * Time.deltaTime
            );

            audioSource.pitch = Mathf.Lerp(0.9f, 1.1f, progress);
        }
        else
        {
            // 🧊 Release phase
            stopTimer += Time.deltaTime;

            audioSource.volume = Mathf.MoveTowards(
                audioSource.volume,
                0f,
                releaseFadeSpeed * Time.deltaTime
            );

            audioSource.pitch = Mathf.Lerp(audioSource.pitch, 0.85f, Time.deltaTime * 2f);

            if (stopTimer >= stopDelay &&
                audioSource.volume <= 0.01f &&
                audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMAutoMute : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float normalVolume = 0.5f;
    [SerializeField] private float mutedVolume = 0f;
    [SerializeField] private float smoothSpeed = 5f;

    private AudioSource bgmSource;
    private float targetVolume;

    private void Awake()
    {
        bgmSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        targetVolume = normalVolume;
        bgmSource.volume = normalVolume;
    }

    private void Update()
    {
        // Check all audio sources in scene
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();

        bool otherAudioPlaying = false;

        foreach (AudioSource src in allSources)
        {
            if (src != bgmSource && src.isPlaying)
            {
                otherAudioPlaying = true;
                break;
            }
        }

        // Set target volume
        targetVolume = otherAudioPlaying ? mutedVolume : normalVolume;

        // Smooth transition
        bgmSource.volume = Mathf.Lerp(
            bgmSource.volume,
            targetVolume,
            Time.deltaTime * smoothSpeed
        );
    }
}
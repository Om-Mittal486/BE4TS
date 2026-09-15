using System.IO;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class CutsceneEndLoader : MonoBehaviour
{
    [Header("Video Settings")]
    [Tooltip("Name of the video file inside Assets/StreamingAssets (e.g., 'Background.mp4' or 'Background')")]
    [SerializeField] private string videoFileName = "Background.mp4";

    [Tooltip("Optional direct VideoClip fallback if URL streaming is not used")]
    [SerializeField] private VideoClip fallbackVideoClip;

    [Tooltip("Whether to loop the video")]
    [SerializeField] private bool loopVideo = true;

    [Tooltip("Optional toggle to lock cursor (keep disabled for menus with UI input)")]
    [SerializeField] private bool lockCursor = false;

    private VideoPlayer videoPlayer;
    private AudioSource audioSource;

    void Awake()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();

        ConfigureVideoPlayer();
    }

    void Start()
    {
        PlayVideoFromStreamingAssets();
    }

    private void ConfigureVideoPlayer()
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.skipOnDrop = true;
        videoPlayer.isLooping = loopVideo;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane; // Or CameraFarPlane / RenderTexture depending on setup

        // Hook callbacks
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;

        // Configure audio safely
        if (audioSource != null)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
        else
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        }
    }

    public void PlayVideoFromStreamingAssets()
    {
        string resolvedPath = GetStreamingAssetsVideoPath(videoFileName);

        if (!string.IsNullOrEmpty(resolvedPath))
        {
            Debug.Log($"[VideoPlayer] Loading video from: {resolvedPath}");
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = resolvedPath;
            videoPlayer.Prepare();
        }
        else if (fallbackVideoClip != null)
        {
            Debug.Log("[VideoPlayer] Using fallback VideoClip asset.");
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = fallbackVideoClip;
            videoPlayer.Prepare();
        }
        else
        {
            Debug.LogError($"[VideoPlayer] Video file '{videoFileName}' not found in StreamingAssets and no fallback clip assigned!");
        }
    }

    private string GetStreamingAssetsVideoPath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = "Background.mp4";

        string cleanName = fileName.Trim();

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL: StreamingAssets is served via HTTP URL
        string webUrl = Application.streamingAssetsPath + "/" + cleanName;
        webUrl = webUrl.Replace("\\", "/");
        return webUrl;
#else
        // Editor / Standalone Windows, Mac, Linux
        string streamingDir = Application.streamingAssetsPath;
        string directPath = Path.Combine(streamingDir, cleanName);

        if (File.Exists(directPath))
            return directPath;

        // Try checking common extensions if user omitted or changed extension (.mp4, .webm, .ogv)
        string nameWithoutExt = Path.GetFileNameWithoutExtension(cleanName);
        string[] candidateExtensions = { ".mp4", ".webm", ".ogv", ".mov" };

        foreach (string ext in candidateExtensions)
        {
            string candidate = Path.Combine(streamingDir, nameWithoutExt + ext);
            if (File.Exists(candidate))
                return candidate;
        }

        // Check if file exists directly in StreamingAssets as Background.mp4
        string defaultCandidate = Path.Combine(streamingDir, "Background.mp4");
        if (File.Exists(defaultCandidate))
            return defaultCandidate;

        return directPath;
#endif
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("[VideoPlayer] Video prepared successfully. Playing...");

        // Safely enable audio track if present
        if (vp.audioTrackCount > 0 && audioSource != null)
        {
            vp.EnableAudioTrack(0, true);
            vp.SetTargetAudioSource(0, audioSource);
        }

        vp.Play();
    }

    private void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogWarning($"[VideoPlayer] Error loading video: {message}");

        if (fallbackVideoClip != null && vp.source != VideoSource.VideoClip)
        {
            Debug.Log("[VideoPlayer] Attempting fallback VideoClip playback...");
            vp.source = VideoSource.VideoClip;
            vp.clip = fallbackVideoClip;
            vp.Prepare();
        }
    }
}

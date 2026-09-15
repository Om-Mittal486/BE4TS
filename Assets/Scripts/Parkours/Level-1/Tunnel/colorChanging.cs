using UnityEngine;

public class ChangeColorOnContact : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private bool revertOnExit = true;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip touchSound;
    [SerializeField][Range(0f, 1f)] private float volume = 1f;

    [Header("Pro Audio Variation")]
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private bool playerTouching;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        Color desiredColor = playerTouching ? targetColor : originalColor;

        // 🔥 Pro smooth interpolation
        spriteRenderer.color = Color.Lerp(
            spriteRenderer.color,
            desiredColor,
            1f - Mathf.Exp(-transitionSpeed * Time.deltaTime)
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerTouching = true;

        // 🔊 Play sound with pitch variation (pro tip)
        if (audioSource != null && touchSound != null)
        {
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSource.PlayOneShot(touchSound, volume);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (revertOnExit)
            playerTouching = false;
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class LiftPlatform : MonoBehaviour
{
    [Header("Lift Settings")]
    [SerializeField] private float liftHeight = 5f;
    [SerializeField] private float liftSpeed = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource moveSound;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 targetPos;

    private bool shouldLift;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (moveSound == null)
            moveSound = GetComponent<AudioSource>();

        startPos = rb.position;
        targetPos = startPos + Vector2.up * liftHeight;
    }

    void FixedUpdate()
    {
        if (!shouldLift) return;

        // Play sound when movement starts
        if (!moveSound.isPlaying)
        {
            moveSound.Play();
        }

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            targetPos,
            liftSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        // Stop when reached target
        if (Vector2.Distance(rb.position, targetPos) < 0.01f)
        {
            shouldLift = false;

            if (moveSound.isPlaying)
                moveSound.Stop();
        }
    }

    // 🔥 CALLED FROM OTHER SCRIPT
    public void StartLift()
    {
        Debug.Log("Lift triggered");
        shouldLift = true;
    }
}
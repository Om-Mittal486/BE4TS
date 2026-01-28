using UnityEngine;

public class PianoPlatform : MonoBehaviour
{
    [Header("Sink Settings")]
    [SerializeField] private float sinkDistance = 0.25f;
    [SerializeField] private float sinkSpeed = 6f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 sinkPos;

    private Transform player;

    private bool counted; // 🔒 only once

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = rb.position;
        sinkPos = startPos + Vector2.down * sinkDistance;
    }

    void FixedUpdate()
    {
        Vector2 target = player != null ? sinkPos : startPos;

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            target,
            sinkSpeed * Time.fixedDeltaTime
        );

        Vector2 delta = newPos - rb.position;

        rb.MovePosition(newPos);

        if (player != null)
        {
            player.position += (Vector3)delta;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        player = collision.transform;

        // ✅ count ONLY first time
        if (!counted)
        {
            counted = true;
            PianoKeyCounter.Instance.RegisterKey();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = null;
        }
    }
}

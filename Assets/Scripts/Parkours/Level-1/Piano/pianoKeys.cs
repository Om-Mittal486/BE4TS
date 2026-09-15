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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.transform;

        if (!counted)
        {
            counted = true;
            PianoKeyCounter.Instance.RegisterKey();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = null;
    }
}
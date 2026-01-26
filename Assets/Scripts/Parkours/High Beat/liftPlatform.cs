using UnityEngine;

public class LiftPlatform : MonoBehaviour
{
    [Header("Lift Settings")]
    [SerializeField] private float liftHeight = 5f;
    [SerializeField] private float liftSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 targetPos;

    private bool shouldLift;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startPos = rb.position;
        targetPos = startPos + Vector2.up * liftHeight;
    }

    void FixedUpdate()
    {
        if (!shouldLift) return;

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            targetPos,
            liftSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);
    }

    // 🔥 CALLED FROM OTHER SCRIPT
    public void StartLift()
    {
        Debug.Log("Lift triggered");
        shouldLift = true;
    }
}

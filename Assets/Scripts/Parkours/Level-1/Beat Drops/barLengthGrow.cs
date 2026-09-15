using UnityEngine;

public class BarGrowth : MonoBehaviour
{
    [Header("Growth Settings")]
    [SerializeField] private float requiredContactTime = 0.1f;
    [SerializeField] private Vector3 maxScale = new Vector3(1.4f, 1.4f, 1f);
    [SerializeField] private float growSpeed = 5f;

    private Vector3 originalScale;
    private float contactTimer;
    private bool playerTouching;

    private float lastScaleX;

    public enum State { Idle, Growing, Shrinking }
    public State CurrentState { get; private set; }

    public float CurrentScaleX => transform.localScale.x;
    public float MinScaleX => originalScale.x;
    public float MaxScaleX => maxScale.x;

    void Start()
    {
        originalScale = transform.localScale;
        lastScaleX = transform.localScale.x;
    }

    void Update()
    {
        // Growth logic
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

        // Detect state
        float currentScaleX = transform.localScale.x;

        if (currentScaleX > lastScaleX + 0.0001f)
            CurrentState = State.Growing;
        else if (currentScaleX < lastScaleX - 0.0001f)
            CurrentState = State.Shrinking;
        else
            CurrentState = State.Idle;

        lastScaleX = currentScaleX;
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
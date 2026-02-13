using UnityEngine;

public class GrowOnContact : MonoBehaviour
{
    [Header("Growth Settings")]
    [SerializeField] private float requiredContactTime = 0.1f;

    [SerializeField] private Vector3 maxScale = new Vector3(1.4f, 1.4f, 1f);
    [SerializeField] private float growSpeed = 5f;

    private Vector3 originalScale;
    private float contactTimer;
    private bool playerTouching;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
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
            // smoothly return to original scale
            transform.localScale = Vector3.MoveTowards(
                transform.localScale,
                originalScale,
                growSpeed * Time.deltaTime
            );
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTouching = true;
            contactTimer = 0f;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTouching = false;
            contactTimer = 0f;
        }
    }
}

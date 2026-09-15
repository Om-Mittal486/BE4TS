using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float bottomY;
    [SerializeField] private float topY;
    [SerializeField] private float moveSpeed = 1.5f;

    private Rigidbody2D rb;
    private int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 pos = rb.position;
        pos.y += moveSpeed * direction * Time.fixedDeltaTime;

        if (pos.y >= topY)
        {
            pos.y = topY;
            direction = -1;
        }
        else if (pos.y <= bottomY)
        {
            pos.y = bottomY;
            direction = 1;
        }

        rb.MovePosition(pos);
    }
}

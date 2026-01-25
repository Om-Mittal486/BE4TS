using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 14f;

    [Header("Jump Rotation")]
    [SerializeField] private float rotationDuration = 0.3f;

    private Rigidbody2D rb;
    private float moveInput;

    private int groundContacts;

    // rotation
    private bool isRotating;
    private float rotationTimer;
    private float startZ;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startZ = transform.eulerAngles.z;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        // REAL JUMP EVENT
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && groundContacts > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // 🔥 ROTATION TRIGGER — works on moving platforms too
            isRotating = true;
            rotationTimer = 0f;
        }

        if (isRotating)
        {
            rotationTimer += Time.deltaTime;
            float t = rotationTimer / rotationDuration;

            float z = Mathf.Lerp(startZ, startZ + 180f, t);
            transform.rotation = Quaternion.Euler(0, 0, z);

            if (t >= 1f)
            {
                startZ += 180f;
                transform.rotation = Quaternion.Euler(0, 0, startZ);
                isRotating = false;
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            groundContacts++;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            groundContacts--;
    }
}

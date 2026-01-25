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

    // grounded handling
    private int groundContacts;

    // jump handling
    private bool jumpRequest;

    // rotation
    private bool isRotating;
    private float rotationTimer;
    private float currentZ;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // very important for smooth visuals
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // keep physics stable
        rb.freezeRotation = true;

        currentZ = rb.rotation;
    }

    void Update()
    {
        // INPUT ONLY
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            jumpRequest = true;
        }

        // VISUAL ROTATION (smooth + jitter-free)
        if (isRotating)
        {
            rotationTimer += Time.deltaTime;
            float t = rotationTimer / rotationDuration;

            float z = Mathf.Lerp(currentZ, currentZ + 180f, t);
            rb.SetRotation(z);

            if (t >= 1f)
            {
                currentZ += 180f;
                rb.SetRotation(currentZ);
                isRotating = false;
            }
        }
    }

    void FixedUpdate()
    {
        // horizontal movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // jump happens in physics step
        if (jumpRequest && groundContacts > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // trigger rotation exactly when jump happens
            isRotating = true;
            rotationTimer = 0f;
        }

        jumpRequest = false;
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

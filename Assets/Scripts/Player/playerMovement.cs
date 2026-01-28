using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 14f;

    [Header("Rotation Durations")]
    [SerializeField] private float jumpRotationDuration = 0.3f;
    [SerializeField] private float springRotationDuration = 0.8f;

    private Rigidbody2D rb;
    private float moveInput;

    // grounded
    private int groundContacts;

    // jump
    private bool jumpRequest;

    // rotation
    private bool isRotating;
    private float rotationTimer;
    private float currentZ;
    private float rotationAmount;
    private float currentRotationDuration;

    // input lock
    private bool inputLocked;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;

        currentZ = rb.rotation;
    }

    void Update()
    {
        if (!inputLocked)
        {
            moveInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
                jumpRequest = true;
        }

        // 🔁 ROTATION HANDLER
        if (isRotating)
        {
            rotationTimer += Time.deltaTime;
            float t = rotationTimer / currentRotationDuration;

            float targetZ = currentZ - rotationAmount; // clockwise
            float z = Mathf.Lerp(currentZ, targetZ, t);

            rb.SetRotation(z);

            if (t >= 1f)
            {
                currentZ = targetZ;
                rb.SetRotation(currentZ);
                isRotating = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (!inputLocked)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        // 🟦 normal jump → 180°
        if (!inputLocked && jumpRequest && groundContacts > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            rotationAmount = 180f;
            currentRotationDuration = jumpRotationDuration;

            isRotating = true;
            rotationTimer = 0f;
        }

        jumpRequest = false;
    }

    // 🔥 spring / piston launch → 360°
    public void Launch(Vector2 force)
    {
        inputLocked = true;
        rb.linearVelocity = force;

        rotationAmount = 360f;
        currentRotationDuration = springRotationDuration;

        isRotating = true;
        rotationTimer = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContacts++;
            inputLocked = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContacts--;
        }
    }
}

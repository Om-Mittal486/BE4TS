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

    // rotation variables
    private bool isRotating;
    private bool hasRotatedThisJump;
    private float rotationTimer;
    private float currentZ;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentZ = transform.eulerAngles.z;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        // JUMP
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && groundContacts > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // start rotation
            if (!hasRotatedThisJump)
            {
                isRotating = true;
                hasRotatedThisJump = true;
                rotationTimer = 0f;
            }
        }

        // RESET when grounded
        if (groundContacts > 0 && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            hasRotatedThisJump = false;
        }

        // ROTATION LOGIC
        if (isRotating)
        {
            rotationTimer += Time.deltaTime;
            float t = rotationTimer / rotationDuration;

            float z = Mathf.Lerp(currentZ, currentZ + 180f, t);
            transform.rotation = Quaternion.Euler(0f, 0f, z);

            if (t >= 1f)
            {
                currentZ += 180f;
                transform.rotation = Quaternion.Euler(0f, 0f, currentZ);
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

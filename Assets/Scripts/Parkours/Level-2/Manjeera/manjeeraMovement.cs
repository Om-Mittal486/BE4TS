using UnityEngine;

public class SmoothMovingPlatform : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform targetPoint;

    [Header("Motion")]
    [SerializeField] private float moveTime = 2f;

    [Header("Pause (only at start)")]
    [SerializeField] private float pauseTime = 0.5f;

    [Header("Activation")]
    [SerializeField] private bool isActive = true;

    [Header("Camera Shake (Distance Based)")]
    [SerializeField] private bool enableCameraShake = true;
    [SerializeField] private float maxShakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float maxShakeDistance = 10f;

    [Header("References")]
    [SerializeField] private Transform player; // assign Player transform

    private Vector3 startPos;

    private float moveTimer;
    private float pauseTimer;

    private bool goingToTarget = true;
    private bool isPaused;
    private bool shakeTriggered;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!isActive) return;
        if (targetPoint == null) return;

        // ⏸ pause ONLY at original position
        if (isPaused)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseTime)
            {
                pauseTimer = 0f;
                isPaused = false;
                moveTimer = 0f;
                goingToTarget = true;
                shakeTriggered = false;
            }
            return;
        }

        // ▶ movement
        moveTimer += Time.deltaTime / moveTime;
        float t = Mathf.SmoothStep(0f, 1f, moveTimer);

        if (goingToTarget)
        {
            transform.position = Vector3.Lerp(startPos, targetPoint.position, t);

            // reached TARGET → camera shake (distance based)
            if (moveTimer >= 1f)
            {
                TriggerCameraShake();

                moveTimer = 0f;
                goingToTarget = false;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(targetPoint.position, startPos, t);

            // reached START → pause
            if (moveTimer >= 1f)
            {
                isPaused = true;
            }
        }
    }

    // 🔥 Distance-based camera shake
    void TriggerCameraShake()
    {
        if (!enableCameraShake || shakeTriggered) return;
        if (player == null) return;

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance >= maxShakeDistance)
            return; // too far → no shake

        float factor = 1f - (distance / maxShakeDistance);
        float finalIntensity = maxShakeIntensity * factor;

        if (finalIntensity <= 0.001f) return;

        CameraFollow2D cam = FindObjectOfType<CameraFollow2D>();
        if (cam != null)
        {
            cam.Shake(finalIntensity, shakeDuration);
        }

        shakeTriggered = true;
    }

    // 🔁 Called by trigger
    public void ToggleActive()
    {
        isActive = !isActive;
        Debug.Log("Platform active: " + isActive);
    }
}

using UnityEngine;

public class SmoothMovingPlatform : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform targetPoint;

    [Header("Motion")]
    [SerializeField] private float moveTime = 2f;

    [Header("Pause (only at start)")]
    [SerializeField] private float pauseTime = 0.5f;

    private Vector3 startPos;

    private float moveTimer;
    private float pauseTimer;

    private bool goingToTarget = true;
    private bool isPaused;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
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
                goingToTarget = true; // always go to target after pause
            }
            return;
        }

        // ▶ movement
        moveTimer += Time.deltaTime / moveTime;
        float t = Mathf.SmoothStep(0f, 1f, moveTimer);

        if (goingToTarget)
        {
            transform.position = Vector3.Lerp(startPos, targetPoint.position, t);

            // reached target → immediately go back (NO pause)
            if (moveTimer >= 1f)
            {
                moveTimer = 0f;
                goingToTarget = false;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(targetPoint.position, startPos, t);

            // reached start → PAUSE
            if (moveTimer >= 1f)
            {
                isPaused = true;
            }
        }
    }
}

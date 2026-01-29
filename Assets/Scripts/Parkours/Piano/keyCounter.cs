using UnityEngine;

public class PianoKeyCounter : MonoBehaviour
{
    public static PianoKeyCounter Instance;

    [Header("Key Goal")]
    [SerializeField] private int requiredKeys = 9;

    [Header("Platform Movement")]
    [SerializeField] private Transform platform;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeIntensity = 0.08f;
    [SerializeField] private float shakeDuration = 1.2f;

    private int keyCount;
    private bool platformMoving;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!platformMoving) return;

        platform.position = Vector3.MoveTowards(
            platform.position,
            targetPosition.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(platform.position, targetPosition.position) < 0.01f)
        {
            platformMoving = false;
        }
    }

    public void RegisterKey()
    {
        keyCount++;
        Debug.Log("Keys stepped: " + keyCount);

        if (keyCount == requiredKeys)
        {
            platformMoving = true;

            CameraFollow2D cam = FindObjectOfType<CameraFollow2D>();
            if (cam != null)
                cam.Shake(shakeIntensity, shakeDuration);
        }
    }

    public int GetCount()
    {
        return keyCount;
    }
}

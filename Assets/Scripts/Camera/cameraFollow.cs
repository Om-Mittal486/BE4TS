using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private Vector3 offset;

    [Header("Follow Speeds")]
    [SerializeField] private float horizontalSpeed = 8f;
    [SerializeField] private float verticalUpSpeed = 3f;
    [SerializeField] private float verticalDownSpeed = 10f;

    [Header("Camera Borders")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    // shake
    private float shakeTimer;
    private float shakeIntensity;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            offset.z
        );

        Vector3 current = transform.position;

        // horizontal follow
        current.x = Mathf.Lerp(current.x, desired.x, horizontalSpeed * Time.deltaTime);

        // vertical dampening
        float verticalSpeed =
            desired.y > current.y ? verticalUpSpeed : verticalDownSpeed;

        current.y = Mathf.Lerp(current.y, desired.y, verticalSpeed * Time.deltaTime);

        // clamp
        current.x = Mathf.Clamp(current.x, minX, maxX);
        current.y = Mathf.Clamp(current.y, minY, maxY);

        transform.position = current;

        // 🔥 CAMERA SHAKE
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            Vector2 shake = Random.insideUnitCircle * shakeIntensity;
            transform.position += (Vector3)shake;
        }
    }

    // called by other scripts
    public void Shake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeTimer = duration;
    }
}

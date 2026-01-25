using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private Vector3 offset;

    [Header("Follow")]
    [SerializeField] private float followSpeed = 5f;

    [Header("Vertical Dampening")]
    [Range(0f, 1f)]
    [SerializeField] private float verticalMultiplier = 0.5f;

    [Header("Camera Borders")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y * verticalMultiplier + offset.y,
            offset.z
        );

        // smooth follow
        Vector3 smoothPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // clamp camera inside borders
        smoothPosition.x = Mathf.Clamp(smoothPosition.x, minX, maxX);
        smoothPosition.y = Mathf.Clamp(smoothPosition.y, minY, maxY);

        transform.position = smoothPosition;
    }
}

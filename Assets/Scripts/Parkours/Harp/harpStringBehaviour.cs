using UnityEngine;

public class HarpStringOscillator : MonoBehaviour
{
    [Header("Oscillation Settings")]
    [SerializeField] private float shakeIntensity = 0.15f;
    [SerializeField] private float shakeSpeed = 25f;
    [SerializeField] private float damping = 4f;

    private Vector3 startLocalPos;
    private float currentIntensity;
    private float time;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (currentIntensity <= 0.001f)
            return;

        time += Time.deltaTime * shakeSpeed;

        float offset = Mathf.Sin(time) * currentIntensity;

        transform.localPosition = startLocalPos + Vector3.right * offset;

        // smooth decay
        currentIntensity = Mathf.Lerp(currentIntensity, 0f, damping * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // reset vibration
        currentIntensity = shakeIntensity;
        time = 0f;
    }
}

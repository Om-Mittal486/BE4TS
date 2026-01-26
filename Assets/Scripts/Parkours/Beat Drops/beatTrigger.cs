using UnityEngine;
using System.Collections;

public class PlatformEntranceTrigger : MonoBehaviour
{
    [Header("Platforms in Order")]
    [SerializeField] private PushPlatform[] platforms;

    [SerializeField] private float delayBetween = 0.4f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.12f;
    [SerializeField] private float shakeStrength = 0.15f;

    private bool triggered;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;

        foreach (var p in platforms)
        {
            p.OnReachedDestination += TriggerShake;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(PlayEntrance());
    }

    IEnumerator PlayEntrance()
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].PushIn();
            yield return new WaitForSeconds(delayBetween);
        }
    }

    void TriggerShake()
    {
        StartCoroutine(CameraShake());
    }

    IEnumerator CameraShake()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            Vector3 basePos = cam.position;

            Vector2 offset = Random.insideUnitCircle * shakeStrength;
            cam.position = basePos + new Vector3(offset.x, offset.y, 0);

            timer += Time.deltaTime;
            yield return null;
        }
    }
}

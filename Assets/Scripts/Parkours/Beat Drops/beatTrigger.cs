using UnityEngine;
using System.Collections;

public class PlatformEntranceTrigger : MonoBehaviour
{
    [Header("Platforms in Order")]
    [SerializeField] private PushPlatform[] platforms;

    [SerializeField] private float delayBetween = 0.4f;

    private bool triggered;

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
}

using UnityEngine;

public class PlatformLifter : MonoBehaviour
{
    [SerializeField] private LiftPlatform platformToLift;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (platformToLift != null)
            {
                platformToLift.StartLift();
            }
        }
    }
}

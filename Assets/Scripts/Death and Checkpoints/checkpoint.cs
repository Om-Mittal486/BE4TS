using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        HardRespawn respawn = other.GetComponent<HardRespawn>();
        if (respawn == null) return;

        respawn.SetRespawnPoint(respawnPoint);

        // optional: disable checkpoint after use
        // gameObject.SetActive(false);
    }
}

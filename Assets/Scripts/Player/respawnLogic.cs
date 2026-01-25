using UnityEngine;
using System.Collections;

public class HardRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 0.5f;

    private Rigidbody2D rb;
    private bool isRespawning;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Respawn()
    {
        if (isRespawning) return;
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        yield return new WaitForSeconds(respawnDelay);

        transform.position = respawnPoint.position;

        // 🔥 RESET PARKOUR COUNTER
        MovingPlatformSequence.HardResetSequence();

        rb.simulated = true;
        isRespawning = false;
    }
}

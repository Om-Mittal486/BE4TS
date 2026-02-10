using UnityEngine;
using System.Collections;

public class HardRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 0.5f;

    private Rigidbody2D rb;
    private PlayerMovement2D movement;
    private bool isRespawning;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement2D>();
    }

    public void SetRespawnPoint(Transform newPoint)
    {
        respawnPoint = newPoint;
    }

    public void Respawn()
    {
        if (isRespawning) return;
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        // stop physics
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;

        yield return new WaitForSeconds(respawnDelay);

        // reset position
        transform.position = respawnPoint.position;

        // 🔥 RESET ROTATION COMPLETELY
        rb.rotation = 0f;
        transform.rotation = Quaternion.identity;

        // 🔥 reset player movement rotation state
        if (movement != null)
        {
            movement.ResetRotationState();
        }

        // reset parkour / puzzles
        MovingPlatformSequence.HardResetSequence();

        // re-enable physics
        rb.simulated = true;
        isRespawning = false;
    }
}

using UnityEngine;
using System.Collections;

public class MovingPlatformSequence : MonoBehaviour
{
    [Header("Platform Order")]
    [SerializeField] private int platformIndex;

    [Header("Mini Respawn")]
    [SerializeField] private Transform miniRespawnPoint;
    [SerializeField] private float respawnDelay = 0.6f;

    [Header("Contact Requirement")]
    [SerializeField] private float requiredContactTime = 0.5f;

    private static int currentIndex = 1;
    private static int successCount = 0;

    private bool alreadyUsed;
    private bool playerOnPlatform;
    private float contactTimer;

    private static bool isRespawning;

    void Update()
    {
        if (playerOnPlatform)
        {
            contactTimer += Time.deltaTime;

            if (contactTimer >= requiredContactTime && !alreadyUsed)
            {
                ValidatePlatform();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isRespawning) return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerOnPlatform = true;
        contactTimer = 0f;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerOnPlatform = false;
        contactTimer = 0f;
    }

    void ValidatePlatform()
    {
        alreadyUsed = true;
        playerOnPlatform = false;

        // ❌ wrong platform
        if (platformIndex != currentIndex)
        {
            StartCoroutine(MiniRespawn());
            return;
        }

        // ✅ correct platform
        successCount++;
        currentIndex++;

        Debug.Log("Correct platform after hold: " + platformIndex);
    }

    IEnumerator MiniRespawn()
    {
        isRespawning = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        yield return new WaitForSeconds(respawnDelay);

        // reset sequence
        currentIndex = 1;
        successCount = 0;
        ResetAllPlatforms();

        player.transform.position = miniRespawnPoint.position;

        rb.simulated = true;
        isRespawning = false;
    }

    void ResetAllPlatforms()
    {
        MovingPlatformSequence[] platforms =
            FindObjectsOfType<MovingPlatformSequence>();

        foreach (var p in platforms)
        {
            p.alreadyUsed = false;
            p.contactTimer = 0f;
            p.playerOnPlatform = false;
        }
    }

    public static int GetCount()
    {
        return successCount;
    }

    public static void HardResetSequence()
    {
        currentIndex = 1;
        successCount = 0;

        MovingPlatformSequence[] platforms =
            FindObjectsOfType<MovingPlatformSequence>();

        foreach (var p in platforms)
        {
            p.alreadyUsed = false;
            p.contactTimer = 0f;
            p.playerOnPlatform = false;
        }
    }

}

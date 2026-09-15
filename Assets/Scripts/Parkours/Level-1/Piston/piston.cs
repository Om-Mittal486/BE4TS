using UnityEngine;

public class Piston : MonoBehaviour
{
    [Header("Piston Movement")]
    [SerializeField] private float pistonHeight = 3f;
    [SerializeField] private float extendSpeed = 6f;

    [Header("Player Push")]
    [SerializeField] private float pushDistance = 300f;
    [SerializeField] private float pushHeight = 16f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool shouldExtend;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * pistonHeight;
    }

    void Update()
    {
        if (!shouldExtend) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            extendSpeed * Time.deltaTime
        );
    }

    public void ActivatePiston()
    {
        shouldExtend = true;
    }

    public void PushPlayer(PlayerMovement2D player)
    {
        Vector2 force = new Vector2(pushDistance, pushHeight);
        player.Launch(force);
    }
}

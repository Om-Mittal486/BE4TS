using UnityEngine;

public class PistonTrigger : MonoBehaviour
{
    private Piston piston;

    void Start()
    {
        piston = GetComponentInParent<Piston>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement2D player =
            other.GetComponentInParent<PlayerMovement2D>();

        if (player == null) return;

        piston.ActivatePiston();
        piston.PushPlayer(player);
    }
}

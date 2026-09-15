using UnityEngine;

public class PlayerBeatStick : MonoBehaviour
{
    private MusicNote currentBeat;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out MusicNote beat))
        {
            currentBeat = beat;
            beat.AttachPlayer(transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out MusicNote beat))
        {
            if (currentBeat == beat)
            {
                beat.DetachPlayer();
                currentBeat = null;
            }
        }
    }

    void Update()
    {
        // Optional: detach on jump
        if (Input.GetKeyDown(KeyCode.Space) && currentBeat != null)
        {
            currentBeat.DetachPlayer();
            currentBeat = null;
        }
    }
}

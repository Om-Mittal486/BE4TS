using UnityEngine;

public class MusicNote : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private float speed;
    private float journeyLength;
    private float startTime;
    private bool isActive;

    private float curveHeight;
    private float curveFrequency;

    private Transform attachedPlayer;
    private Vector3 playerOffset;

    public void Init(Vector3 start, Vector3 end, float moveSpeed, float height = 0.5f, float frequency = 1f)
    {
        startPos = start;
        endPos = end;
        speed = moveSpeed;
        curveHeight = height;
        curveFrequency = frequency;

        journeyLength = Vector3.Distance(startPos, endPos);
        startTime = Time.time;

        transform.position = startPos;
        attachedPlayer = null;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        float t = (Time.time - startTime) * speed / journeyLength;

        if (t >= 1f)
        {
            DetachPlayer();
            isActive = false;
            MusicNotePool.Instance.ReturnToPool(this);
            return;
        }

        Vector3 pos = Vector3.Lerp(startPos, endPos, t);
        float curve = Mathf.Sin(t * Mathf.PI * curveFrequency) * curveHeight;
        pos.y += curve;

        transform.position = pos;

        if (attachedPlayer != null)
        {
            attachedPlayer.position = transform.position + playerOffset;
        }
    }

    public void AttachPlayer(Transform player)
    {
        attachedPlayer = player;
        playerOffset = player.position - transform.position;
    }

    public void DetachPlayer()
    {
        attachedPlayer = null;
    }
}

using UnityEngine;

public class MusicNoteSpawner : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    public float spawnInterval = 0.5f;
    public float noteSpeed = 2f;

    [Header("Curve Settings")]
    public float curveHeight = 0.6f;
    public float curveFrequency = 1f;

    [Header("Control")]
    public bool allowSpawning = true; // 🔥 ADD THIS

    private float timer;

    void Update()
    {
        if (!allowSpawning) return; // 🔥 STOP SPAWNING

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnNote();
            timer = 0f;
        }
    }

    void SpawnNote()
    {
        MusicNote note = MusicNotePool.Instance.GetNote();
        note.Init(
            startPoint.position,
            endPoint.position,
            noteSpeed,
            curveHeight,
            curveFrequency
        );
    }
}
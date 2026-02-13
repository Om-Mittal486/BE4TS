using System.Collections.Generic;
using UnityEngine;

public class MusicNotePool : MonoBehaviour
{
    public static MusicNotePool Instance;

    public MusicNote notePrefab;
    public int poolSize = 20;

    private Queue<MusicNote> pool = new Queue<MusicNote>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            MusicNote note = Instantiate(notePrefab, transform);
            note.gameObject.SetActive(false);
            pool.Enqueue(note);
        }
    }

    public MusicNote GetNote()
    {
        MusicNote note = pool.Count > 0 ? pool.Dequeue() : Instantiate(notePrefab);
        note.gameObject.SetActive(true);
        return note;
    }

    public void ReturnToPool(MusicNote note)
    {
        note.gameObject.SetActive(false);
        pool.Enqueue(note);
    }
}

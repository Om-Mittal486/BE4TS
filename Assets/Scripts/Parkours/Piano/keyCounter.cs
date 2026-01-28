using UnityEngine;

public class PianoKeyCounter : MonoBehaviour
{
    public static PianoKeyCounter Instance;

    private int keyCount;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterKey()
    {
        keyCount++;
        Debug.Log("Keys stepped: " + keyCount);
    }

    public int GetCount()
    {
        return keyCount;
    }
}

using UnityEngine;

public class BGMPersistent : MonoBehaviour
{
    private static BGMPersistent instance;

    private void Awake()
    {
        // If another BGM already exists → destroy this one
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Keep this one
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
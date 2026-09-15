using UnityEngine;

public class SimpleTrigger : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;

    private void Start()
    {
        if (objectToActivate != null)
            objectToActivate.SetActive(false); // optional (hide at start)
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (objectToActivate != null)
                objectToActivate.SetActive(true);
        }
    }
}
using UnityEngine;

public class XGlobalTrigger : MonoBehaviour
{
    private Animator[] animators;

    [SerializeField] private GameObject targetObject; // extra feature

    private bool hasTriggered = false;

    void Start()
    {
        // Auto-find all Animators in the scene
        animators = FindObjectsOfType<Animator>();
    }

    void Update()
    {
        if (hasTriggered) return;

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            // Trigger all animations
            foreach (Animator anim in animators)
            {
                if (anim != null)
                {
                    anim.SetTrigger("XPressed");
                }
            }

            // Activate the target object
            if (targetObject != null)
            {
                targetObject.SetActive(true);
            }

            hasTriggered = true; // one-time only
        }
    }
}
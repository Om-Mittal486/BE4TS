using UnityEngine;

public class XGlobalTrigger : MonoBehaviour
{
    private Animator[] animators;
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
            foreach (Animator anim in animators)
            {
                if (anim != null)
                {
                    anim.SetTrigger("XPressed");
                }
            }

            hasTriggered = true; // ensures it only happens once
        }
    }
}
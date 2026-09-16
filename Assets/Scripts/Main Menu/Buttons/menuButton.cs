using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] Animator animator;
    [SerializeField] AnimatorFunctions animatorFunctions;
    [SerializeField] int thisIndex;

    public int ThisIndex => thisIndex;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animatorFunctions == null)
            animatorFunctions = GetComponent<AnimatorFunctions>();

        if (menuButtonController == null)
            menuButtonController = FindObjectOfType<MenuButtonController>();
    }

    void Update()
    {
        if (menuButtonController != null && menuButtonController.index == thisIndex)
        {
            if (animator != null)
                animator.SetBool("isselected", true);

            // Press started
            if (Input.GetButtonDown("Submit"))
            {
                if (animator != null)
                    animator.SetBool("ispressed", true);
            }

            // Press released (perfect sync point)
            if (Input.GetButtonUp("Submit"))
            {
                if (animator != null)
                    animator.SetBool("ispressed", false);

                if (animatorFunctions != null)
                    animatorFunctions.disableOnce = true;

                // Notify action script
                SendMessage("OnMenuSubmit", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("isselected", false);
                animator.SetBool("ispressed", false);
            }
        }
    }

    // --- Pointer / Mouse Interaction ---
    public void OnPointerHoverEnter()
    {
        if (menuButtonController != null)
        {
            menuButtonController.index = thisIndex;
        }

        if (animator != null)
        {
            animator.SetBool("isselected", true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerHoverEnter();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.SetBool("ispressed", false);
        }

        if (animatorFunctions != null)
            animatorFunctions.disableOnce = true;

        SendMessage("OnMenuSubmit", SendMessageOptions.DontRequireReceiver);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonController : MonoBehaviour
{
    public int index;
    [SerializeField] bool keyDown;
    [SerializeField] int maxIndex = 2;
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        RefreshMaxIndex();
    }

    void OnEnable()
    {
        RefreshMaxIndex();
    }

    public int GetCurrentMaxIndex()
    {
        MenuButton[] buttons = FindObjectsOfType<MenuButton>();
        int highest = 0;
        foreach (var b in buttons)
        {
            if (b.isActiveAndEnabled && b.gameObject.activeInHierarchy)
            {
                if (b.ThisIndex > highest)
                    highest = b.ThisIndex;
            }
        }
        return Mathf.Max(highest, maxIndex);
    }

    public void RefreshMaxIndex()
    {
        maxIndex = GetCurrentMaxIndex();
    }

    public void SetMaxIndex(int max)
    {
        maxIndex = Mathf.Max(0, max);
    }

    void Update()
    {
        int effectiveMax = GetCurrentMaxIndex();

        float vert = Input.GetAxis("Vertical");
        if (vert != 0)
        {
            if (!keyDown)
            {
                int prevIndex = index;

                if (vert < 0)
                {
                    if (index < effectiveMax)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                else if (vert > 0)
                {
                    if (index > 0)
                    {
                        index--;
                    }
                    else
                    {
                        index = effectiveMax;
                    }
                }

                if (prevIndex != index && audioSource != null && audioSource.clip != null)
                {
                    audioSource.Play();
                }

                keyDown = true;
            }
        }
        else
        {
            keyDown = false;
        }
    }
}

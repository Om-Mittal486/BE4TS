using UnityEngine;
using System;

public class PushPlatform : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private Vector3 pushDirection = Vector3.right;
    [SerializeField] private float pushDistance = 5f;
    [SerializeField] private float pushSpeed = 4f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip reachSound;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;

    [Tooltip("Time before reaching target to play sound")]
    [SerializeField] private float playBeforeReachTime = 0.3f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isPushing;
    private bool hasPlayedSound;

    public Action OnReachedDestination;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + pushDirection.normalized * pushDistance;
    }

    void Update()
    {
        if (!isPushing) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            pushSpeed * Time.deltaTime
        );

        float remainingDistance = Vector3.Distance(transform.position, targetPos);
        float remainingTime = remainingDistance / pushSpeed;

        // 🔊 Play sound slightly before reaching
        if (!hasPlayedSound && remainingTime <= playBeforeReachTime)
        {
            if (audioSource != null && reachSound != null)
            {
                audioSource.PlayOneShot(reachSound, volume);
            }

            hasPlayedSound = true;
        }

        if (remainingDistance < 0.01f)
        {
            transform.position = targetPos;
            isPushing = false;

            OnReachedDestination?.Invoke();
        }
    }

    public void PushIn()
    {
        isPushing = true;
        hasPlayedSound = false; // reset for reuse
    }
}
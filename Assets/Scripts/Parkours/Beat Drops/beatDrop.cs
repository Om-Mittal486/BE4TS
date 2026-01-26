using UnityEngine;
using System;

public class PushPlatform : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private Vector3 pushDirection = Vector3.right;
    [SerializeField] private float pushDistance = 5f;
    [SerializeField] private float pushSpeed = 4f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isPushing;

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

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            isPushing = false;

            // 🔥 notify trigger
            OnReachedDestination?.Invoke();
        }
    }

    public void PushIn()
    {
        isPushing = true;
    }
}

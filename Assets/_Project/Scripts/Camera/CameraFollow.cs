using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -10f);

    [Header("Smooth Follow")]
    [SerializeField] private float minFollowSpeed = 2f;
    [SerializeField] private float maxFollowSpeed = 10f;
    [SerializeField] private float maxDistance = 6f;

    [Header("Dead Zone")]
    [SerializeField] private float stopDistance = 0.05f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= stopDistance)
            return;

        float distanceRatio = Mathf.Clamp01(distance / maxDistance);

        float currentFollowSpeed = Mathf.Lerp(
            minFollowSpeed,
            maxFollowSpeed,
            distanceRatio
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            currentFollowSpeed * Time.deltaTime
        );
    }
}
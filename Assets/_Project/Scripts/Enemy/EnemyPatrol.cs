using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float reachDistance = 0.1f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Transform currentTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentTarget = rightPoint;
    }

    private void FixedUpdate()
    {
        if(leftPoint == null || rightPoint == null) return;

        MoveToTarget();
        CheckArrived();
    }

    private void MoveToTarget()
    {
        float direction = Mathf.Sign(currentTarget.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        if (spriteRenderer != null) spriteRenderer.flipX = direction < 0;
    }

    private void CheckArrived()
    {
        float distance = Mathf.Abs(currentTarget.position.x - transform.position.x);
        if(distance <= reachDistance)
        {
            if (currentTarget == rightPoint) currentTarget = leftPoint;
            else currentTarget = rightPoint;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (leftPoint == null || rightPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        Gizmos.DrawWireSphere(leftPoint.position, 0.2f);
        Gizmos.DrawWireSphere(rightPoint.position, 0.2f);
    }
}
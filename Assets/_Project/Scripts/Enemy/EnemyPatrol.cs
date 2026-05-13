using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float reachDistance = 0.15f;

    [Header("Chase Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectRange = 4f;
    [SerializeField] private float chaseSpeed = 3f;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 0.9f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float attackHitDelay = 0.25f;
    [SerializeField] private float attackMotionDuration = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Transform currentPatrolTarget;
    private Transform playerTarget;

    private bool isAttacking;
    private float attackCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentPatrolTarget = rightPoint;
    }

    private void Update()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        FindPlayer();
    }

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            StopMoving();
            return;
        }

        if (playerTarget != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                StopMoving();
                TryAttack();
            }
            else
            {
                ChasePlayer();
            }

            return;
        }

        Patrol();
    }

    private void FindPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            detectRange,
            playerLayer
        );

        if (playerCollider != null)
        {
            playerTarget = playerCollider.transform;
        }
        else
        {
            playerTarget = null;
        }
    }

    private void Patrol()
    {
        if (leftPoint == null || rightPoint == null) return;

        if (currentPatrolTarget == null)
        {
            currentPatrolTarget = rightPoint;
        }

        float xDifference = currentPatrolTarget.position.x - transform.position.x;

        if (Mathf.Abs(xDifference) <= reachDistance)
        {
            ChangePatrolTarget();
            return;
        }

        float direction = Mathf.Sign(xDifference);

        rb.linearVelocity = new Vector2(
            direction * patrolSpeed,
            rb.linearVelocity.y
        );

        Flip(direction);
    }

    private void ChasePlayer()
    {
        float xDifference = playerTarget.position.x - transform.position.x;

        if (Mathf.Abs(xDifference) <= 0.05f)
        {
            StopMoving();
            return;
        }

        float direction = Mathf.Sign(xDifference);

        rb.linearVelocity = new Vector2(
            direction * chaseSpeed,
            rb.linearVelocity.y
        );

        Flip(direction);
    }

    private void TryAttack()
    {
        if (attackCooldownTimer > 0f) return;
        if (isAttacking) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        attackCooldownTimer = attackCooldown;

        StopMoving();

        if (playerTarget != null)
        {
            float direction = Mathf.Sign(playerTarget.position.x - transform.position.x);
            Flip(direction);
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Debug.Log("Slime Attack Start");

        yield return new WaitForSeconds(attackHitDelay);

        DealDamageToPlayer();

        float remainTime = attackMotionDuration - attackHitDelay;

        if (remainTime > 0f)
        {
            yield return new WaitForSeconds(remainTime);
        }

        isAttacking = false;
    }

    private void DealDamageToPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            attackRange,
            playerLayer
        );

        if (playerCollider == null)
        {
            Debug.Log("Slime Attack Miss");
            return;
        }

        Health playerHealth = playerCollider.GetComponentInParent<Health>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Slime Attack Hit");
        }
        else
        {
            Debug.LogWarning("Player에 Health가 없습니다.");
        }
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void ChangePatrolTarget()
    {
        if (currentPatrolTarget == rightPoint)
        {
            currentPatrolTarget = leftPoint;
        }
        else
        {
            currentPatrolTarget = rightPoint;
        }
    }

    private void Flip(float direction)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.flipX = direction < 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
            Gizmos.DrawWireSphere(leftPoint.position, 0.2f);
            Gizmos.DrawWireSphere(rightPoint.position, 0.2f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
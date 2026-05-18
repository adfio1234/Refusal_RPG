using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 7f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 30f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private float upwardGroundCheckIgnoreVelocity = 0.05f;

    [Header("Jump Feel")]
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float lowJumpGravityMultiplier = 2f;

    [Header("Attack")]
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackDuration = 0.2f;
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private float attackPointDistance = 0.8f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 15f;
    [SerializeField] private float dodgeDuration = 0.3f;
    [SerializeField] private float dodgeCooldown = 1f;
    [SerializeField] private bool canDodgeInAir = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private float facingDirection = 1f;

    private bool isGrounded;
    private bool isAttacking;
    private bool isDodging;

    private int jumpCount;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private float attackTimer;
    private float dodgeTimer;
    private float dodgeCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Update()
    {
        CheckGround();
        HandleInput();
        UpdateAttackPointPosition();
        UpdateAnimation();
        UpdateTimers();
    }

    private void FixedUpdate()
    {
        if (isDodging)
        {
            rb.linearVelocity = new Vector2(facingDirection * dodgeSpeed, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        ApplyBetterJumpGravity();
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            facingDirection = Mathf.Sign(moveInput);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            jumpBufferTimer = jumpBufferTime;
        }

        if (jumpBufferTimer > 0f && CanJump() && !isDodging)
        {
            Jump();
            jumpBufferTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking && !isDodging)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.C) && CanDodge())
        {
            Dodge();
        }
    }

    private void CheckGround()
    {
        bool detectedGround = false;

        bool shouldIgnoreGroundCheck =
            jumpCount > 0 &&
            rb.linearVelocity.y > upwardGroundCheckIgnoreVelocity;

        if (!shouldIgnoreGroundCheck)
        {
            detectedGround = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        isGrounded = detectedGround;

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            jumpCount = 0;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;

            if (coyoteTimer <= 0f && jumpCount == 0)
            {
                jumpCount = 1;
            }
        }
    }

    private bool CanJump()
    {
        if (isGrounded)
        {
            return true;
        }

        if (coyoteTimer > 0f)
        {
            return true;
        }

        return jumpCount < maxJumpCount;
    }

    private void Jump()
    {
        jumpCount++;

        isGrounded = false;
        coyoteTimer = 0f;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        SetTriggerIfExists("Jump");

        Debug.Log("Jump");
    }

    private void ApplyBetterJumpGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up *
                Physics2D.gravity.y *
                (fallGravityMultiplier - 1f) *
                Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetKey(KeyCode.Z))
        {
            rb.linearVelocity += Vector2.up *
                Physics2D.gravity.y *
                (lowJumpGravityMultiplier - 1f) *
                Time.fixedDeltaTime;
        }
    }

    private void Attack()
    {
        isAttacking = true;
        attackTimer = attackDuration;

        SetTriggerIfExists("Attack");

        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint가 연결되지 않았습니다.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        HashSet<Health> damagedTargets = new HashSet<Health>();

        foreach (Collider2D hit in hits)
        {
            Health enemyHealth = hit.GetComponentInParent<Health>();

            if (enemyHealth == null) continue;
            if (damagedTargets.Contains(enemyHealth)) continue;

            enemyHealth.TakeDamage(attackDamage);
            damagedTargets.Add(enemyHealth);
        }

        Debug.Log($"Player Attack. Hit Count: {damagedTargets.Count}");
    }

    private void Dodge()
    {
        isDodging = true;
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;

        SetTriggerIfExists("Dodge");

        Debug.Log("Dodge");
    }

    private bool CanDodge()
    {
        if (isDodging || dodgeCooldownTimer > 0f) return false;
        if (!canDodgeInAir && !isGrounded) return false;

        return true;
    }

    private void UpdateAttackPointPosition()
    {
        if (attackPoint == null) return;

        attackPoint.localPosition = new Vector3(
            attackPointDistance * facingDirection,
            attackPoint.localPosition.y,
            attackPoint.localPosition.z
        );
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(moveInput) > 0.01f;

        SetBoolIfExists("IsRunning", isRunning);
        SetBoolIfExists("IsGrounded", isGrounded);
        SetBoolIfExists("IsFalling", rb.linearVelocity.y < -0.1f);
        SetBoolIfExists("IsDodging", isDodging);
        SetBoolIfExists("IsAttacking", isAttacking);

        if (spriteRenderer != null)
        {
            if (moveInput > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveInput < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    private void SetBoolIfExists(string parameterName, bool value)
    {
        if (!HasAnimatorParameter(parameterName, AnimatorControllerParameterType.Bool)) return;

        animator.SetBool(parameterName, value);
    }

    private void SetTriggerIfExists(string parameterName)
    {
        if (!HasAnimatorParameter(parameterName, AnimatorControllerParameterType.Trigger)) return;

        animator.SetTrigger(parameterName);
    }

    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType type)
    {
        if (animator == null) return false;
        if (animator.runtimeAnimatorController == null) return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == type)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateTimers()
    {
        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                isAttacking = false;
            }
        }

        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;

            if (dodgeTimer <= 0f)
            {
                isDodging = false;
            }
        }

        if (dodgeCooldownTimer > 0f)
        {
            dodgeCooldownTimer -= Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
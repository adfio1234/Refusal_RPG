using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 7f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 30f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.15f;

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
    
    private float moveInput;
    private float facingDirection = 1f;

    private bool isGrounded;
    private bool isAttacking;
    private bool isDodging;

    private float attackTimer;
    private float dodgeTimer;
    private float dodgeCooldownTimer;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
        
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer=GetComponent<SpriteRenderer > ();
    }

    private void Update() {
        CheckGround();
        HandleInput();
        UpdateAttackPointPosition();
        UpdateAnimation();
        UpdateTimers();
    }

    private void FixedUpdate() {
        if(isDodging) {
            rb.linearVelocity = new Vector2(facingDirection * dodgeSpeed, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            bool isRunning = Mathf.Abs(moveInput) > 0.01f;
            animator.SetBool("IsRunning", isRunning);
        }

        if(spriteRenderer!=null)
        {
            if(moveInput>0)
            {
                spriteRenderer.flipX = false;
            }
            else if(moveInput<0)
            {
                spriteRenderer.flipX = true;
            }
        }
        
    }

    private void HandleInput() {
        moveInput = Input.GetAxisRaw("Horizontal");
        if(moveInput != 0) facingDirection = Mathf.Sign(moveInput);

        if (Input.GetKeyDown(KeyCode.T)&&isGrounded) Debug.Log("IsGround");
        if (Input.GetKeyDown(KeyCode.Z) && isGrounded && !isDodging) Jump();
        if (Input.GetKeyDown(KeyCode.X) && !isAttacking && !isDodging) Attack();
        if (Input.GetKeyDown(KeyCode.C) && CanDodge()) Dodge();
    }

    private void CheckGround() {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        Debug.Log("Jump");
    }

    private void Attack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        if(attackPoint == null)
        {
            Debug.LogWarning("AttackPoint가 연결되지 않았습니다.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        if (animator != null) animator.SetTrigger("Attack");

        HashSet<Health> damagedTargets = new HashSet<Health>();
        foreach(Collider2D hit in hits)
        {
            Health enemyHealth = hit.GetComponentInParent<Health>();
            if(enemyHealth == null) continue;
            if(damagedTargets.Contains(enemyHealth)) continue;

            enemyHealth.TakeDamage(attackDamage);
            damagedTargets.Add(enemyHealth);
        }

        Debug.Log($"Player Attack. Hit Count: {damagedTargets.Count}");
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

    private void Dodge()
    {
        isDodging = true;
        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;

        Debug.Log("Dodge");
    }

    private bool CanDodge()
    {
        if(isDodging || dodgeCooldownTimer > 0f) return false;
        if (!canDodgeInAir && !isGrounded) return false;

        return true;
    }

    private void UpdateTimers()
    {
        if(isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f) isAttacking = false;
        }

        if(isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0f) isDodging = false;
        }

        if(dodgeCooldownTimer > 0f) dodgeCooldownTimer -= Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        if(groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if(attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}

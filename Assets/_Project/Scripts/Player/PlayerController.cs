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
    [SerializeField] private float attackDuration = 0.2f;

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

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        CheckGround();
        HandleInput();
        UpdateTimers();
    }

    private void FixedUpdate() {
        if(isDodging) {
            rb.linearVelocity = new Vector2(facingDirection * dodgeSpeed, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleInput() {
        moveInput = Input.GetAxisRaw("Horizontal");
        if(moveInput != 0) facingDirection = Mathf.Sign(moveInput);

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
    }

    private void Attack()
    {
        isAttacking = true;
        attackTimer = attackDuration;

        Debug.Log("Attack");
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
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}

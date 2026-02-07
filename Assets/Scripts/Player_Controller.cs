using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Crouch Collider Values")]
    public float crouchColliderHeight = 0.5f;
    public float crouchColliderOffsetY = -0.5f;

    [Header("Climb")]
    public float climbSpeed = 3f;

    [Header("Knockback")]
    public bool isKnockedBack;
    public float knockbackDuration = 0.3f;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D col;

    private bool isGrounded;
    private bool isCrouching;

    // Climb
    private bool isInClimbArea;
    private bool isClimbing;

    // Collider values
    private float standColliderHeight;
    private Vector2 standColliderOffset;
    private float originalGravity;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<CapsuleCollider2D>();

        standColliderHeight = col.size.y;
        standColliderOffset = col.offset;
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Crouch
        bool crouchInput = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        isCrouching = crouchInput && isGrounded && !isClimbing && !isKnockedBack;

        // Start climbing
        if (isInClimbArea && Input.GetAxis("Vertical") != 0 && !isKnockedBack)
        {
            StartClimbing();
        }

        // Stop climbing
        if (isClimbing && Input.GetAxis("Vertical") == 0)
        {
            StopClimbing();
        }

        // Animations
        anim.SetFloat("speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        anim.SetBool("isgrounded", isGrounded);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isClimbing", isClimbing);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && !isKnockedBack)
        {
            if (isClimbing)
            {
                StopClimbing();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (isGrounded && !isCrouching)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
    }

    void FixedUpdate()
    {
        // ❌ أثناء الضربة: لا حركة
        if (isKnockedBack) return;

        // Climbing
        if (isClimbing)
        {
            float vertical = Input.GetAxis("Vertical");
            rb.velocity = new Vector2(0f, vertical * climbSpeed);
            return;
        }

        // Horizontal movement
        float moveX = Input.GetAxis("Horizontal");
        float speed = isCrouching ? moveSpeed * 0.5f : moveSpeed;

        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        if (moveX != 0)
            sr.flipX = moveX < 0;

        // Crouch collider
        if (isCrouching)
        {
            col.size = new Vector2(col.size.x, crouchColliderHeight);
            col.offset = new Vector2(standColliderOffset.x, crouchColliderOffsetY);
        }
        else
        {
            col.size = new Vector2(col.size.x, standColliderHeight);
            col.offset = standColliderOffset;
        }
    }

    // ================= CLIMB =================

    public void EnterClimbArea()
    {
        isInClimbArea = true;
    }

    public void ExitClimbArea()
    {
        isInClimbArea = false;
        if (isClimbing)
            StopClimbing();
    }

    void StartClimbing()
    {
        if (isClimbing) return;

        isClimbing = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    void StopClimbing()
    {
        isClimbing = false;
        rb.gravityScale = originalGravity;
    }

    // ================= KNOCKBACK =================

    public void ApplyKnockback(Vector2 force)
    {
        if (isKnockedBack) return;

        StartCoroutine(KnockbackCoroutine(force));
    }

    IEnumerator KnockbackCoroutine(Vector2 force)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
    }

    // ================= GIZMO =================

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}

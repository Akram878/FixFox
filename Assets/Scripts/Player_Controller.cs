using UnityEngine;

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

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D col;

    private bool isGrounded;
    private bool isCrouching;

    // تسلق
    private bool isInClimbArea;
    private bool isClimbing;

    // القيم الأصلية للوقوف
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
        // Ground Check
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // إدخال الانحناء
        bool crouchInput = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        isCrouching = crouchInput && isGrounded && !isClimbing;

        //  بدء التسلق: داخل المنطقة + W أو S
        if (isInClimbArea && Input.GetAxis("Vertical") != 0)
        {
            StartClimbing();
        }

        //  إفلات التسلق: لا W ولا S
        if (isClimbing && Input.GetAxis("Vertical") == 0)
        {
            StopClimbing();
        }

        // أنيميشن
        anim.SetFloat("speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        anim.SetBool("isgrounded", isGrounded);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isClimbing", isClimbing);

        // قفز
        if (Input.GetKeyDown(KeyCode.Space))
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
        // أثناء التسلق
        if (isClimbing)
        {
            float vertical = Input.GetAxis("Vertical");
            rb.velocity = new Vector2(0f, vertical * climbSpeed);
            return;
        }

        // حركة أفقية عادية
        float moveX = Input.GetAxis("Horizontal");
        float speed = isCrouching ? moveSpeed * 0.5f : moveSpeed;

        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        if (moveX != 0)
            sr.flipX = moveX < 0;

        // تطبيق الانحناء بالقيم اليدوية
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

    // ====== تسلق ======

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

    // Gizmo لرؤية GroundCheck
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}

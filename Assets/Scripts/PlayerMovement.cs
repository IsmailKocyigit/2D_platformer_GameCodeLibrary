using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    
    bool isFacingRight = true;

    public Animator animator;

    public ParticleSystem smokeFX;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    public int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2f;
    bool isWallSliding;
    // wall jumping
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();
        

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();
        }

        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
    }

    private void ProcessGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            // makes player fall speed increase, the longer he falls
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            // limits the max fall speed
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        // not grounded && on a wall && movement is not 0
        if (!isGrounded && WallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            // limit wall slide fall speed
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    private void JumpFX()
    {
        animator.SetTrigger("jump");
        smokeFX.Play();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > -1)
        {
            if (context.performed && jumpsRemaining > 0) 
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                JumpFX();
            }
            // light tap allows smaller jumps
            else if (context.canceled )
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
                JumpFX();
            }
        }

        // wall jump
        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0f;
            JumpFX();

            // force a flip
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            // wall jump lasts "wallJumpTime" and you can jump again only after this time
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            if (rb.linearVelocity.y == 0f)
            {
                smokeFX.Play();
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheckPos.position, groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(wallCheckPos.position, wallCheckSize);
    }
}

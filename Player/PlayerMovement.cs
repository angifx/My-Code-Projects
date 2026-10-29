using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator; 
    bool isFacingRight = true;
    public ParticleSystem smokeFX;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;


    [Header("Jumping")]
    public float jumpPower = 5f; 
    public int maxJumps = 2;
    int jumpsRemaining; 


    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    bool isGrounded;


    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;


    [Header("WallCheck")]
    public Transform WallCheckPos;
    public Vector2 WallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask WallLayer;


    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.2f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(2f, 10f);


    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate;
    private float fireTimer;
    private bool isShooting;



    void Update()
    {
        GroundCheck();
        Gravity();
        ProcessWallslide();
        ProcessWallJump();
        HandleShooting();

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y); rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            Flip();
        }

        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0f;
            JumpFX();
            SoundEffectMangager.Play("jump"); 

            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); 
            return;
        }

        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                JumpFX();
                SoundEffectMangager.Play("jump");
            }
            else if (context.canceled && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    private void JumpFX()
        {
        animator.SetTrigger("jump");
        smokeFX.Play();
        }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(WallCheckPos.position, WallCheckSize, WallCheckPos.eulerAngles.z, WallLayer);
    }

    private void ProcessWallslide()
    {
        if (!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)); 
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }
    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void GroundCheck()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer)) 
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else 
        {
            isGrounded = false;
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            if(rb.linearVelocity.y == 0)
            {
                smokeFX.Play();
            }
        }
    }

private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;

        if (WallCheckPos != null)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(WallCheckPos.position, WallCheckPos.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, WallCheckSize);
            Gizmos.matrix = oldMatrix;
        }
    }
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isShooting = true;
        }
        else if (context.canceled)
        {
            isShooting = false;
        }
    }
    private void HandleShooting()
    {
        fireTimer -= Time.deltaTime;

        if (isShooting && fireTimer <= 0f)
        {
            FireBullet();
            fireTimer = fireRate;
        }
    }
    private void FireBullet()
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        Vector2 shootDirection = isFacingRight ? Vector2.right : Vector2.left;
        bulletScript.SetDirection(shootDirection);
        SoundEffectMangager.Play("playershoot"); 
        Destroy(bullet, 2f); 
    }


}
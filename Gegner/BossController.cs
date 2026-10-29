using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class BossController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform player;

    [Header("Sprung-Angriff")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float jumpChance = 0.4f;
    [SerializeField] private float jumpCooldown = 4f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Boss Health Bar")]
    [SerializeField] private Slider bossHealthBar;


    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private int currentHealth;
    private float lastAttackTime;
    private float lastJumpTime = -999f;
    private bool isDead = false;
    private bool isJumping = false;
    private bool isGrounded;
    private bool isActive = false;



    void OnEnable()
    {
        isActive = false; 
        GameController.OnLevelLoad += HandleLevelLoaded;
    }

    void OnDisable()
    {
        GameController.OnLevelLoad -= HandleLevelLoaded;
    }

    private void HandleLevelLoaded()
    {
        isActive = true;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true);
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }
    }


    void Update()
    {
        if (!isActive) return; 
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isJumping && isGrounded)
        {
            isJumping = false;
        }

        if (isDead || player == null || isJumping) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

            animator.SetBool("IsWalking", true);
            sr.flipX = direction.x < 0;
        }
        else
        {
            animator.SetBool("IsWalking", false);

            if (Time.time - lastAttackTime > attackCooldown)
            {
                bool canJump = isGrounded && Time.time - lastJumpTime > jumpCooldown;

                if (canJump && Random.value < jumpChance)
                {
                    JumpAttack();
                }
                else
                {
                    Attack();
                }

                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        Debug.Log("Boss greift an!");
    }

    void JumpAttack()
    {
        Debug.Log("Boss springt an!");
        isJumping = true;
        lastJumpTime = Time.time;
        animator.SetTrigger("Jump");

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed * 2f, jumpForce);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Boss Leben: " + currentHealth);

        if (bossHealthBar != null)
        {
            bossHealthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }

        GameController gc = FindFirstObjectByType<GameController>();
        if (gc != null)
        {
            gc.WinGame();
        }

        Destroy(gameObject, 1.5f);
    }

void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GegnerMovement : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;
    public int damage = 1;

    //LootTable
    [Header("Loot")]
    public List<Lootitem> lootTable = new List<Lootitem>();

    [Header("Movement")]
    public float moveSpeed = 5f;
    private bool isFacingRight = true;

    [Header("Chase")]
    public Transform player;
    public float chaseSpeed = 2f;
    public float detectionRadius = 5f;

    [Header("Jumping")]
    public float jumpForce = 2f;
    private bool shouldJump;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("Spawn Effekt")]
    public float spawnDuration = 0.4f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Puff Partikel")]
    public GameObject spawnPuffPrefab; 

    private SpriteRenderer sr;
    private Vector3 targetScale;

    private Rigidbody2D rb;
    private Animator animator;
    private bool wasVisible = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        targetScale = transform.localScale;
        currentHealth = maxHealth;
        ogColor = spriteRenderer.color;
    }

    void Update()
    {
        GroundCheck();
        Gravity();

        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetBool("isGrounded", isGrounded);

        HandleChaseAndJumpDetection();
    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 jumpDirection = direction * jumpForce;

            rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer)) 
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Gravity()
    {
        GroundCheck(); 
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

    private void HandleChaseAndJumpDetection()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        bool playerInRange = distanceToPlayer <= detectionRadius;

        if (playerInRange && !wasVisible)
        {
            SpawnPuff();
        }
        wasVisible = playerInRange;

        spriteRenderer.enabled = playerInRange;

        if (!playerInRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float direction = Mathf.Sign(player.position.x - transform.position.x);

        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << player.gameObject.layer);

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer);

            if ((!groundInFront.collider && !gapAhead.collider))
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                shouldJump = true;
            }
        }

        Flip(direction);
    }

    private void Flip(float direction)
    {
        if ((isFacingRight && direction < 0) || (!isFacingRight && direction > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(transform.parent.gameObject);
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());
        SoundEffectMangager.Play("GegnerHit");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = ogColor;
    }

    void Die()
    {
        foreach (Lootitem lootItem in lootTable)
        {
            if (UnityEngine.Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                InstantiateLoot(lootItem.itemPrefab);
            }
            break;
        }

        Destroy(gameObject);

        SoundEffectMangager.Play("GegnerDeath");
    }

    void InstantiateLoot(GameObject loot)
    {
        if (loot)
        {
            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);

            droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    void SpawnPuff()
    {
        SpawnIn(); 
        if (spawnPuffPrefab != null)
        {
            GameObject puff = Instantiate(spawnPuffPrefab, transform.position, Quaternion.identity);
            Destroy(puff, 0.3f);
        }
    }

    IEnumerator SpawnIn()
    {

        transform.localScale = Vector3.zero;
        Color c = sr.color;
        c.a = 0f;
        sr.color = c;
        float t = 0f;
        while (t < spawnDuration)
        {
            t += Time.deltaTime;
            float progress = t / spawnDuration;

            float scaleT = scaleCurve.Evaluate(progress);
            transform.localScale = targetScale * scaleT;

            c.a = Mathf.Clamp01(progress * 1.5f);
            sr.color = c;

            yield return null;
        }

        transform.localScale = targetScale;
        c.a = 0.3f;
        sr.color = c;
    }
}
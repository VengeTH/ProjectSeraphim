using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class CharacterScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveInput;
    public float jumpForce = 5f;
    public float speed = 5f;
    [SerializeField] private bool isGrounded = true;
    private Animator animator;
    public Transform groundCheck;
    public float checkRadius = 0.5f;
    public LayerMask groundLayer;
    public float deathThreshold = -6f;
    public TMP_Text deathText;
    private SpriteRenderer spriteRenderer;
    public static CharacterScript instance;
    public bool isGameOver = false;
    public bool isWin = false;
    public Transform winZone; // Added for win condition [[1]]
    public float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private TMP_Text healthText; // Added for health display [[2]]
    // Ensure this is assigned in the Unity Editor to a valid TextMeshPro component.

    // Attack cooldown variables
    public float attackCooldown = 1f; // Cooldown time in seconds
    private float lastAttackTime = 0f; // Tracks the last time the player attacked

    void Start()
    {
        currentHealth = maxHealth; // Initialize health [[2]]
        UpdateHealthText(); // Update health display [[2]]
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        // Null checks for critical components
        if (rb == null || animator == null || spriteRenderer == null)
            Debug.LogError("Missing component in CharacterScript!");
    }

    public void TakeDamage(float damage) // Added for health management [[2]]
    {
        Debug.Log("TakeDamage method called.");
        currentHealth -= damage;
        UpdateHealthText(); // Update health display

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthText() // Added for health management [[2]]
    {
        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
        else
            Debug.LogError("Health Text component not assigned!");
    }

    void FixedUpdate()
    {
        moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); // Kept as per request

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        if (isGrounded)
            animator.SetBool("isJumping", false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time; // Update the last attack time
            animator.SetTrigger("Attack");

            // Perform range-based attack
            float attackRange = 1.5f; // Define the attack range
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));

            if (hitEnemies.Length == 0)
            {
                Debug.Log("No enemies detected within attack range.");
            }

            foreach (Collider2D enemyCollider in hitEnemies)
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log($"Attacking enemy: {enemy.name}");
                    enemy.TakeDamage(20f); // Apply damage to the enemy
                }
                else
                {
                    Debug.LogError($"Enemy script not found on {enemyCollider.name}!");
                }
            }
        }

        if (moveInput != 0)
            spriteRenderer.flipX = (moveInput > 0); // Keep your existing logic for flipping the sprite

        if ((Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
            animator.SetBool("isJumping", true);
        }

        // Win condition using trigger zone [[1]]
        if (!isWin && winZone != null && transform.position.x >= winZone.position.x)
            Win();

        if (transform.position.y < deathThreshold)
            Die();
    }

    void Die()
    {
        if (isGameOver) return;
        animator.SetTrigger("Die");
        isGameOver = true;
        SceneManager.LoadScene(3);
    }

    void Win()
    {
        if (isWin) return;
        isWin = true;
        SceneManager.LoadScene(2);
        Destroy(this);
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Kept as per request
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<TilemapCollider2D>(out _))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<TilemapCollider2D>(out _))
            isGrounded = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw the attack range in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f); // 1.5f is the attack range
    }
}
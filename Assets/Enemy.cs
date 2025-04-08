using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Walking, Floating, Flying }
    public EnemyType enemyType;
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float detectionRange = 5f; // Range to detect the player
    public float attackRange = 1.5f; // Range to attack the player
    public float attackDamage = 10f; // Damage dealt to the player
    public float attackCooldown = 1f; // Cooldown time between attacks
    private float lastAttackTime = 0f; // Tracks the last time the enemy attacked
    private bool isChasing = false;
    public float flyingGravityScale = 0f;
    public float floatingBobSpeed = 1f;
    public LayerMask groundLayer;
    private int currentPointIndex = 0;
    private Rigidbody2D rb;

    public float maxHealth = 50f; // Default max health
    private float currentHealth;

    [SerializeField] private TMP_Text healthText; // Text to display health
    public Transform player; // Reference to the player

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = (enemyType == EnemyType.Flying) ? flyingGravityScale : 1f;

        if ((enemyType == EnemyType.Walking || enemyType == EnemyType.Flying) && patrolPoints.Length < 2)
            Debug.LogError("Enemy needs at least two patrol points!");

        if (player == null)
            Debug.LogError("Player reference not set!");

        currentHealth = maxHealth; // Initialize health
        UpdateHealthText(); // Update health display
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within detection range
        isChasing = (distanceToPlayer <= detectionRange);

        // Check if the player is within attack range and attack cooldown has passed
        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            AttackPlayer();
            lastAttackTime = Time.time; // Update the last attack time
        }
    }

    void FixedUpdate()
    {
        switch (enemyType)
        {
            case EnemyType.Walking:
                MoveWalking();
                break;
            case EnemyType.Floating:
                MoveFloating();
                break;
            case EnemyType.Flying:
                MoveFlying();
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Enemy taking damage: {damage}"); // Debug log to confirm damage is applied
        currentHealth -= damage;
        UpdateHealthText(); // Update health display

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
        else
            Debug.LogWarning("Health Text component not assigned for Enemy!");
    }

    private void Die()
    {
        Destroy(gameObject); // Destroy the enemy object
    }

    void MoveWalking()
    {
        if (isChasing)
            rb.linearVelocity = new Vector2(moveSpeed * Mathf.Sign(player.position.x - transform.position.x), rb.linearVelocity.y);
        else
        {
            Transform target = patrolPoints[currentPointIndex];
            rb.linearVelocity = new Vector2(moveSpeed * Mathf.Sign(target.position.x - transform.position.x), rb.linearVelocity.y);

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void MoveFloating()
    {
        float yOffset = Mathf.Sin(Time.time * floatingBobSpeed) * 0.5f;
        rb.linearVelocity = new Vector2(moveSpeed, yOffset);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(moveSpeed), 0.5f, groundLayer);
        if (hit.collider != null)
            moveSpeed *= -1;
    }

    void MoveFlying()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        if (!isChasing)
        {
            Transform target = patrolPoints[currentPointIndex];
            rb.linearVelocity = (target.position - transform.position).normalized * moveSpeed;

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void AttackPlayer()
    {
        Debug.Log($"Enemy {name} is attempting to attack the player.");
        Debug.Log("Enemy is attacking the player!"); // Debug log for attack
        CharacterScript playerScript = player.GetComponent<CharacterScript>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(attackDamage); // Apply damage to the player
            Debug.Log($"Player took {attackDamage} damage from {name}.");
        }
        else
        {
            Debug.LogError("Player reference is not set or missing CharacterScript!");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var point in patrolPoints)
            Gizmos.DrawSphere(point.position, 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enum to define enemy types
    public enum EnemyType { Walking, Floating, Flying }
    public EnemyType enemyType; // Set this in the Inspector

    // Shared variables
    public Transform[] patrolPoints; // Points between which the enemy will roam
    public float moveSpeed = 2f; // Speed of movement
    public float detectionRange = 5f; // Range within which the enemy detects the player
    public Transform player; // Reference to the player's transform
    public float attackRange = 1.5f; // Range within which the enemy can attack the player
    public float attackDamage = 10f; // Damage dealt to the player
    public float attackCooldown = 1f; // Cooldown between attacks
    private float lastAttackTime = 0f; // Timer to track the last attack time
    private bool isChasing = false; // Flag to check if the enemy is chasing the player

    // Type-specific variables
    public float flyingGravityScale = 0f; // Gravity scale for flying enemies
    public float floatingBobSpeed = 1f; // Speed of up/down motion for floating enemies
    public LayerMask groundLayer; // Ground layer for walking enemies

    // Private variables
    private int currentPointIndex = 0; // Index of the current patrol point
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = (enemyType == EnemyType.Flying) ? flyingGravityScale : 1f;

        // Ensure there are at least two patrol points for roaming behavior
        if (patrolPoints.Length < 2 && (enemyType == EnemyType.Walking || enemyType == EnemyType.Flying))
        {
            Debug.LogError("Enemy needs at least two patrol points for roaming!");
        }

        // Ensure the player reference is assigned
        if (player == null)
        {
            Debug.LogError("Player reference not assigned in the Inspector!");
        }
    }

    void Update()
    {
        // Check if the player is within detection range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isChasing = (distanceToPlayer <= detectionRange);

        // Attack if within range
        if (isChasing && distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
    }

    void FixedUpdate()
    {
        // Handle movement based on enemy type
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

    void MoveWalking()
    {
        if (isChasing)
        {
            // Chase the player
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Roam between patrol points
            Transform targetPoint = patrolPoints[currentPointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            // Switch to the next patrol point when close enough
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }
        }
    }

    void MoveFloating()
    {
        // Smooth up/down bobbing motion
        float yOffset = Mathf.Sin(Time.time * floatingBobSpeed) * 0.5f;
        rb.linearVelocity = new Vector2(moveSpeed, yOffset);

        // Reverse direction when hitting walls or obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(rb.linearVelocity.x), 0.5f, groundLayer);
        if (hit.collider != null)
        {
            moveSpeed = -moveSpeed; // Reverse direction
        }
    }

    void MoveFlying()
    {
        if (isChasing)
        {
            // Fly directly toward the player
            rb.linearVelocity = (player.position - transform.position).normalized * moveSpeed;
        }
        else
        {
            // Roam between patrol points
            Transform targetPoint = patrolPoints[currentPointIndex];
            rb.linearVelocity = (targetPoint.position - transform.position).normalized * moveSpeed;

            // Switch to the next patrol point when close enough
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }
        }
    }

    void AttackPlayer()
    {
        // Check if enough time has passed since the last attack
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Apply damage to the player
            // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            // {
            //     playerHealth.TakeDamage(attackDamage);
            // }

            // Update the last attack time
            lastAttackTime = Time.time;
        }
    }

    // Optional: Draw gizmos in the editor to visualize detection and attack ranges
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
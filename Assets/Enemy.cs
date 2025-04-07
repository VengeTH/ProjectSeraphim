using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Walking, Floating, Flying }
    public EnemyType enemyType;
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public Transform player;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;
    private bool isChasing = false;
    public float flyingGravityScale = 0f;
    public float floatingBobSpeed = 1f;
    public LayerMask groundLayer;
    private int currentPointIndex = 0;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = (enemyType == EnemyType.Flying) ? flyingGravityScale : 1f;

        if ((enemyType == EnemyType.Walking || enemyType == EnemyType.Flying) && patrolPoints.Length < 2)
            Debug.LogError("Enemy needs at least two patrol points!");

        if (player == null)
            Debug.LogError("Player reference not set!");
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isChasing = (distanceToPlayer <= detectionRange);

        if (isChasing && distanceToPlayer <= attackRange)
            AttackPlayer();
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
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Uncomment to enable damage [[6]]
            // PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            // if (playerHealth != null) playerHealth.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var point in patrolPoints)
            Gizmos.DrawSphere(point.position, 0.2f);
    }
}
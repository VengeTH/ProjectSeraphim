using UnityEngine;
using TMPro;
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        // Null checks for critical components
        if (rb == null || animator == null || spriteRenderer == null)
            Debug.LogError("Missing component in CharacterScript!");
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
            animator.SetTrigger("Attack");

        if (moveInput != 0)
            spriteRenderer.flipX = (moveInput > 0);

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
}
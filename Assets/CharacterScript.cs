using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class CharacterScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveInput;
    public float jumpForce = 5f;
    public float speed = 5f;
    private bool isGrounded;
    private Animator animator;

    public Transform groundCheck;
    public float checkRadius = 0.5f;
    public LayerMask groundLayer;

    public float deathThreshold = -6f;
    public TMP_Text deathText;

    private SpriteRenderer spriteRenderer;

    public static CharacterScript instance; // Singleton instance
    public bool isGameOver = false; // Added variable to track game over state
    public bool isWin = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    void FixedUpdate()
    {
        moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        Debug.DrawLine(transform.position, groundCheck.position, Color.green);

        animator.SetFloat("Horizontal", moveInput);
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("isGrounded", isGrounded);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            animator.SetTrigger("Attack");
        }
        // if (moveInput != 0)
        //     spriteRenderer.flipX = (moveInput < 0);

        if ((Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
        }

        // Check if player is winning
        if (!isWin && transform.position.x >= 473.5f) Win();

        // Check if player is dead
        if (transform.position.y < deathThreshold) Die();
    }

    void Die()
    {
        if (isGameOver) return; // Prevent multiple calls to Die()
        animator.SetTrigger("Die");
        Debug.Log("Player is dead!");
        isGameOver = true; // Set game over state
        SceneManager.LoadScene(3);
    }

    void Win()
    {
        if (isWin) return;
        Debug.Log("Player has won!");
        isWin = true; // Set win state
        isGameOver = false;
        SceneManager.LoadScene(2); // Load the win scene
        Destroy(this);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this; // Assign the singleton instance
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
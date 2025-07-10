using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float swimUpForce = 5f;
    public float swimSpeed = 3f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject spriteObject;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isSwimming;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Get Animator and SpriteRenderer from child
        if (spriteObject != null)
        {
            anim = spriteObject.GetComponent<Animator>();
            spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning("SpriteObject not assigned");
        }
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Flip sprite
        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;

        if (isSwimming)
        {
            // Flappy Bird swim mechanic
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, swimUpForce); // push up
            }
        }
        else
        {
            // Regular land movement
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }

            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        // Animator parameters
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSwimming", isSwimming);
    }

    void FixedUpdate()
    {
        // Ground check only when not swimming
        if (!isSwimming)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isSwimming = true;
            rb.gravityScale = 1.5f; // simulate buoyancy
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isSwimming = false;
            rb.gravityScale = 3f; // reset to normal
        }
    }
}

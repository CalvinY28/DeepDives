using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject spriteObject;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
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

        // Flip the sprite based on movement direction
        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;

        // Play jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Update Animator parameters
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("isGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}

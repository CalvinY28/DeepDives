using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float swimUpForce = 5f;
    public float waterLerpSpeed = 3f; // lower = more slippery
    public float landLerpSpeed = 10f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject spriteObject;
    public PhysicsMaterial2D landMaterial;
    public PhysicsMaterial2D waterMaterial;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isSwimming;
    private float currentVelocityX;
    private Collider2D playerCollider;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
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
        if (Time.timeScale == 0) return; // Prevent input if paused by menu
        moveInput = Input.GetAxisRaw("Horizontal");

        // Flip sprite
        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;

        float targetVelocityX = moveInput * moveSpeed;
        float lerpSpeed = isSwimming ? waterLerpSpeed : landLerpSpeed;

        currentVelocityX = Mathf.Lerp(currentVelocityX, targetVelocityX, Time.deltaTime * lerpSpeed);

        // Handle vertical movement
        if (isSwimming)
        {
            rb.velocity = new Vector2(currentVelocityX, rb.velocity.y);

            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, swimUpForce);
            }
        }
        else
        {
            rb.velocity = new Vector2(currentVelocityX, rb.velocity.y);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
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
            rb.gravityScale = 1f; // simulate buoyancy
            currentVelocityX = rb.velocity.x;

            if (playerCollider != null && waterMaterial != null)
            {
                playerCollider.sharedMaterial = waterMaterial;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isSwimming = false;
            rb.gravityScale = 3f; // reset to normal
            currentVelocityX = rb.velocity.x;

            if (playerCollider != null && landMaterial != null)
            {
                playerCollider.sharedMaterial = landMaterial;
            }
        }
    }

    public bool IsSwimming()
    {
        return isSwimming;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

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
    private bool wasSwimming;
    private float currentVelocityX;
    private Collider2D playerCollider;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("FMOD Footsteps")] // note this could be done as an animation event wouldve made it easier
    public EventReference footstepEvent;
    public LayerMask footstepSurfaceLayers;
    public float footstepRayDistance = 0.3f;

    private string currentSurfaceLabel = "Wood";

    public float walkStepInterval = 0.45f;

    private float footstepTimer = 0f;

    [Header("FMOD Player Jump/Swim")]
    public EventReference playerJump;
    public EventReference playerSwim;

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
            Debug.LogWarning("not assigned");
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
        bool startedSwimming = isSwimming && !wasSwimming;

        if (isSwimming)
        {

            if (startedSwimming)
            {
                RuntimeManager.PlayOneShot(playerSwim, transform.position);
            }
            rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);

            if (Input.GetButtonDown("Jump"))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, swimUpForce);
                RuntimeManager.PlayOneShot(playerSwim, transform.position);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                RuntimeManager.PlayOneShot(playerJump, transform.position);
            }
        }
        wasSwimming = isSwimming;

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput));
            anim.SetBool("isGrounded", isGrounded);
            anim.SetBool("isSwimming", isSwimming);
        }

        DetectFootstepSurface();
        HandleFootsteps();
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
            currentVelocityX = rb.linearVelocity.x;

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
            currentVelocityX = rb.linearVelocity.x;

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

    void DetectFootstepSurface()
    {
        if (groundCheck == null) return;

        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            footstepRayDistance,
            footstepSurfaceLayers
        );

        if (!hit.collider) return;

        if (hit.collider.CompareTag("Sand")) // if im hitting sand tag change label to sand and etc
            currentSurfaceLabel = "Sand";
        else if (hit.collider.CompareTag("Wood"))
            currentSurfaceLabel = "Wood";
        else
            currentSurfaceLabel = "Wood";
    }

    void HandleFootsteps()
    {
        bool isMoving = Mathf.Abs(moveInput) > 0.1f;

        if (!isGrounded || isSwimming || !isMoving) // not playing footstep
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer += Time.deltaTime;

        if (footstepTimer >= walkStepInterval) // playing footstep with interval to make slower or faster.
        {
            PlayFootstep();
            footstepTimer = 0f;
        }
    }

    public void PlayFootstep() // make footstep noise at player position and make sure to check the type.
    {
        EventInstance step = RuntimeManager.CreateInstance(footstepEvent);
        RuntimeManager.AttachInstanceToGameObject(step, gameObject);
        step.setParameterByNameWithLabel("SurfaceType", currentSurfaceLabel);
        step.start();
        step.release(); // release smoothes it just in case (ialready tried to smooth it)
    }
}


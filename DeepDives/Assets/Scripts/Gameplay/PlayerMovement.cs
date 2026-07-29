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

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int IsSwimmingHash = Animator.StringToHash("isSwimming");

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isSwimming;
    private bool wasSwimming;
    private float currentVelocityX;
    private Collider2D playerCollider;
    private int waterLayer;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("FMOD Footsteps")]
    public EventReference footstepEvent;
    public LayerMask footstepSurfaceLayers;
    public float footstepRayDistance = 0.3f;

    private string currentSurfaceLabel = "Wood";

    public float walkStepInterval = 0.45f;

    private float footstepTimer;

    [Header("FMOD Player Jump/Swim")]
    public EventReference playerJump;
    public EventReference playerSwim;

    [Header("underwater filter")]
    public EventReference snapshotEvent;
    private EventInstance snapshotInstance;

    private void Start()
    {
        snapshotInstance = RuntimeManager.CreateInstance(snapshotEvent);
        playerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        waterLayer = LayerMask.NameToLayer("Water");

        if (spriteObject != null)
        {
            anim = spriteObject.GetComponent<Animator>();
            spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning("Player sprite object is not assigned.", this);
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (spriteRenderer != null && moveInput != 0f)
            spriteRenderer.flipX = moveInput < 0f;

        float targetVelocityX = moveInput * moveSpeed;
        float lerpSpeed = isSwimming ? waterLerpSpeed : landLerpSpeed;
        currentVelocityX = Mathf.Lerp(currentVelocityX, targetVelocityX, Time.deltaTime * lerpSpeed);

        bool startedSwimming = isSwimming && !wasSwimming;
        rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);

        if (isSwimming)
        {
            if (startedSwimming)
                RuntimeManager.PlayOneShot(playerSwim, transform.position);

            if (Input.GetButtonDown("Jump"))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, swimUpForce);
                RuntimeManager.PlayOneShot(playerSwim, transform.position);
            }
        }
        else if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            RuntimeManager.PlayOneShot(playerJump, transform.position);
        }

        wasSwimming = isSwimming;

        if (anim != null)
        {
            anim.SetFloat(SpeedHash, Mathf.Abs(moveInput));
            anim.SetBool(IsGroundedHash, isGrounded);
            anim.SetBool(IsSwimmingHash, isSwimming);
        }

        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        if (!isSwimming && groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != waterLayer)
            return;

        isSwimming = true;
        rb.gravityScale = 1f;
        currentVelocityX = rb.linearVelocity.x;

        if (playerCollider != null && waterMaterial != null)
            playerCollider.sharedMaterial = waterMaterial;

        if (snapshotInstance.isValid())
            snapshotInstance.start();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != waterLayer)
            return;

        isSwimming = false;
        rb.gravityScale = 3f;
        currentVelocityX = rb.linearVelocity.x;

        if (playerCollider != null && landMaterial != null)
            playerCollider.sharedMaterial = landMaterial;

        if (snapshotInstance.isValid())
            snapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        if (!snapshotInstance.isValid())
            return;

        snapshotInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        snapshotInstance.release();
    }

    public bool IsSwimming()
    {
        return isSwimming;
    }

    private void DetectFootstepSurface()
    {
        if (groundCheck == null)
            return;

        RaycastHit2D hit = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            footstepRayDistance,
            footstepSurfaceLayers
        );

        if (!hit.collider)
            return;

        currentSurfaceLabel = hit.collider.CompareTag("Sand") ? "Sand" : "Wood";
    }

    private void HandleFootsteps()
    {
        bool isMoving = Mathf.Abs(moveInput) > 0.1f;

        if (!isGrounded || isSwimming || !isMoving)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer += Time.deltaTime;

        if (footstepTimer < walkStepInterval)
            return;

        DetectFootstepSurface();
        PlayFootstep();
        footstepTimer = 0f;
    }

    public void PlayFootstep()
    {
        EventInstance step = RuntimeManager.CreateInstance(footstepEvent);
        RuntimeManager.AttachInstanceToGameObject(step, gameObject);
        step.setParameterByNameWithLabel("SurfaceType", currentSurfaceLabel);
        step.start();
        step.release();
    }
}

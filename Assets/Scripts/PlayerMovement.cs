// using NUnit.Framework;
// using UnityEngine;

// public class PlayerController : MonoBehaviour
// {
//     // Private variables
//     private float movementInputDirection;
//     private Rigidbody2D body;
//     private bool isFacingRight = true;
//     private float jumpCharge = 5f;
//     private bool isChargingJump = false;
//     private bool movementLocked = false;
//     private float velocitySmoothing;
//     private float lastGroundedTime;
//     private float lastJumpInputTime;

//     [Header("Movement Settings")]
//     [SerializeField] private float movementSpeed = 10.0f;
//     [SerializeField] private float acceleration = 8f;
//     [SerializeField] private float deceleration = 8f;
//     [SerializeField] private float airControlFactor = 0.75f;

//     [Header("Jump Settings")]
//     [SerializeField] private float jumpForce = 16.0f;
//     [SerializeField] private float maxJumpValue = 20f;
//     [SerializeField] private float jumpChargeRate = 40f;
//     [SerializeField] private float coyoteTime = 0.2f;
//     [SerializeField] private float jumpBufferTime = 0.2f;

//     [Header("Ground Detection")]
//     [SerializeField] private Transform groundCheck;
//     [SerializeField] private Vector2 boxSize;
//     [SerializeField] private float castDistance;
//     [SerializeField] private float groundCheckRadius = 0.2f;
//     [SerializeField] private LayerMask whatIsGround;
//     [SerializeField] private LayerMask whatIsIcy;

//     [Header("Wall Detection")]
//     [SerializeField] private Transform wallCheck;
//     [SerializeField] private float wallCheckRadius = 0.2f;

//     private int bounceDir = 1;
//     public bool ignoreInput;
//     public bool isWalking;
//     public bool isGrounded;
//     public bool isWall;
//     public bool canJump;

//     void Start()
//     {
//         body = GetComponent<Rigidbody2D>();
//         body.freezeRotation = true;
//         body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
//         body.constraints = RigidbodyConstraints2D.FreezeRotation;
//     }

//     void Update()
//     {
//         CheckInput();
//         CheckMovementDirection();
//         UpdateJumpState();
//     }

//     private void FixedUpdate()
//     {
//         if (!movementLocked && !ignoreInput)
//         {
//             ApplyMovement();
//         } else if (movementLocked)
//         {
//             body.linearVelocity = Vector2.zero;
//         }
//         CheckSurroundings();
//     }

//     private void CheckSurroundings()
//     {
//         bool wasGrounded = isGrounded;
//         bool touchedWall = isWall;

//         isGrounded = Physics2D.BoxCast(groundCheck.position, boxSize, 0, -transform.up, castDistance, whatIsGround);
//         //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
//         isWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsGround);
//         // isIcy = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsIcy);

//         if (isGrounded) {
//             ignoreInput = false;
//             lastGroundedTime = Time.time; // Update coyote time
//         }
//         if (!wasGrounded && isGrounded)
//             jumpCharge = 5f; // Reset jump charge on landing
//     }

//     private void UpdateJumpState()
//     {
//         canJump = Time.time - lastGroundedTime <= coyoteTime;
//     }

//     private void CheckMovementDirection()
//     {
//         if (isFacingRight && movementInputDirection < 0)
//             Flip();
//         else if (!isFacingRight && movementInputDirection > 0)
//             Flip();

//         isWalking = Mathf.Abs(body.linearVelocity.x) > 0.1f && Mathf.Abs(movementInputDirection) > 0;
//     }

//     private void CheckInput()
//     {
//         if (!ignoreInput)
//         {
//             movementInputDirection = Input.GetAxisRaw("Horizontal");
//             if (movementInputDirection < 0)
//             {
//                 bounceDir = 1;
//             }
//             else if (movementInputDirection > 0)
//             {
//                 bounceDir = -1;
//             }
//         }
//         else 
//         {
//             movementInputDirection = 1.0f;
//         }



//         if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
//             lastJumpInputTime = Time.time; // Buffer jump input

//         if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) && /*canJump*/ isGrounded)
//         {
//             isChargingJump = true;
//             movementLocked = true;
//             jumpCharge += jumpChargeRate * Time.deltaTime;
//             jumpCharge = Mathf.Clamp(jumpCharge, 5f, maxJumpValue);
//         }

//         if (isChargingJump && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow)))
//         {
//             if (Time.time - lastGroundedTime <= coyoteTime || Time.time - lastJumpInputTime <= jumpBufferTime)
//                 body.linearVelocity = new Vector2(body.linearVelocity.x, jumpCharge);

//             jumpCharge = 5f;
//             isChargingJump = false;
//             movementLocked = false;
//         }
//         // bounce off wall collision
//         if (isWall && !isGrounded)
//         {
//             ignoreInput = true;
//             body.linearVelocity = new Vector2(bounceDir * (movementSpeed / 2), body.linearVelocity.y);
//         }
//     }

//     private void ApplyMovement()
//     {
//         float targetSpeed = movementInputDirection * movementSpeed;
//         float smoothTime = isGrounded ? (movementInputDirection == 0 ? deceleration : acceleration) : acceleration * airControlFactor;
        
//         float newSpeed = Mathf.SmoothDamp(body.linearVelocity.x, targetSpeed, ref velocitySmoothing, 1f / smoothTime);
//         body.linearVelocity = new Vector2(newSpeed, body.linearVelocity.y);
//     }

//     private void Flip()
//     {
//         isFacingRight = !isFacingRight;
//         transform.Rotate(0.0f, 180.0f, 0.0f);
//     }

//     private void OnDrawGizmos()
//     {
//         //Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
//         Gizmos.DrawWireCube(groundCheck.position-transform.up*castDistance, boxSize);
//     }
// }

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // === Components ===
    private Rigidbody2D body;

    // === Input & Movement ===
    private float movementInputDirection;
    private float velocitySmoothing;
    private bool isFacingRight = true;

    // === Jumping ===
    private float jumpCharge = 5f;
    private bool isChargingJump = false;
    private float lastGroundedTime;
    private float lastJumpInputTime;

    // === State Flags ===
    public bool isGrounded { get; private set; }
    public bool isWall { get; private set; }
    public bool canJump { get; private set; }
    public bool isWalking { get; private set; }
    public bool ignoreInput { get; private set; }
    private bool movementLocked = false;
    private bool isOnIce = false;

    // === Movement Settings ===
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float airControlFactor = 0.75f;

    // === Jump Settings ===
    [Header("Jump Settings")]
    // [SerializeField] private float jumpForce = 16.0f;
    [SerializeField] private float maxJumpValue = 20f;
    [SerializeField] private float jumpChargeRate = 40f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    // === Environment Detection ===
    [Header("Environment Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance = 0.1f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsIcy;

    private int bounceDir = 1;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        HandleInput();
        UpdateJumpState();
        CheckDirectionFlip();
    }

    void FixedUpdate()
    {
        DetectEnvironment();

        if (!movementLocked && !ignoreInput)
        {
            ApplyMovement();
        }
        else if (movementLocked)
        {
            body.linearVelocity = Vector2.zero;
        }
    }

    private void HandleInput()
    {
        movementInputDirection = ignoreInput ? 0 : Input.GetAxisRaw("Horizontal");
        bounceDir = movementInputDirection < 0 ? 1 : (movementInputDirection > 0 ? -1 : bounceDir);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            lastJumpInputTime = Time.time;

        bool jumpHeld = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow);
        bool jumpReleased = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow);

        if (jumpHeld && isGrounded)
        {
            isChargingJump = true;
            movementLocked = true;
            jumpCharge += jumpChargeRate * Time.deltaTime;
            jumpCharge = Mathf.Clamp(jumpCharge, 5f, maxJumpValue);
        }

        if (isChargingJump && jumpReleased)
        {
            if (Time.time - lastGroundedTime <= coyoteTime || Time.time - lastJumpInputTime <= jumpBufferTime)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpCharge);
            }

            jumpCharge = 5f;
            isChargingJump = false;
            movementLocked = false;
        }

        if (isWall && !isGrounded)
        {
            ignoreInput = true;
            body.linearVelocity = new Vector2(bounceDir * (movementSpeed / 2), body.linearVelocity.y);
        }
    }

    private void CheckDirectionFlip()
    {
        if ((isFacingRight && movementInputDirection < 0) || (!isFacingRight && movementInputDirection > 0))
            Flip();

        isWalking = Mathf.Abs(body.linearVelocity.x) > 0.1f && Mathf.Abs(movementInputDirection) > 0;
    }

    private void ApplyMovement() {
        float targetSpeed = movementInputDirection * movementSpeed; // Base target speed
        float smoothTime;

        if (isOnIce)
        {
            if (movementInputDirection == 0)
            {
                smoothTime = 0.8f; // Very slow deceleration — maintain slide
                targetSpeed = body.linearVelocity.x; // Keep momentum
            }
            else
            {
                // On ice: slower to start, but if moving, allow high speed
                float currentSpeed = Mathf.Abs(body.linearVelocity.x);
                float boostedSpeed = Mathf.Clamp(currentSpeed + 2f, movementSpeed, movementSpeed * 2f); // gradually build up

                targetSpeed = movementInputDirection * boostedSpeed; // build momentum over time
                smoothTime = 0.5f; // slower acceleration initially
            }
        }
        else if (isGrounded)
        {
            smoothTime = movementInputDirection == 0 ? 1f / deceleration : 1f / acceleration;
        }
        else
        {
            smoothTime = 1f / (acceleration * airControlFactor);
        }

        float newSpeed = Mathf.SmoothDamp(body.linearVelocity.x, targetSpeed, ref velocitySmoothing, smoothTime);
        body.linearVelocity = new Vector2(newSpeed, body.linearVelocity.y);
    }

    private void UpdateJumpState()
    {
        canJump = Time.time - lastGroundedTime <= coyoteTime;
    }

    private void DetectEnvironment()
    {
        RaycastHit2D hit = Physics2D.BoxCast(groundCheck.position, boxSize, 0f, -transform.up, castDistance, whatIsGround | whatIsIcy);

        isGrounded = false;
        isWall = false;
        isOnIce = false;

        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            int layer = hit.collider.gameObject.layer;

            if (((1 << layer) & whatIsIcy) != 0) // This checks if the surface the player is standing on belongs to the whatIsIcy layer
            {
                isOnIce = true; //it sets isOnIce to true.
            }

            if (Vector2.Angle(normal, Vector2.up) < 45f)
            {
                isGrounded = true;
                lastGroundedTime = Time.time;
                ignoreInput = false;
            }
            else if (Vector2.Angle(normal, Vector2.right) < 45f || Vector2.Angle(normal, Vector2.left) < 45f)
            {
                isWall = true;
            }
        }

        if (isGrounded && jumpCharge < 5f)
        {
            jumpCharge = 5f;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position - transform.up * castDistance, boxSize);
        }
    }
}


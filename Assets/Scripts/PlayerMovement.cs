using NUnit.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Private variables
    private float movementInputDirection;
    private Rigidbody2D body;
    private bool isFacingRight = true;
    private float jumpCharge = 5f;
    private bool isChargingJump = false;
    private bool movementLocked = false;
    private float velocitySmoothing;
    private float lastGroundedTime;
    private float lastJumpInputTime;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float airControlFactor = 0.75f;

    [SerializeField] private float iceAccel = 2f;
    [SerializeField] private float iceDrag = 1f;

    [SerializeField] private float mudAccel = 6f;
    [SerializeField] private float mudDrag = .8f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 16.0f;
    [SerializeField] private float maxJumpValue = 20f;
    [SerializeField] private float jumpChargeRate = 40f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsIce;
    [SerializeField] private LayerMask whatIsMud;

    [Header("Wall Detection")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckRadius = 0.2f;

    private int bounceDir = 1;
    public bool ignoreInput;
    public bool isWalking;
    public bool isGrounded;
    public bool isIce;
    public bool isWall;
    public bool isMud;
    public bool canJump;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateJumpState();
    }

    private void FixedUpdate()
    {
        if (!movementLocked && !ignoreInput)
        {
            ApplyMovement();
        } else if (movementLocked)
        {
            body.linearVelocity = Vector2.zero;
        }
        CheckSurroundings();
    }

    private void CheckSurroundings()
    {
        bool wasGrounded = isGrounded;
        bool touchedWall = isWall;

        isGrounded = Physics2D.BoxCast(groundCheck.position, boxSize, 0, -transform.up, castDistance, whatIsGround | whatIsIce | whatIsMud);
        isIce = Physics2D.BoxCast(groundCheck.position, boxSize, 0, -transform.up, castDistance, whatIsIce);
        isMud = Physics2D.BoxCast(groundCheck.position, boxSize, 0, -transform.up, castDistance, whatIsMud);
        isWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsGround);


        if (isGrounded) {
            ignoreInput = false;
            lastGroundedTime = Time.time; // Update coyote time
        }
        if (!wasGrounded && isGrounded)
            jumpCharge = 5f; // Reset jump charge on landing
    }

    private void UpdateJumpState()
    {
        canJump = Time.time - lastGroundedTime <= coyoteTime;
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
            Flip();
        else if (!isFacingRight && movementInputDirection > 0)
            Flip();

        isWalking = Mathf.Abs(body.linearVelocity.x) > 0.1f && Mathf.Abs(movementInputDirection) > 0;
    }

    private void CheckInput()
    {
        if (!ignoreInput)
        {
            movementInputDirection = Input.GetAxisRaw("Horizontal");
            if (movementInputDirection < 0)
            {
                bounceDir = 1;
            }
            else if (movementInputDirection > 0)
            {
                bounceDir = -1;
            }
        }
        else 
        {
            movementInputDirection = 1.0f;
        }



        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            lastJumpInputTime = Time.time; // Buffer jump input

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) && /*canJump*/ isGrounded)
        {
            isChargingJump = true;
            movementLocked = true;
            jumpCharge += jumpChargeRate * Time.deltaTime;
            jumpCharge = Mathf.Clamp(jumpCharge, 5f, maxJumpValue);
        }

        if (isChargingJump && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow)))
        {
            if (Time.time - lastGroundedTime <= coyoteTime || Time.time - lastJumpInputTime <= jumpBufferTime)
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpCharge);

            jumpCharge = 5f;
            isChargingJump = false;
            movementLocked = false;
        }
        // bounce off wall collision
        if (isWall && !isGrounded)
        {
            ignoreInput = true;
            body.linearVelocity = new Vector2(bounceDir * (movementSpeed / 2), body.linearVelocity.y);
        }
    }

    private void ApplyMovement()
    {
        /*
        float targetSpeed = movementInputDirection * movementSpeed;
        float smoothTime = isGrounded ? (movementInputDirection == 0 ? deceleration : acceleration) : acceleration * airControlFactor;
        
        float newSpeed = Mathf.SmoothDamp(body.linearVelocity.x, targetSpeed, ref velocitySmoothing, 1f / smoothTime);
        float moveForce = Mathf.Clamp(targetSpeed, 0f, newSpeed);
        if (isIce && body.linearVelocityX < newSpeed)
        {
            body.AddForce(new Vector2(moveForce, body.linearVelocity.y));
        } else
        {
            body.linearVelocity = new Vector2(newSpeed, body.linearVelocity.y);
        } */
        
         if (isIce)
        {

            float desiredVelocityX = movementInputDirection * movementSpeed * 1.25f;

            // Smooth velocity change on ice
            float newVelocityX = Mathf.Lerp(body.linearVelocity.x, desiredVelocityX, Time.fixedDeltaTime * iceAccel);

            // Preserve some of the previous velocity (simulate sliding)
            newVelocityX *= iceDrag;

            body.linearVelocity = new Vector2(newVelocityX, body.linearVelocity.y);
        } 
        
        else if (isMud)
        {

            float desiredVelocityX = movementInputDirection * movementSpeed * 1.25f;

            // Smooth velocity change on mud
            float newVelocityX = Mathf.Lerp(body.linearVelocity.x, desiredVelocityX, Time.fixedDeltaTime * mudAccel);

            // Preserve some of the previous velocity (simulate sliding)
            newVelocityX *= mudDrag;

            body.linearVelocity = new Vector2(newVelocityX, body.linearVelocity.y);
        }
        else
        {
            float groundAccel = 10f;
            float desiredVelocityX = movementInputDirection * movementSpeed;
            float newVelocityX = Mathf.Lerp(body.linearVelocity.x, desiredVelocityX, Time.fixedDeltaTime * groundAccel);
            body.linearVelocity = new Vector2(newVelocityX, body.linearVelocity.y);

            // body.linearVelocity = Mathf.Lerp(body.linearVelocity.x, desiredVelocityX, Time.deltaTime * groundAccel);
        }






    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        // makes raycast outline visible
        Gizmos.DrawWireCube(groundCheck.position-transform.up*castDistance, boxSize);
        Gizmos.DrawWireSphere(wallCheck.position * castDistance, wallCheckRadius);
    }
}

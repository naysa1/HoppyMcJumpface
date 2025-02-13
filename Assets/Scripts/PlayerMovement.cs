using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] private float speed; // Speed of horizontal movement
    [SerializeField] private float maxJumpValue = 20f; // Maximum jump force
    [SerializeField] private float jumpChargeRate = 40f; // Rate at which jump force increases
    private float jumpCharge = 5f; // Current jump charge
    private float slideFactor = 5f;
    private Vector3 originalScale;
    private Animator anim;
    [SerializeField] private bool Grounded;

    private bool isChargingJump = false;
    private bool movementLocked = false;
    private bool isSliding = false;
    private bool ignoreInput;
    private float bounceDir = 0f;
    private const float wallCheckRadius = 0.5f;

    public Transform wallCheckCollider;
    public LayerMask wallLayer;

    

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        anim = GetComponent<Animator>();
        body.freezeRotation = true;
        ignoreInput = false;
    }

    private void Update()
    {
        // movemvent script
        float horizontalInput;
        if (!ignoreInput)
        {
            horizontalInput = Input.GetAxis("Horizontal"); // saving user input
            if (horizontalInput < 0f)
            {
                bounceDir = 1.0f;
            } else if (horizontalInput > 0f)
            {
                bounceDir = -1.0f;
            }
        } else
        {
            horizontalInput = 1.0f;
        }
        if (!movementLocked && !ignoreInput)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y); // moving user horizontally 
        } else if (movementLocked)
        {
            body.linearVelocity = Vector2.zero; // lock movement 
        }

        // Flip sprite based on movement direction
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        }
        if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
        if (body.linearVelocity.y < 0f)
        {
            Grounded = false;
        }
        // Start charging jump
        if (Grounded && (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))) 
        {
            //movementLocked = true; //Frog cannot move
            isChargingJump = movementLocked = true;
            jumpCharge += jumpChargeRate * Time.deltaTime; // Increase jump charge
            jumpCharge = Mathf.Clamp(jumpCharge, 0f, maxJumpValue); // Limit to maxJumpValue
        }

        // Release jump
        if (isChargingJump && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow)))
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpCharge); // Apply jump force
            jumpCharge = 0f; // Reset jump charge
            isChargingJump = false;
            Grounded = false; // Frog is airborne
            movementLocked = false; //Frog can move
        }

        // Update animator parameters
        anim.SetBool("Run", body.linearVelocity != Vector2.zero);
        anim.SetBool("Grounded", Grounded);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = true;
            ignoreInput = false;
            movementLocked = false;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            ignoreInput = true;
            Grounded = false;

            body.linearVelocity = new Vector2(bounceDir*(speed/2), body.linearVelocity.y); // moving user horizontally
            
            //WallCheck();
        }
    }

    void WallCheck()
    {
        //if touching a wall
        //moving towards wall
        //falling
        //not grounded
        //slide on wall
        if(Physics2D.OverlapCircle(wallCheckCollider.position, wallCheckRadius, wallLayer)
            && Mathf.Abs(Input.GetAxis("Horizontal")) > 1
            && body.linearVelocityY <= 0
            && !Grounded)
        {
            Vector2 v = body.linearVelocity;
            v.y = -slideFactor;
            body.linearVelocity = v;
            isSliding = true;
            if (!isSliding)
            {
                //Grounded = movementLocked = false;
            }
        }
        else
        {
            isSliding = false;
        }
    }
}


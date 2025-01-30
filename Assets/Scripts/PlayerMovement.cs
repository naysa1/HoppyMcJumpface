using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] private float speed; // Speed of horizontal movement
    [SerializeField] private float maxJumpValue = 20f; // Maximum jump force
    [SerializeField] private float jumpChargeRate = 40f; // Rate at which jump force increases
    private float jumpCharge = 5f; // Current jump charge
    private Vector3 originalScale;
    private Animator anim;
    [SerializeField] private bool Grounded;

    private bool isChargingJump = false;
    private bool movementLocked = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // saving user input 
        if (movementLocked == false)
        {
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y); // moving user horizontally 
        } else
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
        if (collision.gameObject.CompareTag("Tilemap"))
        {
            Grounded = true;
        }
    }
}


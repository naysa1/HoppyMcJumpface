using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   private Rigidbody2D body; // sets up a reference to the rigidbody in Unity and will be refered to as body
   [SerializeField]private float speed;
   [SerializeField]private float jumpspeed;
   private Vector3 originalScale;
   private Animator anim;
   private bool Grounded;

   private void Awake() // this method will be called everytime the script loads
   {
        // grab references for rigidbody & animator from object
        body = GetComponent<Rigidbody2D>(); // a way to access rigidbody2D
        originalScale = transform.localScale;
        anim = GetComponent<Animator>();
   }

   private void Update()
   {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.linearVelocity.y);
       
        //flip player horizontally
        if(horizontalInput>0.01f)
            {
                transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            }
        if(horizontalInput<-0.01f)
            {
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
            }

        if (Grounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)))
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpspeed);
            }
        
        // set animator parameters
        anim.SetBool("Run", horizontalInput != 0); // if input = 0 and if 0 != 0 then thats FALSE so, no input means player not running
        anim.SetBool("Grounded", Grounded);
   }


   private void Jump() {
        body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
        Grounded = false;
   }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Ground") {
            Grounded = true;
        }
    }
}

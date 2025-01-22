using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   private Rigidbody2D body; // sets up a reference to the rigidbody in Unity and will be refered to as body
   [SerializeField]private float speed;
   [SerializeField]private float jumpspeed;
   private Vector3 originalScale;
   private Animator 

   private void Awake() // this method will be called everytime the script loads
   {
        body = GetComponent<Rigidbody2D>(); // a way to access rigidbody2D
        originalScale = transform.localScale;
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

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpspeed);
            }
   }
}

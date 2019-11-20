using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float runSpeed;
    public float jumpForce;
    public Animator animator;
    public Rigidbody2D rb2d;
    public float scaleModifier; // this is to convert the players scale into meters
    public float moveAcceleration;
    public GameObject leftFoot;
    public GameObject rightFoot;

    private float moveHorizontal = 0f;
    private bool jump = false;
    private bool falling = false;
    private bool canDoubleJump = true;
    private bool facingRight = true;
    private bool grounded = false;
 
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

        // calculate animation effects
        moveHorizontal = Input.GetAxisRaw("Horizontal") * runSpeed;

        if(falling) //animator.SetBool("IsJumping", false);

        if (Input.GetButtonDown("Jump")) {
            if (!grounded) {
                if (canDoubleJump){
                    jump = true;
                    canDoubleJump = false;
                    //animator.SetBool("DoubleJump", true);
                    //animator.SetBool("IsJumping", true);
                }
            } else {
                jump = true;
                //animator.SetBool("IsJumping", true);
            }
        }

        //animator.SetFloat("Speed", Mathf.Abs(moveHorizontal));
        //animator.SetBool("IsFalling", falling);
        OrientPlayer();
    }

    // Fixed update is called just before calculating any physics
    private void FixedUpdate() {
        bool wasGrounded = grounded;
        grounded = Grounded();
        Vector2 velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y);

        if (!wasGrounded && grounded && velocity.y <= 0) OnLanded();

        falling = velocity.y < 0 && !grounded;
        velocity.x += moveHorizontal * moveAcceleration * Time.fixedDeltaTime;
        if (velocity.x > runSpeed * scaleModifier){
            velocity.x = runSpeed * scaleModifier;
        } else if (velocity.x < -runSpeed * scaleModifier){
            velocity.x = -runSpeed * scaleModifier;
        }
        if (jump) {
            velocity.y = jumpForce * scaleModifier;
        }
        rb2d.velocity = velocity;
        jump = false;
        
    }

    //OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
    void OnTriggerEnter2D(Collider2D other)
    {
        //Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
        if (other.gameObject.CompareTag("Pickup")) {
            other.gameObject.SetActive(false);
        }
    }

    public void OnLanded(){
        canDoubleJump = true;
        //animator.SetBool("DoubleJump", false);
        //animator.SetBool("IsJumping", false);
        //animator.SetBool("IsFalling", false);
    }

    private void OrientPlayer()
    {
        if ((moveHorizontal > 0 && !facingRight) || (moveHorizontal < 0 && facingRight)){
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public bool Grounded() {
        // check left foot
        Vector3 bottomPosition = leftFoot.transform.position;
        // Debug.DrawRay(bottomPosition, Vector3.down * 0.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(bottomPosition, Vector3.down, 0.1f);
        if (hit.collider) return true;

        // check right foot
        bottomPosition = rightFoot.transform.position;
        // Debug.DrawRay(bottomPosition, Vector3.down * 0.1f, Color.red);
        hit = Physics2D.Raycast(bottomPosition, Vector3.down, 0.1f);
        return hit.collider != null;
    }
}

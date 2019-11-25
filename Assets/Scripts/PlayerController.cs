using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {


    
    private GameObject leftFoot;
    private GameObject rightFoot;
    private Rigidbody2D rb2d;
    private Animator animator;
    private float moveHorizontal = 0f;
    private bool jump = false;
    private bool falling = false;
    private bool canDoubleJump = true;
    private bool facingRight = true;
    private bool grounded = false;
    private Vector2 previousVelocity;

    // gravity increases velocity of an object at 9.8 meters per second
    // example, after 3 seconds of free fall, an object will be travelling at 9.8 * 3 meters per second
    private static float GRAVITY = 9.8f;

    // https://www.angio.net/personal/climb/speed
    // terminal velocity is around 320kph, or lying flat, 195kph, therefore I'm setting this in the middle at 257.5
    // 257.5kph converts to 71.527777777777800 mps
    private static float TERMINAL_VELOCITY = 71.527777777777800f;

    // https://en.wikipedia.org/wiki/Walking
    // Average walking speed is 5.32kph, which converts to 1.4777777777777800 mps
    private static float MAX_WALK_SPEED = 1.4777777777777800f;
    private static float WALK_SPEED_ACCELERATION = MAX_WALK_SPEED * 2;

    // https://www.iamlivingit.com/running/average-human-running-speed
    // Average running speed is 13.62kph, which converts to 3.7833333333333300 mps
    private static float MAX_RUN_SPEED = 3.7833333333333300f;
    private static float RUN_SPEED_ACCELERATION = MAX_RUN_SPEED * 2;

    private static float FLOOR_FRICTION = RUN_SPEED_ACCELERATION * 0.75f;
    private static float JUMP_ACCELERATION = MAX_RUN_SPEED * 1.2f;


    // Start is called before the first frame update
    void Start() {
        leftFoot = GameObject.Find("LeftFoot");
        rightFoot = GameObject.Find("RightFoot");
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        // calculate animation effects
        moveHorizontal = Input.GetAxisRaw("Horizontal") * MAX_RUN_SPEED;

        if(falling)
		{ 
			//animator.SetBool("IsJumping", false);
		}

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

        Vector2 velocity = rb2d.velocity;
        bool wasGrounded = grounded;
        grounded = Grounded();


        if (!wasGrounded && grounded && previousVelocity != null && previousVelocity.y < 0) {
            OnLanded(velocity.y >= 0 ? previousVelocity : velocity);
        }
        
        falling = velocity.y < 0 && !grounded;

        // apply enviromental forces (gravity / friction)
        if (velocity.x < 0) {
            velocity.x += FLOOR_FRICTION * Time.fixedDeltaTime;
            // stops friction changing player from sliding left to right, instead they should stop
            if (velocity.x > 0) {
                velocity.x = 0;
            }
        } else if (velocity.x > 0) {
            velocity.x -= FLOOR_FRICTION * Time.fixedDeltaTime;
            // stops friction changing player from sliding left to right, instead they should stop
            if (velocity.x < 0) {
                velocity.x = 0;
            }
        }

        velocity.y -= GRAVITY * Time.fixedDeltaTime;
        if (velocity.y < -TERMINAL_VELOCITY) {
            velocity.y = -TERMINAL_VELOCITY;
        }


        velocity.x += moveHorizontal * RUN_SPEED_ACCELERATION * Time.fixedDeltaTime;
        if (velocity.x > MAX_RUN_SPEED) {
            velocity.x = MAX_RUN_SPEED;
        } else if (velocity.x < -MAX_RUN_SPEED) {
            velocity.x = -MAX_RUN_SPEED;
        }
        if (jump) {
            velocity.y = JUMP_ACCELERATION;
            AudioController.PlaySound("Jump");
        }
        rb2d.velocity = velocity;
        previousVelocity = velocity;
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

    public void OnLanded(Vector2 velocity){
        if (velocity.y <= -(TERMINAL_VELOCITY / 10)) {
            AudioController.PlaySound("PlayerHit");
        }
        
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

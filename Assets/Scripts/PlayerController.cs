using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

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
    private Vector2 hitVelocity;
	public bool paused;
    private int health;
    private bool inWater = false;
    private float timeInWater = 0;
    private bool dying = false;
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
        health = 3;
    }

    // Update is called once per frame
    void Update() {

        // calculate animation effects
        moveHorizontal = Input.GetAxisRaw("Horizontal") * MAX_RUN_SPEED;

        if(falling)
		{ 
			animator.SetBool("IsJumping", false);
            animator.SetBool("DoubleJump", false);
        }

        if (Input.GetButtonDown("Jump") && !inWater) {
            if (!grounded) {
                if (canDoubleJump){
                    jump = true;
                    canDoubleJump = false;
                    animator.SetBool("DoubleJump", true);
                    animator.SetBool("IsJumping", true);
                }
            } else {
                jump = true;
                animator.SetBool("IsJumping", true);
            }
        }

        animator.SetBool("IsWalking", Mathf.Abs(rb2d.velocity.x) > 0.001);
        animator.SetBool("IsFalling", falling);
        OrientPlayer();
    }

    // Fixed update is called just before calculating any physics
    private void FixedUpdate() {

        Vector2 velocity = rb2d.velocity;
        bool wasGrounded = grounded;
        grounded = Grounded();
        TileSwitch tileSwitch = GetComponent<TileSwitch>();
        Tilemap tiles = tileSwitch.GetActiveTileset();
        TileBase leftFootTile = tiles.GetTile(tiles.WorldToCell(leftFoot.transform.position));
        TileBase rightFootTile = tiles.GetTile(tiles.WorldToCell(rightFoot.transform.position));


        if ((leftFootTile != null && (leftFootTile.name == "jungleTilemap_9" || leftFootTile.name == "jungleTilemap_19" || leftFootTile.name == "jungleTilemap_8" || leftFootTile.name == "jungleTilemap_18")) ||
            (rightFootTile != null && (rightFootTile.name == "jungleTilemap_9" || rightFootTile.name == "jungleTilemap_19" || rightFootTile.name == "jungleTilemap_8" || rightFootTile.name == "jungleTilemap_18"))) {
            if (!inWater) {
                velocity.x /= 2;
                velocity.y /= 3;
                AudioController.PlaySound("Splash");
                timeInWater = Time.time;
            }
            inWater = true;
            if (Time.time - timeInWater > 1) {
                AudioController.PlaySound("PlayerHit");
                health--;
                timeInWater++;
            }
        } else {
            inWater = false;
        }

        if (!wasGrounded && grounded && previousVelocity != null && previousVelocity.y < 0) {
            OnLanded(velocity.y >= 0 ? previousVelocity : velocity);
        }
        
        // apply enviromental forces (gravity / friction / hits)
        velocity.x += hitVelocity.x;

        if (velocity.x < 0) {
            velocity.x += FLOOR_FRICTION * Time.fixedDeltaTime * (inWater ? 2f : 1f);
            // stops friction changing player from sliding left to right, instead they should stop
            if (velocity.x > 0) {
                velocity.x = 0;
            }
        } else if (velocity.x > 0) {
            velocity.x -= FLOOR_FRICTION * Time.fixedDeltaTime * (inWater ? 2f : 1f);
            // stops friction changing player from sliding left to right, instead they should stop
            if (velocity.x < 0) {
                velocity.x = 0;
            }
        }

        velocity.y -= GRAVITY * Time.fixedDeltaTime * (inWater ? 0.1f : 1f);
        if (velocity.y < -TERMINAL_VELOCITY) {
            velocity.y = -TERMINAL_VELOCITY;
        }


        velocity.x += moveHorizontal * RUN_SPEED_ACCELERATION * Time.fixedDeltaTime * (inWater ? 0.5f : 1f);
        if (velocity.x > MAX_RUN_SPEED * (inWater ? 0.5f : 1f)) {
            velocity.x = MAX_RUN_SPEED * (inWater ? 0.5f : 1f);
        } else if (velocity.x < -(MAX_RUN_SPEED * (inWater ? 0.5f : 1f))) {
            velocity.x = -(MAX_RUN_SPEED * (inWater ? 0.5f : 1f));
        }
        if (jump) {
            velocity.y = JUMP_ACCELERATION;
            AudioController.PlaySound("Jump");
        }
        rb2d.velocity = velocity;
        previousVelocity = velocity;
        hitVelocity.x = 0;
        jump = false;
        falling = velocity.y < 0 && !grounded;
        if (health <= 0) {
            Die();
        }

    }

    //OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
    void OnTriggerEnter2D(Collider2D obj)
    {
        //Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
        if (obj.gameObject.CompareTag("Projectile")) {
            hitVelocity = obj.GetComponent<Rigidbody2D>().velocity;
            obj.gameObject.SetActive(false);
            AudioController.PlaySound("PlayerHit");
            health--;
        }
    }

    public void OnLanded(Vector2 velocity){
        if (velocity.y <= -(TERMINAL_VELOCITY / 10)) {
            AudioController.PlaySound("PlayerHit");
        }
        
        canDoubleJump = true;
        animator.SetBool("DoubleJump", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
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
        if (hit.collider) {
            return true;
        }

        // check right foot
        bottomPosition = rightFoot.transform.position;
        // Debug.DrawRay(bottomPosition, Vector3.down * 0.1f, Color.red);
        hit = Physics2D.Raycast(bottomPosition, Vector3.down, 0.1f);
        return hit.collider != null;
    }

    public bool OnWater() {
        return false;
    }

    public int GetHealth() {
        return health;
    }

    public void Die() {
        if (!dying) {
            dying = true;
            AudioController.PlaySound("GameOver");
            KillObject kill = gameObject.AddComponent<KillObject>();
            kill.PlayerKill();
        }
    }
}

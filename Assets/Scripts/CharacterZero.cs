using UnityEngine;
using System.Collections;

public class CharacterZero : MonoBehaviour {

	[SerializeField] private float maxSpeed = 10f;                    // The fastest the player can travel in the x axis.
	[SerializeField] private float jumpForce = 400f;                  // Amount of force added when the player jumps.
	[SerializeField] private bool airControl = true;                 // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask whatIsGround;                  // A mask determining what is ground to the character
	[SerializeField] private bool doubleJump = false;
	[SerializeField] private float dashForce = 6000.0f;

	private Transform groundCheck;    // A position marking where to check if the player is grounded.
	const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool grounded;            // Whether or not the player is grounded.
	const float ceilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
	private Animator anim;            // Reference to the player's animator component.
	private Rigidbody2D rBody;
	private bool facingRight = true;  // For determining which way the player is currently facing.

	private float attackTime = 0f;
	private int attackCount = 0;
	private bool isAttacking = false;
	private bool airAttacking = false;

	private float dashTime = 0f;
	private bool isDashing = false;

	private void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("GroundCheck");
		anim = GetComponent<Animator> ();
		rBody = GetComponent<Rigidbody2D> ();
	}

	private void FixedUpdate()
	{
		grounded = false;
		
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject) {
				grounded = true;
				doubleJump = true;
			}
		}
		anim.SetBool("ground", grounded);
		
		// Set the vertical animation
		anim.SetFloat("verticalSpeed", rBody.velocity.y);


		// Stop attack if the time for animation is over
		if (attackTime != 0f && Time.time - attackTime > 0.4f) {
			isAttacking = false;
			anim.SetBool ("isAttacking", isAttacking);
			anim.SetBool ("attack3", false);
			airAttacking = false;
			anim.SetBool ("airAttack", airAttacking);
			attackCount = 0;
			attackTime = 0f;
		}

		if (isDashing && dashTime != 0f && Time.time - dashTime > 0.3f && grounded) {
			isDashing = false;
			dashTime = 0f;
		}
	}

	public void move(float move, bool jump, bool dash)
	{
		
		//only control the player if grounded or airControl is turned on
		if ((grounded || airControl) && !isAttacking) {
			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat("speed", Mathf.Abs(move));

			if((dash && grounded) || isDashing) {
				rBody.AddForce(dashForce*(facingRight ? Vector2.right : Vector2.left));
				isDashing = true;
				if(dashTime == 0f) {
					dashTime = Time.time;
					anim.SetTrigger("dash");
				}	
			}

			// Move the character
			rBody.velocity = new Vector2(move*maxSpeed, rBody.velocity.y);
			
			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !facingRight)
			{
				// ... flip the player.
				flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && facingRight)
			{
				// ... flip the player.
				flip();
			}
		}
		// If the player should jump...
		if (grounded && jump && anim.GetBool("ground"))
		{
			// Add a vertical force to the player.
			grounded = false;
			anim.SetBool("ground", false);
			rBody.AddForce(new Vector2(0f, jumpForce));
		} else if (!grounded && jump && doubleJump && !isDashing) {
			stopMotion ();
			rBody.AddForce(new Vector2(0f, jumpForce));
			anim.SetTrigger("jump");
			doubleJump = false;
		}


	}

	public void attack () {

		if (grounded) {
			if (attackCount == 0) {
				anim.SetTrigger ("attack");
				attackCount++;
				attackTime = Time.time;
			} else if (attackCount == 1) {
				attackTime = Time.time;
				anim.SetTrigger ("attack2");
				attackCount++;
			} else if (attackCount == 2) {
				attackTime = Time.time;
				anim.SetBool ("attack3", true);
				attackCount = 0;
			}
		} else {
			if(!anim.GetBool ("isAttacking")) {
				attackTime = Time.time;
				airAttacking = true;
				anim.SetBool ("airAttack", airAttacking);
			}
		} 
		isAttacking = true;
		anim.SetBool ("isAttacking", isAttacking);
		if(!airAttacking) stopMotion ();
	}

	private void stopMotion() {
		rBody.velocity = new Vector2(0, 0);
	}
	
	private void flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}

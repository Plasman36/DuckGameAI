using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public BoxCollider2D bc;
    public Animator Animator;

    [Header("Horizontal Movement")]
    public float speed;
    private float moveInput;
    private bool isFacingRight;
    public float maxSpeed;
    public float accelerationDecay;
    public float accelerationAddition;

    [Header("Slide & Crouch")]
    public float slideFriction;
    private float slideCounter;
    public float slideDecreaseAmount;
    public float slideHardCutoff;
    private bool isSliding;
    private bool isCrouching;

    [Header("Jump")]
    public float jumpForce;
    private float jumpTimeCounter;
    public float jumpTime;

    [Header("Hovering")]
    public float hoverSpeed;
    private bool isHoldingWDuringJump;

    [Header("Coyote Time")]
    public float coyoteTime;
    private float coyoteTimeCounter;

    [Header("Jump Buffering")]
    public float jumpBufferTime;
    private float jumpBufferingCounter;

    //checks
    [Header("Ground Checks")]
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;
    private bool isGrounded;
    private bool isJumping;

    [Header("Head Checks")]
    public Transform headPos;
    public float headRadius;
    private bool isHeadHit;

    //gravity Mods
    [Header("Gravity Modifiers")]
    public float gravityMod;
    public float gravityCut;
    private float gravity;
    private float gravityCounter;
    private float zeroGravityTime;
    public float fallClamp;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
        gravityCounter = 0;
    }

    private void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        //Horizontal Movement
        Debug.Log(moveInput);

        speed = rb.velocity.x;

        if (!isSliding && !isCrouching)
        {
            if (moveInput == 1 && speed < maxSpeed)
            {
                speed += accelerationAddition;
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else if (moveInput == 1 && speed >= maxSpeed)
            {
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
            if (moveInput == -1 && speed > -maxSpeed)
            {
                speed -= accelerationAddition;
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else if (moveInput == -1 && speed <= -maxSpeed)
            {
                rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
            }
            if (moveInput == 0)
            {
                if (speed > 0)
                {
                    speed -= accelerationDecay;
                    if (speed < 0) { 
                        speed = 0; 
                    }
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                else if (speed < 0)
                {
                    speed += accelerationDecay;
                    if (speed > 0)
                    {
                        speed = 0;
                    }
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
            }
        }

        //Fall Clamp and Hover
        if (rb.velocity.y < 0)
        {
            if(rb.velocity.y < hoverSpeed && Input.GetKey(KeyCode.W) && isHoldingWDuringJump == false)
            {
                rb.velocity = new Vector2(rb.velocity.x, hoverSpeed);
            }
            else if (rb.velocity.y < fallClamp)
            {
                rb.velocity = new Vector2(rb.velocity.x, fallClamp);
            }
        }

        //Animation Flip
        if(isFacingRight == false && moveInput < 0)
        {
            Flip();
        }
        else if (isFacingRight == true && moveInput > 0)
        {
            Flip();
        }
    }

    private void Update()
    {
        slideFriction = Mathf.Abs(rb.velocity.x);

        Animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        if (Input.GetKey(KeyCode.S))
        {
            if (Mathf.Abs(rb.velocity.x) > 0.1 && isGrounded)
            {
                Animator.SetBool("isSliding", true);
                Animator.SetBool("isCrouching", false);

                isSliding = true;
                isCrouching = false;
            }
            else if(!isSliding)
            {
                Animator.SetBool("isSliding", false);
                Animator.SetBool("isCrouching", true);

                isSliding = false;
                isCrouching = true;
            }
        }

        if (rb.velocity.y < -0.01 && (!isCrouching && !isSliding))
        {
            Animator.SetBool("isFalling", true);
            Animator.SetBool("isJumping", false);
        }
        if(rb.velocity.y > -0.01 && rb.velocity.y < 0.01)
        {
            Animator.SetBool("isFalling", false);
            Animator.SetBool("isJumping", false);
        }

        if (rb.velocity.y > 0.01 && (!isCrouching && !isSliding))
        {
            Animator.SetBool("isJumping", true);
            Animator.SetBool("isFalling", false);
        }

        if (isJumping)
        {
            isSliding = false;
        }

        if (!isSliding)
        {
            slideCounter = slideFriction;
        }

        if (isSliding && slideCounter >= slideHardCutoff)
        {
            rb.velocity = new Vector2(moveInput * slideCounter, rb.velocity.y);
            slideCounter -= slideDecreaseAmount;
        }else if (isSliding)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            Animator.SetBool("isCrouching", false);
            Animator.SetBool("isSliding", false);

            isSliding = false;
            isCrouching = false;

        }

        //Ground Check
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        //Head Check
        isHeadHit = Physics2D.OverlapCircle(headPos.position, headRadius, whatIsGround);

        if (isHeadHit)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            isJumping = false;
            rb.gravityScale = gravity;
        }

        if (isGrounded)
        {
            gravityCounter = 0;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpBufferingCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferingCounter -= Time.deltaTime;
        }

        //Jump Initiation
        if(coyoteTimeCounter > 0f && jumpBufferingCounter > 0f)
        {
            //rb.velocity = Vector2.up * jumpForce;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            jumpTimeCounter = jumpTime;
            GravUp();
            isHoldingWDuringJump = true;
            jumpBufferingCounter = 0f;
        }

        //Grav Change
        if (isJumping)
        {
            ++gravityCounter;
            if (gravityCounter >= zeroGravityTime)
            {
                rb.gravityScale = rb.gravityScale + gravityCut;
            }
            if (rb.gravityScale == gravity)
            {
                rb.gravityScale = gravity;
            }
        }

        //Jump Hold
        if (Input.GetKey(KeyCode.W) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                //rb.velocity = Vector2.up * jumpForce;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
                isHoldingWDuringJump = true;
            }
            else
            {
                isJumping = false;
                rb.gravityScale = gravity;
            }
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            isJumping = false;
            rb.gravityScale = gravity;
            isHoldingWDuringJump = false;
            coyoteTimeCounter = 0f;
        }
        if (rb.velocity.y < 0.01)
        {
            GravDown();

        }
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    void GravUp()
    {
        rb.gravityScale = gravityMod;
    }

    void GravDown()
    {
        rb.gravityScale = gravity;
    }
}
/*If I press down and there is a horizontal velocity
	Change player scale to fit a slide
	Slowly decrease velocity until it’s 0

	(ALSO REMOVE THE CAPABILITY TO WALK, SLIDING AND WALKING AT THE SAME TIME IS KINDA WEIRD)
Else if I press down (without a horizontal velocity)
	Squat
*/
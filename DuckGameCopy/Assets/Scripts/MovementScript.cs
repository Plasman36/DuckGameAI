using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Horizontal Movement")]
    public float speed;
    private float moveInput;

    [Header("Jump")]
    public float jumpForce;

    //Hold Counters
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
        //Horizontal Movement
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        //Fall Clamp and Hover
        if(rb.velocity.y < 0)
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
    }

    private void Update()
    {
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
            rb.velocity = Vector2.up * jumpForce;
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
                rb.velocity = Vector2.up * jumpForce;
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
        if (rb.velocity.y < 0f)
        {
            GravDown();
        }
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

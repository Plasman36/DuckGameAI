using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //movement
    public float speed;
    public float jumpForce;
    private float moveInput;

    private Rigidbody2D rb;

    private bool facingRight = true;

    //ground checks
    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    //jumping stuff
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    //gravity modifications
    private float gravity;
    public float gravitymod;

    //coyote stuff
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    //jump buffering
    public float jumpBufferingTime = 0.2f;
    private float jumpBufferingCounter;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveInput = Input.GetAxis("Horizontal");

        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
        }
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        if (isGrounded == true)
        {
            jumpTimeCounter = jumpTime;
            GravDown();
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpBufferingCounter = jumpBufferingTime;
        }
        else
        {
            jumpBufferingCounter -= Time.deltaTime;
        }

        //check for jump
        if (jumpBufferingCounter > 0f && coyoteTimeCounter > 0f)
        {
            jumpBufferingCounter = 0f;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
            GravDown();
        }

        //hold to jump higher
        if (Input.GetKey(KeyCode.W) && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            isJumping = false;
            coyoteTimeCounter = 0f;
            GravUp();
        }

        //checks for grav
        if (rb.velocity.y < 0f)
        {
            GravUp();
        }

    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    void GravUp()
    {
        rb.gravityScale = gravitymod;
    }

    void GravDown()
    {
        rb.gravityScale = gravity;
    }
}

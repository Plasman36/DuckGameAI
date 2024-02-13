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

    //checks
    [Header("Checks")]
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;
    private bool isGrounded;
    private bool isJumping;

    [Header("Gravity Modifiers")]
    private float gravity;
    public float gravityMod;
    public float gravityCut;
    public float gravityCounter;
    public float zeroGravityTime;
    public float thisisgravity;

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
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    private void Update()
    {
        thisisgravity = rb.gravityScale;
        //Ground Check
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        if (isGrounded)
        {
            gravityCounter = 0;
        }

        //Jump Initiation
        if(isGrounded == true && Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = Vector2.up * jumpForce;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            GravUp();
        }

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
        if (Input.GetKey(KeyCode.W)&& isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
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

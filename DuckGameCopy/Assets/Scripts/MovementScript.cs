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
    public float gravitymod;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
    }

    private void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    private void Update()
    {
        //Ground Check
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        
        //Jump Initiation
        if(isGrounded == true && Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = Vector2.up * jumpForce;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            GravUp();
        }

        //Jump Hold
        if (Input.GetKey(KeyCode.W)&& isJumping == true)
        {
            if(jumpTimeCounter > 0)
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
        }
        if (rb.velocity.y < 0f)
        {
            GravDown();
        }
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

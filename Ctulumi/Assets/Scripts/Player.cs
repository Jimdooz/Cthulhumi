using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    #region Inspector vars
    [Header("Move")]
    public float walkSpeed;
    public float runSpeed;
    public bool instantStop;
    public bool airControl;
    public float airControlSpeed;
    [Header("Jump")]
    public float jumpSpeed;
    public float maxJumpTimer;
    public float checkGroundSize = 0.2f;
    public float groundedDelay = 0.1f;
    public Transform groundCheck;
    [Header("Controller settings")]
    public float axisMargin; // marge de detection du controller
    #endregion

    #region Movement vars
    //collisions
    bool hitWall;
    bool grounded;
    float groundedTimer;
    //jump
    bool canJump;
    bool jumping;
    float jumpTimer;

    //move
    float speed;
    Vector2 velocity;
    #endregion

    #region Components
    Animator animator;
    Rigidbody2D rb2d;
    #endregion

    #region MonoBehaviour API
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator.GetComponent<Animator>();
    }
    void Update()
    {
        CheckGrounded();
        Move();
        Jump();
        groundedTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (jumping)
            rb2d.velocity = velocity;
        else
            rb2d.velocity = new Vector2(velocity.x, rb2d.velocity.y);
    }
    #endregion
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            velocity.y = jumpSpeed / 10;
            jumping = true;
        }
        if (Input.GetButton("Jump") && jumping && jumpTimer < maxJumpTimer)
        {
            velocity.y = jumpSpeed / 10;
            jumpTimer += Time.deltaTime;
        }
        else
        {
            jumping = false;
        }
    }

    void Move()
    {
        if (IsGrounded())
        {
            if (Input.GetButton("Run"))
            {
                speed = runSpeed;
                animator.SetBool("Run", true);
            }
            else
            {
                speed = walkSpeed;
                animator.SetBool("Run", false);
            }
        }
        else
        {
            if (airControl)
                speed = airControlSpeed;
            else
                speed = 0;
        }
        float vx = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(vx) > axisMargin) //permet de ne pas se déplacer si le joystick est à l'arrêt
        {
            velocity.x = vx * walkSpeed / 10;
        }
        else
        {
            if (instantStop)
                velocity.x = 0;
            else
                velocity.x = rb2d.velocity.x;
        }
        animator.SetFloat("Moving", Mathf.Abs(rb2d.velocity.x));

    }

    void CheckGrounded()
    {
        Collider2D c = Physics2D.OverlapBox(groundCheck.position, new Vector2(1, checkGroundSize), 0, LayerMask.GetMask("Wall"));
        if (c != null)
        {
            OnGrounded();
        }
    }
    void OnGrounded()
    {
        grounded = true;
        groundedTimer = 0;
    }
    bool IsGrounded()
    {
        return grounded || groundedTimer < groundedDelay;
    }
}

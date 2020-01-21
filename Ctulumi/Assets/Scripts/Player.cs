using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    #region Inspector vars
    [Header("Move")]
    public float walkSpeed;
    public float runSpeed;
    public float airControlSpeed;
    public bool instantStop = true;
    public bool airControl;
    //public float airControlSpeed;
    [Header("Jump")]
    public float jumpSpeed;
    public float jumpTimerMax = 0.5f;
    public bool cuttingJumpOnKeyUp;
    public float cuttingJumpFactor = 0.5f;
    public float preJumpDelay = 0.1f;
    public Vector2 checkGroundSize;
    public float groundedDelay = 0.1f;
    public Transform groundCheck;
    [Header("Gravity")]
    public float gravity = 8;
    public float jumpGravity = 3;
    [Header("Controller settings")]
    public float axisMargin; // marge de detection du controller
    #endregion

    #region Movement vars
    //collisions
    bool hitWall;
    bool grounded;
    float inAirTimer;

    //flip
    bool right = true;
    //move
    bool run;
    float speed;
    Vector2 velocity;

    //jump;
    bool jumpInput;
    float preJumpTimer;
    #endregion

    #region Components
    Animator animator;
    Rigidbody2D rb2d;
    #endregion

    #region MonoBehaviour API
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        rb2d.gravityScale = gravity;
    }
    void Update()
    {
        SetJumpInput();
        ResetVelocity();
        CheckGrounded();
        Move();
        Jump();
        SetAnimator();
    }

    void FixedUpdate()
    {
        rb2d.velocity = velocity;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.position, new Vector3(checkGroundSize.x, checkGroundSize.y, 1));
    }
    #endregion
    void ResetVelocity()
    {
        velocity.x = rb2d.velocity.x;
        velocity.y = rb2d.velocity.y;
    }
    void SetJumpInput()
    {
        preJumpTimer += Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            jumpInput = true;
            preJumpTimer = 0;
        }
        if (preJumpTimer >preJumpDelay)
        {
            jumpInput = false;
        }
    }
    void Jump()
    {
        if (jumpInput && IsGrounded())
        {
            velocity.y = jumpSpeed;
            rb2d.gravityScale = jumpGravity;
        }

        if (Input.GetButtonUp("Jump") || (inAirTimer > jumpTimerMax))
        {
            if (Input.GetButtonUp("Jump") && cuttingJumpOnKeyUp)
                CutJump();
            rb2d.gravityScale = gravity;
        }
    }
    void CutJump()
    {
        if (rb2d.velocity.y > 0)
        {
            velocity.y = velocity.y * cuttingJumpFactor;
        }
    }

    void Move()
    {
        float vx = Input.GetAxisRaw("Horizontal");
        if (IsGrounded())
        {
            if (Mathf.Abs(Input.GetAxisRaw("Run")) > axisMargin)
            {
                speed = runSpeed;
                run = IsGrounded();
            }
            else
            {
                speed = walkSpeed;
                run = false;
            }
        }
        else if(airControl)
        {
            speed = airControlSpeed;
        }

        if (Mathf.Abs(vx) > axisMargin && CanMove()) //permet de ne pas se déplacer si le joystick est à l'arrêt
        {
            if ((right && vx < 0) || (!right && vx > 0))
                Flip();
            if (IsGrounded()) 
                velocity.x = vx * speed;
            else if (airControl)
            {
                float x = vx * speed;
                float rx = rb2d.velocity.x;
                if (x > 0 && rx > 0)
                    x = Mathf.Max(x, rx);
                else if (x < 0 && rx < 0)
                    x = Mathf.Min(x, rx);
                velocity.x = x;
            }
        }
        else
        {
            if (instantStop)
                velocity.x = 0;
            else
                velocity.x = rb2d.velocity.x;
        }


    }

    void CheckGrounded()
    {
        Collider2D c = Physics2D.OverlapBox(groundCheck.position, checkGroundSize, 0, LayerMask.GetMask("Wall"));
        if (c != null)
        {
            OnGrounded();
        }
        else
        {
            OnNotGrounded();
        }
    }
    void OnNotGrounded()
    {
        grounded = false;
        inAirTimer += Time.deltaTime;
    }
    void OnGrounded()
    {
        grounded = true;
        inAirTimer = 0;
    }
    bool IsGrounded()
    {
        return grounded || inAirTimer < groundedDelay;
    }
    bool CanMove()
    {
        return airControl || IsGrounded();
    }
    void Flip()
    {
        Vector3 v = transform.localScale;
        v.x *= -1;
        transform.localScale = v;
        right = !right;
    }

    void SetAnimator()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(rb2d.velocity.x));
        animator.SetFloat("VelocityY", Mathf.Abs(rb2d.velocity.y));
        animator.SetBool("Run", run);
        animator.SetBool("Grounded", grounded);
    }
    #region Player API 
    public void Die()
    {

    }
    public void Stun()
    {

    }
    #endregion
}

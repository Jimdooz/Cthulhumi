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
    [Header("Jump")]
    public float jumpSpeed;
    public float jumpTimerMax = 0.5f;
    public bool cuttingJumpOnKeyUp;
    public float cuttingJumpFactor = 0.5f;
    public float preJumpDelay = 0.1f;
    public Vector2 checkGroundSize;
    public float groundedDelay = 0.1f;
    public Transform groundCheckTransform;

    [Header("WallJump")]
    public Transform hitWallTransform;
    public float hitWallDelay = 0.1f;
    public Vector2 hitWallSize;
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
    float hitWallTimer;

    //life
    bool dead = false;
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
        dead = false;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            hitWall = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheckTransform.position, new Vector3(checkGroundSize.x, checkGroundSize.y, 1));
    }
    #endregion

    #region Player API
    public void Die()
    {
        dead = true;
    }
    public void Stun()
    {

    }
    public bool IsDead()
    {
        return dead;
    }
    #endregion
    
    void ResetVelocity()
    {
        velocity.x = rb2d.velocity.x;
        velocity.y = rb2d.velocity.y;
    }
   
    #region Jump
    void SetJumpInput()
    {
        preJumpTimer += Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            jumpInput = true;
            preJumpTimer = 0;
        }
        if (preJumpTimer > preJumpDelay)
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
            if (true;)
            {

            }
        }

        if (!Input.GetButton("Jump") || (inAirTimer > jumpTimerMax))
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
    #endregion

    #region Move
    bool CanMove()
    {
        return IsGrounded() || airControl;
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
        else
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
    #endregion

    #region HitWall
    void CheckHitWall()
    {
        Collider2D c = Physics2D.OverlapBox(hitWallTransform.position, hitWallSize, 0, LayerMask.NameToLayer("Wall"));
        if (c != null)
        {
            OnHitWall();
        }
        else
        {
            OnNoHitWall();
        }
    }
    void OnHitWall()
    {
        hitWall = true;
        hitWallTimer = 0;
    }
    void OnNoHitWall()
    {
        hitWall = false;
        hitWallTimer += Time.deltaTime;
    }
    #endregion

    #region Grounded
    void CheckGrounded()
    {
        Collider2D c = Physics2D.OverlapBox(groundCheckTransform.position, checkGroundSize, 0, LayerMask.NameToLayer("Wall"));
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
    #endregion

    #region Graphics
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
    #endregion
}

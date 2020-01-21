using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    #region Inspector vars
    [Header("Move")]
    public float walkSpeed;
    public float runSpeed;
    public bool instantStop = true;
    public bool airControl;
    public float airControlSpeed;
    [Header("Jump")]
    public float multiplyVelocityX;
    public float jumpSpeed;
    public float jumpTimerMax=0.5f;
    public bool cuttingJumpOnKeyUp;
    public float cuttingJumpFactor = 0.5f;
    public Vector2 checkGroundSize;
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
    bool jumping;

    //flip
    bool right = true;
    //move
    bool run;
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
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        CheckGrounded();
        Move();
        Jump();
        SetAnimator();
    }

    void FixedUpdate()
    {
        if (jumping)
            rb2d.velocity = velocity;
        else
            rb2d.velocity = new Vector2(velocity.x, rb2d.velocity.y);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.position, new Vector3(checkGroundSize.x, checkGroundSize.y, 1));
    }
    #endregion

    void Jump()
    {
        if (Input.GetButton("Jump") && CanJump())
        {
            velocity.y = jumpSpeed;
            jumping = true;
        }
        else
        {
            velocity.y = rb2d.velocity.y;
        }

        if (Input.GetButtonUp("Jump") && !IsGrounded())
        {
            CutJump();
            jumping = false;
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
            if (Input.GetButton("Run"))
            {
                speed = runSpeed;
                run = true;
            }
            else
            {
                speed = walkSpeed;
                run = false;
            }
        }
        else
        {
            if (airControl)
                speed = airControlSpeed;
            else
                speed = 0;
        }

        if (Mathf.Abs(vx) > axisMargin && CanMove()) //permet de ne pas se déplacer si le joystick est à l'arrêt
        {
            if ((right && vx < 0) || (!right && vx > 0))
                Flip();
            velocity.x = vx * speed;
        }
        else
        {
            if (instantStop && IsGrounded())
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
        groundedTimer += Time.deltaTime;
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
    bool CanJump()
    {
        return IsGrounded() || (cuttingJumpOnKeyUp && groundedTimer < jumpTimerMax);
    }
    bool CanMove()
    {
        return IsGrounded() || airControl;
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
        animator.SetBool("Jumping", jumping);
        animator.SetBool("Grounded", grounded);
    }
}

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
    public float hitWallDelay = 0.1f;   
    public float wallJumpSpeed = 20;
    public float wallJumpTimerMax = 0.3f;
    public Transform hitWallTransform;
    public Vector2 hitWallSize;
    public bool wallJumpFromBehind = false;
    public Transform hitWallTransformBehind;
    public Vector2 hitWallSizeBehind;
    [Header("Gravity")]
    public float gravity = 8;
    public float jumpGravity = 3;
    [Header("Controller settings")]
    public float axisMargin; // marge de detection du controller

    [Header("Accessories")]
    public GameObject Candy;
    public GameObject Cosmetic;
    public GameObject Flower;
    public GameObject Hat;
    public GameObject Node;

    [Header("Grappling")]
    public GrapplingView grapplingView;
    #endregion

    #region Private vars
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
    //wall jump
    bool wallJumpInput;
    float hitWallTimer;
    float wallJumpDir;
    bool wallJumping;
    float wallJumpTimer;

    //life
    bool dead = false;
    bool stunned = false;
    float timeStunned = 0;
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
    void FixedUpdate()
    {
        CheckStunTimer();
        ResetVelocity();
        CheckHitWall();
        CheckGrounded();
        SetJumpInput();
        Move();
        Jump();
        SetAnimator();
        rb2d.velocity = velocity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheckTransform.position, new Vector3(checkGroundSize.x, checkGroundSize.y, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(hitWallTransform.position, new Vector3(hitWallSize.x, hitWallSize.y, 1));
        Gizmos.DrawCube(hitWallTransformBehind.position, new Vector3(hitWallSizeBehind.x, hitWallSizeBehind.y, 1));
    }
    #endregion

    #region Player API
    public void Die()
    {
        dead = true;
    }
    public void Stun(float timeStunned)
    {
        stunned = true;
        this.timeStunned = Mathf.Max(this.timeStunned,timeStunned);
    }
    public bool IsDead()
    {
        return dead;
    }
    public bool IsStunned()
    {
        return stunned;
    }
    public void EquipCandy()
    {
        Candy.SetActive(true);
    }
    public void EquipCosmetic()
    {
        Cosmetic.SetActive(true);
    }
    public void EquipFlower()
    {
        Flower.SetActive(true);
    }
    public void EquipHat()
    {
        Hat.SetActive(true);
    }
    public void EquipNode()
    {
        Node.SetActive(true);
    }
    #endregion

    #region Control
    void CheckStunTimer()
    {
        if (stunned)
        {
            if (timeStunned > 0)
            {
                timeStunned -= Time.fixedDeltaTime;
            }
            else
            {
                timeStunned = 0;
                stunned = false;
            }
        }
    }
    #endregion

    #region Jump
    void SetJumpInput()
    {
        preJumpTimer += Time.fixedDeltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            jumpInput = true;
            if (!IsGrounded())
            {
                wallJumpInput = true;
            }
            else
            {
                wallJumpInput = false;
            }
            preJumpTimer = 0;
        }
        if (preJumpTimer > preJumpDelay)
        {
            jumpInput = false;
            wallJumpInput = false;
        }
    }
    void Jump()
    {
        if (jumpInput && CanJump())
        {
            velocity.y = jumpSpeed;
            rb2d.gravityScale = jumpGravity;
            if (!IsGrounded() && IsHittingWall() && wallJumpInput)
            {
                velocity.x = wallJumpSpeed * wallJumpDir;
                Debug.Log(wallJumpDir);
                wallJumpTimer = 0;
                wallJumping = true;
            }
        }

        if (!Input.GetButton("Jump") || (inAirTimer > jumpTimerMax))
        {
            if (Input.GetButtonUp("Jump") && cuttingJumpOnKeyUp)
                CutJump();
            rb2d.gravityScale = gravity;
        }
        if (wallJumping && wallJumpTimer > wallJumpTimerMax)
        {
            wallJumping = false;
            wallJumpTimer = 0;
        }
        else if(wallJumping)
        {
            wallJumpTimer += Time.fixedDeltaTime;
        }
    }
    void CutJump()
    {
        if (rb2d.velocity.y > 0)
        {
            velocity.y = velocity.y * cuttingJumpFactor;
        }
    }
    bool CanJump()
    {
        return IsGrounded() || IsHittingWall();
    }
    #endregion

    #region Climb
    void SetClimbInput()
    {

    }
    void Climb()
    {
        //if(IsHittingWall())
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
            {
                Debug.Log("1");
                velocity.x = vx * speed;
            }
            else if (airControl && !wallJumping)
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
    void ResetVelocity()
    {
        velocity.x = rb2d.velocity.x;
        velocity.y = rb2d.velocity.y;
    }
    #endregion

    #region HitWall
    void CheckHitWall()
    {
        Collider2D c = Physics2D.OverlapBox(hitWallTransform.position, hitWallSize, 0, LayerMask.GetMask("Wall"));
        Collider2D b = Physics2D.OverlapBox(hitWallTransformBehind.position, hitWallSizeBehind, 0, LayerMask.GetMask("Wall"));
        if (c != null || (wallJumpFromBehind && b!=null))
        {
            bool behind= c== null;
            OnHitWall(behind);
        }
        else
        {
            OnNoHitWall();
        }
    }
    void OnHitWall(bool behind)
    {
        hitWall = true;
        hitWallTimer = 0;
        if (behind)
        {
            if (right)
                wallJumpDir = 1;
            else
                wallJumpDir = -1;
        }
        else
        {
            if (right)
                wallJumpDir = -1;
            else
                wallJumpDir = 1;
        }
    }
    void OnNoHitWall()
    {
        hitWall = false;
        hitWallTimer += Time.fixedDeltaTime;
    }
    bool IsHittingWall()
    {
        return hitWall || hitWallTimer < hitWallDelay;
    }
    #endregion

    #region Grounded
    void CheckGrounded()
    {
        Collider2D c = Physics2D.OverlapBox(groundCheckTransform.position, checkGroundSize, 0, LayerMask.GetMask("Wall"));
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
        inAirTimer += Time.fixedDeltaTime;
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
        animator.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        animator.SetFloat("VelocityY", rb2d.velocity.y);
        animator.SetBool("Run", run);
        animator.SetBool("Grounded", IsGrounded());
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    public enum STATE { idle, run, itsend, pls }
    private STATE currentState = STATE.idle;

    public FieldOfView field;
    private Rigidbody2D humanPhysics;
    private Animator animator;

    public List<GameObject> deadBody = new List<GameObject>();

    public PlsCheck plsCheck;

    private bool dead = false;

    public float speedHuman = 2;

    public float viewRadius = 8;
    public float viewAngle = 80;

    public GameObject bloodEffect;

    private Transform target;
    // Start is called before the first frame update
    void Start() {
        field.viewRadius = viewRadius;
        field.viewAngle = viewAngle;
        humanPhysics = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(plsCheck.isTrigger && currentState != STATE.pls)
        {
            animator.SetBool("pls", true);
            currentState = STATE.pls;
        }
    }

    void FixedUpdate()
    {
        if (currentState == STATE.idle) idleFunction();
        else if (currentState == STATE.run) runFunction();
        else if (currentState == STATE.itsend) itsendFunction();
    }

    void idleFunction()
    {
        if(field.visibleTargets.Count > 0)
        {
            target = field.visibleTargets[0];
            for (int i = 0; i < field.visibleTargets.Count; i++)
            {
                Player player = field.visibleTargets[i].gameObject.GetComponent<Player>();
                if ((player && !player.IsDead())) { target = field.visibleTargets[0]; break; }
            }
            if(currentState != STATE.run)
            {
                GetComponent<AudioSource>().Play();
                currentState = STATE.run;
                animator.SetBool("Afraid", true);
            }
        }
    }

    void runFunction()
    {
        if(target.position.x > transform.position.x)
        {
            humanPhysics.velocity = new Vector2(-speedHuman, humanPhysics.velocity.y);
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            humanPhysics.velocity = new Vector2(speedHuman, humanPhysics.velocity.y);
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void itsendFunction()
    {

    }

    public void Die() {
        if (!dead)
        {
            GameObject smokePuff = Instantiate(bloodEffect, transform.position, transform.rotation) as GameObject;
            ParticleSystem parts = smokePuff.GetComponent<ParticleSystem>();
            float totalDuration = parts.main.duration + parts.main.startLifetime.constantMax;
            for(int i = 0; i < deadBody.Count; i++)
            {
                GameObject part = Instantiate(deadBody[i], transform.position, transform.rotation) as GameObject;
                Destroy(part, Random.Range(0.5f, 8f));
                part.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-30.0f, 30.0f), Random.Range(-30.0f, 30.0f)), ForceMode2D.Impulse);
            }
            Destroy(smokePuff, totalDuration);
            Destroy(this.gameObject);
        }

        dead = true;
        humanPhysics.freezeRotation = true;
        currentState = STATE.pls;
        animator.SetBool("pls", true);
    }

    public bool IsDead() {
        return dead;
    }

    public bool isEnPls()
    {
        return currentState == STATE.pls;
    }
}

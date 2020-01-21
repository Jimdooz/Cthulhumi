using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Follower : MonoBehaviour
{

    public Light2D observerLight;
    public FieldOfView field;
    public FieldOfView fieldMin;
    private Rigidbody2D followerPhysics;

    public Transform basePoint;

    public float speedFollower = 2;

    public float viewRadius = 8;
    public float viewAngle = 80;
    public float maxTimeAlert = 5; //Secondes
    public float maxTimeChangeObserve = 2; //Secondes

    private float timerAlert = 0;
    private float timerChangeObserve = 0;

    private Vector3 randomPos = new Vector3();

    public enum STATE { idle, found, alert }
    private STATE currentState = STATE.idle;

    private Transform target;

    public bool dash = false;


    // Start is called before the first frame update
    void Start()
    {
        observerLight.pointLightInnerAngle = viewAngle;
        observerLight.pointLightOuterAngle = viewAngle;
        observerLight.pointLightInnerRadius = viewRadius;
        observerLight.pointLightOuterRadius = viewRadius;
        field.viewRadius = viewRadius;
        field.viewAngle = viewAngle;

        followerPhysics = GetComponent<Rigidbody2D>();

        changeStateToIdle();
    }

    // Update is called once per frame
    void Update() {
        
    }
    
    void FixedUpdate() {
        if (currentState == STATE.idle) idleFunction();
        else if (currentState == STATE.found) foundFunction();
        else if (currentState == STATE.alert) alertFunction();
    }

    void setObserver(Vector3 targetPosition)
    {
        Vector3 upTransform = observerLight.gameObject.transform.up;
        Vector3 upTarget = targetPosition - observerLight.gameObject.transform.position;
        Vector3 yVelocity = new Vector3();
        observerLight.gameObject.transform.up = Vector3.SmoothDamp(upTransform, upTarget, ref yVelocity, 0.1f);
    }

    void idleFunction() {
        //If player found --> Found
        if(field.visibleTargets.Count > 0 || fieldMin.visibleTargets.Count > 0) {
            changeStateToFound();
        }
        if (basePoint)
        {
            transform.eulerAngles = new Vector3(0, basePoint.position.x > transform.position.x ? 0 : 180, 0);
            followerPhysics.MovePosition(Vector2.Lerp(transform.position, basePoint.position, Time.deltaTime * speedFollower));
        }


        timerChangeObserve += Time.deltaTime;
        if (timerChangeObserve >= maxTimeChangeObserve)
        {
            randomPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized + transform.position;
            timerChangeObserve = 0;
        }
        setObserver(randomPos);
    }

    void foundFunction() {
        if(field.visibleTargets.Count <= 0 && fieldMin.visibleTargets.Count <= 0) {
            changeStateToAlert();
        }

        //Look at 2D
        setObserver(target.position);

        //Go to the target
        if (dash)
        {
            followerPhysics.AddForce((target.position - transform.position).normalized * 110);
            dash = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, target.position.x > transform.position.x ? 0 : 180, 0);
            //followerPhysics.MovePosition(Vector2.Lerp(transform.position, target.position, Time.deltaTime * speedFollower));
        }
    }

    void alertFunction() {
        if (field.visibleTargets.Count > 0 || fieldMin.visibleTargets.Count > 0) {
            changeStateToFound();
        }
        timerAlert += Time.deltaTime;
        if(timerAlert >= maxTimeAlert) {
            changeStateToIdle();
        }
        timerChangeObserve += Time.deltaTime;
        if(timerChangeObserve >= maxTimeChangeObserve)
        {
            randomPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized + transform.position;
            timerChangeObserve = 0;
        }
        setObserver(randomPos);
    }

    //States changements

    void changeStateToAlert() {
        currentState = STATE.alert;
        observerLight.color = new Color(1, 0, 0, 0.5f);
        observerLight.intensity = 0.3f;
    }

    void changeStateToFound()
    {
        target = null;
        List<Transform> allTargets = new List<Transform>();
        allTargets.AddRange(fieldMin.visibleTargets);
        allTargets.AddRange(field.visibleTargets);
        for (int i = 0; i < allTargets.Count; i++)
        {
            if (allTargets[i])
            {
                Player player = allTargets[i].gameObject.GetComponent<Player>();
                Human human = allTargets[i].gameObject.GetComponent<Human>();
                if ((player && !player.IsDead()) || (human && !human.IsDead())) { target = allTargets[i]; break; }
            }
        }
        if (target)
        {
            currentState = STATE.found;
            observerLight.color = new Color(1, 0, 0, 0.5f);
            observerLight.intensity = 1;
            timerAlert = 0;
        }
    }

    void changeStateToIdle() {
        currentState = STATE.idle;
        observerLight.color = new Color(1, 1, 1, 0.5f);
        observerLight.intensity = 0.2f ;
        timerAlert = 0;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") || col.gameObject.layer == LayerMask.NameToLayer("Humans"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (player) {
                player.Die();
            }

            Human human = col.gameObject.GetComponent<Human>();
            if (human) {
                human.Die();
            }
            changeStateToAlert();
        }
    }
}

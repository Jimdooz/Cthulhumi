using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Follower : MonoBehaviour
{

    public Light2D observerLight;
    public FieldOfView field;
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
        if(field.visibleTargets.Count > 0) {
            changeStateToFound();
        }
        followerPhysics.MovePosition(Vector2.Lerp(transform.position, basePoint.position, Time.deltaTime * speedFollower));


        timerChangeObserve += Time.deltaTime;
        if (timerChangeObserve >= maxTimeChangeObserve)
        {
            randomPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized + transform.position;
            timerChangeObserve = 0;
        }
        setObserver(randomPos);
    }

    void foundFunction() {
        if(field.visibleTargets.Count <= 0) {
            changeStateToAlert();
        }

        //Look at 2D
        setObserver(target.position);

        //Go to the target
        if (dash)
        {
            Debug.Log((target.position - transform.position).normalized);
            followerPhysics.AddForce((target.position - transform.position).normalized * 110);
            dash = false;
        }
        else
        {
            //followerPhysics.MovePosition(Vector2.Lerp(transform.position, target.position, Time.deltaTime * speedFollower));
        }
    }

    void alertFunction() {
        if (field.visibleTargets.Count > 0) {
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

    void changeStateToFound() {
        currentState = STATE.found;
        observerLight.color = new Color(1, 0, 0, 0.5f);
        observerLight.intensity = 1;
        timerAlert = 0;
        target = field.visibleTargets[0];
    }

    void changeStateToIdle() {
        currentState = STATE.idle;
        observerLight.color = new Color(1, 1, 1, 0.5f);
        observerLight.intensity = 0.2f ;
        timerAlert = 0;
    }
}

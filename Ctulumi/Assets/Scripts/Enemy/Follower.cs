using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Follower : MonoBehaviour
{

    public Light2D observerLight;
    public FieldOfView field;
    private Rigidbody2D followerPhysics;

    public float speedFollower = 2;

    public float viewRadius = 8;
    public float viewAngle = 80;
    public float maxTimeAlert = 5; //Secondes

    private float timerAlert = 0;

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

    void idleFunction() {
        //If player found --> Found
        if(field.visibleTargets.Count > 0) {
            changeStateToFound();
        }
    }

    void foundFunction() {
        if(field.visibleTargets.Count <= 0) {
            changeStateToAlert();
        }

        //Look at 2D
        Vector3 upTransform = observerLight.gameObject.transform.up;
        Vector3 upTarget = target.position - observerLight.gameObject.transform.position;
        Vector3 yVelocity = new Vector3();
        observerLight.gameObject.transform.up = Vector3.SmoothDamp(upTransform, upTarget, ref yVelocity, 0.1f);

        //Go to the target
        if (dash)
        {
            followerPhysics.AddForce((target.position - transform.position).normalized * Time.deltaTime * 100, ForceMode2D.Impulse);
            dash = false;
        }
        else
        {
            followerPhysics.MovePosition(Vector2.Lerp(transform.position, target.position, Time.deltaTime * speedFollower));
        }
    }

    void alertFunction() {
        if (field.visibleTargets.Count > 0) {
            changeStateToFound();
        }
        timerAlert += Time.deltaTime;
        Debug.Log(timerAlert);
        if(timerAlert >= maxTimeAlert) {
            changeStateToIdle();
        }
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

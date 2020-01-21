using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;


public class Sentry : MonoBehaviour
{
    public Light2D observerLight;
    public FieldOfView field;

    public LineRenderer tongue;

    public float viewRadius = 8;
    public float viewAngle = 80;
    public List<float> positions; //Différentes positions

    public bool blinking;//si la sentinelle cligne ou non (incompatible avec les sentinelles mobiles)
    public int currentPosition;//position actuelle
    public int watchtime;//temps avant que la sentinelle change d'orientation
    public int blinktime;//temps avant clignage

    private bool blinked;
    private float chronometer;

    private Transform target;
    private Animator animator;

    private bool initTongue = true;
    private float animationTongueMax = 1f;
    private float animationTongue = 0;

    void setObserver(Vector3 targetPosition)
    {
        Debug.Log(targetPosition);
        Vector3 upTransform = observerLight.gameObject.transform.up;
        Vector3 upTarget = targetPosition - observerLight.gameObject.transform.position;
        Vector3 yVelocity = new Vector3();
        observerLight.gameObject.transform.up = Vector3.SmoothDamp(upTransform, upTarget, ref yVelocity, 0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        chronometer = 0;
        currentPosition = 0;
        observerLight.pointLightInnerAngle = viewAngle;
        observerLight.pointLightOuterAngle = viewAngle;
        observerLight.pointLightInnerRadius = viewRadius;
        observerLight.pointLightOuterRadius = viewRadius;
        field.viewRadius = viewRadius;
        field.viewAngle = viewAngle;
        animator = GetComponent<Animator>();
        if (blinking) field.gameObject.transform.eulerAngles = new Vector3(0,0,0);//hacky math
        tongue.SetPosition(1, tongue.GetPosition(0));
    }

    // Update is called once per frame
    void Update()
    {
        if (!blinking)//si on a un watchtime (0 permet de garder la sentinelle fixe)
        {
            chronometer += Time.deltaTime;//update chrono
            Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, positions[currentPosition]) * Vector2.right);
            setObserver(new Vector3(field.gameObject.transform.position.x + dir.x, field.gameObject.transform.position.y + dir.y, 0));
            if (chronometer > watchtime)//à la fin du chrono
            {
                if (currentPosition == positions.Count - 1)//si on dépasse la liste orientations
                    currentPosition = 0;//on revient au début
                else
                    currentPosition++;//sinon on passe à la valeur suivante
                chronometer = 0;//reset chrono
            }
        }
        else
        {
            chronometer += Time.deltaTime;
            if (chronometer > blinktime)
            {
                blinked = !blinked;
                observerLight.intensity = blinked ? 1 : 0;
                chronometer = 0;
            }
        }
        //si le joueur est en vue, le tuer
        if (!target) {
            initTongue = true;
            CheckTargets();
        }
        if (target)
        {
            animator.SetBool("eat", true);
            TongueEat();
        }
    }

    void TongueEat()
    {
        animationTongue += Time.deltaTime;
        if (initTongue) {
            animationTongue = 0;
            initTongue = false;
        }
        if(animationTongue < animationTongueMax)
        {
            Vector3 oldPos = tongue.GetPosition(1);
            Vector3 posTarget = target.position - transform.position;
            Vector3 yVelocity = new Vector3();
            tongue.SetPosition(1, Vector3.SmoothDamp(oldPos, posTarget, ref yVelocity, 0.02f));
            if (Vector3.Distance(tongue.GetPosition(1), target.position - transform.position) < 0.1f) {
                animationTongue = animationTongueMax;
            }
        }
        else
        {
            Vector3 oldPos = tongue.GetPosition(1);
            Vector3 posTarget = tongue.GetPosition(0);
            Vector3 yVelocity = new Vector3();
            tongue.SetPosition(1, Vector3.SmoothDamp(oldPos, posTarget, ref yVelocity, 0.08f));
            target.position = tongue.GetPosition(1) + transform.position;

            if(Vector3.Distance(tongue.GetPosition(1), tongue.GetPosition(0)) < 0.1f)
            {
                animator.SetBool("eat", false);
                Player player = target.gameObject.GetComponent<Player>();
                Human human = target.gameObject.GetComponent<Human>();
                if (player) player.Die();
                else if (human) Destroy(human.gameObject);
                target = null;
            }
        }
    }

    void CheckTargets()
    {
        if(field.visibleTargets.Count > 0)
        {
            for (int i = 0; i < field.visibleTargets.Count; i++)
            {
                Player player = field.visibleTargets[i].gameObject.GetComponent<Player>();
                Human human = field.visibleTargets[i].gameObject.GetComponent<Human>();
                if ((player && !player.IsDead()) || (human && !human.IsDead())) { target = field.visibleTargets[i]; break; }
            }
        }
    }
}

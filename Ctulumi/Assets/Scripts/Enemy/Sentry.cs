using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;


public class Sentry : MonoBehaviour
{
    public Light2D observerLight;
    public FieldOfView field;

    public float viewRadius = 8;
    public float viewAngle = 80;
    public List<float> positions; //Différentes positions

    public bool blinking;//si la sentinelle cligne ou non (incompatible avec les sentinelles mobiles)
    public int currentPosition;//position actuelle
    public int watchtime;//temps avant que la sentinelle change d'orientation
    public int blinktime;//temps avant clignage

    private bool blinked;
    private float chronometer;

    void setObserver(Vector3 targetPosition)
    {
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
        if(blinking) field.gameObject.transform.eulerAngles = new Vector3(0,0,0);//hacky math
    }

    // Update is called once per frame
    void Update()
    {
        if (!blinking)//si on a un watchtime (0 permet de garder la sentinelle fixe)
        {
            chronometer += Time.deltaTime;//update chrono
            setObserver(new Vector3(transform.position.x + Mathf.Sin(positions[currentPosition]), transform.position.y + Mathf.Cos(positions[currentPosition]), 0));
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
    }
}

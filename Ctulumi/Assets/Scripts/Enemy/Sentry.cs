using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;


public class Sentry : MonoBehaviour
{
    public Light2D observerLight;
    public FieldOfView field;

    public float viewRadius;
    public float viewAngle;

    public bool blinking;//si la sentinelle cligne ou non (incompatible avec les sentinelles mobiles)
    public bool[] orientations; //liste des différentes orientations de la sentinelle
    public int currentPosition;//position actuelle
    public int watchtime;//temps avant que la sentinelle change d'orientation
    public int blinktime;//temps avant clignage

    private bool blinked;
    private float chronometer;

    // Start is called before the first frame update
    void Start()
    {
        chronometer = 0;
        currentPosition = 0;
        viewRadius = 8;
        viewAngle = 80;
        blinking = false;
        observerLight.pointLightInnerAngle = viewAngle;
        observerLight.pointLightOuterAngle = viewAngle;
        observerLight.pointLightInnerRadius = viewRadius;
        observerLight.pointLightOuterRadius = viewRadius;
        field.viewRadius = viewRadius;
        field.viewAngle = viewAngle;
        field.gameObject.transform.eulerAngles = new Vector3(0,0,((currentPosition - 1) * 45));//hacky math
    }

    // Update is called once per frame
    void Update()
    {
        if (watchtime > 0)//si on a un watchtime (0 permet de garder la sentinelle fixe)
        {
            chronometer += Time.deltaTime;//update chrono
            if (chronometer > watchtime)//à la fin du chrono
            {
                if (currentPosition == 2)//si on dépasse la liste orientations
                    currentPosition = 0;//on revient au début
                else
                    currentPosition++;//sinon on passe à la valeur suivante
                if (orientations[currentPosition])
                {
                    field.gameObject.transform.eulerAngles = new Vector3(0, 0, ((currentPosition - 1) * 45));
                    chronometer = 0;//reset chrono
                }
            }
        }
        else if(blinking)
        {
            chronometer += Time.deltaTime;
            if (chronometer > blinktime)
            {
                blinked = !blinked;
                chronometer = 0;
            }
        }
        //si le joueur est en vue, le tuer
    }
}

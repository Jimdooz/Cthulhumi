using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    private bool blinking;//si la sentinelle cligne ou non (incompatible avec les sentinelles mobiles)
    private bool blinked;
    private bool[] orientations; //liste des différentes orientations de la sentinelle
    private int watchtime;//temps avant que la sentinelle change d'orientation
    private float chronometer;
    private int index;//savoir quelle orientation choisir dans la liste
    private int blinktime;//temps avant clignage

    public Sentry(bool blink, bool[] oris, int startingOrientation, int watime=2)
    {
        blinking = blink;
        orientations = oris;
        watchtime = watime;
        index = startingOrientation;
    }
    public Sentry(bool blink, int soleOrientation)//si on veut faire une sentinelle fixe
    {
        blinking = blink;
        orientations = new bool[3];
        orientations[soleOrientation] = true;
        watchtime = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        chronometer = 0;
        index = 0;
        blinktime = 2;
        blinking = false;
        //orienter la sentinelle dans son orientation par défaut (orientations[])
    }

    // Update is called once per frame
    void Update()
    {
        if (watchtime > 0)//si on a un watchtime (0 permet de garder la sentinelle fixe)
        {
            chronometer += Time.deltaTime;//update chrono
            if (chronometer > watchtime)//à la fin du chrono
            {
                chronometer = 0;//reset chrono
                if (index == 2)//si on dépasse la liste orientations
                    index = 0;//on revient au début
                else
                    index++;//sinon on passe à la valeur suivante
                //changer l'orientation en fonction de l'index
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

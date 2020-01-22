using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Activable
{
    public bool on = false;

    public override void activate()
    {
        on = !on;
        GetComponent<Animator>().SetBool("on", on);
    }

    public override bool IsActive()
    {
        return on;
    }
}

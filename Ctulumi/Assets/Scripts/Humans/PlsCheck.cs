using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlsCheck : MonoBehaviour
{
    public bool isTrigger = false;

    void Start()
    {
        isTrigger = false;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Wall")) isTrigger = true;
    }
}

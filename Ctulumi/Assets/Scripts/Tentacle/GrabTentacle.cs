using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTentacle : MonoBehaviour
{
    public GameObject grab;
    public GameObject activable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Items"))
        {
            grab = col.gameObject;
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Activable"))
        {
            activable = col.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject == grab)
        {
            grab = null;
        }
        if (col.gameObject == activable)
        {
            activable = null;
        }
    }
}

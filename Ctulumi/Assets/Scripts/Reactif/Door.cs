using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Activable active;
    private Animator animatorDoor;
    // Start is called before the first frame update
    void Start()
    {
        animatorDoor = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animatorDoor.SetBool("open", active.IsActive());
    }
}

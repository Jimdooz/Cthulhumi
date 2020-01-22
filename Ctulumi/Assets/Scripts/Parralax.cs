using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parralax : MonoBehaviour
{
    private float startpos;
    public GameObject cam;
    public float ParralaxEffect;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = (cam.transform.position.x * ParralaxEffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        foreach (Transform child in transform) {
            Vector3 size = child.GetComponent<Renderer>().bounds.size;
            if (child.position.x > cam.transform.position.x + size.x)
            {
                child.position = new Vector3(child.position.x - size.x * 2, child.position.y, child.position.z);
            }
            else if (child.position.x < cam.transform.position.x - size.x)
            {
                child.position = new Vector3(child.position.x + size.x * 2, child.position.y, child.position.z);
            }
        }
    }
}

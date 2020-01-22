using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingView : MonoBehaviour
{

    public float maxDistance = 21f;
    private FieldOfView field;

    public Vector2 couocu;
    // Start is called before the first frame update
    void Start()
    {
        field = GetComponent<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Project(couocu);
    }

    Vector2 Project(Vector2 direction)
    {
        float angle = getAngle(direction, new Vector2(0, 0));
        Debug.Log(angle);
        transform.eulerAngles = new Vector3(0,0,angle);
        field.FindVisibleTargets();

        if (field.visibleTargets.Count <= 0) return new Vector2(0, 0);
        return neerElement();
    }
    public float getAngle(Vector2 me, Vector2 target)
    {
        return Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI);
    }

    Vector2 neerElement()
    {
        Transform minDistance = field.visibleTargets[0];
        float distanceActual = Vector2.Distance(transform.localPosition, field.visibleTargets[0].position);
        for (int i = 1; i < field.visibleTargets.Count; i++) {
            float distance = Vector2.Distance(transform.localPosition, field.visibleTargets[0].position);
            if (distanceActual > distance)
            {
                minDistance = field.visibleTargets[i];
                distanceActual = distance;
            }
        }
        return new Vector2(minDistance.position.x, minDistance.position.y);
    }
}

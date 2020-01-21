using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TantacleHead : MonoBehaviour
{
    // Start is called before the first frame update

    public float speedTantacle = 2;
    public float speedRewind = 5;

    public GameObject HeadTantacle;
    public LineRenderer Tantacle;
    private Rigidbody2D HeadPhysics;

    private List<Vector2> allPoints = new List<Vector2>();

    void Start() {
        HeadPhysics = HeadTantacle.GetComponent<Rigidbody2D>();
        Tantacle.positionCount = 2;
        Tantacle.SetPosition(0, new Vector3(0, 0, 0));
        allPoints.Add(new Vector2(0, 0));
    }

    // Update is called once per frame
    void Update() {
    }

    void FixedUpdate() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //Max distance
        HeadPhysics.velocity = new Vector2(horizontal, vertical) * speedTantacle;

        //Calcul distance dernier point
        Vector2 lastPoint = allPoints[allPoints.Count - 1];
        Vector2 headPosition = new Vector2(HeadTantacle.transform.position.x, HeadTantacle.transform.position.y);
        float distanceLastPoint = Vector2.Distance(lastPoint, headPosition);

        float isRetracting = Input.GetAxisRaw("Rewind");

        Debug.Log(isRetracting);


        if (allPoints.Count > 100 || isRetracting > 0) {
            HeadPhysics.MovePosition(Vector2.Lerp(headPosition, lastPoint, Time.deltaTime * (isRetracting > 0 ? isRetracting * speedRewind : 1f)));
        }
        if (distanceLastPoint > 0.15) {
            allPoints.Add((lastPoint + headPosition) /2);

            Tantacle.positionCount = allPoints.Count + 1;
        }
        else if(distanceLastPoint < 0.5 && allPoints.Count > 1)
        {
            allPoints.RemoveAt(allPoints.Count - 1);
            Tantacle.positionCount = allPoints.Count + 1;
        }
        Vector3 headPosition3D = new Vector3(HeadTantacle.transform.position.x, HeadTantacle.transform.position.y, 0f);
        Tantacle.SetPosition(allPoints.Count, headPosition3D);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleHead : MonoBehaviour
{
    // Start is called before the first frame update

    public float speedTentacle = 2;
    public float speedRewind = 5;

    public GameObject HeadTentacle;
    public LineRenderer Tentacle;
    private Rigidbody2D HeadPhysics;

    public GrabTentacle grabTentacle;

    private List<Vector2> allPoints = new List<Vector2>();

    bool resetActive = true;

    bool Begin = false;
    bool rewinded = false;
    void Start() {
        HeadPhysics = HeadTentacle.GetComponent<Rigidbody2D>();
        Tentacle.positionCount = 2;
        Tentacle.SetPosition(0, new Vector2(0,0));
        allPoints.Add(new Vector2(HeadTentacle.transform.localPosition.x, HeadTentacle.transform.localPosition.y));
    }

    // Update is called once per frame
    void Update() {
        if(grabTentacle.grab && Input.GetAxisRaw("Jump") > 0)
        {
            grabTentacle.grab.transform.position = HeadTentacle.transform.position;
        }
        if(grabTentacle.activable && Input.GetAxisRaw("Jump") > 0 && resetActive)
        {
            Activable activeObject = grabTentacle.activable.GetComponent<Activable>();
            Debug.Log(activeObject);
            if(activeObject) activeObject.activate();
            resetActive = false;
        }

        if(Input.GetAxisRaw("Jump") == 0)
        {
            resetActive = true;
        }
    }

    void FixedUpdate() {
        if (allPoints.Count > 3) Begin = true;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //Max distance
        HeadPhysics.velocity = new Vector2(horizontal, vertical) * speedTentacle;

        //Calcul distance dernier point
        Vector2 lastPoint = allPoints[allPoints.Count - 1];
        Vector2 headPosition = new Vector2(HeadTentacle.transform.localPosition.x, HeadTentacle.transform.localPosition.y);
        float distanceLastPoint = Vector2.Distance(lastPoint, headPosition);

        float isRetracting = Input.GetAxisRaw("Rewind");
        if (Begin && isRetracting > 0.001f)
        {
            rewinded = true;
        }

        if (allPoints.Count > 100 || isRetracting > 0) {
            HeadPhysics.MovePosition(Vector2.Lerp(headPosition + getPosition2D(), lastPoint + getPosition2D(), Time.deltaTime * (isRetracting > 0 ? isRetracting * speedRewind : 1f)));
        }
        if (distanceLastPoint > 0.15) {
            allPoints.Add((lastPoint + headPosition) /2);

            Tentacle.positionCount = allPoints.Count + 1;
        }
        else if(distanceLastPoint < 0.5 && allPoints.Count > 1)
        {
            allPoints.RemoveAt(allPoints.Count - 1);
            Tentacle.positionCount = allPoints.Count + 1;
        }
        Vector3 headPosition3D = new Vector3(HeadTentacle.transform.localPosition.x, HeadTentacle.transform.localPosition.y, 0f);
        Tentacle.SetPosition(allPoints.Count, headPosition3D);
    }

    Vector2 getPosition2D()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public bool Retracted()
    {
        if(allPoints.Count <= 2 && Begin && rewinded)
        {
            Begin = false;
            return true;
        }
        return false;
    }


}

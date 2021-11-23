using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMaster : MonoBehaviour
{
    public float speedLimit;
    public float separationValue = 100;
    public int averageVelocityModifier = 8;
    public int cohesionStrength = 100;
    public float borderNudgeValue;

    private Camera cam;
    [SerializeField] private GameObject boidObject;
    [SerializeField] private int boidAmount;
    private bool trackMouse;
    private List<Boid> boids = new List<Boid>();

    void Start()
    {
        cam = Camera.main;
        InitializeBoids();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            trackMouse = !trackMouse;
        }
        MoveBoidsToNewPosition();
    }

    private void InitializeBoids()
    {
        for(int i = 0; i < boidAmount; i++)
        {
            Vector3 startPosition = new Vector3(Random.Range(-10.5f, 10.5f), Random.Range(-5, 5));
            Boid b = Instantiate(boidObject, startPosition, Quaternion.identity).GetComponent<Boid>();
            boids.Add(b);
        }
    }
    private void MoveBoidsToNewPosition()
    {
        Vector3 v1, v2, v3, v4, v5;
        foreach(Boid b in boids)
        {
            v1 = CohesionRule(b);
            v2 = SeparationRule(b);
            v3 = AverageRule(b);
            v4 = AvoidBorders(b);
            v5 = TrackMouse(b);

            b.velocity += v1 + v2 + v3 + v4 + v5;
            LimitVelocity(b);
            b.position += b.velocity * Time.deltaTime;
        }
    }

    private Vector3 CohesionRule(Boid boid)
    {
        Vector3 perceivedCenter = Vector3.zero;
        foreach(Boid b in boids)
        {
            if(b != boid)
            {
                perceivedCenter = perceivedCenter + b.position;
            }
        }
        perceivedCenter = perceivedCenter / (boids.Count - 1);
        return (perceivedCenter - boid.position) / cohesionStrength;
    }
    private Vector3 SeparationRule(Boid boid)
    {
        Vector3 separatePosition = Vector3.zero;
        foreach(Boid b in boids)
        {
            if(b != boid)
            {
                if(Vector3.Magnitude(b.position - boid.position) < separationValue)
                {
                    separatePosition = separatePosition - (b.position - boid.position) * 1.5f;
                }
            }
        }
        return separatePosition;
    }
    private Vector3 AverageRule(Boid boid)
    {
        Vector3 perceivedVelocity = Vector3.zero;
        foreach(Boid b in boids)
        {
            if(b != boid)
            {
                perceivedVelocity = perceivedVelocity + b.velocity;
            }
        }
        perceivedVelocity = perceivedVelocity / (boids.Count - 1);
        return ( perceivedVelocity - boid.velocity ) / averageVelocityModifier;
    }
    private Vector3 AvoidBorders(Boid boid)
    {
        float xMin, xMax, yMin, yMax;
        xMax = 10.5f;
        yMax = 5f;
        xMin = -xMax;
        yMin = -yMax;
        Vector3 nudge = Vector3.zero;

        if(boid.position.x < xMin)
        {
            nudge.x = borderNudgeValue;
        }else if(boid.position.x > xMax)
        {
            nudge.x = -borderNudgeValue;
        }
        if (boid.position.y < yMin)
        {
            nudge.y = borderNudgeValue;
        }
        else if (boid.position.y > yMax)
        {
            nudge.y = -borderNudgeValue;
        }
        return nudge;
    }
    private void LimitVelocity(Boid boid)
    {
        if(boid.velocity.magnitude > speedLimit)
        {
            boid.velocity = (boid.velocity / boid.velocity.magnitude) * speedLimit;
        }
    }
    private Vector3 TrackMouse(Boid boid)
    {
        if (trackMouse)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            //Debug.Log(mousePos);
            return (mousePos - boid.position) / 10;
        }
        return Vector3.zero;
    }
}

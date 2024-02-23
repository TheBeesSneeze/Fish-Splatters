using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [Tooltip("if not null, boids will tend to follow target")]
    public GameObject Target;

    [Tooltip("Boids will be within a certain distance of center of boids")]
    public Transform CenterOfBoids;
    [Tooltip("How far you can get from center")]
    public float MaxDistanceFromCenter;

    public float MaxVelocity=3;

    public float NoCollisionDistance = 1;
    [Tooltip("Starting speed is random")]
    public float MaxStartingSpeed = 5;

    public Vector3 Velocity;

    //private Rigidbody rb;
    private Boid[] boids;

    private Vector3 center;

    // Start is called before the first frame update
    void Start()
    {
        RandomizeInitialVelocity();

        //rb = GetComponent<Rigidbody>();
        boids = GameObject.FindObjectsOfType<Boid>();
    }

    // Update is called once per frame
    void Update()
    {
        center = CalculateCenter();

        Vector3 v1 = Rule1();
        Vector3 v2 = Rule2();
        Vector3 v3 = Rule3();
        Vector3 v4 = Rule4();

        if (gameObject.name == "Boid" && false)
        {
            //Debug.Log(v1);
            //Debug.Log(v2);
            //Debug.Log(v3);
        }

        Velocity = Velocity + v1 + v2 + v3 + v4;

        //rb.velocity = Velocity;
        if (!Velocity.IsNaN())
            transform.position = (Velocity * Time.deltaTime) + transform.position;

        BoundPosition();
        LimitVelocity();
        //rb.position = rb.position + rb.velocity;
    }

    private void RandomizeInitialVelocity()
    {
        float r1 = (Random.value - MaxStartingSpeed) * (MaxStartingSpeed * 2);
        float r2 = (Random.value - MaxStartingSpeed) * (MaxStartingSpeed * 2);
        float r3 = (Random.value - MaxStartingSpeed) * (MaxStartingSpeed * 2);

        Velocity = new Vector3(r1, r2, r3);
    }

    /// <summary>
    /// mass wants to be center of flock
    /// </summary>
    /// <returns></returns>
    private Vector3 Rule1()
    {
        Vector3 center = CalculateCenter();

        foreach (Boid boid in boids)
        {
            if (boid != this)
            {
                center = center + transform.position;
            }
        }

        center = center / (boids.Length - 1);


        return (center - transform.position) / 1000;
    }

    /// <summary>
    /// avoid collisions
    /// </summary>
    /// <returns></returns>
    private Vector3 Rule2()
    {
        Vector3 result = Vector3.zero;

        foreach (Boid boid in boids)
        {
            if (boid != this)
            {
                if (Vector3.Distance(boid.transform.position, this.transform.position) < NoCollisionDistance)
                    result = result - (boid.transform.position - this.transform.position);
            }
        }

        return result;
    }

    /// <summary>
    /// average all velocities
    /// </summary>
    private Vector3 Rule3()
    {
        Vector3 result = CalculateAverageVelocity();

        foreach (Boid boid in boids)
        {
            if (boid != this)
            {
                result = result + boid.Velocity;
            }
        }

        result = result / (boids.Length - 1);

        return (result - Velocity) / 8f;
    }

    //follow a target
    public Vector3 Rule4()
    {
        if(Target == null)
        {
            return Vector3.zero;
        }

        return (Target.transform.position - transform.position) / 100;
    }

    public void BoundPosition()
    {
        if (center == null)
            return;

        float distance = Vector3.Distance(transform.position, CenterOfBoids.position);

        if(distance > MaxDistanceFromCenter) { }
        {
            Vector3 relativePositon = transform.position - CenterOfBoids.position;

            transform.position = relativePositon.normalized * MaxDistanceFromCenter;
        }
    }

    public void LimitVelocity()
    {
        float m = Velocity.magnitude;

        if(m> MaxVelocity)
        {
            Velocity = Velocity.normalized * MaxVelocity;
        }
    }

    /// <summary>
    /// SO INEFFICIENT THIS IS CALLED ON EVERY BOID
    /// </summary>
    public Vector3 CalculateCenter()
    {
        Vector3 sum = Vector3.zero;

        foreach (Boid boid in boids)
        {
            sum += boid.transform.position;
        }

        return sum / (boids.Length);
    }

    public Vector3 CalculateAverageVelocity()
    {
        Vector3 sum = Vector3.zero;

        foreach (Boid boid in boids)
        {
            sum += boid.Velocity;
        }

        return sum / (boids.Length);
    }


}

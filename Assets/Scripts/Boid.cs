using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 Velocity;

    public float NoCollisionDistance=1;
    [Tooltip("Starting speed is random")]
    public float MaxStartingSpeed=5;

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

        if (gameObject.name == "Boid" && false)
        {
            Debug.Log(v1);
            Debug.Log(v2);
            Debug.Log(v3);
        }

        Velocity = Velocity + v1 + v2 + v3;

        //rb.velocity = Velocity;
        if(!Velocity.IsNaN())
            transform.position = (Velocity*Time.deltaTime) + transform.position;

        //rb.position = rb.position + rb.velocity;
    }

    private void RandomizeInitialVelocity()
    {
        float r1 = (Random.value - MaxStartingSpeed) * (MaxStartingSpeed*2);
        float r2 = (Random.value - MaxStartingSpeed) * (MaxStartingSpeed*2);
        float r3 = (Random.value - MaxStartingSpeed) * (MaxStartingSpeed*2);

        Velocity = new Vector3(r1, r2, r3);
    }

    /// <summary>
    /// mass wants to be center of flock
    /// </summary>
    /// <returns></returns>
    private Vector3 Rule1()
    {
        Vector3 result = center;

        foreach (Boid boid in boids)
        {
            if(boid != this)
            {
                result = result + transform.position;
            }
        }

        result = result / (boids.Length - 1);


        return (result - transform.position) / 100;
    }

    /// <summary>
    /// avoid collisions
    /// </summary>
    /// <returns></returns>
    private Vector3 Rule2()
    {
        Vector3 result = Vector3.zero;

        foreach(Boid boid in boids)
        {
            if(boid != this)
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
        Vector3 result = center;

        foreach (Boid boid in boids)
        {
            if(boid != this)
            {
                result = result + boid.Velocity;
            }
        }

        result = result / (boids.Length - 1);

        return (result - Velocity) / 8f;
        /*
        Vector pvJ

		FOR EACH BOID b
			IF b != bJ THEN
				pvJ = pvJ + b.velocity
			END IF
		END

		pvJ = pvJ / N-1

		RETURN (pvJ - bJ.velocity) / 8
         */
    }

    /// <summary>
    /// SO INEFFICIENT THIS IS CALLED ON EVERY BOID
    /// </summary>
    public Vector3 CalculateCenter()
    {
        Vector3 sum = Vector3.zero;

        foreach(Boid boid in boids)
        {
            sum += boid.transform.position;
        }

        return sum / (boids.Length -1);
    }
}

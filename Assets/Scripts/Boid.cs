using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 Velocity;

    private Rigidbody rb;
    private Boid[] boids;
   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boids = GameObject.Find
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v1 = Rule1() * Time.deltaTime;

        Vector3 v2 = Rule2() * Time.deltaTime;

        Vector3 v3 = Rule3() * Time.deltaTime;

        rb.velocity = rb.velocity + v1 + v2 + v3;

        rb.position = rb.position + rb.velocity;
    }

    private Vector3 Rule1()
    {
        Vector3 result = Vector3.zero;

        foreach(Boid boid in boids)
        {
            if(boid != this)
            {
                result = result + transform.position;
            }
        }

        result = result / (boids.Length - 1);

        return result - transform.position;
    }

    private Vector3 Rule2()
    {
        Vector3 result = Vector3.zero;

        foreach(Boid boid in boids)
        {
            if(boid != this)
            {
                if (Vector3.Distance(boid.transform.position, this.transform.position) < 10)
                    result = result - (boid.transform.position - this.transform.position);
            }
        }

        return result;

        /*
        Vector c = 0;

		FOR EACH BOID b
			IF b != bJ THEN
				IF |b.position - bJ.position| < 100 THEN
					c = c - (b.position - bJ.position)
				END IF
			END IF
		END

		RETURN c
         */
    }

    private Vector3 Rule3()
    {
        Vector3 result = Vector3.zero;

        foreach(Boid boid in boids)
        {
            if(boid != this)
            {
                result = result + boid.Velocity;
            }
        }

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
}

/*******************************************************************************
 * File Name :         Boid.cs
 * Author(s) :         Toby
 * Creation Date :     idk
 *
 * Brief Description : boid
 *****************************************************************************/


using Cinemachine.Utility;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [HideInInspector] public Transform Target;

    //[Header("Weights")]
    [Foldout("Debug")][ReadOnly] public Boid[] boids;
    [Foldout("Debug")][ReadOnly] public float Speed;
    [Foldout("Debug")][ReadOnly] public float MaxVelocity = 3;
    [Foldout("Debug")][ReadOnly] public float NoCollisionDistance = 1;
    [Foldout("Debug")][ReadOnly] public float FlockWeight        ;
    [Foldout("Debug")][ReadOnly] public float SeperationWeight   ;
    [Foldout("Debug")][ReadOnly] public float AvgVelocityWeight  ;
    [Foldout("Debug")][ReadOnly] public float FollowTargetWeight ;
    [Foldout("Debug")][ReadOnly] public Vector3 Velocity;
    [Foldout("Debug")][ReadOnly] public int SkipXFrames;
    [Foldout("Debug")][ReadOnly] public int SkipFrameOffset;

    [HideInInspector] public BoidTargetRotation Center;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        RandomizeInitialVelocity();

        rb = GetComponent<Rigidbody>();

        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        //if ((Time.frameCount + SkipFrameOffset) % SkipXFrames != 0) return;

        Vector3 v1 = Flock() * FlockWeight;
        Vector3 v2 = Seperation() * SeperationWeight;
        Vector3 v3 = AvgVelocity() * AvgVelocityWeight;
        Vector3 v4 = FollowTarget() * FollowTargetWeight;

        if (false && gameObject.name == "Boid")
        {
            Debug.Log("v1:" + v1);
            Debug.Log("v2:" + v2);
            Debug.Log("v3:" + v3);
            Debug.Log("v4:" + v4);
        }

        Velocity = v1 + v2 + v3 + v4 + Velocity;
        float multiplier =  Speed * (float)SkipXFrames;

        //rb.velocity = Velocity;
        if (!Velocity.IsNaN())
        {
            //transform.position = (Velocity * multiplier) + transform.position;
            rb.velocity = Velocity * multiplier;
        }

        //BoundPosition();
        LimitVelocity();
        //rb.position = rb.position + rb.velocity;

        Rotate();
    }

    private void RandomizeInitialVelocity()
    {
        float r1 = (Random.value * Speed * 2) - Speed;
        float r2 = (Random.value * Speed * 2) - Speed;
        float r3 = (Random.value * Speed * 2) - Speed;

        Velocity = new Vector3(r1, r2, r3);
    }

    /// <summary>
    /// mass wants to be center of flock
    /// </summary>
    /// <returns></returns>
    private Vector3 Flock()
    {

        return (Center.BoidCenter - transform.position).normalized;
    }

    /// <summary>
    /// avoid collisions
    /// </summary>
    /// <returns></returns>
    private Vector3 Seperation()
    {
        Vector3 result = Vector3.zero;

        foreach (Boid boid in boids)
        {
            if (boid != this)
            {
                if (Vector3.Distance(boid.transform.position, this.transform.position) <= NoCollisionDistance)
                {
                    result += (this.transform.position - boid.transform.position);
                }
            }
        }

        return result.normalized;
    }

    /// <summary>
    /// average all velocities
    /// </summary>
    private Vector3 AvgVelocity()
    {
        Vector3 result = Center.AverageVelocity;

        foreach (Boid boid in boids)
        {
            if (boid != this)
            {
                result = result + boid.Velocity;
            }
        }

        result = result / (boids.Length - 1);

        return (result - Velocity).normalized;
    }

    //follow a target
    public Vector3 FollowTarget()
    {
        if(Target == null)
        {
            Debug.LogWarning("no target");
            return Vector3.zero;
        }

        return (Target.position - transform.position).normalized;
    }

    /*
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
    */

    public void LimitVelocity()
    {
        float m = Velocity.magnitude;

        if(m> MaxVelocity)
        {
            Velocity = Velocity.normalized * MaxVelocity;
        }
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position + Velocity,Vector3.up);
    }

    //didnt cook with this one.
    /*
    [Button]
    private void BalanceWeightsForAllBoids()
    {
        foreach(Boid boid in boids)
        {
            boid.BalanceWeight();
        }
    }

    public void BalanceWeight()
    {
        float[] weights = { FlockWeight, SeperationWeight, AvgVelocityWeight, FollowTargetWeight };
        float max = Mathf.Max(weights);

        FlockWeight /= max;
        SeperationWeight /= max;
        AvgVelocityWeight /= max;
        FollowTargetWeight /= max;

        Speed *= max;
    }
    */
}

/*******************************************************************************
 * File Name :         BoidTargetRotation.cs
 * Author(s) :         Toby
 * Creation Date :     idk
 *
 * Brief Description : boid controller.
 *****************************************************************************/


using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BoidTargetRotation : MonoBehaviour
{
    [Header("Boid Settings")]
    [SerializeField] private Boid[] BoidGroup;
    [SerializeField] private Transform BoidTarget;
    [SerializeField] private float BoidSpeed;
    [SerializeField] private float MaxVelocity;
    [SerializeField] private float NoCollisionDistance=1;
    [SerializeField] private int BoidFrameSkip = 1;

    [Header("Weights")]
    [SerializeField] private float RandomWeightVariation = 0.1f;
    [SerializeField] private float FlockWeight = 1;
    [SerializeField] private float SeperationWeight = 1;
    [SerializeField] private float AvgVelocityWeight = 1;
    [SerializeField] private float FollowTargetWeight = 1;

    [Header("General")]
    [Tooltip("idk might increase framerate if this guy runs less")]
    [SerializeField] private int SkipXFrames = 1;

    [Header("Target settings")]
    [SerializeField] private float MinDistance = 25;
    [SerializeField] private float MaxDistance = 50;

    [Header("Speeds")]
    [SerializeField] private float DistanceSpeed;
    [SerializeField] private float xSpeed;
    [SerializeField] private float ySpeed;
    [SerializeField] private float zSpeed;

    [Header("Debug")]
    [ReadOnly] public Vector3 BoidCenter;
    [ReadOnly] public Vector3 AverageVelocity;

    //stuff
    private Transform child;

    private Vector3 rotation;

    [ReadOnly] private float targetDistance;
    private float startDistance;
    private float distancePercent;

    private void Awake()
    {
        InitializeAllBoids();
    }
    private void Start()
    {
        child = transform.GetChild(0);
        RandomizeLocation();
    }

    private void Update()
    {
        BoidCenter = CalculateCenter();
        AverageVelocity = CalculateAverageVelocity();

        if (Time.frameCount % SkipXFrames != 0) return;

        Rotate();
        ExtendDistance();
    }

    private void Rotate()
    {
        rotation.x = (rotation.x + (Time.deltaTime * xSpeed * SkipXFrames)) % 360;
        rotation.y = (rotation.y + (Time.deltaTime * ySpeed * SkipXFrames)) % 360;
        rotation.z = (rotation.z + (Time.deltaTime * zSpeed * SkipXFrames)) % 360;
        transform.eulerAngles = rotation;
    }

    /// <summary>
    /// distance stuff
    /// </summary>
    private void ExtendDistance()
    {
        distancePercent += Time.deltaTime * DistanceSpeed * (float)SkipXFrames;
        float x = Mathf.Lerp(startDistance, targetDistance, distancePercent);

        child.localPosition = new Vector3(x,0,0);

        if(distancePercent > 1)
        {
            RandomizeLocation();
            distancePercent = 0;
        }
    }

    private void RandomizeLocation()
    {
        startDistance = child.localPosition.x;
        targetDistance = Random.Range(MinDistance, MaxDistance);
    }

    [Button]
    private void InitializeAllBoids()
    {
        if (BoidFrameSkip < 1)
            BoidFrameSkip = 1;

        foreach (Boid boid in BoidGroup)
        {
            boid.Center = this;
            boid.Target = BoidTarget;
            boid.SkipXFrames = BoidFrameSkip;
            boid.Speed = BoidSpeed;
            boid.MaxVelocity = MaxVelocity;
            boid.boids = BoidGroup;

            boid.SkipFrameOffset = Mathf.RoundToInt(Random.Range(0, (float)BoidFrameSkip));

            boid.FlockWeight = FlockWeight + RandomWeightOffset();
            boid.SeperationWeight = SeperationWeight + RandomWeightOffset();
            boid.AvgVelocityWeight = AvgVelocityWeight + RandomWeightOffset();
            boid.FollowTargetWeight = FollowTargetWeight + RandomWeightOffset();
        }
    }

    private float RandomWeightOffset()
    {
        return Random.Range(-RandomWeightVariation, RandomWeightVariation);
    }

    private Vector3 CalculateCenter()
    {
        Vector3 sum = Vector3.zero;

        foreach (Boid boid in BoidGroup)
        {
            sum += boid.transform.position;
        }

        return sum / (BoidGroup.Length);
    }

    private Vector3 CalculateAverageVelocity()
    {
        Vector3 sum = Vector3.zero;

        foreach (Boid boid in BoidGroup)
        {
            sum += boid.Velocity;
        }

        return sum / (BoidGroup.Length);
    }
}

/*******************************************************************************
 * File Name :   WaterVolume.cs
 * Author(s) :         Alec
 * Creation Date :     2/19/24
 *
 * Brief Description : Trigger for water representations.
 *
 * TODO:
 * -
 *****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class WaterVolumeData
{
    [Tooltip("How much drag to apply to the object whilst submerged.")]
    public float DragFactor = 0.25f;
    [Tooltip("How much buoyant force to apply.")]
    public float BuoyancyForce = 8f;
    public float CirculationSpeed;
    [Tooltip("The direction to apply the buoyancy in.")]
    public Vector3 BuoyancyDirection = Vector3.up;
    public float GravityAmount = 9.81f;
    [Tooltip("How far above the top of the volume the object will sit.")]
    public float SurfaceLevelOffset = 0.05f;
    [Tooltip("How much force will be applied proportionally to the depth of the object.")]
    [Range(0f, 1f)] public float DepthModifier = 1f;
}

[RequireComponent(typeof(Collider))]
public class WaterVolume : MonoBehaviour
{
    [field: SerializeField] public WaterVolumeData WaterData { get; private set; }
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void OnDrawGizmos()
    {
        Collider c = GetComponent<Collider>();
        Gizmos.DrawWireCube(
            new Vector3(transform.position.x, c.bounds.max.y + WaterData.SurfaceLevelOffset, transform.position.z),
            new Vector3(c.bounds.size.x, 0.05f, c.bounds.size.z));
    }

    public float GetSurfaceLevel()
    {
        return col.bounds.max.y + WaterData.SurfaceLevelOffset;
    }

    public bool CheckWithinBounds2D(Vector3 v)
    {
        var bounds = col.bounds;
        return v.x < bounds.max.x
               && v.z < bounds.max.z
               && v.x > bounds.min.x
               && v.z > bounds.min.z;
    }
}
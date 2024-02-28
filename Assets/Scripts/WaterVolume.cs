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
    public float BuoyancyDamper = 10f;
    [Tooltip("How much buoyant force to apply.")]
    public float BuoyancyForce = 60f;

    [Tooltip("Multiplier for jump boost upon exiting the water.")]
    public float JumpBoostMultiplier = 1f;

    [Tooltip("Direction of the current relative to the rotation of this volume.")]
    public Vector3 CirculationDirection;
    [Tooltip("Direction of the current relative to the rotation of this volume.")]
    public float CirculationSpeed;
    [Tooltip("The direction to apply the buoyancy in.")]
    public Vector3 BuoyancyDirection = Vector3.up;
    [Tooltip("How far above the top of the volume the object will sit.")]
    public float SurfaceLevelOffset = 0.05f;
}

[RequireComponent(typeof(Collider))]
public class WaterVolume : MonoBehaviour
{
    [field: SerializeField] public WaterVolumeData WaterData { get; private set; }
    private Collider col;

    private Transform bottom;
    private float waterHeight;

    private void Awake()
    {
        col = GetComponent<Collider>();

        bottom = transform.GetChild(0);

        if (bottom == null || bottom.transform.childCount != 1)
        {
            Debug.LogWarning("make sure " + gameObject.name + " has a bottom component, and it is sorted at the top");
        }
        
        if(bottom != null)
        {
            waterHeight = GetSurfaceLevel() - bottom.position.y;
        }
    }

    private void OnDrawGizmos()
    {
        Collider c = GetComponent<Collider>();
        Gizmos.DrawWireCube(
            new Vector3(transform.position.x, transform.position.y + WaterData.SurfaceLevelOffset, transform.position.z),
            new Vector3(c.bounds.size.x, 0.05f, c.bounds.size.z));
        Gizmos.DrawLine(transform.position, GetWaterCurrentForce());
    }

    public float GetSurfaceLevel()
    {
        return transform.position.y + WaterData.SurfaceLevelOffset;
    }

    public Vector3 GetWaterCurrentForce()
    {
        return transform.rotation * WaterData.CirculationDirection.normalized * WaterData.CirculationSpeed;
    }

    /// <summary>
    /// returns number 0-1 for the players position relative of the surface level (0) and bottom (1).
    /// </summary>
    public float GetPlayerPercentFromBottom()
    {
        if (bottom == null)
            return -1;

        float y = GetSurfaceLevel() - InputManager.Instance.transform.position.y;
        float t = y / waterHeight;

        t = Mathf.Clamp(t, 0, 1);

        return t;

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
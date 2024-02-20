/*******************************************************************************
 * File Name :   BuoyancyTest.cs
 * Author(s) :         Alec
 * Creation Date :     2/19/24
 *
 * Brief Description : Test script to math out buoyancy.
 *
 * TODO:
 * -
 *****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyTest : MonoBehaviour
{
    private Rigidbody rb;
    private WaterVolume volume;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.useGravity = false;
        rb.drag = 0f;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WaterVolume water))
        {
            volume = water;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (volume == null)
        {
            if (other.TryGetComponent(out WaterVolume water))
            {
                volume = water;
            }
            else
            {
                return;
            }
        }

        bool withinBounds = volume.CheckWithinBounds2D(transform.position);
        if (withinBounds)
        {
            float yPos = transform.position.y + 0.05f;
            float maxYPos = volume.GetSurfaceLevel();
            if (yPos < maxYPos)
            {
                float upForce = volume.WaterData.BuoyancyForce * rb.mass;
                float underWaterBuoyantForce = (maxYPos - yPos) * volume.WaterData.DepthModifier;
                float force = upForce + (upForce * underWaterBuoyantForce);
                var vel = rb.velocity;
                // vel += (volume.WaterData.BuoyancyDirection * volume.WaterData.GravityAmount * Time.fixedDeltaTime);
                vel *= Mathf.Clamp01(1f - volume.WaterData.DragFactor * Time.deltaTime);
                rb.velocity = vel;
                rb.AddForce(volume.WaterData.BuoyancyDirection * force);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (volume == null) return;
        if (other.gameObject == volume.gameObject)
        {
            volume = null;
        }
    }
}

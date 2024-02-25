/*******************************************************************************
 * File Name :       PlatformEffector3D.cs
 * Author(s) :       Toby
 * Creation Date :   2/25/24
 *
 * Brief Description : play on the PlatformEffector2D, as seen in Unity 2D.
 * Allows the player to go through it, if approached from below.
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlatformEffector3D : MonoBehaviour
{
    private Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    /// <summary>
    /// return true if collision obj is below this thing
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool CheckObjectBelow(Collider obj)
    {
        if (obj.transform.position.y < transform.position.y)
        {
            return true;
        }
        else 
            return false;
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }
}

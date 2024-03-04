/*******************************************************************************
 * File Name :         FishEvents.cs
 * Author(s) :         Toby 
 * Creation Date :     2/25/2024
 *
 * Brief Description : Events (intended for camera manager to listen to) that
 * activate when the fish does certain things.
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FishEvents : MonoBehaviour
{
    public static FishEvents Instance;

    public UnityEvent FishStartSinking;
    public UnityEvent FishStartAscending;
    public UnityEvent FishExitWater;
    public UnityEvent FishEnterWater;
    public UnityEvent EquilibriumEnter;
    public UnityEvent EquilibriumExit;
    public UnityEvent RailEnter;
    public UnityEvent RailExit;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

  


    
}

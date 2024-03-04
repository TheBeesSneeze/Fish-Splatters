/*******************************************************************************
 * File Name :         RailBubble.cs
 * Author(s) :         Toby Schamberger,
 * Creation Date :     2/29/2024
 *
 * Brief Description : the bubble that appears when u go on the rails. singleton.
 * hidden for most of game lol.
 * listens for game listeners bc its awesome and secretive. like a ninja or something.
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailBubble : MonoBehaviour
{
    public static RailBubble Instance;

    public float ExpandedScale;
    public float ShrinkScale;

    private Vector3 targetScale;

    public void Start()
    {
        FishEvents.Instance.RailEnter.AddListener(ExpandBubble);
        FishEvents.Instance.RailExit.AddListener(ShrinkBubble);

        ShrinkBubble();
    }

    private void ExpandBubble()
    {
        targetScale = Vector3.one * ExpandedScale;
    }

    private void ShrinkBubble()
    {
        targetScale = Vector3.one * ShrinkScale;
    }

    /// <summary>
    /// this code is THE WORST and goes against many of my (personal) core beliefs and values.
    /// if you see this comment and read this code you should shame me at beat me up and call me names and spit on me. 
    /// i do not deserve your respect.
    /// you must understand. i am tired and i am lazy. i do not wish to do this "the proper way" 
    /// i prefer to run some fuckass computation constantly. every frame.
    /// every frame you play fish splatters for this fucking evil dogshit function gets called and your compputer is going to HATE IT.
    /// never forgive me and never let this down.
    /// </summary>
    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime*10);
    }

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

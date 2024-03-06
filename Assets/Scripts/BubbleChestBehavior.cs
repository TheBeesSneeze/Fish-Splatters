/*******************************************************************************
 * File Name :         BubbleChestBehavior.cs
 * Author(s) :         Sky Beal, Toby Schamberger
 * Creation Date :     
 *
 * Brief Description : 
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// hi it's me sky i'm coding this
/// </summary>

public class BubbleChestBehavior : MonoBehaviour
{
    [Header ("Changable Variables")]
    private bool ChestOpen;
    [Tooltip("Seconds between each bubble spawning")]
    public float BubbleSpawnSeconds;
    [Tooltip("How long the chest is closed for")]
    public float ChestClosedSeconds;
    [Tooltip("The amount of bubbles spawned per open chest time")]
    public int AmountOfBubbles;
    [Tooltip("Bubble speed going up")]
    public float BubbleSpeed;

    [Header("Unity")]
    [Tooltip("Bubble Prefab")]
    public GameObject Bubble;
    public GameObject DeathPlane;

    private void Start()
    {
        ChestControls();
    }

    public void ChestControls()
    {
        if (ChestOpen)
        {
            StartCoroutine(SpawnBubbles());
        }

        else
        {
            StartCoroutine(ClosedTime());
        }
        
    }

    public IEnumerator SpawnBubbles()
    {
        for (int i = 0; i < AmountOfBubbles; i++)
        {
            SpawnOneBubble();
            yield return new WaitForSeconds(BubbleSpawnSeconds);
        }
        ChestOpen = false;
        ChestControls();
    }

    public IEnumerator ClosedTime()
    {
        yield return new WaitForSeconds(ChestClosedSeconds);
        ChestOpen = true;
        ChestControls();
    }

    private void SpawnOneBubble()
    {
        GameObject bubbleObj = Instantiate(Bubble, transform.position, Bubble.transform.rotation);
        //Rigidbody bubbleRB = bubbleObj.GetComponent<Rigidbody>();
        //bubbleRB.velocity = (Vector3.up * BubbleSpeed);

        BubbleBehavior bubble = bubbleObj.GetComponent<BubbleBehavior>();
        if (bubble == null) return;

        Vector3 velocity = transform.up * BubbleSpeed;

        bubble.Initialize(DeathPlane, velocity, BubbleSpeed);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// hi it's me sky i'm coding this
/// </summary>

public class BubbleChestBehavior : MonoBehaviour
{
    [Header ("Changable Variables")]
    [Tooltip("Determines if the chest is open or not")]
    public bool ChestOpen;
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

    private void Start()
    {
        ChestControls();
    }

    public void ChestControls()
    {
        if (ChestOpen)
        {
            StartCoroutine(SpawnBubble());
        }

        else
        {
            StartCoroutine(ClosedTime());
        }
        
    }

    public IEnumerator SpawnBubble()
    {
        //can you tell i'm taking a cs course
        for (int i = 0; i < AmountOfBubbles; i++)
        {
            Bubble = Instantiate(Bubble, transform.position, transform.rotation);
            Rigidbody bubbleRB = Bubble.GetComponent<Rigidbody>();
            bubbleRB.AddForce(transform.up * BubbleSpeed);
            yield return new WaitForSeconds(BubbleSpawnSeconds);
        }
        ChestOpen = false;
        ChestControls();
    }

    public IEnumerator ClosedTime()
    {
        ChestOpen = true;
        yield return new WaitForSeconds(ChestClosedSeconds);
        ChestControls();
    }
}


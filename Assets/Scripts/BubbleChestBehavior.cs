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
        for (int i = 0; i < AmountOfBubbles; i++)
        {
            GameObject bubble = Instantiate(Bubble, transform.position, Bubble.transform.rotation);
            Rigidbody bubbleRB = bubble.GetComponent<Rigidbody>();
            //bubbleRB.velocity = (Vector3.up * BubbleSpeed);
            bubbleRB.velocity = (transform.up * BubbleSpeed);
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
}


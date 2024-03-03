using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// hi it's sky i'm also here (coding)
/// </summary>

public class BubbleBehavior : MonoBehaviour
{
    //[Header("Changable Variables")]
    //[Tooltip("Time until bubble despawns after hitting death plane")]
    //public float BubbleDespawnSeconds;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //play enter bubble noise
            Debug.LogWarning("ENTER BUBBLE");
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "BubbleDeathPlane")
        {
            DestroyBubble();
        }
    }

    public void DestroyBubble()
    {
        //yield return new WaitForSeconds(BubbleDespawnSeconds);
        Destroy(gameObject);


    }
}

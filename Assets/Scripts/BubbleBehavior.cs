using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
/// <summary>
/// hi it's sky i'm also here (coding)
/// </summary>
/// 



public class BubbleBehavior : MonoBehaviour
{
    //[Header("Changable Variables")]
    //[Tooltip("Time until bubble despawns after hitting death plane")]
    //public float BubbleDespawnSeconds;

    [Header("Sounds")]
    private float bubbleEnterVolume = 1.0f;
    private AudioClip bubbleEnterSound;
    


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(bubbleEnterSound != null)
            {
                AudioSource.PlayClipAtPoint(bubbleEnterSound, transform.position, bubbleEnterVolume); 
            }
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

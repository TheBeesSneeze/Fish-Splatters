using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// hi it's sky i'm also here (coding)
/// </summary>

public class BubbleBehavior : MonoBehaviour
{
    [Header("Changable Variables")]
    [Tooltip("Time until bubble despawns")]
    public float BubbleDespawnSeconds;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BubbleDeathPlane")
        {
            StartCoroutine(DestroyBubble());
        }
    }

    public IEnumerator DestroyBubble()
    {
        yield return new WaitForSeconds(BubbleDespawnSeconds);
        Destroy(gameObject);
    }
}

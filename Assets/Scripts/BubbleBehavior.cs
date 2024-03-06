/*******************************************************************************
 * File Name :         BubbleBehavior.cs
 * Author(s) :         Sky Beal, Toby Schamberger
 * Creation Date :     
 *
 * Brief Description : 
 *****************************************************************************/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// hi it's sky i'm also here (coding)
/// </summary>

public class BubbleBehavior : MonoBehaviour
{
    [Tooltip("time until bubble gets destroyed. -1 for no timer")]
    public float DeathTimer = -1;
    [Tooltip("Bubble starts flashing after x seconds")]
    public float FlashSeconds = 5;
    [ReadOnly] public float SecondsUntilDestroyed = -1;

    //magic number
    private float transparentColor = 0.25f;

    private GameObject DeathPlane;
    private Material material;
    private Color baseColor;
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");

    /// <summary>
    /// called in bubble chese
    /// </summary>
    public void Initialize( GameObject deathPlane, Vector3 velocity,float bubbleSpeed)
    {
        GetComponent<Rigidbody>().velocity = velocity;

        if(DeathTimer != -1)
        {
            SecondsUntilDestroyed = DeathTimer;
            return;
        }

        if (deathPlane == null) return;

        DeathPlane = deathPlane;

        float distance = Vector3.Distance(transform.position, deathPlane.transform.position);

        SecondsUntilDestroyed = distance / bubbleSpeed;
    }

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        baseColor = material.color;
    }

    private void Update()
    {
        if(SecondsUntilDestroyed <= 0)
        {
            Pop();
            return;
        }

        SecondsUntilDestroyed -= Time.deltaTime;

        if (SecondsUntilDestroyed > FlashSeconds) return;

        //flash transparency

        float a = Mathf.Sin(SecondsUntilDestroyed * Mathf.PI * 2) + 1; // 0<a<2;
        a = a / 2;
        a = Mathf.Lerp(transparentColor, 0.9f, a);
        baseColor.a = a;

        material.SetFloat(Alpha, -a);
    }

    /// <summary>
    /// pop!
    /// </summary>
    public void Pop()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "BubbleDeathPlane")
        {
            Pop();
        }
    }

}

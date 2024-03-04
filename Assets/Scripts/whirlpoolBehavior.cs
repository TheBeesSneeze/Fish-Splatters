using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this script goes onto the player
public class whirlpoolBehavior : MonoBehaviour
{
    public bool beingPulled = false;
    public float pullDepth = 1;
    public float pullForce = 1f;
    public float launchForce = 10f;
    public GameObject whirlpool;
    void Update()
    {
        if (beingPulled && whirlpool != null) {
            Vector3 direction = whirlpool.transform.position - transform.position;
            gameObject.GetComponent<Rigidbody>().AddForce(pullForce * direction.x, pullForce * direction.y * 10, pullForce * direction.z);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Whirlpool") {
            beingPulled = true;
            whirlpool = other.gameObject;
        }
        if (other.tag == "WhirlpoolPoint")
        {
            launch();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Whirlpool")
        {
            beingPulled = false;
            whirlpool = null;
        }
    }
    private void launch()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(0, launchForce ,0);
    }
}

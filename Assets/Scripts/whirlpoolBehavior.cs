using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this script goes onto the player
public class whirlpoolBehavior : MonoBehaviour
{
    public bool beingPulled = false;
    public bool beingLaunched = false;
    public float pullDepth = 1;
    public float pullForce = 1f;
    public float launchForce = 10f;
    public float depth = 10;
    public GameObject whirlpool;
    void FixedUpdate()
    {
        if (beingPulled && whirlpool != null && !beingLaunched) {
            Vector3 direction = whirlpool.transform.position - transform.position;
            gameObject.GetComponent<Rigidbody>().AddForce(pullForce * direction.x, (pullForce * direction.y - depth) * 10, pullForce * direction.z);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Whirlpool") {
            beingLaunched = false;
            beingPulled = true;
            whirlpool = other.gameObject;
        }
        if (other.tag == "WhirlpoolPoint")
        {
            print("touching");
            launch();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Whirlpool")
        {
            beingPulled = false;
            //whirlpool = null;
        }
    }
    private void launch()
    {
        beingLaunched = true;
        gameObject.GetComponent<Rigidbody>().AddForce(0, launchForce ,0);
    }
}

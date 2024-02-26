using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidTargetRotation : MonoBehaviour
{
    
    public float MinDistance = 25;
    public float MaxDistance = 50;

    public float xSpeed; //approx
    public float ySpeed; 
    public float zSpeed;

    //public float speed_speed; //acceleration, if you wont

    private Transform child;

    private float t = 0;

    private Vector3 targetRotation;
    private float targetDistance;

    private Vector3 initialRotation;
    private float initialDistance;

    private void Start()
    {
        child = transform.GetChild(0);
        //RandomizeLocation();
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        Vector3 rot = transform.eulerAngles;
        rot.x = rot.x + (Time.deltaTime * xSpeed);
        rot.y = rot.y + (Time.deltaTime * ySpeed);
        rot.z = rot.z + (Time.deltaTime * zSpeed);
        transform.eulerAngles = rot;
    }

    /*
    private void Update()
    {
        t += Time.deltaTime;
        float tScaled = t * speed;

        if (tScaled >= 1)
        {
            RandomizeLocation();
        }

        transform.eulerAngles = Vector3.Slerp(initialRotation, targetRotation, tScaled);

        Vector3 d = child.localPosition;
        d.x = Mathf.Lerp(initialDistance, targetDistance, tScaled);
        child.localPosition = d;
    }

    void RandomizeLocation()
    {
        initialRotation = transform.eulerAngles;
        initialDistance = Vector3.Distance(child.position, transform.position);

        float x = Random.value * 360;
        float y = Random.value * 360;
        float z = Random.value * 360;

        targetDistance = ((Random.value * (MaxDistance - MinDistance)) + MinDistance);
        targetRotation = new Vector3(x, y, z);
    }
    */
}

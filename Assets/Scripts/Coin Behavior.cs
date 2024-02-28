using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CoinBehavior : MonoBehaviour
{
    public float rotateSpeed = 1;
    void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime, 0, 0);
    }
}

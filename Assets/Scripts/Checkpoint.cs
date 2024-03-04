using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sky's workin on this one
public class Checkpoint : MonoBehaviour
{
    public bool IsCheckpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (IsCheckpoint)
        {
            //if this checkpoint != the last checkpoint i touched
            //store this checkpoint = to the current checkpoint
        }
    }


}

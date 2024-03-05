using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sky's workin on this one
public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CheckpointManager checkpointManager = other.GetComponent<CheckpointManager>();

        if (checkpointManager != null)
        {
            checkpointManager.CurrentCheckpoint = transform.position;
        }
    }


}

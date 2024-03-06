using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       CheckpointManager checkpointManager = other.GetComponent<CheckpointManager>();

       if(checkpointManager != null)
        {
            checkpointManager.RespawnAtCheckpoint();
        }

    }
}

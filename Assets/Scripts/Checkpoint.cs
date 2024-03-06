using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sky's workin on this one
public class Checkpoint : MonoBehaviour
{
    public float DistanceFromCheckpoint = 50;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("help");
        CheckpointManager checkpointManager = other.GetComponent<CheckpointManager>();

        if (checkpointManager != null)
        {
            checkpointManager.CurrentCheckpoint = transform.position;
            MoveDeathPlane();
        }
    }

    public void MoveDeathPlane()
    {

        Debug.Log("please");
        DeathPlane[] deathPlanes = GameObject.FindObjectsOfType<DeathPlane>();

        foreach (DeathPlane deathPlane in deathPlanes)
        {
            Vector3 deathPlanePosition = deathPlane.transform.position;
            deathPlanePosition.y = transform.position.y - DistanceFromCheckpoint;
            deathPlane.transform.position = deathPlanePosition;
        }
    }


}

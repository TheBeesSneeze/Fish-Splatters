using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//sky's workin on this one
public class Checkpoint : MonoBehaviour
{
    public float DistanceFromCheckpoint = 50;
    public bool hitCheckpoint = false; 
    private void OnTriggerEnter(Collider other)
    {
        hitCheckpoint = true;
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
        DistanceFromCheckpoint = Mathf.Abs(DistanceFromCheckpoint);

        Debug.Log("please");
        DeathPlane[] deathPlanes = GameObject.FindObjectsOfType<DeathPlane>();

        foreach (DeathPlane plane in deathPlanes)
        {
            plane.SetHeight(transform.position.y - DistanceFromCheckpoint);
            /*
            Vector3 deathPlanePosition = deathPlane.transform.position;
            deathPlanePosition.y = transform.position.y - DistanceFromCheckpoint;
            deathPlane.transform.position = deathPlanePosition;
            */
        }
    }
}
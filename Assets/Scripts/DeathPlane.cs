using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPlane : MonoBehaviour
{
    public bool MoveAutomatically = true;

    private Vector3 targetPosition;
    private Coroutine movingPlane;

    private void OnTriggerEnter(Collider other)
    {
       CheckpointManager checkpointManager = other.GetComponent<CheckpointManager>();

       if(checkpointManager != null)
        {
            checkpointManager.RespawnAtCheckpoint();
        }

    }

    public void SetHeight(float y)
    {
        if (!MoveAutomatically) return;

        targetPosition = new Vector3(transform.position.x,y, transform.position.z);


        if(movingPlane == null)
            movingPlane = StartCoroutine(smoothHeight());
    }

    private IEnumerator smoothHeight()
    {
        while(transform.position.y != targetPosition.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime);

            yield return null;
        }
        movingPlane = null;
    }
}

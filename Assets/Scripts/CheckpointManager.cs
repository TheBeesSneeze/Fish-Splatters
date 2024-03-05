using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    public Vector3 CurrentCheckpoint;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        else
        {
            Instance = this;
        }
    }

    public void RespawnAtCheckpoint()
    {
        InputManager.Instance.transform.position = CurrentCheckpoint;
    }
}

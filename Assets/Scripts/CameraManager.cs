using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;


    [HideInInspector] public Vector3 CameraOffsetFromPlayer; //use the cameras starting point

    private InputManager player;

    // Start is called before the first frame update
    void Start()
    {
        player = InputManager.Instance;
        CameraOffsetFromPlayer = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}

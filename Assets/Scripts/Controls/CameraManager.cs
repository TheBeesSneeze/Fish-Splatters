/*******************************************************************************
 * File Name :         CameraManager.cs
 * Author(s) :         Toby 
 * Creation Date :     2/25/2024
 *
 * Brief Description : follows player normal style when normal swim. angles up when diving.
 * 
 * todo:
 * center camera more when sinking
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public float CameraSpeed = 10;
    public float ZoomMultiplier=1;

    public float MaxDownwardTilt = 30;

    public bool AngleCameraDown;

    [Tooltip("Starts moving the camera up if the player jumps higher than this number")]
    public float JumpHeightToMoveCamera = 5;

    public enum CameraMode
    {
        DefaultFollow,
        FishSinking,
        FishAscending,
        Repositioning
    }
    public CameraMode Mode;

    [HideInInspector] public Vector3 CameraOffsetFromPlayer; //use the cameras starting point
    private InputManager player;

    private float defaultAngle;
    private float currentZoomMultiplier = 1;

    private Vector3 targetPosition; //the cameras relaitive anchor thing
    private Vector3 realPosition;

    //y position of when space was held
    private float playerYPoint; 
    private float cameraYPoint; 

    // Start is called before the first frame update
    void Start()
    {
        player = InputManager.Instance;
        CameraOffsetFromPlayer = transform.position - player.transform.position;

        defaultAngle = transform.eulerAngles.x;

        FishEvents.Instance.FishEnterWater.     AddListener(OnPlayerEnterWater);
        FishEvents.Instance.FishExitWater.      AddListener(OnPlayerExitWater);
        FishEvents.Instance.FishStartAscending. AddListener(OnPlayerAscendingStart);
        FishEvents.Instance.FishStartSinking.   AddListener(OnPlayerSinkingStart);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPosition = player.transform.position + (CameraOffsetFromPlayer * currentZoomMultiplier);
        realPosition = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * CameraSpeed);

        if (!AngleCameraDown)
        {
            FollowPlayer();
            return;
        }

        switch (Mode)
        {
            case CameraMode.DefaultFollow:
                FollowPlayer();
                break;
            case CameraMode.FishSinking:
                RotateCameraForUnderwater();
                break;
            case CameraMode.FishAscending:
                HandleCameraWhileAscending();
                break;
        }

        
    }

    private void FollowPlayer()
    {
        transform.position = realPosition;
    }
    private void RotateCameraForUnderwater()
    {
        //get player percent through water
        WaterVolume water = player.currentVolume;
        float t = 0;
        if(water != null )
            t = water.GetPlayerPecentFromBottom();

        //zoom out
        //currentZoomMultiplier = Mathf.Max( (1 + t), 1);

        //move camera up slightly
        Vector3 pos = realPosition;
        pos.y = cameraYPoint;// + t;
        transform.position = pos;

        //angle the damn thing
        Vector3 rot = transform.eulerAngles;
        rot.x = Mathf.LerpAngle(defaultAngle, MaxDownwardTilt, t);
        transform.eulerAngles = rot;
    }

    private void HandleCameraWhileAscending()
    {
        if(player.transform.position.y > playerYPoint + JumpHeightToMoveCamera)
        {
            Debug.Log("player jump so high!");
            playerYPoint = player.transform.position.y- JumpHeightToMoveCamera;
            cameraYPoint = targetPosition.y;
        }

        Vector3 pos = realPosition;
        pos.y = cameraYPoint;
        transform.position = pos;
    }

    public void OnPlayerSinkingStart()
    {
        Debug.Log("sink");
        Mode = CameraMode.FishSinking;

        playerYPoint = player.transform.position.y;
        cameraYPoint = transform.position.y;
    }
    public void OnPlayerAscendingStart()
    {
        Mode = CameraMode.FishSinking;
    }

    public void OnPlayerEnterWater()
    {
        currentZoomMultiplier = ZoomMultiplier;
        Mode = CameraMode.DefaultFollow;
    }
    public void OnPlayerExitWater()
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

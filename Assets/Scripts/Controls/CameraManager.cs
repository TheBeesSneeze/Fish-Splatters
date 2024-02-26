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

    public float MaxDownwardTilt = 50;

    public bool AngleCameraDown;

    [Tooltip("Starts moving the camera up if the player jumps higher than this number")]
    public float JumpHeightToMoveCamera = 5;

    private float t = 0;

    public enum CameraMode
    {
        DefaultFollow,
        FishSinking,
        FishAscending,
        Repositioning
    }
    public CameraMode Mode;

    [HideInInspector] public Vector3 CameraOffsetFromPlayer; //use the cameras starting point
    private float defaultAngle;

    private InputManager player;
    private Rigidbody playerRB;
    private bool fishInEquilibrium;

    private Vector3 targetPosition; //the cameras relaitive anchor thing
    private Vector3 realPosition;

    private float targetRotationX;
    private float realRotationX;

    //y position of when space was held
    private float playerYPoint; 
    private float cameraYPoint;

    private float balanceOffset = 0.3f; //allow the player to be balanced if within this number

    // Start is called before the first frame update
    void Start()
    {
        player = InputManager.Instance;
        playerRB = player.GetComponent<Rigidbody>();
        CameraOffsetFromPlayer = transform.position - player.transform.position;

        defaultAngle = transform.eulerAngles.x;
        targetRotationX = defaultAngle;

        FishEvents.Instance.FishEnterWater.     AddListener(OnPlayerEnterWater);
        FishEvents.Instance.FishExitWater.      AddListener(OnPlayerExitWater);
        FishEvents.Instance.FishStartAscending. AddListener(OnPlayerAscendingStart);
        FishEvents.Instance.FishStartSinking.   AddListener(OnPlayerSinkingStart);
        FishEvents.Instance.EquilibriumEnter   .AddListener(OnPlayerEquilibriumEnter);
        FishEvents.Instance.EquilibriumExit    .AddListener(OnPlayerEquilibriumEnter);
    }

    private void Update()
    {
        CheckPlayerBalance();

        if(playerRB.velocity.y > balanceOffset)
        {
            
            //Mode = CameraMode.FishAscending;
        }
        if(playerRB.velocity.y < -balanceOffset)
        {
            Mode = CameraMode.FishSinking;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPosition = player.transform.position + (CameraOffsetFromPlayer);

        if (!AngleCameraDown)
        {
            FollowPlayer();
            return;
        }

        switch (Mode)
        {
            case CameraMode.DefaultFollow:
                //FollowPlayer();
                break;
            case CameraMode.FishSinking:
                TiltCameraForUnderwater();
                break;
            case CameraMode.FishAscending:
                HandleCameraWhileAscending();
                break;
        }

        Vector3 rot = transform.eulerAngles;
        rot.x = Mathf.LerpAngle(transform.eulerAngles.x, targetRotationX, Time.fixedDeltaTime * CameraSpeed);
        transform.eulerAngles = rot;

        realPosition = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * CameraSpeed);

        FollowPlayer();
    }

    private void FollowPlayer()
    {
        //force camera above player
        Vector3 pos = realPosition;
        pos.y = Mathf.Max(pos.y, CameraOffsetFromPlayer.y + player.transform.position.y);

        targetRotationX = defaultAngle;
        transform.position = realPosition;
    }
    private void TiltCameraForUnderwater()
    {
        //get player percent through water
        WaterVolume water = player.currentVolume;

        if(water != null )
            t = water.GetPlayerPecentFromBottom();

        //zoom out
        //currentZoomMultiplier = Mathf.Max( (1 + t), 1);

        //move camera up slightly
        Vector3 pos = targetPosition;
        pos.y = cameraYPoint + (t* ZoomMultiplier);
        targetPosition = pos;

        //angle the damn thing
        targetRotationX = Mathf.LerpAngle(defaultAngle, MaxDownwardTilt, t);
    }

    private void HandleCameraWhileAscending()
    {
        if(player.transform.position.y > playerYPoint + JumpHeightToMoveCamera)
        {
            Debug.Log("player jump so high!");
            playerYPoint = player.transform.position.y- JumpHeightToMoveCamera;
            cameraYPoint = targetPosition.y - JumpHeightToMoveCamera;
        }

        TiltCameraForUnderwater();
    }

    /// <summary>
    /// if player is at water line and velocity =0
    /// </summary>
    private void CheckPlayerBalance()
    {
        if (player.currentVolume == null) return;

        float y = player.currentVolume.WaterData.SurfaceLevelOffset + player.currentVolume.transform.position.y;
        //check for player equiblirium
        if (!player.isHoldingJump && SomewhatEqual(player.transform.position.y, y, balanceOffset) && SomewhatEqual(playerRB.velocity.y, 0, balanceOffset))
        {
            if (!fishInEquilibrium)
                FishEvents.Instance.EquilibriumEnter.Invoke();

            fishInEquilibrium = true;
            return;
        }

        if (fishInEquilibrium)
        {
            FishEvents.Instance.EquilibriumExit.Invoke();
        }
        fishInEquilibrium = false;
    }

    private bool SomewhatEqual(float n, float target, float roomForError)
    {
        float a = target + roomForError;
        float b = target - roomForError;

        if (n <= a && n >= b)
        {
            return true;
        }

        return false;
    }

    public void OnPlayerSinkingStart()
    {
        Debug.Log("sink");
        Mode = CameraMode.FishSinking;

        playerYPoint = player.transform.position.y;
        cameraYPoint = targetPosition.y;
    }
    public void OnPlayerAscendingStart()
    {
        Mode = CameraMode.FishAscending;

        //cameraYPoint = transform.position.y;
        //playerYPoint = player.transform.position.y;
    }

    public void OnPlayerEnterWater()
    {
        //currentZoomMultiplier = ZoomMultiplier;
        
    }
    public void OnPlayerExitWater()
    {
        
    }

    public void OnPlayerEquilibriumEnter()
    {
        Debug.Log("equilibrium");
        //Mode = CameraMode.DefaultFollow;
        Mode = CameraMode.DefaultFollow;
    }

    public void OnPlayerEquilibriumExit()
    {
        //Debug.Log("equilibrium");
        
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

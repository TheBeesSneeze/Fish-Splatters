/*******************************************************************************
 * File Name :         CameraManager.cs
 * Author(s) :         Toby 
 * Creation Date :     2/25/2024
 *
 * Brief Description : follows player normal style when normal swim. angles up when diving.
 * i want to fucking killmyself
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

    [Tooltip("if the player can control the camera vertically")]
    public bool FullPlayerControl = true;

    public bool AngleCameraDown;

    public float CameraHorizontalSpeed = 10;
    public float CameraVerticalSpeed = 10;
    public float RotateSpeed = 10;
    public float ZoomMultiplier=1;

    public float MaxDownwardTilt = 50;

    [Tooltip("Starts moving the camera up if the player jumps higher than this number")]
    public float JumpHeightToMoveCamera = 5;

    private float t = 0; //this is the lerp between default camera angle and down camera angle

    public enum CameraMode
    {
        DefaultFollow,
        FishSinking,
        FishAscending,
        Repositioning
    }
    [HideInInspector] public CameraMode Mode;

    [HideInInspector] public Vector3 CameraOffsetFromPlayer; //use the cameras starting point
    private float defaultAngle;

    private InputManager player;
    private Rigidbody playerRB;
    private bool fishInEquilibrium;

    private Vector3 targetPosition; //the cameras relaitive anchor thing

    public Vector3 targetRotation;

    //y surface level of current (or last) water block
    private float playerYPoint; 
    //private float cameraYPoint;

    private float balanceOffset = 0.4f; //allow the player to be balanced if within this number

    // Start is called before the first frame update
    void Start()
    {
        player = InputManager.Instance;
        playerRB = player.GetComponent<Rigidbody>();

        if (FullPlayerControl)
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        CameraOffsetFromPlayer = transform.position - player.transform.position;

        defaultAngle = transform.eulerAngles.x;
        targetRotation = transform.eulerAngles;

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
        targetPosition = HorizontalCameraManager.Instance.GetTargetPosition();
        //targetPosition = player.transform.position + (CameraOffsetFromPlayer);

        if (!AngleCameraDown)
        {
            //FollowPlayer();
            //return;
        }

        switch (Mode)
        {
            case CameraMode.DefaultFollow:
                if(!FullPlayerControl) targetRotation.x = defaultAngle;
                break;
            case CameraMode.FishSinking:
                TiltCameraForUnderwater();
                break;
            case CameraMode.FishAscending:
                HandleCameraWhileAscending();
                break;
        }

        FollowPlayer();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    /// <summary>
    /// this is *THE* FUNCTION
    /// </summary>
    private void FollowPlayer()
    {
        //force camera above player
        ForceCameraUp();

        //smoothing shit
        targetRotation = HorizontalCameraManager.Instance.GetHorizontalRotation(targetRotation);

        Vector3 realRotation = Vector3.zero;
        realRotation.x = Mathf.LerpAngle(transform.eulerAngles.x, targetRotation.x, Time.fixedDeltaTime * RotateSpeed);
        realRotation.y = Mathf.LerpAngle(transform.eulerAngles.y, targetRotation.y, Time.fixedDeltaTime * RotateSpeed);
        realRotation.z = Mathf.LerpAngle(transform.eulerAngles.z, targetRotation.z, Time.fixedDeltaTime * RotateSpeed);
        transform.eulerAngles = realRotation;

        Vector3 realPosition = Vector3.zero;
        realPosition.x = Mathf.Lerp(transform.position.x, targetPosition.x, Time.fixedDeltaTime * CameraHorizontalSpeed);
        realPosition.y = Mathf.Lerp(transform.position.y, targetPosition.y, Time.fixedDeltaTime * CameraVerticalSpeed);
        realPosition.z = Mathf.Lerp(transform.position.z, targetPosition.z, Time.fixedDeltaTime * CameraHorizontalSpeed);
        transform.position = realPosition;
    }
    private void TiltCameraForUnderwater()
    {
        t = (playerYPoint - player.transform.position.y) / 20;
        t = Mathf.Clamp(t, 0, 1);

        t = t * t;

        //targetRotation.x = Mathf.Lerp(defaultAngle, AngleCameraDown, t);

        //move camera up slightly
        Vector3 pos = targetPosition;
        pos.y = playerYPoint + CameraOffsetFromPlayer.y + (t* ZoomMultiplier);
        targetPosition = pos;

        //angle the damn thing
        if(!FullPlayerControl)
            targetRotation.x = Mathf.LerpAngle(defaultAngle, MaxDownwardTilt, t);
    }

    private void HandleCameraWhileAscending()
    {
        if(player.transform.position.y > playerYPoint + JumpHeightToMoveCamera)
        {
            Debug.Log("player jump so high!");
            playerYPoint = player.transform.position.y- JumpHeightToMoveCamera;
        }

        TiltCameraForUnderwater();
    }

    private void ForceCameraUp()
    {
        Vector3 pos = targetPosition;
        pos.y = Mathf.Max(pos.y, CameraOffsetFromPlayer.y + player.transform.position.y);
        targetPosition = pos;
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
            player.isInEquilibrium = true;
            return;
        }

        if (fishInEquilibrium)
        {
            FishEvents.Instance.EquilibriumExit.Invoke();
            player.isInEquilibrium = false;
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
    }
    public void OnPlayerAscendingStart()
    {
        Mode = CameraMode.FishAscending;

        //cameraYPoint = transform.position.y;
        //playerYPoint = player.transform.position.y;
    }

    public void OnPlayerEnterWater()
    {
        playerYPoint = player.currentVolume.GetSurfaceLevel();
        Mode = CameraMode.DefaultFollow;
    }
    public void OnPlayerExitWater()
    {
        Mode = CameraMode.FishSinking;
    }

    public void OnPlayerEquilibriumEnter()
    {
        //Debug.Log("equilibrium");
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

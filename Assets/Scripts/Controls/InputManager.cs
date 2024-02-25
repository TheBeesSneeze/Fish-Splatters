/*******************************************************************************
 * File Name :         InputManager.cs
 * Author(s) :         Toby Schamberger, Clare Grady, Alec
 * Creation Date :     2/19/2024
 *
 * Brief Description :
 *
 * TODO:
 * - jumping
 *****************************************************************************/

using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Tooltip("The fastest the player will go (without an external force)")]
    public float Speed;

    [Tooltip("The fastest the player will go (midair)")]
    public float SpeedMidair;

    [Tooltip("How long it will take the player to reach their max speed")]
    public float AccelerationSeconds;

    [Tooltip("Deadzone to stop bobbing, an offset from the position of the fish.")]
    public float bobbingDeadZone = 0.05f;

    [Header("Jumping")]

    //Clare's variables (clariables)
    [Tooltip("How fast the descent speed is")]
    public float descentSpeed;

    //[Tooltip("How fast the ascent and fall down is")]
    //public float ascentSpeed;

    //[Tooltip("How high the fish can jump")]
    //public float jumpHeightLimit;

    //[Tooltip("How low the fish can dive")] 
    //public float diveHeightLimit;

    //[Tooltip("The amount you multiplier for how high the jump is")]
    //public float heightMultiplier;

    [Tooltip("How aggressively to move up the water. Default is 100")]
    public float buoyantForceSpring = 100f;

    [Tooltip("How aggressively to counter upward movement. Default is 10")]
    public float buoyantForceDamper = 10f;

    [Tooltip("How much to slow the players descent. Default is 2")]
    public float buoyantDownwardForceDamper = 50;

    [Tooltip("this is the camera")] public Transform movementOrigin;

    bool isHoldingJump;

    private PlayerInput playerInput;
    [HideInInspector] public InputAction Move, Jump, Pause;

    private Rigidbody rigidbody;

    [HideInInspector] public bool CurrentlyMoving;
    [HideInInspector] public bool InWater => currentVolume != null;
    private float currentAccelerationTime;
    private Vector3 movement;
    [HideInInspector] public WaterVolume currentVolume;


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

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody>();

        playerInput.currentActionMap.Enable();

        Move = playerInput.currentActionMap.FindAction("Move");
        Jump = playerInput.currentActionMap.FindAction("Jump");
        Pause = playerInput.currentActionMap.FindAction("Pause");
        playerInput.currentActionMap.FindAction("Quit").started += context => { Application.Quit(); };
        playerInput.currentActionMap.FindAction("Restart").started += context =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        };

        Move.started += Move_started;
        Move.canceled += Move_canceled;

        Jump.started += Jump_started;
        Jump.canceled += Jump_canceled;

        //Pause.started += Pause_started;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WaterVolume water))
        {
            FishEvents.Instance.FishEnterWater.Invoke();
            currentVolume = water;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentVolume == null) return;
        if (!other.gameObject.TryGetComponent(out WaterVolume volume)) return;
        if (volume == currentVolume)
        {
            FishEvents.Instance.FishExitWater.Invoke();
            currentVolume = null;
        }
    }


    private void ManageMovement()
    {
        if (CurrentlyMoving)
        {
            if (currentAccelerationTime < AccelerationSeconds)
                currentAccelerationTime += Time.fixedDeltaTime;
        }
        else
        {
            if (currentAccelerationTime > 0)
                currentAccelerationTime -= Time.fixedDeltaTime;
        }

        float accelerationPercent = currentAccelerationTime / AccelerationSeconds; // 0.0 - 1.0
        accelerationPercent = Mathf.Pow(accelerationPercent, 0.5f);

        float currentSpeed = Speed * accelerationPercent;

        //rigidbody.velocity = Move.ReadValue<Vector2>() * currentSpeed;

        Vector2 move = Move.ReadValue<Vector2>();
        movement = movementOrigin.TransformDirection(new Vector3(move.x, 0f, move.y));

        var targetV = movement * currentSpeed;
        targetV.y = rigidbody.velocity.y;
        Vector3 force = targetV - rigidbody.velocity;
        if (float.IsNaN(force.x) || float.IsNaN(force.y) || float.IsNaN(force.z))
        {
            force = Vector3.zero;
        }

        rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    private void HandleBuoyancy()
    {
        if (currentVolume == null) return;
        float yPosition = transform.position.y + bobbingDeadZone;
        float maxYPos = currentVolume.GetSurfaceLevel();
        //
        // if (yPosition < maxYPos)
        // {
        //     //float upForce = currentVolume.WaterData.BuoyancyForce;
        //     float underWaterBuoyantForce = (maxYPos - yPosition) * currentVolume.WaterData.BuoyancyForce;
        //     //float force = upForce + (upForce * underWaterBuoyantForce);
        //     var vel = rigidbody.velocity;
        //     // vel += (volume.WaterData.BuoyancyDirection * volume.WaterData.GravityAmount * Time.fixedDeltaTime); only if we're applying custom gravity
        //     vel *= Mathf.Clamp01(1f - currentVolume.WaterData.DragFactor * Time.deltaTime);
        //     rigidbody.AddForce(vel - rigidbody.velocity, ForceMode.VelocityChange); //drag force
        //     // rigidbody.AddForce(currentVolume.GetWaterCurrentForce());
        //     rigidbody.AddForce(currentVolume.WaterData.BuoyancyDirection * underWaterBuoyantForce);
        // }
    }

    private void ManageMidairMovement()
    {
        //acceleration doesnt change midair!!!

        float accelerationPercent = currentAccelerationTime / AccelerationSeconds; // 0.0 - 1.0
        accelerationPercent = Mathf.Pow(accelerationPercent, 0.5f);

        float currentSpeed = SpeedMidair * accelerationPercent;

        //rigidbody.velocity = Move.ReadValue<Vector2>() * currentSpeed;

        Vector2 move = Move.ReadValue<Vector2>();
        movement = movementOrigin.TransformDirection(new Vector3(move.x, 0f, move.y));

        var targetV = movement * currentSpeed;
        targetV.y = rigidbody.velocity.y;
        targetV -= rigidbody.velocity;
        if (float.IsNaN(targetV.x) || float.IsNaN(targetV.y) || float.IsNaN(targetV.z))
        {
            targetV = Vector3.zero;
        }

        rigidbody.AddForce(targetV, ForceMode.VelocityChange);
    }

    private void Move_started(InputAction.CallbackContext obj)
    {
        CurrentlyMoving = true;
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        CurrentlyMoving = false;
    }

    private void JumpManagment()
    {
        float yPosition = transform.position.y + bobbingDeadZone;

        //sinking
        if (isHoldingJump)
        {
            //if (position.y > diveHeightLimit)
            //{
            float waterForce = DeepWaterForce();

            rigidbody.AddForce(Vector3.down * descentSpeed, ForceMode.Force);
            rigidbody.AddForce(Vector3.up * descentSpeed * waterForce, ForceMode.Force);

            if (rigidbody.velocity.y >= 0)
            {
                Vector3 vel = rigidbody.velocity;
                vel.y = 0;
                rigidbody.velocity = vel;
            }
        }
        else
        {
            if (currentVolume != null)
            {
                if (yPosition >= currentVolume.GetSurfaceLevel())
                {
                    //at the top, stop!
                }
                else
                {
                    rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);
                    //rigidbody.AddForce(vel - rigidbody.velocity, ForceMode.VelocityChange);
                    var posError = (-yPosition + currentVolume.GetSurfaceLevel());
                    var velError = 0 - rigidbody.velocity.y;
                    var force = currentVolume.WaterData.BuoyancyDirection *
                                (posError * currentVolume.WaterData.BuoyancyForce +
                                 velError * currentVolume.WaterData.BuoyancyDamper);
                    //force = Vector3.ClampMagnitude(force, 100f);
                    rigidbody.AddForce(force);
                }
                //apply some up force
            }
        }
        // else
        // {
        //     if (!hasPastSwimLine)
        //     {
        //         if (position.y < -depth * heightMultiplier)
        //         {
        //             rigidbody.AddForce(Vector3.up * ascentSpeed, ForceMode.VelocityChange);
        //         }
        //         else
        //         {
        //             hasPastSwimLine = true;
        //         }
        //     }
        //     else if (position.y > swimLine)
        //     {
        //         rigidbody.AddForce(Vector3.down * ascentSpeed, ForceMode.VelocityChange);
        //     }
        // }
    }

    private float DeepWaterForce()
    {
        float m = 0;
        if (currentVolume != null)
        {
            m = currentVolume.GetPlayerPecentFromBottom();
            m = m * buoyantDownwardForceDamper;
        }

        //Debug.Log(m);
        return m;
    }

    private void Jump_started(InputAction.CallbackContext obj)
    {
        isHoldingJump = true;

        if (currentVolume != null)
        {
            FishEvents.Instance.FishStartSinking.Invoke();
        }
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        isHoldingJump = false;

        if (currentVolume != null)
        {
            FishEvents.Instance.FishStartAscending.Invoke();
        }
    }


    private void Pause_started(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void FixedUpdate()
    {
        if (!InWater)
        {
            ManageMidairMovement();
        }
        else
        {
            ManageMovement();
            //HandleBuoyancy();
        }

        JumpManagment();
        RotateFish();
    }

    private void Update()
    {
        //moved camera stuff to CameraManager - toby
    }

    private void RotateFish()
    {
        Vector3 rotation = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        //transform.forward = Vector3.Lerp(transform.forward, rigidbody.velocity, Time.deltaTime * 10);

        //transform.rotation = Quaternion.LookRotation(rigidbody.velocity.normalized);

        /*
        float angle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;

        Vector3 rotate = new Vector3(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(movement), Time.deltaTime * 10);
        */
    }
}
/*******************************************************************************
 * File Name :         InputManager.cs
 * Author(s) :         Toby Schamberger, Clare Grady
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

    [Header("Jumping")]
    //Clare's variables (clariables)
    [Tooltip("How fast the descent speed is")]
    public float descentSpeed;
    [Tooltip("How fast the ascent and fall down is")]
    public float ascentSpeed;
    [Tooltip("How high the fish can jump")]
    public float jumpHeightLimit;
    [Tooltip("How low the fish can dive")]
    public float diveHeightLimit;
    [Tooltip("The line (y level) the fish wants to return to")]
    public float swimLine;
    [Tooltip("The amount you multiplier for how high the jump is")]
    public float heightMultiplier;

    [Header("Unity")]
    public GameObject Pivot;

    bool isHoldingJump;
    bool hasPastSwimLine = true;
    float depth;

    private PlayerInput playerInput;
    [HideInInspector] public InputAction Move, Jump, Pause;

    private Rigidbody rigidbody;

    [HideInInspector] public bool CurrentlyMoving;
    [HideInInspector] public bool InWater;
    [HideInInspector] public float VerticalVelocity;
    private float currentAccelerationTime;
    private Vector3 movement;


    private void Awake()
    {
        if(Instance == null)
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

        Move.started += Move_started;
        Move.canceled += Move_canceled;

        Jump.started += Jump_started;
        Jump.canceled += Jump_canceled;

        Pause.started += Pause_started;
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
        JumpManagment(); 

        float accelerationPercent = currentAccelerationTime / AccelerationSeconds; // 0.0 - 1.0
        accelerationPercent = Mathf.Pow(accelerationPercent, 0.5f);

        float currentSpeed = Speed * accelerationPercent;

        //rigidbody.velocity = Move.ReadValue<Vector2>() * currentSpeed;

        Vector2 move = Move.ReadValue<Vector2>();
        movement = transform.TransformDirection(new Vector3(move.x, 0, move.y));

        rigidbody.velocity = movement * currentSpeed;
    }

    private void ManageMidairMovement()
    {
        //acceleration doesnt change midair!!!

        float accelerationPercent = currentAccelerationTime / AccelerationSeconds; // 0.0 - 1.0
        accelerationPercent = Mathf.Pow(accelerationPercent, 0.5f);

        float currentSpeed = SpeedMidair * accelerationPercent;

        //rigidbody.velocity = Move.ReadValue<Vector2>() * currentSpeed;

        Vector2 move = Move.ReadValue<Vector2>();
        movement = transform.TransformDirection(new Vector3(move.x, VerticalVelocity, move.y));

        rigidbody.velocity = movement * currentSpeed;
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
        Vector3 position = rigidbody.position;
        if (isHoldingJump)
        {
            if(position.y > diveHeightLimit)
            {
                rigidbody.AddForce(Vector3.down * descentSpeed, ForceMode.VelocityChange);
            }
            depth = position.y;
        }
        else
        {
            if(!hasPastSwimLine)
            {
                if (position.y < -depth * heightMultiplier)
                {
                    rigidbody.AddForce(Vector3.up * ascentSpeed , ForceMode.VelocityChange);
                }
                else
                {
                    hasPastSwimLine = true;
                }
            }
            else if(position.y > swimLine)
            {
                rigidbody.AddForce(Vector3.down * ascentSpeed, ForceMode.VelocityChange);
            }
        }
    }


    private void Jump_started(InputAction.CallbackContext obj)
    {
        isHoldingJump = true;
        hasPastSwimLine = false;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        isHoldingJump = false; 
    }

    
    private void Pause_started(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void FixedUpdate()
    {
        if (VerticalVelocity != 0)
        {
            ManageMidairMovement();
            return;
        }
        else
        {
            ManageMovement();
        }

        RotateFish();
    }

    private void RotateFish()
    {
        Vector3 rotation = new Vector3(movement.x, 0, movement.z);

        Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);

        Pivot.transform.rotation = Quaternion.Slerp(Pivot.transform.rotation, targetRotation, Time.deltaTime *10);

    }
}

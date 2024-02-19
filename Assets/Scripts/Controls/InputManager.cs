/*******************************************************************************
 * File Name :         InputManager.cs
 * Author(s) :         Toby Schamberger
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

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Tooltip("The fastest the player will go (without an external force)")]
    public float Speed;
    [Tooltip("The fastest the player will go (midair)")]
    public float SpeedMidair;
    [Tooltip("How long it will take the player to reach their max speed")]
    public float AccelerationSeconds;

    private PlayerInput playerInput;
    [HideInInspector] public InputAction Move, Jump, Pause;

    private Rigidbody rigidbody;

    [HideInInspector] public bool CurrentlyMoving;
    [HideInInspector] public bool InWater;
    [HideInInspector] public float VerticalVelocity;
    private float currentAccelerationTime;


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

        float accelerationPercent = currentAccelerationTime / AccelerationSeconds; // 0.0 - 1.0
        accelerationPercent = Mathf.Pow(accelerationPercent, 0.5f);

        float currentSpeed = Speed * accelerationPercent;

        //rigidbody.velocity = Move.ReadValue<Vector2>() * currentSpeed;

        Vector2 movement = Move.ReadValue<Vector2>();
        Vector3 move = transform.TransformDirection(new Vector3(movement.x, VerticalVelocity, movement.y));

        rigidbody.velocity = move * currentSpeed;
    }

    private void ManageMidairMovement()
    {
        //acceleration doesnt change midair!!!

        float accelerationPercent = currentAccelerationTime / AccelerationSeconds; // 0.0 - 1.0
        accelerationPercent = Mathf.Pow(accelerationPercent, 0.5f);

        float currentSpeed = SpeedMidair * accelerationPercent;

        //rigidbody.velocity = Move.ReadValue<Vector2>() * currentSpeed;

        Vector2 movement = Move.ReadValue<Vector2>();
        Vector3 move = transform.TransformDirection(new Vector3(movement.x, VerticalVelocity, movement.y));

        rigidbody.velocity = move * currentSpeed;
    }

    private void Move_started(InputAction.CallbackContext obj)
    {
        CurrentlyMoving = true;
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        CurrentlyMoving = false;
    }

    private void Jump_started(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
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

        ManageMovement();
    }
}

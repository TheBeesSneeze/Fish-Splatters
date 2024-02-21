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

        float accelerationPercent = currentAccelerationTime / AccelerationSeconds; // 0.0 - 1.0
        accelerationPercent = Mathf.Pow(accelerationPercent, 0.5f);

        float currentSpeed = Speed * accelerationPercent;

        //rigidbody.velocity = Move.ReadValue<Vector2>() * currentSpeed;

        Vector2 move = Move.ReadValue<Vector2>();
        movement = transform.TransformDirection(new Vector3(move.x, VerticalVelocity, move.y));

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
        else
        {
            ManageMovement();
        }

        RotateFish();
    }

    private void RotateFish()
    {

        //transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, transform.position + movement, 1, 1);

        /*
        float angle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg;

        //angle = Mathf.Clamp(angle, rangedPlayerController.MaxDownAngle, rangedPlayerController.MaxUpAngle);

        rangedPlayerController.RotationPivot.transform.localEulerAngles = new Vector3(0, 0, angle);

        //flip gun to aim in right direction
        if (aimDirection.x < 0)
            GunSprite.flipY = true;

        if (aimDirection.x > 0)
            GunSprite.flipY = false;

        //change layer to look right
        if (aimDirection.y > 0)
            GunSprite.sortingOrder = -5;

        if (aimDirection.y < 0)
            GunSprite.sortingOrder = 5;

        */
    }
}

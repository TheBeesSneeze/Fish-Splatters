/*******************************************************************************
 * File Name :         InputManager.cs
 * Author(s) :         Toby Schamberger, Clare Grady, Alec, Sky Beal
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
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("Moving")][Tooltip("The fastest the player will go (without an external force)")]
    public float Speed;

    [Tooltip("Speed the fish SPRINTS at.")]
    public float SprintSpeed;

    //[Tooltip("Speed the fish DASHES at.")]
    //public float DashForce;

    [Tooltip("The fastest the player will go (midair)")]
    public float SpeedMidair;

    [Tooltip("What rate the fish slows down (higher it is the quicker it slows)")]
    public float CounterForceMultiplier = 0.5f;

    [Tooltip("The fastest the player will SPRINT (midair)")]
    public float SprintSpeedMidair;

    [Tooltip("How long it will take the player to reach their max speed")]
    public float AccelerationSeconds;

    [Tooltip("Deadzone to stop bobbing, an offset from the position of the fish.")]
    public float bobbingDeadZone = 0.05f;


    [Header("Jumping")]

    //Clare's variables (clariables)
    [Tooltip("How fast the descent speed is")]
    public float descentSpeed;


    public float bottomSurfaceMotorLeftSpeed = 0.1f;
    public float bottomSurfaceMotorRightSpeed = 0.1f;

    //[Tooltip("How fast the ascent and fall down is")]
    //public float ascentSpeed;

    //[Tooltip("How high the fish can jump")]
    //public float jumpHeightLimit;

    //[Tooltip("How low the fish can dive")] 
    //public float diveHeightLimit;

    //[Tooltip("The amount you multiplier for how high the jump is")]
    //public float heightMultiplier;

    [Tooltip("How much to slow the players descent. Default is 2")]
    public float buoyantDownwardForceDamper = 50;

    [Tooltip("Color of fish at max depth.")]
    public Color DepthColor;

    [Header("Unity")][Tooltip("this is the camera")]
    public Transform movementOrigin;

    public Transform ModelPivot;
    public Transform Model;

    [HideInInspector] public bool isHoldingJump;
    [HideInInspector] private bool isHoldingSprint;

    private PlayerInput playerInput;
    [HideInInspector] public InputAction Move, Jump, Pause, cameraMovement, Sprint, Dash;

    [HideInInspector] public Rigidbody rigidbody;

    [HideInInspector] public bool CurrentlyMoving;
    public bool InWater => currentVolume != null;
    private float currentAccelerationTime;
    [HideInInspector] public Vector3 movement;
    [HideInInspector] public WaterVolume currentVolume;
    [HideInInspector] public RailNode currentRailNode;
    private float depth;
    private bool jumpWasHeld;

    [Header("Sounds")]

    private bool hasEnteredAir = false;
    private bool hasHitJump = false;

    [SerializeField] private float swimVolume = 1f;
    [SerializeField] private AudioClip swimSound;
    [SerializeField] private float jumpVolume = 1f;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private float hitBottomVolume = 1f;
    [SerializeField] private AudioClip hitBottomSound;
    [SerializeField] private float splashVolume = 1f;
    [SerializeField] private AudioClip splashSound;
    [SerializeField] private float sprintVolume = 1f;
    [SerializeField] private AudioClip sprintSound;
    [SerializeField] private AudioSource soundOrigin; 


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
        Sprint = playerInput.currentActionMap.FindAction("Sprint");
        Dash = playerInput.currentActionMap.FindAction("Dash");

        Pause = playerInput.currentActionMap.FindAction("Pause");
        cameraMovement = playerInput.currentActionMap.FindAction("Camera");
        playerInput.currentActionMap.FindAction("Quit").started += context => { Application.Quit(); };
        playerInput.currentActionMap.FindAction("Restart").started += context =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        };

        Move.started += Move_started;
        Move.canceled += Move_canceled;

        Jump.started += Jump_started;
        Jump.canceled += Jump_canceled;

        Sprint.started += Sprint_started;
        Sprint.canceled += Sprint_canceled;

        //Dash.started += Dash_started;
        //Pause.started += Pause_started;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WaterVolume water))
        {
            currentVolume = water;

            if (isHoldingJump)
            {
                FishEvents.Instance.FishStartSinking.Invoke();
            }

            FishEvents.Instance.FishEnterWater.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentVolume == null) return;
        if (!other.gameObject.TryGetComponent(out WaterVolume volume)) return;
        if (volume == currentVolume)
        {
            FishEvents.Instance.FishExitWater.Invoke();
            //prevVolume = currentVolume;
            currentVolume = null;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Water Bottom")) return;
        //do a haptic
        Gamepad.current?.SetMotorSpeeds(bottomSurfaceMotorLeftSpeed, bottomSurfaceMotorRightSpeed);
        
        if(hitBottomSound != null)
        {
            soundOrigin.PlayOneShot(hitBottomSound, hitBottomVolume);
        }
        
        
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Water Bottom")) return;
        Gamepad.current?.ResetHaptics();
    }

    private void ManageMovement()
    {
        if (CurrentlyMoving)
        {
            if (currentAccelerationTime < AccelerationSeconds)
                currentAccelerationTime += Time.fixedDeltaTime;
            
            if(swimSound != null && !isHoldingSprint)
            {
                soundOrigin.PlayOneShot(swimSound, swimVolume);
            }
            else if (sprintSound != null && isHoldingSprint)
            {
                soundOrigin.PlayOneShot(sprintSound, sprintVolume); 
            }
            
        }
        else
        {
            if (currentAccelerationTime > 0)
                currentAccelerationTime -= Time.fixedDeltaTime;

            currentAccelerationTime = MathF.Max(currentAccelerationTime, 0.1f);
        }

        if(hasEnteredAir && InWater && hasHitJump)
        {
            if(splashSound != null)
            {
                soundOrigin.PlayOneShot(splashSound, splashVolume); 
            }
            hasEnteredAir = false;
        }

        float accelerationPercent = currentAccelerationTime / AccelerationSeconds; // 0.0 - 1.0
        accelerationPercent = Mathf.Pow(accelerationPercent, 0.5f);

        float currentSpeed = 0;

        if (isHoldingSprint)
        {
            currentSpeed = SprintSpeed * accelerationPercent;
        }
        else
        {
            currentSpeed = Speed * accelerationPercent;
        }


        var targetV = movement * currentSpeed;
        targetV.y = rigidbody.velocity.y;
        Vector3 force = targetV - rigidbody.velocity;
        if (float.IsNaN(force.x) || float.IsNaN(force.y) || float.IsNaN(force.z))
        {
            force = Vector3.zero;
        }

        rigidbody.AddForce(force);
        var counterForce = rigidbody.velocity;
        counterForce *= -1f;
        counterForce *= CounterForceMultiplier;
        counterForce.y = 0f;
        rigidbody.AddForce(counterForce, ForceMode.Acceleration);
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
        hasEnteredAir = true;

        float accelerationPercent = currentAccelerationTime / AccelerationSeconds; // 0.0 - 1.0 this never gets updated
        // accelerationPercent = Mathf.Pow(accelerationPercent, 0.5f);

        float currentSpeed = 0;

        if (isHoldingSprint)
        {
            currentSpeed = SprintSpeedMidair * accelerationPercent;
        }

        else
        {
            currentSpeed = SpeedMidair * accelerationPercent;
        }

        //rigidbody.velocity = Move.ReadValue<Vector2>() * currentSpeed;

        

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

            rigidbody.AddForce(Vector3.down * descentSpeed, ForceMode.Force);
            if (currentVolume != null)
            {
                depth = Mathf.Abs(yPosition - currentVolume.GetSurfaceLevel());
            }
            else
            {
                depth = 0f;
            }

            // rigidbody.AddForce(Vector3.up * descentSpeed * waterForce, ForceMode.Force);
            //
            // if (rigidbody.velocity.y >= 0)
            // {
            //     Vector3 vel = rigidbody.velocity;
            //     vel.y = 0;
            //     rigidbody.velocity = vel;
            // }
            jumpWasHeld = true;
        }
        else
        {
            if (currentVolume != null)
            {
                if (yPosition >= currentVolume.GetSurfaceLevel())
                {
                    // //at the top, stop!
                    if (jumpWasHeld)
                    {
                        rigidbody.AddForce(Vector3.up * -rigidbody.velocity.y,
                            ForceMode.VelocityChange); //cancel out y veloicty
                        //add impulse force to reach -depth height
                        rigidbody.AddForce(
                            Vector3.up * (Mathf.Sqrt(2 * depth * Physics.gravity.magnitude) *
                                          currentVolume.WaterData.JumpBoostMultiplier), ForceMode.Impulse);

                        depth = 0f;
                        jumpWasHeld = false;
                    }
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

        //jumpWasHeld = isHoldingJump;
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
            m = currentVolume.GetPlayerPercentFromBottom();
            m = m * buoyantDownwardForceDamper;
        }

        //Debug.Log(m);
        return m;
    }

    private void Jump_started(InputAction.CallbackContext obj)
    {
        isHoldingJump = true;
        hasEnteredAir = false;
        hasHitJump = true;

        if(currentRailNode != null)
        {
            Debug.Log("exit node!");
            currentRailNode.ExitRail();
            rigidbody.AddForce(Vector3.up * 10, ForceMode.Impulse);
            FishEvents.Instance.RailExit.Invoke();
        }

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
        
        if(jumpSound != null)
        {
            soundOrigin.PlayOneShot(jumpSound, jumpVolume); 
        }
        
        
    }

    private void Sprint_started(InputAction.CallbackContext obj)
    {
        isHoldingSprint = true;
    }

    private void Sprint_canceled(InputAction.CallbackContext obj)
    {
        isHoldingSprint = false;
    }

    /*private void Dash_started(InputAction.CallbackContext obj)
    {
        rigidbody.AddForce(ModelPivot.forward * DashForce, ForceMode.Impulse);
        //cooldown
        //drag near end (decrease acceleration)
    }*/


    private void Pause_started(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void FixedUpdate()
    {
        Vector2 move = Move.ReadValue<Vector2>().normalized;
        movement = movementOrigin.TransformDirection(new Vector3(move.x, 0f, move.y));
        movement.y = 0f;

        if (currentRailNode != null) 
        {
            return;
        }

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
    }

    private void Update()
    {
        //moved camera stuff to CameraManager - toby
        MakeFishBluer();
        RotateFish();
    }

    private void RotateFish()
    {
        if (currentRailNode != null) return;

        //if (rigidbody.velocity.x == 0 && rigidbody.velocity.z==0)
        //    return;

        Vector3 rotation = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);

        ModelPivot.rotation = Quaternion.Slerp(ModelPivot.rotation, targetRotation, Time.deltaTime * 10f);
    }


    private void MakeFishBluer()
    {
        if (currentVolume == null)
        {
            Model.GetComponent<Renderer>().material.color = Color.white;
            return;
        }

        float t = currentVolume.GetPlayerPercentFromBottom();

        Color AppliedColor = Color.Lerp(Color.white, DepthColor, t);

        Model.GetComponent<Renderer>().material.color = AppliedColor;
    }
}
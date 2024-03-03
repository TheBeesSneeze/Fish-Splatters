/*******************************************************************************
 * File Name :         RailNode.cs
 * Author(s) :         Toby Schamberger
 * Creation Date :     2/28/2024
 *
 * Brief Description : NODE THEORY!!!! Rail nodes link to other rail nodes and theyre awesome.
 * look at the variables for a good description of whats going on here.
 * uses lerps n shit.
 *
 * TODO:
 * - jumping
 * - test continued mommy with controller
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
using System;

public class RailNode : MonoBehaviour
{
    [Tooltip("Rail that is rail points to. Leave empty if this is the end")]
    public RailNode NextRail;
    [ReadOnly][HideInInspector] public RailNode LastRail; // assigned automatically :D

    [Header("Head Chain Settings")]
    [InfoBox("Only define these varibles on the first node in the chain.", EInfoBoxType.Normal)]

    public float MetersPerSecond = 1;

    [Header("Advanced settings")]

    [Tooltip("Updates rail line renderer and math live (bad for performance). necessary if you want to have rails move for some reason")]
    [Foldout("Advanced settings")] public bool LiveUpdate = false;
    [Tooltip("ONLY USE FOR CLOSED LOOPS. If true, all next nodes will copy the settings from this one.")]
    [Foldout("Advanced settings")] public bool IsHeadNode = false;
    [Tooltip("How long until player can reenter the rail")]
    [Foldout("Advanced settings")] private float cooldownTime = 0.15f;
    [Tooltip("this shit dont work well yet. dont set it true")]
    [Foldout("Advanced settings")] private bool ContinueMomentum = false;

    [Foldout("Debug")][SerializeField][ReadOnly] private bool playerInRail;
    [Foldout("Debug")][SerializeField][ReadOnly] private bool continueMomentum;
    [Foldout("Debug")][SerializeField][ReadOnly] private float interpolationPercent = 0; //t
    [Foldout("Debug")][SerializeField][ReadOnly] private float currentInputMomentum = 0;
    [Foldout("Debug")][SerializeField][ReadOnly] private float cooldownElapsed;
    
    

    private LineRenderer lineRenderer;
    private float distance;
    private Vector3 direction; //points at next node
    private float metersPerSecondOffset; //makes player lerp meters/seconf


    [Header("Sounds")]
    private float railVolume = 1.0f;
    private AudioClip railSound;
    


    private Transform playerTransform;

    /// <summary>
    /// hooks player to rail. works if player was already on a different rail.
    /// </summary>
    public void EnterRail(float t)
    {
        if (LastRail != null)
            LastRail.TransitionRailExit();

        if(railSound != null)
        {
            AudioSource.PlayClipAtPoint(railSound, transform.position, railVolume);
        }
       

        InputManager.Instance.rigidbody.isKinematic = true;
        InputManager.Instance.currentRailNode = this;
        playerInRail = true;
        continueMomentum = false;
        interpolationPercent = t;
    }

    /// <summary>
    /// so the player can keep holding w or whatever!!
    /// </summary>
    public void EnterRail(float t, float carriedMomentum)
    {
        EnterRail(t);
        currentInputMomentum = carriedMomentum;
        continueMomentum = true;
    }

    public void TransitionRailExit()
    {
        cooldownElapsed = 0;
        //interpolationPercent = -1;
        currentInputMomentum = -1;
        playerInRail = false;
        continueMomentum = false;
    }

    public void ExitRail()
    {
        DetatchPlayer();
        cooldownElapsed = 0;
        InputManager.Instance.currentRailNode = null;
        //interpolationPercent = -1;
        currentInputMomentum = -1;
        playerInRail = false;
        continueMomentum = false;
    }

    public void DetatchPlayer()
    {
        InputManager.Instance.rigidbody.isKinematic = false;
    }

    /// <summary>
    /// redraws the line. does important math. CALL THIS FUNCTION IF YOU PLAN ON MOVING ANY RAIL NODE I CANT SRESS THIS ENOUGH
    /// </summary>
    public void UpdateVisual()
    {
        //if nothing bitch baby fuck node
        if (NextRail == null)
        {
            distance = 0;
            direction = Vector3.zero;

            if(lineRenderer != null)
                lineRenderer.enabled = false;

            return;
        }

        distance = Vector3.Distance(transform.position, NextRail.transform.position);
        direction = (NextRail.transform.position - transform.position).normalized;

        if (lineRenderer == null) return;

        lineRenderer.enabled = true; //yes this does need to be here

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, NextRail.transform.position);

        

        metersPerSecondOffset = distance / MetersPerSecond;
    }

    private void InterpolatePlayerPosition()
    {
        if (!continueMomentum || !ContinueMomentum)
            CalculateInputMomentum();

        //Vector3 playerInputDirection = InputManager.Instance.movement;
        //currentInputMomentum = Vector3.Dot(direction, playerInputDirection);

        interpolationPercent += currentInputMomentum * Time.fixedDeltaTime / metersPerSecondOffset;

        //enter next rail
        if (interpolationPercent > 1 && NextRail != null && NextRail.NextRail != null)
        {
            NextRail.EnterRail(0, currentInputMomentum);
            TransitionRailExit();
            return;
        }

        //enter last rail
        if (interpolationPercent < 0 && LastRail != null)
        {
            LastRail.EnterRail(1, currentInputMomentum);
            TransitionRailExit();
            return;
        }

        interpolationPercent = Mathf.Clamp(interpolationPercent,0,1);

        Vector3 playerPosition = Vector3.Lerp(transform.position, NextRail.transform.position, interpolationPercent);
        Vector3 faceDirection = direction * currentInputMomentum;

        UpdatePlayerPosition(playerPosition, faceDirection);
    }

    /// <summary>
    /// sorry this gets called on EVERY RAIL whoops. called when a player touches/ untouches a WASD
    /// </summary>
    private void CalculateInputMomentum()
    {
        //oh FUCK is this gonna work with controller
        //if (!playerInRail) return;

        //if(continueMomentum) return;

        Debug.Log("calc input in " + gameObject.name);

        Vector3 playerInputDirection = InputManager.Instance.movement;
        currentInputMomentum = Vector3.Dot(direction, playerInputDirection);
    }



    /// <summary>
    /// smoothens player position
    /// </summary>
    private void UpdatePlayerPosition(Vector3 targetPlayerPosition, Vector3 facingDirection)
    {
        Vector3 realPosition = Vector3.Lerp(playerTransform.position, targetPlayerPosition, Time.fixedDeltaTime * 100);
        playerTransform.position = realPosition;

        InputManager.Instance.ModelPivot.LookAt(playerTransform.position+facingDirection);
    }

    /// <summary>
    /// raycats go prrrrrrr
    /// </summary>
    private void SearchForPlayer()
    {
        if (playerInRail) return;
        if (NextRail == null) return;

        Ray ray = new Ray(transform.position, direction);
        bool hit = Physics.Raycast(ray,out RaycastHit raycastHit, distance);

        if (!hit) return;
        
        PlayerInput player = raycastHit.transform.GetComponent<PlayerInput>();

        if (player == null) return;

        //oh shit the players actually there

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        EnterRail(playerDistance / distance);

        FishEvents.Instance.RailEnter.Invoke();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (cooldownElapsed < cooldownTime) return;

        InputManager player = other.GetComponent<InputManager>();

        if(player == null) return;
        if (player.currentRailNode != null) return;

        EnterRail(0);

        FishEvents.Instance.RailEnter.Invoke();
    }

    private void Awake()
    {
        SetLastLink();
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerTransform = InputManager.Instance.transform;

        UpdateVisual();

        if(LastRail == null)
            IsHeadNode = true;

        if (IsHeadNode)
            UpdateSettingsForNextRail(this);

        InputManager.Instance.Move.started += PlayerInputChange;
        InputManager.Instance.Move.canceled += PlayerInputChange;
    }

    private void PlayerInputChange(InputAction.CallbackContext obj)
    {
        continueMomentum = false;
    }

    private void FixedUpdate()
    {
        if (playerInRail)
        {
            InterpolatePlayerPosition();
            return;
        }
    }

    private void Update()
    {
        if(cooldownElapsed < cooldownTime)
        {
            cooldownElapsed += Time.deltaTime;
            return;
        }

        if (LiveUpdate)
        {
            UpdateVisual();

            if(LastRail != null && !LastRail.LiveUpdate)
                LastRail.UpdateVisual();
        }

        SearchForPlayer();
    }

    /// <summary>
    /// RECURSIVE function called in the first node.
    /// i LOVE cs 102!!!!!!!!! :D
    /// </summary>
    public void UpdateSettingsForNextRail(RailNode template)
    {
        if (NextRail == null) return;

        MetersPerSecond = template.MetersPerSecond;
        UpdateVisual();

        if (NextRail == template) return; //no infinite loops please!

        NextRail.UpdateSettingsForNextRail(template);
    }

    /// <summary>
    /// sets last node variable for this next rail.
    /// if the last rail is already defined then we have problems.
    /// </summary>
    public void SetLastLink()
    {
        if (NextRail == null)
        {
            return;
        }

        if(NextRail.LastRail == null)
        {
            NextRail.LastRail = this;
            return;
        }

        //oh shit okay so two rails are pointing at this one rail. FUCK.
        NextRail.SwapLinks();
        NextRail.LastRail = this;
    }

    /// <summary>
    /// correcting two nodes pointing to one
    /// </summary>
    public void SwapLinks()
    {
        RailNode temp = NextRail;
        NextRail = LastRail;
        LastRail = temp;

        if(NextRail != null)
        {
            NextRail.SwapLinks();
        }

        UpdateVisual();
    }

    private void OnDrawGizmos()
    {
        if (NextRail != null)
        {
            Gizmos.DrawLine(NextRail.transform.position, transform.position);

            GetComponent<LineRenderer>().SetPosition(0, NextRail.transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position);
            return;
        }
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        GetComponent<LineRenderer>().SetPosition(1, transform.position);
    }

    [Button]
    public void Create_New_Rail()
    {
        Debug.LogWarning("sorry guys i havent done this yet");
        //if(NextRail)
    }
}

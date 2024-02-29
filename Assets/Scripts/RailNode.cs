/*******************************************************************************
 * File Name :         RailNode.cs
 * Author(s) :         Toby Schamberger
 * Creation Date :     2/28/2024
 *
 * Brief Description :
 *
 * TODO:
 * - jumping
 *****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;


public class RailNode : MonoBehaviour
{
    [Tooltip("Rail that is rail points to. Leave empty if this is the end")]
    public RailNode NextRail;
    [HideInInspector] public RailNode LastRail; // assigned automatically :D

    [Header("Head Chain Settings")]
    [InfoBox("Only define these varibles on the first node in the chain.", EInfoBoxType.Normal)]

    public float MetersPerSecond = 1;

    [Header("Advanced settings")]

    [Tooltip("Updates rail line renderer and math live (bad for performance). necessary if you want to have rails move for some reason")]
    [Foldout("Advanced settings")] public bool LiveUpdate = false;

    [Tooltip("ONLY USE FOR CLOSED LOOPS. If true, all next nodes will copy the settings from this one.")]
    [Foldout("Advanced settings")] public bool IsHeadNode = false;

    private LineRenderer lineRenderer;
    private float distance;
    private Vector3 direction; //points at next node

    [HideInInspector] public bool PlayerInRail;
    private float interpolationPercent = 0; //t

    private float cooldownElapsed;
    private float cooldownTime = 0.15f;

    private Transform playerTransform;

    /// <summary>
    /// hooks player to rail. works if player was already on a different rail.
    /// </summary>
    /// <param name="t"></param>
    public void EnterRail(float t)
    {
        if (LastRail != null)
            LastRail.TransitionRail();

        InputManager.Instance.rigidbody.isKinematic = true;
        InputManager.Instance.currentRailNode = this;
        PlayerInRail = true;
        interpolationPercent = t;
    }

    public void TransitionRail()
    {
        cooldownElapsed = 0;
        interpolationPercent = -1;
        PlayerInRail = false;
    }

    public void ExitRail()
    {
        DetatchPlayer();
        cooldownElapsed = 0;
        InputManager.Instance.currentRailNode = null;
        interpolationPercent = -1;
        PlayerInRail = false;
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
        if (NextRail == null)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, NextRail.transform.position);

        distance = Vector3.Distance(transform.position, NextRail.transform.position);
        direction = (NextRail.transform.position - transform.position).normalized;
    }

    private void InterpolatePlayerPosition()
    {
        Vector3 playerInputDirection = InputManager.Instance.movement; 
        float playerDirection = Vector3.Dot(direction, playerInputDirection);

        interpolationPercent += playerDirection * Time.fixedDeltaTime;

        //enter next rail
        if (interpolationPercent > 1 && NextRail != null)
        {
            TransitionRail();
            NextRail.EnterRail(0);
            return;
        }

        //enter last rail
        if(interpolationPercent < 0 && LastRail != null)
        {
            TransitionRail();
            LastRail.EnterRail(1);
            return;
        }

        Vector3 playerPosition = Vector3.Lerp(transform.position, NextRail.transform.position, interpolationPercent);
        Vector3 faceDirection = direction * playerDirection;

        UpdatePlayerPosition(playerPosition, faceDirection);
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

    private void SearchForPlayer()
    {
        if (PlayerInRail) return;
        if (NextRail == null) return;

        Ray ray = new Ray(transform.position, direction);
        bool hit = Physics.Raycast(ray,out RaycastHit raycastHit, distance);

        if (!hit) return;
        
        PlayerInput player = raycastHit.transform.GetComponent<PlayerInput>();

        if (player == null) return;

        //oh shit the players actually there

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        EnterRail(playerDistance / distance);
    }

    private void OnTriggerEnter(Collider other)
    {
        InputManager player = other.GetComponent<InputManager>();

        if(player == null) return;

        if (player.currentRailNode != null) return;

        EnterRail(0);
    }

    private void Awake()
    {
        if (NextRail != null)
        {
            NextRail.LastRail = this;
        }
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
    }

    private void FixedUpdate()
    {
        if (PlayerInRail)
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

        if (NextRail == template) return; //no infinite loops please!

        NextRail.UpdateSettingsForNextRail(template);
    }

    private void OnDrawGizmos()
    {
        if (NextRail != null)
        {
            Gizmos.DrawLine(NextRail.transform.position, transform.position);

            GetComponent<LineRenderer>().SetPosition(0, NextRail.transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position);
        }
    }

    [Button]
    public void Create_New_Rail()
    {
        Debug.LogWarning("sorry guys i havent done this yet");
        //if(NextRail)
    }
}

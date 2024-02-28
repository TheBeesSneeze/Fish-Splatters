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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RailNode : MonoBehaviour
{
    [Tooltip("Rail that is rail points to. Leave empty if this is the end")]
    public RailNode NextRail;
    [HideInInspector] public RailNode LastRail; // assigned automatically :D

    private LineRenderer lineRenderer;
    private float distance;
    private Vector3 direction; //points at next node

    [HideInInspector] public bool PlayerInRail;
    private float interpolationPercent = 0; //t

    private Transform playerTransform;

    /// <summary>
    /// hooks player to rail. works if player was already on a different rail.
    /// </summary>
    /// <param name="t"></param>
    public void EnterRail(float t)
    {
        if (LastRail != null)
            LastRail.ExitRail();

        InputManager.Instance.currentRailNode = this;
        PlayerInRail = true;
        interpolationPercent = t;
    }

    public void ExitRail()
    {
        interpolationPercent = -1;
        PlayerInRail = false;
    }

    private void InterpolatePlayerPosition()
    {
 
        Vector3 playerInputDirection = InputManager.Instance.movement; //DOTPRODUCT GOES HERE ISH
        float playerDirection = Vector3.Dot(direction, playerInputDirection);

        interpolationPercent += playerDirection * Time.deltaTime;

        //enter next rail
        if (interpolationPercent > 1 && NextRail != null)
        {
            NextRail.EnterRail(0);
            ExitRail();
            return;
        }

        //enter last rail
        if(interpolationPercent < 0 && LastRail != null)
        {
            LastRail.EnterRail(1);
            ExitRail();
            return;
        }

        Vector3 playerPosition = Vector3.Lerp(transform.position, NextRail.transform.position, interpolationPercent);
        playerTransform.position = playerPosition;
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

        distance = Vector3.Distance(transform.position, LastRail.transform.position);
        direction = (LastRail.transform.position - NextRail.transform.position).normalized;
    }

    private void SearchForPlayer()
    {
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

    private void Start()
    {
        if (NextRail != null)
        {
            NextRail.LastRail = this;
        }

        lineRenderer = GetComponent<LineRenderer>();
        playerTransform = InputManager.Instance.transform;
    }

    private void Update()
    {
        if(PlayerInRail)
        {
            InterpolatePlayerPosition();
            return;
        }
        SearchForPlayer();
    }
}

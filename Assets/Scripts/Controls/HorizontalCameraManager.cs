/*******************************************************************************
 * File Name :         HorizontalCameraManager.cs
 * Author(s) :         Toby
 * Creation Date :     2/26/2024
 *
 * Brief Description : The part of the camera that moves around
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HorizontalCameraManager : MonoBehaviour
{
    
    public static HorizontalCameraManager Instance;

    public float Sensitivity = 0.5f;
    public float ControllerSensitivity = 15;

    public bool RotateAutomatically;
    public bool FullControl = false;

    public Transform Pivot;
    public Transform Point;
    private Transform player;

    private InputAction cameraMovement;

    public void Start()
    {
        cameraMovement = InputManager.Instance.cameraMovement;
        player = InputManager.Instance.transform;

        Point.position = transform.position;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Vector3 pivotRotation = Pivot.eulerAngles;
        float sensitivityTarget = Gamepad.current != null ? ControllerSensitivity : Sensitivity;
        pivotRotation.y += cameraMovement.ReadValue<Vector2>().x * Time.deltaTime * sensitivityTarget;

        if (CameraManager.Instance.FullPlayerControl) 
            pivotRotation.x -= cameraMovement.ReadValue<Vector2>().y * Time.deltaTime * sensitivityTarget;

        Pivot.eulerAngles = pivotRotation;
        //up and down rotato
        //Vector3 pointRotation = Point.eulerAngles;
        //pointRotation.x += cameraMovement.ReadValue<Vector2>().y * Time.fixedDeltaTime * Sensitivity;
        //Point.eulerAngles = pointRotation;
    }

    public Vector3 GetHorizontalRotation(Vector3 currentTarget)
    {
        if (CameraManager.Instance.FullPlayerControl)
        {
            return Point.eulerAngles;
        }

        Vector3 target = Point.eulerAngles;
        target.x = currentTarget.x;

        return target;
    }

    public Vector3 GetTargetPosition()
    {
        return Point.transform.position;
    }

    //private Vector3 

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

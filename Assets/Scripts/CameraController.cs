using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    new private Camera camera;
    public Vector3 localPos = new Vector3(0, 10, 0);
    public int thickness = 5;
    public float panSpeed = 10f;

    public enum CameraFSM
    {
        clickOrDrag,
        clickSelect,
        clickDeselect
    }

    public CameraFSM cameraFSM;
    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove == false)
            return;
        CameraPositionUpdate();
        CameraStateMachine();
    }

    private void CameraStateMachine()
    {
        switch (cameraFSM)
        {
            case CameraFSM.clickOrDrag:
                ClickOrDrag();
                break;
            case CameraFSM.clickSelect:
                ClickSelect();
                break;
            case CameraFSM.clickDeselect:
                ClickDeselect();
                break;
            default:
                break;
        }
    }

    private void ClickDeselect()
    {

    }

    private void ClickSelect()
    {

    }

    private void ClickOrDrag()
    {

    }

    private void CameraPositionUpdate()
    {
        Vector3 cameraLocalPos = localPos, pos = transform.position;
        camera.transform.localPosition = cameraLocalPos;
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - thickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= thickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - thickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= thickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        transform.position = pos;
    }
}

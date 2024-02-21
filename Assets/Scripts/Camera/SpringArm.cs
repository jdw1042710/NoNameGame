using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour
{
    [SerializeField] private float length = 0f;
    [SerializeField] private float cameraSpeed = 10000;
    [Space]
    [SerializeField] private bool isFollowParentRotation = true;

    private InputManager inputManager;

    private Camera target;
    private Quaternion offsetRotation = Quaternion.identity;


    private void Start()
    {
        inputManager = InputManager.Instance;

        target = GetComponentInChildren<Camera>();

        offsetRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        SetCameraPosition();
        SetSpringArmRotation();

    }

    private void SetCameraPosition()
    {
        Vector3 direction = -transform.forward;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, length))
        {
            target.transform.position = hitInfo.point;
        }
        else
        {
            target.transform.localPosition = new Vector3(0, 0, -length);
        }
    }

    private void SetSpringArmRotation()
    {
        transform.rotation = offsetRotation;

        var mouseInputVector = new Vector3(-inputManager.mouseVertical, inputManager.mouseHorizontal, 0) * cameraSpeed * Time.deltaTime;
        var newLocalEulerAngles = transform.localEulerAngles + mouseInputVector;
        transform.localRotation = Quaternion.Euler(newLocalEulerAngles);

        offsetRotation = transform.rotation;
    }
}

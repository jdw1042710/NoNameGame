using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    private InputManager inputManager;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private Camera _camera;

    public Vector3 direction;

    private float movementSpeed = 50f;
    private float rotationSpeed = 720f;

    private readonly int isMovingId = Animator.StringToHash("isMoving");
    private readonly int isRunningId = Animator.StringToHash("isRunning");


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();

    }

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        var cameraRotation = Quaternion.Euler(Vector3.up * _camera.transform.eulerAngles.y);
        Vector3 movement = cameraRotation * (new Vector3(inputManager.horizontal, 0 , inputManager.vertical) * movementSpeed * Time.deltaTime);
        Quaternion dest = transform.rotation;

        bool isMoving = movement != Vector3.zero;
        if (isMoving)
        {
            direction = movement.normalized;
            dest = Quaternion.LookRotation(direction, transform.up);
        }
        
        //_rigidbody.MovePosition(transform.position + movement);
        _animator.SetBool(isMovingId, isMoving);

        bool isRunning = inputManager.isHoldingShift;
        _animator.SetBool(isRunningId, isRunning);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, dest, rotationSpeed * Time.deltaTime);
    }
}

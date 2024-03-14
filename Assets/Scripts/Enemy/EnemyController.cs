using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    private NavMeshAgent _agent;
    private EnemyAnimationController _animationController;
    private Transform _player;
    [SerializeField] private LayerMask _groundMask, _playerMask;

    public EnemyState currentState { get; private set; }
    public bool isMoving {  get; private set; }
    public float angleWithPlayer { get; private set; }

    //Patroling
    private Vector3 _patrolPoint;
    private bool _isSetPatrolPoint;
    [SerializeField] private float _patrolPointRange;

    //Chasing
    private Vector3 _lastPlayerPosition;
    private bool _isChasing;
    private float _missingTime;

    //Attacking
    [SerializeField] private float _timeBetweenAttacks;
    private bool _isAlreadyAttacked;

    //States
    [SerializeField] private float _sightRange;
    [SerializeField] private float _attackRange;
    private bool _isPlayerInSightRange;
    private bool _isPlayerInAttackRange;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animationController = GetComponent<EnemyAnimationController>();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        CheckCondition();
        SwitchState();

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Missing:
                FindPlayerInLastPosition();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private void CheckCondition()
    {
        Vector3 playerDir = (_player.position - transform.position);
        angleWithPlayer = Vector3.Dot(transform.forward, playerDir.normalized);
        bool isPlayerInViewingAngle = angleWithPlayer > 0.7f;
        _isPlayerInSightRange = Physics.CheckSphere(transform.position, _sightRange, _playerMask)&& isPlayerInViewingAngle;
        _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _playerMask)&& isPlayerInViewingAngle;

        isMoving = _agent.velocity.sqrMagnitude > 0.1f;

        //for debugging
        Debug.DrawLine(transform.position, transform.position + transform.forward * _sightRange, Color.red);
        Debug.DrawLine(transform.position, transform.position + playerDir, Color.magenta);
    }

    private void SwitchState()
    {
        var _previousState = currentState;
        if (!_isPlayerInSightRange && ! _isPlayerInAttackRange)
        {
            currentState = EnemyState.Patrol;
        }

        if (!_isPlayerInSightRange && _isChasing)
        {
            currentState = EnemyState.Missing;
        }

        if(_isPlayerInSightRange && !_isPlayerInAttackRange)
        {
            currentState = EnemyState.Chase;
            _isChasing = true;
            _missingTime = 0;
        }

        if(_isPlayerInAttackRange)
        {
            currentState = EnemyState.Attack;
        }

        if(_previousState != currentState)
        {
            Debug.Log($"{currentState}");
        }
    }

    private void Patrol()
    {
        if (!_isSetPatrolPoint) 
        { 
            SearchPatrolPoint(); 
        }
        else
        {
            _agent.SetDestination(_patrolPoint);

            Vector3 distanceToPatrolPoint = transform.position - _patrolPoint;

            // patrol point에 도착한 경우 새로운 patrol point를 지정해야함
            if (distanceToPatrolPoint.sqrMagnitude < 1f)
            {
                _isSetPatrolPoint = false;
            }
        }
    }

    private void SearchPatrolPoint()
    {
        float randomX = Random.Range(-_patrolPointRange, _patrolPointRange);
        float randomZ = Random.Range(-_patrolPointRange, _patrolPointRange);

        _patrolPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        _isSetPatrolPoint = Physics.Raycast(_patrolPoint, -transform.up, 2f, _groundMask);

        Debug.Log($"{_patrolPoint}");
    }

    private void Chase()
    {
        _lastPlayerPosition = _player.position;
        _agent.SetDestination(_lastPlayerPosition);
    }

    private void FindPlayerInLastPosition()
    {
        bool isArrivedMissingPoint = (transform.position - _lastPlayerPosition).sqrMagnitude < 1;
        // bool isArrivedMissingPoint = _agent.velocity.magnitude < 1;
        if (isArrivedMissingPoint)
        {
            _missingTime += Time.deltaTime;
            if (_missingTime > 5.0f)
            {
                _isChasing = false;
                _missingTime = 0;
            }
        }
    }

    private void Attack()
    {
        _agent.ResetPath();
        transform.LookAt(_player);

        if (!_isAlreadyAttacked)
        {
            //AttackAnimation
            _animationController.PlayAttackAnimation();
            //Debug.Log("Swipe!");
            StartCoroutine(CoolDownCoroutine());
        }
    }

    IEnumerator CoolDownCoroutine()
    {
        _isAlreadyAttacked = true;
        yield return new WaitForSeconds(_timeBetweenAttacks);
        _isAlreadyAttacked = false;
    }
}

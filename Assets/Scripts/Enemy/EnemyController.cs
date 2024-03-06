using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Missing,
        Attack,
    }


    private NavMeshAgent _agent;
    private Transform _player;
    [SerializeField] private LayerMask _groundMask, _playerMask;

    EnemyState _currentState;

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
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        CheckCondition();
        SwitchState();

        switch (_currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Missing:
                MissThePlayer();
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
        float angleWithPlayer = Vector3.Dot(transform.forward, playerDir.normalized);
        bool isPlayerInViewingAngle = angleWithPlayer > 0.7f;
        _isPlayerInSightRange = Physics.CheckSphere(transform.position, _sightRange, _playerMask)&& isPlayerInViewingAngle;
        _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _playerMask)&& isPlayerInViewingAngle;

        Debug.DrawLine(transform.position, transform.position + transform.forward * _sightRange, Color.red);
        Debug.DrawLine(transform.position, transform.position + playerDir, Color.magenta);
    }

    private void SwitchState()
    {
        var _previousState = _currentState;
        if (!_isPlayerInSightRange && ! _isPlayerInAttackRange)
        {
            _currentState = EnemyState.Patrol;
        }

        if (!_isPlayerInSightRange && _isChasing)
        {
            _currentState = EnemyState.Missing;
        }

        if(_isPlayerInSightRange && !_isPlayerInAttackRange)
        {
            _currentState = EnemyState.Chase;
            _isChasing = true;
            _missingTime = 0;
        }

        if(_isPlayerInAttackRange)
        {
            _currentState = EnemyState.Attack;
        }

        if(_previousState != _currentState)
        {
            Debug.Log($"{_currentState}");
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
            if (distanceToPatrolPoint.magnitude < 1f)
            {
                _isSetPatrolPoint = true;
            }
        }
    }

    private void SearchPatrolPoint()
    {
        float randomX = Random.Range(-_patrolPointRange, _patrolPointRange);
        float randomZ = Random.Range(-_patrolPointRange, _patrolPointRange);

        _patrolPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        _isSetPatrolPoint = Physics.Raycast(_patrolPoint, -transform.up, 2f, _groundMask);
    }

    private void Chase()
    {
        _lastPlayerPosition = _player.position;
        _agent.SetDestination(_lastPlayerPosition);
    }

    private void MissThePlayer()
    {
        if((transform.position - _lastPlayerPosition).magnitude > 1) return;
        _missingTime += Time.deltaTime;
        if( _missingTime > 5.0f )
        {
            _isChasing = false;
            _missingTime = 0;
        }
    }

    private void Attack()
    {
        _agent.ResetPath();
        transform.LookAt(_player);

        if (!_isAlreadyAttacked)
        {
            //AttackAnimation
            Debug.Log("Swipe!");
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

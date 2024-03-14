using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private EnemyController _enemyController;
    private Animator _animator;

    private EnemyState _previousState;

    private void Awake()
    {
        _enemyController = GetComponent<EnemyController>();
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        UpdateAnimationState();
        
    }

    private void UpdateAnimationState()
    {
        _animator.SetBool("isMoving", _enemyController.isMoving);
        if (_previousState != _enemyController.currentState)
        {
            switch (_enemyController.currentState)
            {
                case EnemyState.Idle:
                case EnemyState.Attack:
                case EnemyState.Chase:
                case EnemyState.Patrol:
                case EnemyState.Missing:
                    break;
            }
        }
        _previousState = _enemyController.currentState;
    }

    public void PlayAttackAnimation()
    {
        _animator.SetTrigger("attack");
    }
}

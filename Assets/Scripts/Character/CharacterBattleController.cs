using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattleController : MonoBehaviour
{
    private InputManager inputManager;

    private Animator _animator;

    private readonly int attackId = Animator.StringToHash("attack");
    private readonly int attackComboId = Animator.StringToHash("attackCombo");
    private readonly int isAttackingId = Animator.StringToHash("isAttacking");

    private bool isAttacking;
    private readonly int maxCombo = 3;
    private int attackCombo = 0;
    private bool nextComboFlag = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    private void Update()
    {
        if (inputManager.isMouseLeftClicked)
        {
            if (!isAttacking)
            {
                StartAttack();
            }
            else if(attackCombo < maxCombo)
            {
                nextComboFlag = true;
            }
        }
    }

    private void Attack()
    {
        attackCombo++;
        _animator.SetTrigger(attackId);
        _animator.SetInteger(attackComboId, attackCombo);
    }

    private void StartAttack()
    {
        isAttacking = true;
        Attack();
    }


    // Called By Animation Event
    private void OnCheckAttackCombo()
    {
        //Debug.Log("Check Combo");
        if (nextComboFlag)
        {
            nextComboFlag = false;
            Attack();
        }
        else
        {
            attackCombo = 0;
            _animator.SetInteger(attackComboId, attackCombo);
        }
    }

    // Called By Animation Event

    private void FinishAttack()
    {
        isAttacking = false;
    }

    // Called By Animation Event

    private void ResetAttackState()
    {
        isAttacking = false;
        attackCombo = 0;
        nextComboFlag = false;
    }
}

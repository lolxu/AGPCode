using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class MoveSubState : EnemyBaseState
{
    public MoveSubState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }

    private float TimeBetweenAttack = 0.0f;
    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        Ctx.HandleAlertMove();
        TimeBetweenAttack -= Time.deltaTime;
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (TimeBetweenAttack <= 0.0f)
        {
            if (Ctx.WithinAttackDistance() && !Ctx.playerStateMachine.IsDead)
            {
                SwitchState(Factory.Attack());
                TimeBetweenAttack = Ctx.TimeBetweenAttack;
            }
        }
    }

    public override void InitializeSubState()
    {
        
    }

    public override string StateName()
    {
        return "Move";
    }
}

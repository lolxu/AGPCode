using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.Enemies;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public EnemyDeadState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.IsDead = true;
        Ctx.DashTargetPoint.isValidTarget = false;
        
        Ctx.SetIdleSymbol();
        Ctx.HandleEnterDeath();
        InitializeSubState();

        if (Ctx.EnemyType != "CircularShooter")
        {
            EnemyManager.Instance.AddEliminatedEnemy(Ctx.gameObject);
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        Ctx.HandleDeath();
    }

    public override void ExitState()
    {
        Ctx.IsDead = false;
    }

    public override void CheckSwitchStates()
    {
        
    }

    public override void InitializeSubState()
    {
    }

    public override string StateName()
    {
        return "Dead";
    }
}

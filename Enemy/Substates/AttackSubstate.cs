using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;

public class AttackSubstate : EnemyBaseState
{
    public AttackSubstate(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }
    
    public override void EnterState()
    {
        InitializeSubState();
        Ctx.HandleEnterAttack();
        Ctx.currAttackTime = Ctx.attackTime;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        Ctx.HandleAttack();
        Ctx.currAttackTime -= Time.deltaTime;
    }

    public override void ExitState()
    {
        Ctx.HandleExitAttack();
    }

    public override void CheckSwitchStates()
    {
        //swtich states if the attack is over
        if (Ctx.currAttackTime <= 0f)
        {
            SwitchState(Factory.Move());
        }
    }

    public override void InitializeSubState()
    {
        
    }

    public override string StateName()
    {
        return "Attack";
    }
}

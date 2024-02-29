using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    private bool Warning = false;
    public override void EnterState()
    {
        InitializeSubState();
        Ctx.HandleEnterIdle();
    }

    public override void UpdateState()
    {
        Ctx.HandleIdle();
        if (!Warning)//Add warning sign
        {
            if (Ctx.CurrTimeForWarningAndDisengagingAlert > 0f)
            {
                //TODO: add warning signal above player head
                Ctx.SetSuspiciousSymbol();
                Warning = true;
            }
        }
        if (Ctx.hasLOS())
        {
            Ctx.CurrTimeForWarningAndDisengagingAlert += Time.deltaTime;
            Ctx.SetSuspiciousLOSSymbol();
            //we see the player and it raises suspicion
            Ctx.HandleIdleLOSMove();
            CheckSwitchStates();
        }else if (Warning)
        {
            Ctx.CurrTimeForWarningAndDisengagingAlert -= Time.deltaTime;
            if (Ctx.CurrTimeForWarningAndDisengagingAlert <= 0)
            {
                Warning = false;
                Ctx.CurrTimeForWarningAndDisengagingAlert = 0f;
                //we are no longer suspicious of the player being close
                Ctx.HandleIdleMove();
                Ctx.SetIdleSymbol();
            }
            else
            {
                //We recently saw the player
                Ctx.HandleIdleSuspiciousMove();
                Ctx.SetSuspiciousSymbol();
            }
        }
        else
        {
            //We are not suspicious of the player being close
            Ctx.HandleIdleMove();
            Ctx.SetIdleSymbol();
        }
    }

    public override void ExitState()
    {
        Warning = false;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.CurrTimeForWarningAndDisengagingAlert > 0.0f)
        {
            if (Ctx.CurrTimeForWarningAndDisengagingAlert > Ctx.TimeForWarningAndDisengagingAlert)
            {
                Ctx.CurrTimeForWarningAndDisengagingAlert = Ctx.TimeForWarningAndDisengagingAlert;
                SwitchState(Factory.Alert());
            }
            else if (Ctx.LOSAndAlertEnactSameBehavior)
            {
                Ctx.CurrTimeForWarningAndDisengagingAlert = Ctx.TimeForWarningAndDisengagingAlert;
                SwitchState(Factory.Alert());
            }
        }
    }

    public override void InitializeSubState()
    {
    }

    public override string StateName()
    {
        return "Idle";
    }
}

using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

public class AlertState : EnemyBaseState
{
            public AlertState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
        }
            
        public override void EnterState()
        {
            Ctx.currAttackTime = 0.0f;
            Ctx.SetAlertSymbol();
            InitializeSubState();
            //TODO: add alert sign above head
            Ctx.HandleEnterAlert();
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
            Ctx.HandleAlert();
        }

        public override void ExitState()
        {
            Ctx.DisableAlertSymbolBackground();
            Ctx.HandleExitAlert();
        }

        public override void CheckSwitchStates()
        {
            if (!Ctx.hasLOS() && Ctx.currAttackTime <= 0.0f)
            {
                SwitchState(Factory.Idle());
            }
        }

        public override void InitializeSubState()
        {
            if (Ctx.WithinAttackDistance())
            {
                SetSubStateSilent(Factory.Attack());
            }
            else
            {
                SetSubStateSilent(Factory.Move());
            }
        }

        public override string StateName()
        {
            return "Alert";
        }
}

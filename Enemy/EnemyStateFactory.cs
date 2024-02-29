using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace __OasisBlitz.Enemy.StateMachine
{
    public class EnemyStateFactory : MonoBehaviour
    {
        public enum EnemyStates
        {
            Idle,
            Alert,
            Dead,
            Attack,
            Move
        }
        
        EnemyStateMachine context;
        Dictionary<EnemyStates, EnemyBaseState> states = new Dictionary<EnemyStates, EnemyBaseState>();

        public EnemyStateFactory(EnemyStateMachine currentContext, List<EnemyStates> StatesToCreate)
        {
            context = currentContext;
            for (int i = 0; i < StatesToCreate.Count; i++)
            {
                switch (StatesToCreate[i])
                {
                    case EnemyStates.Idle:
                        states[EnemyStates.Idle] = new EnemyIdleState(context, this);
                        break;
                    case EnemyStates.Alert:
                        states[EnemyStates.Alert] = new AlertState(context, this);
                        break;
                    case EnemyStates.Dead:
                        states[EnemyStates.Dead] = new EnemyDeadState(context, this);
                        break;
                    case EnemyStates.Attack:
                        states[EnemyStates.Attack] = new AttackSubstate(context, this);
                        break;
                    case EnemyStates.Move:
                        states[EnemyStates.Move] = new MoveSubState(context, this);
                        break;
                }
            }
        }

        public EnemyBaseState Idle()
        {
            return states[EnemyStates.Idle];
        }
        public EnemyBaseState Alert()
        {
            return states[EnemyStates.Alert];
        }
        public EnemyBaseState Dead()
        {
            return states[EnemyStates.Dead];
        }
        public EnemyBaseState Attack()
        {
            return states[EnemyStates.Attack];
        }
        public EnemyBaseState Move()
        {
            return states[EnemyStates.Move];
        }
    }
}

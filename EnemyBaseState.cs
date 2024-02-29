using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using UnityEngine;

namespace __OasisBlitz.Enemy.StateMachine
{
    public abstract class EnemyBaseState
    {
        private bool isRootState = false;

        // A flag that is set when the state switches, so that it does not update its sub states after they exit
        private bool switchedThisUpdate = false;

        private EnemyStateMachine ctx;
        private EnemyStateFactory factory;
        private EnemyBaseState currentSubState;
        private EnemyBaseState currentSuperState;

        protected bool IsRootState
        {
            set { isRootState = value; }
        }

        protected EnemyStateMachine Ctx
        {
            get { return ctx; }
        }

        protected EnemyStateFactory Factory
        {
            get { return factory; }
        }

        public EnemyBaseState(EnemyStateMachine currentContext, EnemyStateFactory playerStateFactory)
        {
            ctx = currentContext;
            factory = playerStateFactory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void CheckSwitchStates();
        public abstract void InitializeSubState();
        public abstract string StateName();

        protected void SwitchState(EnemyBaseState newState)
        {
#if DEBUG
            if (Constants.DebugRootStateChanges)
            {
                if (newState.isRootState)
                {
                    Debug.Log("Switching from " + StateName() + " to " + newState.StateName());
                }
            }

            if (Constants.DebugSubstateChanges)
            {
                if (!newState.isRootState)
                {
                    Debug.Log("Switching from " + StateName() + " to " + newState.StateName());
                }
            }
#endif
            // current state exits state
            ExitStates();

            // new state enters state
            newState.EnterState();

            if (isRootState)
            {
                // switch current state of context
                ctx.CurrentState = newState;
            }
            else if (currentSuperState != null)
            {
                // set the current super states sub state to the new state
                currentSuperState.SetSubStateSilent(newState);
            }

            switchedThisUpdate = true;
        }

        public void UpdateStates()
        {
            UpdateState();

            // Ctx.DebugText.SetText("Current State: " + StateName() + "\nSubstate: " + _currentSubState?.StateName());
            // Debug.Log(StateName() + " is updating");
            // Don't update sub states if the state switched this update
            if (currentSubState != null && !switchedThisUpdate)
            {
                currentSubState.UpdateStates();
            }

            switchedThisUpdate = false;
        }

        public void ExitStates()
        {
            ExitState();

            if (currentSubState != null)
            {
                currentSubState.ExitState();
            }
        }

        protected void SetSuperState(EnemyBaseState newSuperState)
        {
            currentSuperState = newSuperState;
        }

        protected void SetSubStateSilent(EnemyBaseState newSubState)
        {
#if UNITY_EDITOR
            if (Constants.DebugSubstateChanges)
            {
                Debug.Log("SubstateSet: " + newSubState.StateName());
            }
#endif
            currentSubState = newSubState;
            newSubState.SetSuperState(this);
        }

        protected void SetSubStateInit(EnemyBaseState subState)
        {
            SetSubStateSilent(subState);
            subState.EnterState();
        }

        /// <summary>
        /// Whether or not the player should snap to ground when they land, for use by the CharacterController
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldObeyGroundSnap()
        {
            return true;
        }


        // ********************************** COLLISION HANDLING **********************************
        public void HandleCollision(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            // // Here, we allow the state machine to handle the hit
             if (coll.CompareTag("BounceObject"))
             {
                 ImpactBouncePad(ref coll, hitNormal, hitPoint);
             }
             else if (coll.CompareTag("DeathBarrier"))
             {
                 if (!Ctx.IsDead)
                 {
                     Ctx.InstantKill();
                 }
             }else if (coll.CompareTag("BounceObject"))
             {
                 ImpactBouncePad(ref coll, hitNormal, hitPoint);
             }else if (coll.CompareTag("Player"))
             {
                 Debug.Log("Player Hit by enemy");
             }
        }

        protected virtual void ImpactBouncePad(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            Bounce.Instance.BounceRigid(ref ctx.EnemyRigidBody, hitNormal, Bounce.BounceTypeNormal.Medium, Bounce.BounceTypeReflective.Medium);
        }
    }
}

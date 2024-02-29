using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.__Scripts.Player.Environment.FragileSand;
using __OasisBlitz.__Scripts.Player.Environment.Fruits;
using __OasisBlitz.Enemy;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Utility;
using UnityEngine;

namespace __OasisBlitz.Player.StateMachine
{
    public abstract class BaseState
    {
        private bool isRootState = false;
        
        // A flag that is set when the state switches, so that it does not update its sub states after they exit
        private bool switchedThisUpdate = false;
        
        private PlayerStateMachine ctx;
        private PlayerStateFactory factory;
        private BaseState currentSubState;
        private BaseState currentSuperState;

        protected bool IsRootState
        {
            set { isRootState = value; }
        }

        protected PlayerStates CurrentSuperStateEnum
        {
            get { return currentSuperState.StateEnum; }
        }

        protected PlayerStateMachine Ctx
        {
            get { return ctx; }
        }

        protected PlayerStateFactory Factory
        {
            get { return factory; }
        }

        public BaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
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

        public PlayerStates StateEnum;
        protected void SwitchState(BaseState newState)
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
            
            // new state enters state
            newState.EnterState();

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

            // Check for Elixir Replenish Fruit Ability here
            // if (Ctx.InteractKeyRequested && !Ctx.RequireNewInteractKeyPress 
            //                              && FruitsManager.Instance.CurrentFruit != null)
            // {
            //     // if (FruitsManager.Instance.CurrentFruit.GetFruitName() == "ElixirReplenish")
            //     // {
            //     //     FruitsManager.Instance.ReplenishElixir();
            //     //     Ctx.RequireNewInteractKeyPress = true;
            //     // }
            //     
            // }
            
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

        protected void SetSuperState(BaseState newSuperState)
        {
            currentSuperState = newSuperState;
        }

        protected void SetSubStateSilent(BaseState newSubState)
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

        protected void SetSubStateInit(BaseState subState)
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

        public void HandleGroundHit(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        { 
            // Here, we allow the state machine to handle ground hit (called every frame we are grounded)
            /****** Only add objects to this that fall victim to the walking bug that prevents collision
                    Example: walking onto spiky sand from the same elevation WILL NOT kill the player because
                    it is not registered in OnMovementHit.  Therefore, we MUST add it to this to get collision working properly
                    Example:  DO NOT add any drill specific impact because that impact will ALWAYS be called from OnMovementHit
                    because we are always moving toward colliders in drill mode
                    NOTE: THIS IS NOT RESPONSIBLE FOR KEEPING THE CHARACTER GROUNDED. IT JUST TELLS US THE OBJECT BELOW US WHEN GROUNDED
            *******/
            Ctx.OnSlipperySurface = false;
            if (coll.gameObject.layer == 6 || coll.gameObject.layer == 7)
            {
                Ctx.PlayerPhysics.CurrentOnSurfaceType = PlayerPhysics.OnSurfaceType.Penetrable;
            }
            else
            {
                Ctx.PlayerPhysics.CurrentOnSurfaceType = PlayerPhysics.OnSurfaceType.NotPenetrable;
            }
            
            // Debug.Log(coll.tag);
            switch (coll.tag)
            {
                case "TouchThenFall":
                    ImpactWalkOnly(ref coll, hitNormal, hitPoint);
                    ImpactTouchThenFall(ref coll, hitNormal, hitPoint);
                    break;
                case "DrillOnly":
                    ImpactDrillOnly(ref coll, hitNormal, hitPoint);
                    break;
                case "WalkOnlyKill":
                    ImpactWalkOnlyKill(ref coll, hitNormal, hitPoint);
                    break;
                case "WalkOnly":
                    ImpactWalkOnly(ref coll, hitNormal, hitPoint);
                    break;
                case "BounceObject":
                    ImpactBouncePadNoStick(ref coll, hitNormal, hitPoint);
                    break;
                case "FragileSand":
                    ImpactFragileSand(ref coll, hitNormal, hitPoint);
                    break;
                case "DeathBarrier":
                    Ctx.InstantKill();
                    break;
                case "InstantKill":
                    Ctx.InstantKill();
                    break;
                case "Slide":
                    ImpactSlide(ref coll, hitNormal, hitPoint);
                    break;
                case "SlideWalkOnly":
                    ImpactSlide(ref coll, hitNormal, hitPoint);
                    break;
                case "SlideDanger":
                    ImpactSlideDanger(ref coll, hitNormal, hitPoint);
                    break;
                case "SlideDangerWalkOnly":
                    ImpactSlideDanger(ref coll, hitNormal, hitPoint);
                    break;
                default:
                    break;
            }
        }
        public void HandleCollision(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            // Here, we allow the state machine to handle the hit
            switch (coll.tag)
            {
                case "TouchThenFall":
                    ImpactWalkOnly(ref coll, hitNormal, hitPoint);
                    ImpactTouchThenFall(ref coll, hitNormal, hitPoint);
                    break;
                case "DrillOnly":
                    ImpactDrillOnly(ref coll, hitNormal, hitPoint);
                    break;
                case "WalkOnly":
                    ImpactWalkOnly(ref coll, hitNormal, hitPoint);
                    break;
                case "WalkOnlyKill":
                    ImpactWalkOnlyKill(ref coll, hitNormal, hitPoint);
                    break;
                case "BounceObject":
                    ImpactBouncePadNoStick(ref coll, hitNormal, hitPoint);
                    break;
                case "Enemy":
                    ImpactEnemy(coll.gameObject.transform.parent.GetComponent<BasicEnemyController>());
                    break;
                case "NewEnemy":
                    ImpactNewEnemy(ref coll, hitNormal, hitPoint);
                    break;
                case "BounceKnightShield":
#if DEBUG
                    if (Constants.DebugBounceKnightShield)
                    {
                        Debug.Log("localHitNorm: " + coll.transform.InverseTransformDirection(hitNormal) +
                                  " coll.transform.forward: " +
                                  coll.transform.InverseTransformDirection(coll.transform.forward));
                    }
#endif
                    //check if you hit the front of the bounce pad
                    if (coll.transform.InverseTransformDirection(hitNormal).z >= 0.80f)
                    {
                        //we hit the front
                        ImpactBouncePad(ref coll, hitNormal, hitPoint);
                        //attack if the bounce knight is still alive
                        ShieldCollision shieldCollision = coll.GetComponent<ShieldCollision>();
                        if (!shieldCollision.IsDetached)
                        {
                            Debug.Log("ForceAttack");
                            shieldCollision.machine.ForceAttack();
                        }
                    }
                    else
                    {
                        //we did not hit the front
                        ImpactWalkOnly(ref coll, hitNormal, hitPoint);
                    }
                    break;
                case "FragileSand":
                    ImpactFragileSand(ref coll, hitNormal, hitPoint);
                    break;
                case "Breakable":
                    ImpactBreakables(ref coll, hitNormal, hitPoint);
                    break;
                case "DeathBarrier":
                    Ctx.InstantKill();
                    break;
                case "InstantKill":
                    ImpactInstantKill();
                    break;
                case "SlideWalkOnly":
                    ImpactWalkOnly(ref coll, hitNormal, hitPoint);
                    break;
                case "SlideDangerWalkOnly":
                    ImpactWalkOnly(ref coll, hitNormal, hitPoint);
                    ImpactSlideDanger(ref coll, hitNormal, hitPoint);
                    break;
                // case "Collectable":
                //     ImpactCollectable(ref coll);
                //     break;
                default:
                    // The default behavior for a collision is to apply Newton's third
                    Ctx.PlayerPhysics.HandleContact(hitNormal);
                    break;
            }
        }

        protected virtual void ImpactEnemy(BasicEnemyController enemyController)
        {
        }
        
        protected virtual void ImpactNewEnemy(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            if (!Ctx.IsDead)
            {
                Ctx.CurrentState.SwitchState(Factory.Dead());
            }
        }
        
        protected virtual void ImpactWalkOnly(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            Ctx.PlayerPhysics.HandleContact(hitNormal);
        }
        
        protected virtual void ImpactWalkOnlyKill(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            Ctx.CurrentState.SwitchState(Factory.Dead());
        }

        protected virtual void ImpactDrillOnly(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            Ctx.PlayerPhysics.HandleContact(hitNormal);
        }

        protected virtual void ImpactBouncePad(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            Bounce.Instance.BounceCollider(ref coll, hitNormal, hitPoint, ref Ctx.PlayerPhysics, Bounce.BounceTypeNormal.Large, Bounce.BounceTypeReflective.Large);
        }

        protected virtual void ImpactBouncePadNoStick(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            Bounce.Instance.BounceCollider(ref coll, hitNormal, hitPoint, ref Ctx.PlayerPhysics, Bounce.BounceTypeNormal.Large, Bounce.BounceTypeReflective.Large);
        }
        protected virtual void ImpactFragileSand(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            FragileSandManager.Instance.FragileSandStartShrinkBodyCollide(ref coll, hitNormal, hitPoint);
        }

        protected virtual void ImpactBreakables(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            Ctx.PlayerPhysics.HandleContact(hitNormal);
        }

        protected virtual void ImpactInstantKill()
        {
            Ctx.InstantKill();
        }

        // Code for respawning character and teleporting and rotating
        // key = pos, value = dir?
        public void RespawnCharacter(KeyValuePair<Vector3, Vector3> spawnPos)
        {
            Ctx.PlayerPhysics.SetVelocity(Vector3.zero);
            
            Debug.Log($"Respawning Character at location{spawnPos.Key}");
            Ctx.CharacterController.SetPosition(spawnPos.Key - spawnPos.Value * 2.0f);
            Debug.Log(spawnPos.Value);
            Ctx.ModelRotator.SetFullDirection(spawnPos.Value);
            // Debug.LogError(Ctx.gameObject.transform.position);
            Vector3 camPosRestLocation = Ctx.gameObject.transform.position - (spawnPos.Key - Ctx.gameObject.transform.position).normalized * 15.0f;
            Ctx.cameraStateMachine.CurrentState.ResetCameraPosition();
            // Ctx.cameraStateMachine.CurrentState.ForceSetCameraPosition(camPosRestLocation);
            Quaternion q = Quaternion.FromToRotation(Vector3.forward, spawnPos.Value);
            
            Ctx.cameraStateMachine.SetHorizontalAxis(q.eulerAngles.y);
            Ctx.cameraStateMachine.ResetVerticalAxis();
        }

        protected virtual void ImpactTouchThenFall(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            DetectHit HitTouchThenFall = coll.GetComponent<DetectHit>();
            if (HitTouchThenFall)
            {
                HitTouchThenFall.WaitThenFall();
            }
        }

        protected virtual void ImpactSlide(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            
        }
        protected virtual void ImpactSlideDanger(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {

        }
        
        // protected virtual void ImpactCollectable(ref Collider collider)
        // {
        //     CollectableObject obj = collider.gameObject.GetComponent<CollectableObject>();
        //     obj.StartInteractSequence();
        // }
    }
}
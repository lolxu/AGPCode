using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.Enemies.Flashing;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.__Scripts.UI;
using __OasisBlitz.Player.Physics;
using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.Player.StateMachine.RootStates
{
    public class DashState : BaseState, IRootState
    {

        public DashState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
            StateEnum = PlayerStates.Dash;
        }

        public override void EnterState()
        {
            InitializeSubState();
            
            // Play SFX
            Ctx.PlayerAudio.PlayDash();

            // Shoot drill to target
            // Ctx.Drill.ShootToTarget(Ctx.TargetedDash.ClosestPointOnTargetSurface());
            Ctx.BanditAnimationController.PlayStartGrapple(Ctx.TargetedDash.currentDashTarget.transform.position);
            
            Ctx.TargetedDash.PerformDash();
            Ctx.PlayerPhysics.ApplyGravity = false;
            Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.Dash;
            Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Dash;
            Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Dash;
            
            Ctx.CharacterController.CollisionMode = CollisionMode.Dashing;
            // Ctx.cameraStateMachine.rigChanger.TweenToRig(Ctx.cameraStateMachine.fullLookAtTargetRig, 0.2f);

        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }
        

        public override void ExitState()
        {
            Ctx.TargetedDash.EndDash();
            // Ctx.BanditAnimationController.PlayGrappleImpact();
            Ctx.PlayerPhysics.ApplyGravity = true;
            Ctx.CharacterController.CollisionMode = CollisionMode.Default;
            Ctx.cameraStateMachine.SetToLookAtVelocity();
        }

        public override void CheckSwitchStates()
        {
            if (Ctx.TargetedDash.currentDashStage == TargetedDash.DashStage.Finished)
            {
                SwitchState(Factory.FreeFall());
            }
        }

        public override void InitializeSubState()
        {
        }

        public override string StateName()
        {
            return "Dash";
        }
        
        protected override void ImpactEnemy(BasicEnemyController enemy)
        {
            Debug.Log("Dash state impact enemy");
            // TODO: Kill the enemy and set a flag for switching states
            // if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            enemy.Kill();

        }

        protected override void ImpactGrapplePoint(DashTargetPoint grapplePoint)
        {
            Ctx.TargetedDash.ImpactGrapplePoint(grapplePoint);
        }

        protected override void ImpactNewEnemy(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            // Enemy Drill Collision (stolen from DrillState.cs)
            HitPartOfNewEnemy hitbox = coll.gameObject.GetComponent<HitPartOfNewEnemy>();
            if (hitbox)
            {
                if (hitbox.GetEnemyStateMachine().EnemyType == "Hardening")
                {
                    HardeningEnemy hardeningEnemyEnemy = hitbox.GetEnemyStateMachine().gameObject.GetComponent<HardeningEnemy>();
                    if (hardeningEnemyEnemy._canBePenetrated)
                    {
                        hitbox.CollideWithBody(ref coll, hitNormal, hitPoint);
                    }
                    else
                    {
                        // Rather than straight up dying, you gets bounced off.
                        // Less punishing but we could try and see whats up
                        hitNormal.y = 0.0f; // For better more game-like reflection
                        // Debug.DrawLine(coll.transform.position, coll.transform.position + hitNormal.normalized * 20.0f, Color.green, 30.0f);
                        Ctx.PlayerPhysics.ReflectVelocity(hitNormal, Ctx.TargetedDash.TargetPosition(), hardeningEnemyEnemy.bounceMagnitude);
                        return;
                    }
                }
                else
                {
                    hitbox.CollideWithBody(ref coll, hitNormal, hitPoint);
                }
            }
            Ctx.PlayerFeedbacks.impactEnemyFeedback.PlayFeedbacks();
            Debug.Log("Hit enemy");
            Ctx.TargetedDash.ImpactGrapplePoint(coll.gameObject.GetComponentInChildren<DashTargetPoint>());
        }
        
        /// /////////
        protected override void ImpactDrillOnly(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactDrillOnly(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
            // // TODO: How should dash behave here?
            // if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            // SwitchState(Factory.Dead());
        }

        protected override void ImpactTouchThenFall(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactTouchThenFall(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
        }

        protected override void ImpactSlide(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactSlide(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
        }
        protected override void ImpactSlideDanger(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactSlideDanger(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
        }
        
        protected override void ImpactWalkOnly(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactWalkOnly(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
        }
        
        protected override void ImpactWalkOnlyKill(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactWalkOnlyKill(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
        }

        protected override void ImpactBouncePad(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactBouncePad(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
        }

        protected override void ImpactBouncePadNoStick(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactBouncePadNoStick(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
        }
        protected override void ImpactFragileSand(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactFragileSand(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
        }

        protected override void ImpactBreakables(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            base.ImpactBreakables(ref coll, hitNormal, hitPoint);
            Ctx.TargetedDash.ImpactOther();
        }

        protected override void ImpactInstantKill()
        {
            base.ImpactInstantKill();
            Ctx.TargetedDash.ImpactOther();
        }
    }
}

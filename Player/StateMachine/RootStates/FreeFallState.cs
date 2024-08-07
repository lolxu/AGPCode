using System.Collections;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment.Fruits;
using __OasisBlitz.Enemy;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Utility;
using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.Player.StateMachine.RootStates
{
    public class FreeFallState : BaseState, IRootState
    {
        private const float Acceleration = 80f;

        private const float CoyoteTime = .15f;
        public bool coyoteTimeEnabled = false;
        private Tween CoyoteTimeTween = null;
        private Tween JumpBufferTween = null;
        private float JumpBufferTime = .05f;

        public FreeFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
            StateEnum = PlayerStates.FreeFall;
        }

        public override void EnterState()
        {
            Ctx.BanditAnimationController.PlayFreeFall();
            
            Ctx.DrillReleased = true;
            Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.FreeFall;
            Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Airborne;
            Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Airborne;

            // start a coyote time coroutine:
            CoyoteTimeTween = DOVirtual.DelayedCall(CoyoteTime,
                () => coyoteTimeEnabled = false,
                false);

            InitializeSubState();
        }

        public override void UpdateState()
        {
            if (Ctx.JumpRequested)
            {
                JumpBufferTween?.Kill();
                ((GroundedState)Factory.Grounded()).jumpBuffered = true;
                JumpBufferTween = DOVirtual.DelayedCall(JumpBufferTime, () =>
                {
                    ((GroundedState)Factory.Grounded()).jumpBuffered = false;
                }, false);
            }
            
            
            if (coyoteTimeEnabled
                && Ctx.JumpRequested 
                && !Ctx.RequireNewJumpPress)
            {
                Ctx.RequireNewJumpPress = true;
                Ctx.CharacterController.RequestJump();
                Vector3 velocity = Ctx.PlayerPhysics.Velocity;
                velocity.y = Ctx.InitialJumpVelocity;
                Ctx.PlayerPhysics.SetVelocity(velocity);
                coyoteTimeEnabled = false;
            }
            else if (BounceAbility.Instance.BounceEnabled
                && Ctx.JumpRequested 
                && !Ctx.RequireNewJumpPress)
            {
                // consume current bounce
                Ctx.RequireNewJumpPress = true;
                BounceAbility.BounceAttemptResult bounceAttemptResult = BounceAbility.Instance.AirBounce();
                
                if (bounceAttemptResult.DidBounce)
                {
                    // Begin the blast sequence
                    Ctx.BanditAnimationController.PlayBlastFromFreefall(bounceAttemptResult.BounceVelocity);
                }
            }

            CheckSwitchStates();
        }

        public override void ExitState()
        {
            CoyoteTimeTween?.Kill(true);
            Ctx.DrillReleased = false;
        }

        public override void CheckSwitchStates()
        {
            // If grounded and moving downwards, switch to grounded state
            if (Ctx.CharacterController.IsGrounded)
            {
                JumpBufferTween?.Kill();
                // Play some particles
                if (!Ctx.InWaterTrigger)
                {
                    FeelEnvironmentalManager.Instance.PlayLandFeedback(Ctx.ModelRotator.leftFootTransform.position,
                        0.25f);
                    FeelEnvironmentalManager.Instance.PlayLandFeedback(Ctx.ModelRotator.rightFootTransform.position,
                        0.25f);
                }
                SwitchState(Factory.Grounded());
            }
            else if (Ctx.DrillRequested && !Ctx.DrillLocked && !Ctx.RequireNewDrillPressOrEndGrounded && Ctx.ToggleDrill)
            {
                SwitchState(Factory.Drill());
            }
            // TODO: Uncomment this if we don't want to restrict the dash to drilling
            else if (Ctx.TargetedDashRequested 
                     && Ctx.TargetedDash.CanPerformDash()
                     && !Ctx.RequireNewTargetedDashPress)
            {
                // This comes before the switch because the behavior of exit state on grounded depends upon whether the dash is performed
                Ctx.RequireNewTargetedDashPress = true;
                Ctx.ModelRotator.OnDash(Ctx.TargetedDash.TargetPosition());
                SwitchState(Factory.Dash());
            }
        }

        public override void InitializeSubState()
        {
        }

        public override string StateName()
        {
            return "FreeFall";
        }

        public void HandleGravity()
        {
        }
        
        protected override void ImpactEnemy(BasicEnemyController enemy)
        {
            if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            SwitchState(Factory.Dead());
        }

        protected override void ImpactDrillOnly(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            SwitchState(Factory.Dead());
        }
    }
}
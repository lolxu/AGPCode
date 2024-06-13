using System;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment;
using __OasisBlitz.Enemy;
using __OasisBlitz.Player.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __OasisBlitz.Player.StateMachine.RootStates
{
    public class GroundedState : BaseState, IRootState
    {
        public bool jumpBuffered = false;
        private float idleThreshold = .1f;

        public GroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
            StateEnum = PlayerStates.Grounded;
        }

        public void HandleGravity()
        {
        }

        private void SetGroundedAnimationState()
        {
            BanditAnimations.BanditAnimationTypes animToPlay;
            
            if (!Ctx.IsSliding)
            {
                // surveyorWheel.UpdateWheel(playerPhysics.Velocity, Time.deltaTime);
                // proceduralRun.UpdateProceduralRun(playerPhysics.Velocity.magnitude);
                
                Vector2 curr = new Vector2(Ctx.PlayerPhysics.Velocity.x, Ctx.PlayerPhysics.Velocity.z);
                if (curr.magnitude <= 0.1f)
                {
                    animToPlay = BanditAnimations.BanditAnimationTypes.Idle;
                }
                else
                {
                    animToPlay = BanditAnimations.BanditAnimationTypes.Run;
                }
            }
            else
            {
                animToPlay = BanditAnimations.BanditAnimationTypes.Slide;
            }

            if (Ctx.IsCelebrating)
            {
                animToPlay = BanditAnimations.BanditAnimationTypes.Celebrate;
            }

            if (Ctx.BanditAnimationController.currentAnimationType != animToPlay)
            {
                // grounded animation options
                switch (animToPlay)
                {
                    case BanditAnimations.BanditAnimationTypes.Idle:
                        Ctx.BanditAnimationController.PlayIdle();
                        break;
                    case BanditAnimations.BanditAnimationTypes.Run:
                        Ctx.BanditAnimationController.PlayRun();
                        break;
                    case BanditAnimations.BanditAnimationTypes.Slide:
                        Ctx.BanditAnimationController.PlaySlide();
                        break;
                    case BanditAnimations.BanditAnimationTypes.Celebrate:
                        Ctx.BanditAnimationController.PlayCelebration();
                        break;
                }
            }
        }

        public override void EnterState()
        {
            SetGroundedAnimationState();
            
            Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.Grounded;
            
            InitializeSubState();
            
            BounceAbility.Instance.RefreshBounce();
            HandleGravity();

            Ctx.ModelRotator.SetGrounded(true);
            Ctx.PlayerAudio.PlayLand();

            if (Ctx.InWaterTrigger)
            {
                FeelEnvironmentalManager.Instance.PlayWaterSplashFeedback(Ctx.BanditAnimationController.board.transform.position, 1f);
                Ctx.PlayerAudio.PlaySplashSound();
            }
        }

        public override void UpdateState()
        {
            SetGroundedAnimationState();

            CheckSwitchStates();
        }

        public override void ExitState()
        {
            Ctx.ModelRotator.SetGrounded(false);
            Ctx.OnSlipperySurface = false;
            Ctx.RequireNewDrillPressOrEndGrounded = false;
            Ctx.PlayerPhysics.CurrentOnSurfaceType = PlayerPhysics.OnSurfaceType.NotGrounded;
        }

        private void DefaultJump()
        {
            // Default vertical jump
            Vector3 velocityPlusJump = Ctx.PlayerPhysics.Velocity;
            velocityPlusJump.y = Ctx.InitialJumpVelocity;
            Ctx.PlayerPhysics.SetVelocity(velocityPlusJump);
            Ctx.ModelSquasher.BumpStretch();
        }

        public override void CheckSwitchStates()
        {
            // if player is grounded and jump is pressed, switch to jump state
            if ((Ctx.JumpRequested && !Ctx.RequireNewJumpPress && Ctx.ToggleJump) || jumpBuffered)
            {
                jumpBuffered = false;
                Ctx.CharacterController.RequestJump();

                if (Ctx.IsSliding)
                {
                    // Sliding forward jump
                    Vector3 velocityPlusJump = Ctx.PlayerPhysics.Velocity;
                    
                    Vector3 lateralVelocity = Ctx.PlayerPhysics.Velocity;
                    lateralVelocity.y = 0;
                    lateralVelocity.Normalize();
                    
                    velocityPlusJump.y = Ctx.InitialJumpVelocity;
                    Ctx.PlayerPhysics.SetVelocity(velocityPlusJump);
                    
                    Ctx.PlayerPhysics.AddVelocity(lateralVelocity * Ctx.SlideForwardJumpVelocity, Ctx.MaxJumpVelocity);
                    Ctx.ModelSquasher.BumpStretch();
                    Ctx.BanditAnimationController.PlayJump();
                    Ctx.PlayerAudio.PlayJump();
                }
                else
                {
                    DefaultJump();
                    Ctx.BanditAnimationController.PlayJump();
                    Ctx.PlayerAudio.PlayJump();
                }
                
                Ctx.RequireNewJumpPress = true;
                SwitchState(Factory.FreeFall());
                
                InLevelMetrics.Instance?.LogEvent(MetricAction.Jump);
            }
            else if ((Ctx.DrillRequested && !Ctx.RequireNewJumpPress && !Ctx.RequireNewDrillPressOrEndGrounded && Ctx.ToggleJump))
            // else if ((Ctx.DrillRequested && Ctx.DrillixirManager.CanStartDrilling()) && !Ctx.RequireNewDrillPressOrEndGrounded)
            {
                // Player can perform a short hop into drill by pressing drill while grounded
                Ctx.CharacterController.RequestJump();
                
                Ctx.RequireNewJumpPress = true;
                Vector3 velocityPlusJump = Ctx.PlayerPhysics.Velocity;
                velocityPlusJump.y = Ctx.InitialDrillJumpVelocity;
                Ctx.PlayerPhysics.SetVelocity(velocityPlusJump);
                SwitchState(Factory.FreeFall());

                InLevelMetrics.Instance?.LogEvent(MetricAction.DrillRequest);
            }
            // if player is not grounded and jump is not pressed, switch to fall state
            else if (!Ctx.CharacterController.IsGrounded)
            {
                // Otherwise fall
                SwitchState(Factory.FreeFall());
                ((FreeFallState)Factory.FreeFall()).coyoteTimeEnabled = true;
            }
            else if (Ctx.InteractKeyRequested && !Ctx.RequireNewInteractKeyPress)
            {
                Ctx.CharacterController.RequestInteract();
                Ctx.RequireNewInteractKeyPress = true;
            }
            // TODO: Uncomment this if we don't want to restrict the dash to drilling
            else if (Ctx.TargetedDashRequested 
                     && Ctx.TargetedDash.CanPerformDash()
                     && !Ctx.RequireNewTargetedDashPress)
            {
                Ctx.RequireNewTargetedDashPress = true;
                // This comes before the switch because the behavior of exit state on grounded depends upon whether the dash is performed
                Ctx.ModelRotator.OnDash(Ctx.TargetedDash.TargetPosition());
                SwitchState(Factory.Dash());
            }
        }

        public override void InitializeSubState()
        {
            if (Ctx.PlayerPhysics.CheckBlitzSpeed() && Ctx.ToggleSlide)
            {
                SetSubStateInit(Factory.Slide());
            }
            else
            {
                SetSubStateInit(Factory.Walk());
            }
        }

        public override string StateName()
        {
            return "Grounded";
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

        protected override void ImpactSlide(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            Ctx.PlayerPhysics.CurrentOnSurfaceType = PlayerPhysics.OnSurfaceType.Slide;
            Ctx.OnSlipperySurface = true;
        }

        private Vector3 lastSpikePos;
        private float spikeDiameter = 4.0f;
        private float horizontalOffset = 0.0f;
        private float verticalOffset = 0.0f;
        protected override void ImpactSlideDanger(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            Ctx.PlayerPhysics.CurrentOnSurfaceType = PlayerPhysics.OnSurfaceType.SlideDanger;
            //kill if slow
            if (!Ctx.PlayerPhysics.CheckBlitzSpeed())
            {
                Ctx.InstantKill();
                ObjectPooler.Instance.Allocate("DeathBarrier", hitPoint, Quaternion.FromToRotation(Vector3.up, hitNormal));
            }
            else if (Vector3.Distance(lastSpikePos, hitPoint) > spikeDiameter)
            {
                Vector3 locationToSpawn = hitPoint + Vector3.Cross(hitNormal, Vector3.up) * horizontalOffset +
                                          (hitNormal * verticalOffset);
                if (UnityEngine.Physics.Raycast(locationToSpawn + hitNormal * (verticalOffset + 21.0f), -hitNormal, verticalOffset + 22.0f))
                {
                    ObjectPooler.Instance.Allocate("DeathBarrier", locationToSpawn,
                        Quaternion.FromToRotation(Vector3.up, hitNormal));
                }

                lastSpikePos = hitPoint;
                horizontalOffset = Random.Range(-10.0f, 10.0f);
                verticalOffset = Random.Range(-2.0f, 0.0f);
                spikeDiameter = Random.Range(4.0f, 14.0f);
            }
        }
    }
}
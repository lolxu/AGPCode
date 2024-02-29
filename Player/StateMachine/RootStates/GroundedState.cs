using System;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.__Scripts.Player.Environment;
using __OasisBlitz.Enemy;
using __OasisBlitz.Player.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace __OasisBlitz.Player.StateMachine.RootStates
{
    public class GroundedState : BaseState, IRootState
    {

        public GroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
            StateEnum = PlayerStates.Grounded;
        }

        public void HandleGravity()
        {
        }

        public override void EnterState()
        {
            Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.Grounded;
            
            InitializeSubState();
            
            //Ctx.DrillixirManager.FullRefillDrillixir();
            BounceAbility.Instance.RefreshBounce();
            HandleGravity();

            Ctx.ModelRotator.SetGrounded(true);
            Ctx.BanditAnimationController.SetGrounded(true);
        }

        public override void UpdateState()
        {
            //recharge drillixir
            // Ctx.DrillixirManager.TimeBasedRefill(Time.deltaTime);
            if (Ctx.DrillRequested)
            {
                // Ctx.Drill.SetDrillVisible();
                Ctx.GauntletManager.ExtendBlades();
            }
            else
            {
                // Ctx.Drill.SetDrillInvisible();
                Ctx.GauntletManager.RetractBlades();
            }

            CheckSwitchStates();
        }

        public override void ExitState()
        {
            Ctx.ModelRotator.SetGrounded(false);
            Ctx.BanditAnimationController.SetGrounded(false);
            Ctx.OnSlipperySurface = false;
            Ctx.RequireNewDrillPressOrEndGrounded = false;
            Ctx.PlayerPhysics.CurrentOnSurfaceType = PlayerPhysics.OnSurfaceType.NotGrounded;
        }

        public override void CheckSwitchStates()
        {
            // if player is grounded and jump is pressed, switch to jump state
            if ((Ctx.JumpRequested && !Ctx.RequireNewJumpPress && Ctx.ToggleJump))
            {
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
                    
                }
                else
                {
                    // Default vertical jump
                    Vector3 velocityPlusJump = Ctx.PlayerPhysics.Velocity;
                    velocityPlusJump.y = Ctx.InitialJumpVelocity;
                    Ctx.PlayerPhysics.SetVelocity(velocityPlusJump);
                    Ctx.ModelSquasher.BumpStretch();
                    
                }
                
                Ctx.RequireNewJumpPress = true;
                SwitchState(Factory.FreeFall());
                ((FreeFallState)Factory.FreeFall()).coyoteTimeEnabled = true;
            }
            else if ((Ctx.DrillRequested && Ctx.DrillixirManager.CanStartDrilling()) && !Ctx.RequireNewJumpPress && !Ctx.RequireNewDrillPressOrEndGrounded && Ctx.ToggleJump)
            // else if ((Ctx.DrillRequested && Ctx.DrillixirManager.CanStartDrilling()) && !Ctx.RequireNewDrillPressOrEndGrounded)
            {
                // Player can perform a short hop into drill by pressing drill while grounded
                Ctx.CharacterController.RequestJump();
                
                Ctx.RequireNewJumpPress = true;
                Vector3 velocityPlusJump = Ctx.PlayerPhysics.Velocity;
                velocityPlusJump.y = Ctx.InitialDrillJumpVelocity;
                Ctx.PlayerPhysics.SetVelocity(velocityPlusJump);
                SwitchState(Factory.FreeFall());
                
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
            else if (Ctx.TargetedDashRequested && Ctx.TargetedDash.CanPerformDash())
            {
                Debug.Log("Here trying dash");
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
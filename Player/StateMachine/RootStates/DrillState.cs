using __OasisBlitz.__Scripts.Enemy.Enemies.Flashing;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.__Scripts.Player.Environment;
using __OasisBlitz.__Scripts.Player.Environment.Chest;
using __OasisBlitz.__Scripts.Player.Environment.FragileSand;
using __OasisBlitz.__Scripts.Player.Environment.Fruits;
using __OasisBlitz.Enemy;
using __OasisBlitz.Enemy.StateMachine;
using __OasisBlitz.Player.Physics;
using UnityEngine;

namespace __OasisBlitz.Player.StateMachine.RootStates
{
    public class DrillState : BaseState, IRootState
    {
        private const float MaxVelocity = 30.0f;
        private const float Acceleration = 130.0f;

        private bool submergedLastFrame;

        public DrillState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
            StateEnum = PlayerStates.Drill;
        }

        public void HandleGravity()
        {
        }

        public override void EnterState()
        {
            InitializeSubState();

            // Ctx.BanditAnimationController.PlayDrill();

            Ctx.CharacterController.CollisionMode = CollisionMode.Drilling;

            // Make the player model point in the direction of the drill
            // Ctx.ModelRotator.OnStartDrilling();
            Ctx.BanditAnimationController.PlayFlipIntoDrill();

            
            Ctx.PlayerAudio.StartDrill();
            Ctx.PlayerAudio.PlayFormDrill();

                // Drill visuals
            Ctx.Drilling = true;

            // Other visuals
            Ctx.TrailController.EnableTrail();

            //Bounce Correction Stuff
            Ctx.DrillChecker.PrevPlayerPos = Ctx.transform.position;

            Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Airborne;
            Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Airborne;
            // In this case, delegate gravity settings to the substates
        }

        public override void UpdateState()
        {


            CheckSwitchStates();

            // Handle physics -- if blitz speed, control more loosely
            // if (Ctx.PlayerPhysics.CheckBlitzSpeed())
            // {
            //     Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Blitz;
            //     Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Blitz;
            // }
            // else
            // {
            //     Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Airborne;
            //     Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Airborne;
            // }
            submergedLastFrame = Ctx.IsSubmerged;

            // Ctx.DrillixirManager.TickDrillixirConsume(Time.deltaTime);

        }

        public override void ExitState()
        {
            Ctx.CharacterController.CollisionMode = CollisionMode.Default;

            // Ctx.ModelRotator.OnStopDrilling();
            Ctx.BanditAnimationController.PlayFlipOutOfDrill();

            // TODO: Much of this could be rolled into the "UnformDrill" code
            Ctx.PlayerAudio.StopDrill();
            Ctx.PlayerAudio.PlayUnformDrill();
            Ctx.Drilling = false;
        }

        public override void InitializeSubState()
        {
            SetSubStateSilent(Factory.DrillAbove());
        }

        public override void CheckSwitchStates()
        {
            // Only allow switching when above ground
            if (Ctx.TargetedDashRequested 
                && Ctx.TargetedDash.CanPerformDash()
                && !Ctx.RequireNewTargetedDashPress)
            {
                // This comes before the switch because the behavior of exit state on grounded depends upon whether the dash is performed
                Ctx.RequireNewTargetedDashPress = true;
                Ctx.ModelRotator.OnDash(Ctx.TargetedDash.TargetPosition());
                SwitchState(Factory.Dash());
            }
            else if (Ctx.IsSubmerged)
            {
                return;
            }
            else if (!Ctx.DrillRequested)
            {
                SwitchState(Factory.FreeFall());
            }
            else if (!Ctx.ToggleDrill)
            {
                SwitchState(Factory.FreeFall());
            }
        }

        public override string StateName()
        {
            return "Drill";
        }

        protected override void ImpactEnemy(BasicEnemyController enemyController)
        {
            Debug.Log("Drill impact enemy");
            enemyController.Kill();
        }

        protected override void ImpactNewEnemy(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            //Enemy Drill Collision
            HitPartOfNewEnemy hitbox = coll.gameObject.GetComponent<HitPartOfNewEnemy>();
            if (hitbox)
            {
                if (hitbox.GetEnemyStateMachine().EnemyType == "Hardening")
                {
                    HardeningEnemy hardeningEnemyEnemy = hitbox.GetEnemyStateMachine().gameObject.GetComponent<HardeningEnemy>();
                    if (hardeningEnemyEnemy._canBePenetrated)
                    {
                        hitbox.CollideWithBody(ref coll, hitNormal, hitPoint);
                        Ctx.PlayerFeedbacks.impactEnemyFeedback.PlayFeedbacks();
                    }
                    else
                    {
                        SwitchState(Factory.Dead());
                    }
                }
                else
                {
                    hitbox.CollideWithBody(ref coll, hitNormal, hitPoint);
                    Ctx.PlayerFeedbacks.impactEnemyFeedback.PlayFeedbacks();
                }
                
            }
            
        }
        protected override void ImpactWalkOnly(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            if (Ctx.ToggleWalkOnlyStopsDrill && hitNormal.y > 0.5f && !Ctx.IsSubmerged)//check if we are hitting ground we can stand on
            {
                //if we hit the ground, switch to grounded
                Ctx.RequireNewDrillPressOrEndGrounded = true;
                SwitchState(Factory.FreeFall());
            }
            else
            {
                //small bounce
                // Bounce.Instance.BounceCollider(ref coll, ref Ctx.PlayerPhysics, Bounce.BounceTypeNormal.Small, Bounce.BounceTypeReflective.Tiny);
                Bounce.Instance.WeakBounce(ref coll, hitNormal, ref Ctx.PlayerPhysics);
            }
            Ctx.PlayerAudio.WalkOnlyImpact();
        }

        protected override void ImpactWalkOnlyKill(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            if (Ctx.ToggleWalkOnlyStopsDrill && hitNormal.y > 0.5f && !Ctx.IsSubmerged)//check if we are hitting ground we can stand on
            {
                //if we hit the ground, switch to grounded
                Ctx.RequireNewDrillPressOrEndGrounded = true;
                SwitchState(Factory.FreeFall());
            }
            else
            {
                //small bounce
                // Bounce.Instance.BounceCollider(ref coll, ref Ctx.PlayerPhysics, Bounce.BounceTypeNormal.Small, Bounce.BounceTypeReflective.Tiny);
                Bounce.Instance.WeakBounce(ref coll, hitNormal, ref Ctx.PlayerPhysics);
            }
            Ctx.PlayerAudio.WalkOnlyImpact();
        }

        protected override void ImpactDrillOnly(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
        }

        protected override void ImpactBouncePad(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            Bounce.Instance.StickThenBounce(ref coll, hitNormal, hitPoint, ref Ctx.PlayerPhysics, Bounce.BounceTypeNormal.Large);
        }

        protected override void ImpactBreakables(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            BreakablesManager.Instance.BreakableDrillCollide(ref coll, hitNormal, hitPoint, ref Ctx.PlayerPhysics);
        }

        protected override void ImpactFragileSand(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            FragileSandManager.Instance.FragileSandStartShrinkDrillCollide(coll);
        }

        public override bool ShouldObeyGroundSnap()
        {
            // Don't snap to ground when drilling
            return false;
        }

        private Vector3 lastSpikePos;
        private float spikeDiameter = 4.0f;
        private float horizontalOffset = 10.0f;
        private float verticalOffset = 0.0f;
        protected override void ImpactSlideDanger(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
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
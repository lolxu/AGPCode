using System.Collections;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.Player.Physics;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace __OasisBlitz.Player.StateMachine.SubStates
{
    public class DrillBelowState : BaseState
    {
        private bool collidingWithPenetrable;
        private bool penetrableIsLarge;

        private float jumpBoostMultiplier = 20.0f;
        private float minimumVelocityAgainstNormal = 3f;
        
        private float drillExitBoost = 10f;
        private float drillEnterBoost = 10f;
        
        public DrillBelowState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            StateEnum = PlayerStates.DrillBelow;
        }

        public override void EnterState()
        {
            // Time.timeScale = Ctx.DrillingTimescale;
            MMTimeManager.Instance.NormalTimeScale = Ctx.DrillingTimescale; 
            
            Ctx.IsSubmerged = true;
            Ctx.PlayerAudio.SetSubmerged(true);
            Ctx.PlayerAudio.PlaySandImpact();
            Ctx.Drill.DrillBlip();

            Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.DrillBelow;
            Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Submerged;
            Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Submerged;
            Ctx.CharacterController.IsDrillingInsideTerrain = true;
            
            // AddNormalBoost(true);
            Ctx.PlayerFeedbacks.drillSubmergeFeedback.PlayFeedbacks();
            SurgeJump.Instance.SurgeJumpRequested = false;
            SurgeJump.Instance.RequiresNewSurgeJump = true;
            SurgeJump.Instance.CheckForSubmergedBoost();
            HapticsManager.Instance.PlayEnterDrillBelowHaptic();
            HapticsManager.Instance.StartDrillHaptic();
        }

        public override void UpdateState()
        {
            collidingWithPenetrable = Ctx.DrillChecker.CheckCollidingWithDrillable();
            Ctx.DrillixirManager.TimeBasedRefill(Time.deltaTime);

            CheckSwitchStates();
        }

        public override void ExitState()
        {
            // Give Integer Drillixir Back:
            Ctx.DrillixirManager.AddDrillixirCharge();
            
            // Time.timeScale = 1f;
            MMTimeManager.Instance.NormalTimeScale = 1f;
            
            Ctx.IsSubmerged = false;
            Ctx.PlayerAudio.SetSubmerged(false);

            Ctx.PlayerAudio.PlaySandImpact();
            Ctx.Drill.DrillBlip();
            SurgeJump.Instance.SetCachedExitPos(Ctx.gameObject.transform.position);
            
            // AddDirectionalBoost();
            // AddInputBasedBoost();
            Ctx.PlayerFeedbacks.drillResurfaceFeedback.PlayFeedbacks();
            SurgeJump.Instance.StartSurgeJumpWindow();
            // SurgeJump.Instance.SpawnExitHole();
            Ctx.CharacterController.IsDrillingInsideTerrain = false;
            
            // refresh bounce:
            if (BounceAbility.Instance)
            {
                BounceAbility.Instance.RefreshBounce();
            }
            
            HapticsManager.Instance.StopDrillHaptic();
        }

        private void ExitLaunch()
        {
            AddDirectionalBoost();
        }

        public override void CheckSwitchStates()
        {
            // If no longer in contact with penetrable ground, switch to above state
            if (!collidingWithPenetrable)
            {
                // Play sand feedback
                FeelEnvironmentalManager.Instance.PlaySandBurstFeedback(Ctx.gameObject.transform.position, 1.5f);
                
                SwitchState(Factory.DrillAbove());
            }
        }

        private void AddNormalBoost(bool submerging)
        {
            Vector3 surfaceNormal = Ctx.DrillChecker.GetPenetrableNormal();
            
            if (submerging) surfaceNormal *= -1;
            
            // Only add the normal boost if the player is not already moving fast along that direction
            // Vector3 velocityAlongNormal = Vector3.Project(Ctx.PlayerPhysics.Velocity, surfaceNormal);
            // if (velocityAlongNormal.magnitude > 20f) return;

            Vector3 addedVelocity = surfaceNormal * drillEnterBoost;
            Ctx.PlayerPhysics.AddVelocity(addedVelocity, MaxResultantSpeed:40);
            
        }

        private void AddDirectionalBoost()
        {
            Vector3 playerVelocity = Ctx.PlayerPhysics.Velocity;
            Vector3 addedVelocity = playerVelocity.normalized * drillExitBoost;
            Ctx.PlayerPhysics.AddVelocity(addedVelocity, MaxResultantSpeed:60);
        }


        /// <summary>
        /// This function checks the velocity against the normal of the penetrable terrain. It boosts the player along
        /// the normal if there is not enough velocity in the direction of the normal -- this prevents "glancing"
        /// </summary>
        /// <param name="submerging"> If true, the player is entering terrain. If false, the player is exiting. </param>
        private void CheckSwitchVelocity(bool submerging)
        {
            // Check the dot product between the player's velocity and the surface normal
            // If it is not above a certain threshold, we add velocity to the player along the surface normal
            // This prevents "glancing", unsatisfyingly shallow entry and exit

            Vector3 surfaceNormal = Ctx.DrillChecker.GetPenetrableNormal();
            Vector3 playerVelocity = Ctx.PlayerPhysics.Velocity;

            // If the player is entering the terrain, we want to flip the normal
            if (submerging) surfaceNormal *= -1;
            
            float dotProduct = Vector3.Dot(playerVelocity, surfaceNormal);
            
            
            if (dotProduct < minimumVelocityAgainstNormal)
            {
                // Add velocity to the player along the surface normal
                Vector3 boostVelocity = surfaceNormal * jumpBoostMultiplier;
                Ctx.PlayerPhysics.AddVelocity(boostVelocity);
            }
            
        }

        public override void InitializeSubState()
        {
        }
        public override string StateName()
        {
            return "DrillBelow";
        }
    }
}
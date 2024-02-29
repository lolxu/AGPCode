using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.Enemies.Flashing;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.Player.Physics;
using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.Player.StateMachine.RootStates
{
    public class BallState : BaseState, IRootState
    {

        public BallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
            StateEnum = PlayerStates.Ball;
        }

        public override void EnterState()
        {
            InitializeSubState();
            
            Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.FreeFall;
            Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Ball;
            Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Ball;
            
            Ctx.BanditAnimationController.PlayBall();
            Debug.Log("Enter ball");
            
            AddInputBasedBoost();
            AddDirectionalBoost();

        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }
        

        public override void ExitState()
        {
            Debug.Log("exit ball");
        }

        public override void CheckSwitchStates()
        {
            // Same switch states as FreeFall
            
            // If grounded and moving downwards, switch to grounded state
            if (Ctx.CharacterController.IsGrounded)
            {
                SwitchState(Factory.Grounded());
            }
            else if (Ctx.DrillRequested && Ctx.DrillixirManager.CanStartDrilling() && !Ctx.DrillLocked && !Ctx.RequireNewDrillPressOrEndGrounded && Ctx.ToggleDrill)
            {
                SwitchState(Factory.Drill());
            }
            else if (Ctx.TargetedDashRequested && Ctx.TargetedDash.CanPerformDash())
            {
                SwitchState(Factory.Dash());
            }
        }
        
        private void AddDirectionalBoost()
        {
            Vector3 playerVelocity = Ctx.PlayerPhysics.Velocity;
            Vector3 addedVelocity = playerVelocity.normalized * Ctx.PlayerPhysics.velocityBasedBoost;
            Ctx.PlayerPhysics.AddVelocity(addedVelocity, MaxResultantSpeed:Ctx.PlayerPhysics.maxBoostSpeed);
        }
        
        private void AddInputBasedBoost()
        {
            Vector3 currentInput = Ctx.MovementInput;
            Vector3 addedVelocity = currentInput * (Ctx.PlayerPhysics.inputBasedBoost);
            Ctx.PlayerPhysics.AddVelocity(addedVelocity, MaxResultantSpeed:Ctx.PlayerPhysics.maxBoostSpeed);
        }

        public override void InitializeSubState()
        {
        }

        public override string StateName()
        {
            return "Ball";
        }
        
        protected override void ImpactEnemy(BasicEnemyController enemy)
        {
            // TODO: Kill the enemy and set a flag for switching states
            // if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            SwitchState(Factory.Dead());

        }

        protected override void ImpactDrillOnly(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            // TODO: How should dash behave here?
            if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            SwitchState(Factory.Dead());
        }
    }
}

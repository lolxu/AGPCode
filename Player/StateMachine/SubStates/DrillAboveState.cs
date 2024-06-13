using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.Player.Physics;

namespace __OasisBlitz.Player.StateMachine.SubStates
{
    public class DrillAboveState : BaseState
    {
        public DrillAboveState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            StateEnum = PlayerStates.DrillAbove;
        }

        public override void EnterState()
        {
            Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.DrillAbove;
            Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.DrillAbove;
            Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Airborne;
        }

        public override void UpdateState()
        {
            if (BounceAbility.Instance.BounceEnabled
                && Ctx.JumpRequested 
                && !Ctx.RequireNewJumpPress)
            {
                // consume current bounce
                Ctx.RequireNewJumpPress = true;
                BounceAbility.BounceAttemptResult bounceAttemptResult = BounceAbility.Instance.AirBounce();


                if (bounceAttemptResult.DidBounce)
                {
                    Ctx.BanditAnimationController.PlayBlastFromDrill(bounceAttemptResult.BounceVelocity);
                }
            }
            CheckSwitchStates();
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
            // If in contact with penetrable ground, switch to below state
            if (Ctx.DrillChecker.CheckCollidingWithDrillable())
            {
                if (Ctx.InWaterTrigger)
                {
                    FeelEnvironmentalManager.Instance.PlayWaterBurstFeedback(Ctx.transform.position, 1.25f);
                    Ctx.PlayerAudio.PlaySplashSound();
                }
                else
                {
                    FeelEnvironmentalManager.Instance.PlaySandBurstFeedback(Ctx.transform.position, 1.25f);
                }
                SwitchState(Factory.DrillBelow());
            }
        }

        public override void InitializeSubState()
        {
        }

        public override string StateName()
        {
            return "DrillAbove";
        }
    }
}
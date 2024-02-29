namespace __OasisBlitz.Player.StateMachine.SubStates
{
    public class RestrictedHorizontalMovementState : BaseState
    {
        private const float MaxVelocity = 5.0f;
        private const float Acceleration = 200.0f;

        public RestrictedHorizontalMovementState(PlayerStateMachine currentContext,
            PlayerStateFactory playerStateFactory)
            : base(currentContext, playerStateFactory)
        {
            StateEnum = PlayerStates.RestrictedHorizontalMovement;
        }

        public override void EnterState()
        {
            // Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
            // Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
            // Ctx.PlayerVelocity.MaxVelocity = MaxVelocity;
            // Ctx.PlayerVelocity.SetDrag(0.0f);
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState()
        {
        }

        public override void InitializeSubState()
        {
        }

        public override void CheckSwitchStates()
        {
            if (!Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Idle());
            }
        }

        public override string StateName()
        {
            return "RestrictedHorizontalMovement";
        }
    }
}

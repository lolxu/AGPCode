namespace __OasisBlitz.Camera.StateMachine.RootStates
{
    public class DiveSmallPenetrableState : CameraBaseState
    {
            public DiveSmallPenetrableState(CameraStateMachine currentContext, CameraStateFactory cameraStateFactory)
            : base(currentContext, cameraStateFactory)
        {
            IsRootState = true;
        }
            
        public override void EnterState()
        {
            InitializeSubState();
            Ctx.rigChanger.TweenToRig(Ctx.diveSmallPenetrableRig, 0.5f);
            Ctx.rigChanger.SetOccluderMask(Ctx.smallPenetrableMask);
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
            if (!Ctx.playerStateMachine.IsSubmerged)
            {
                SwitchState(Factory.SurfaceDefault());
            }
            else if (Ctx.playerStateMachine.IsInLargeTerrain)
            {
                SwitchState(Factory.DiveLargePenetrable());
            }
        }

        public override void InitializeSubState()
        {
        }

        public override string StateName()
        {
            return "DiveSmallPenetrableState";
        }
    }
}

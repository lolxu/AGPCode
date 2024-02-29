namespace __OasisBlitz.Camera.StateMachine.RootStates
{
    public class SurfaceAscendingState : CameraBaseState
    {
            public SurfaceAscendingState(CameraStateMachine currentContext, CameraStateFactory cameraStateFactory)
            : base(currentContext, cameraStateFactory)
        {
            IsRootState = true;
        }
            
        public override void EnterState()
        {
            InitializeSubState();
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
        }

        public override void InitializeSubState()
        {
        }

        public override string StateName()
        {
            return "DiveLargePenetrableState";
        }
    }
}

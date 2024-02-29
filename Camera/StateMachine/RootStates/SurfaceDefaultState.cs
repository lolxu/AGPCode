using __OasisBlitz.__Scripts.FEEL;
using UnityEngine;

namespace __OasisBlitz.Camera.StateMachine.RootStates
{
    public class SurfaceDefaultState : CameraBaseState
    {
            public SurfaceDefaultState(CameraStateMachine currentContext, CameraStateFactory cameraStateFactory)
            : base(currentContext, cameraStateFactory)
        {
            IsRootState = true;
        }
            
        public override void EnterState()
        {
            InitializeSubState();
            Ctx.rigChanger.TweenToRig(Ctx.surfaceRig, 0.5f);
            Ctx.rigChanger.SetOccluderMask(Ctx.surfaceMask);

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
            if (Ctx.playerStateMachine.IsInLargeTerrain)
            {
                SwitchState(Factory.DiveLargePenetrable());
            }
            else if (Ctx.playerStateMachine.IsSubmerged)
            {
                SwitchState(Factory.DiveSmallPenetrable());
            }
        }

        public override void InitializeSubState()
        {
        }

        public override string StateName()
        {
            return "SurfaceDefaultState";
        }
    }
}

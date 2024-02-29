using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace __OasisBlitz.Camera.StateMachine.RootStates
{
    public class DiveLargePenetrableState : CameraBaseState
    {
        private CinemachineRotationComposer freeLookRotationComposer;
        private bool cameraAlreadyInsidePenetrable = false;
        
        public DiveLargePenetrableState(CameraStateMachine currentContext, CameraStateFactory cameraStateFactory)
        : base(currentContext, cameraStateFactory)
        {
            freeLookRotationComposer = currentContext.freeLookCam.GetComponent<CinemachineRotationComposer>();
            IsRootState = true;
        }
            
        public override void EnterState()
        {
            InitializeSubState();

            Ctx.rigChanger.SetOccluderMask(Ctx.largePenetrableMask);
            
            // TODO: This logic is only really relevant when entering large penetrable ground from above.
            if (Ctx.playerStateMachine.PlayerPhysics.Velocity.y < -1 * Ctx.DiveCameraVelocity)
            {
                Ctx.rigChanger.TweenToRig(Ctx.diveLargePenetrableRig, 0.5f);
            }
            else
            {
                Ctx.rigChanger.TweenToRig(Ctx.noDiveLargePenetrableRig, 0.7f);
            }
        }

        public override void UpdateState()
        {
            AudioManager.instance.UnderGroundLPF(Ctx.sandCameraCollider.IsInsideLargePenetrable);
            
            CheckSwitchStates();
        }

        public override void ExitState()
        {
            AudioManager.instance.UnderGroundLPF(false);
        }

        public override void CheckSwitchStates()
        {
            if (!Ctx.playerStateMachine.IsInLargeTerrain && Ctx.playerStateMachine.IsSubmerged)
            {
                SwitchState(Factory.DiveSmallPenetrable());
            }
            else if (!Ctx.playerStateMachine.IsInLargeTerrain && !Ctx.playerStateMachine.IsSubmerged)
            {
                SwitchState(Factory.SurfaceDefault());
            }
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
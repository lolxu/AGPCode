using System.Collections;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.Camera.StateMachine.RootStates
{
    public class CinematicsCameraState : CameraBaseState
    {
        private PlayerStateMachine playerCtx;
        
        public CinematicsCameraState(CameraStateMachine currentContext, CameraStateFactory playerStateFactory) 
            : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            GameObject panCam = GameObject.FindGameObjectWithTag("PanCamera");
            playerCtx = Ctx.playerStateMachine;
            if (Ctx && !SceneManager.GetActiveScene().name.Contains("Burrow"))
            {
                if (panCam)
                {
                    // Disable Free look first to pan the level
                    Ctx.freeLookCam.gameObject.SetActive(false);
                    Ctx.playerStateMachine.ToggleDrill = false;
                    Ctx.playerStateMachine.ToggleSlide = false;
                    Ctx.playerStateMachine.ToggleWalk = false;
                    Ctx.playerStateMachine.ToggleJump = false;
                }
            }
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState()
        {
            // SwitchState(Factory.SurfaceDefault());
        }

        public override void CheckSwitchStates()
        {
            if (!Ctx.cinematicsCam)
            {
                Debug.Log("Cinematics Cam not real");
                SwitchState(Factory.SurfaceDefault());
            }
            else if (!Ctx.cinematicsCam.gameObject.activeInHierarchy)
            {
                Debug.Log("Cinematics Cam not active anymore");
                SwitchState(Factory.SurfaceDefault());
            }
        }

        public override void InitializeSubState()
        {
            
        }

        public override string StateName()
        {
            return "CinematicsCameraState";
        }
        
        
    }
}
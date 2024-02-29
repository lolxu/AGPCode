using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using UnityEngine;

namespace __OasisBlitz.Camera.StateMachine
{
    public abstract class CameraBaseState
    {
        private bool isRootState = false;
        
        // A flag that is set when the state switches, so that it does not update its sub states after they exit
        private bool switchedThisUpdate = false;
        
        private CameraStateMachine ctx;
        private CameraStateFactory factory;
        private CameraBaseState currentSubState;
        private CameraBaseState currentSuperState;

        // public bool RequestCameraSkip = false;

        protected bool IsRootState
        {
            set { isRootState = value; }
        }

        protected CameraStateMachine Ctx
        {
            get { return ctx; }
        }

        public CameraStateFactory Factory
        {
            get { return factory; }
        }

        public CameraBaseState(CameraStateMachine currentContext, CameraStateFactory playerStateFactory)
        {
            ctx = currentContext;
            factory = playerStateFactory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void CheckSwitchStates();
        public abstract void InitializeSubState();
        public abstract string StateName();

        public void SwitchState(CameraBaseState newState)
        {
        #if DEBUG
            if (Constants.DebugRootStateChanges)
            {
                if (newState.isRootState)
                {
                    Debug.Log("Switching from " + StateName() + " to " + newState.StateName());
                }
            }
            if (Constants.DebugSubstateChanges)
            {
                if (!newState.isRootState)
                {
                    Debug.Log("Switching from " + StateName() + " to " + newState.StateName());
                }
            }
        #endif 
            // current state exits state
            ExitStates();

            if (isRootState)
            {
                // switch current state of context
                ctx.CurrentState = newState;
            }
            else if (currentSuperState != null)
            {
                // set the current super states sub state to the new state
                currentSuperState.SetSubStateSilent(newState);
            }
            
            // new state enters state
            newState.EnterState();

            switchedThisUpdate = true;
        }

        public void UpdateStates()
        {
            UpdateState();
            
            // Ctx.DebugText.SetText("Current State: " + StateName() + "\nSubstate: " + _currentSubState?.StateName());
            // Debug.Log(StateName() + " is updating");
            // Don't update sub states if the state switched this update
            if (currentSubState != null && !switchedThisUpdate)
            {
                currentSubState.UpdateStates();
            }

            switchedThisUpdate = false;
        }

        public void ExitStates()
        {
            ExitState();

            if (currentSubState != null)
            {
                currentSubState.ExitState();
            }
        }
        
        public void ForceSetCameraPosition(Vector3 pos)
        {
            ctx.freeLookCam.ForceCameraPosition(pos, Quaternion.identity);
        }

        public void ResetCameraPosition()
        {
            //ctx.freeLookCam.ForceCameraPosition(pos, Quaternion.identity);
            ctx.ResetHorizontalAxis();
            ctx.ResetVerticalAxis();
        }

        protected void SetSuperState(CameraBaseState newSuperState)
        {
            currentSuperState = newSuperState;
        }

        protected void SetSubStateSilent(CameraBaseState newSubState)
        {
            #if UNITY_EDITOR
            if (Constants.DebugSubstateChanges)
            {
                Debug.Log("SubstateSet: " + newSubState.StateName());
            }
            #endif
            currentSubState = newSubState;
            newSubState.SetSuperState(this);
        }
        
        protected void SetSubStateInit(CameraBaseState subState)
        {
            SetSubStateSilent(subState);
            subState.EnterState();
        }
    }
}
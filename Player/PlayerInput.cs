using System;
using MoreMountains.Feedbacks;
using System.Runtime.Remoting.Messaging;
using __OasisBlitz.Camera.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using __OasisBlitz.UI;
using Obi;
using Constants = __OasisBlitz.Utility.Constants;

namespace __OasisBlitz.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public struct PlayerCharacterInputs
        {
            public Vector3 MoveInput;
            public Vector2 MoveInputRaw;
            public bool JumpDown;
            public bool DrillDown;
            public bool TargetedDashDown;
            public bool IsMovementPressed;
            public bool InteractDown;
            public bool SurgeJumpDown;
            public bool RecenterCameraDown;
        }

        public enum PlayerInputState
        {
            Character,
            UI,
            CritterInteract,
            SlideShowControls,
            Nothing
        }

        private PlayerInputState currInputState = PlayerInputState.Nothing;
        
        //critter interact
        public static Action CancelCritterInteraction;
        public static Action CritterContinueInteraction;
        
        //SlideShowSkip
        public static Action StartHoldingSkip;
        public static Action StopHoldingSkip;

        public PlayerInputActions playerInputActions; // NOTE: PlayerInput class must be generated from New Input System in Inspector

        private PlayerCharacterInputs currentInputs;

        [SerializeField] private UnityEngine.InputSystem.PlayerInput playerInputComponent;      // Access control schemes in PlayerInputActions
        private string controlScheme;

        // Inputs for internal use -- available for seeing in DrillixirIndicator.cs
        public Vector2 currentMovementInputRaw { get; private set; }
    
        public PlayerCharacterInputs CurrentInputs => currentInputs;

        public UnityEngine.Camera mainCamera;
        
        // [SerializeField] private __OasisBlitz.Camera.PlayerCamera playerCamera;

        [Header("Input thresholds")]
        [SerializeField] private float ShoulderDownThreshhold = .05f;

        // Drillixir reference for the animation
        [SerializeField] private DrillixirIndicator drillixirBar;
        // UIAudio reference in case it does not exist
        [SerializeField] private GameObject UIAudioPrefab;

        void Awake()
        {
            // Set the mouse to be invisible and locked to the center of the screen
            if(!FindObjectOfType<MainMenu>())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            

            playerInputActions = new PlayerInputActions();

            playerInputActions.CharacterControls.Move.started += OnMovementInput;
            playerInputActions.CharacterControls.Move.canceled += OnMovementInput;
            playerInputActions.CharacterControls.Move.performed += OnMovementInput;
            playerInputActions.CharacterControls.Drill.performed += OnDrillPerformed;
            playerInputActions.CharacterControls.Drill.canceled += OnDrillStop;
            playerInputActions.CharacterControls.TargetedDash.started += OnTargetedDashPerformed;
            playerInputActions.CharacterControls.TargetedDash.canceled += OnTargetedDashStop;
            playerInputActions.CharacterControls.Jump.started += OnJumpStart;
            playerInputActions.CharacterControls.Jump.canceled += OnJumpStop;
            playerInputActions.CharacterControls.Pause.canceled += OnPausePressed;
            playerInputActions.CharacterControls.Interact.started += OnInteractStart;
            playerInputActions.CharacterControls.Interact.canceled += OnInteractStop;
            playerInputActions.CharacterControls.SurgeJump.started += OnSurgeJumpStart;
            playerInputActions.CharacterControls.SurgeJump.canceled += OnSurgeJumpStop;
            
            
            playerInputActions.CharacterControls.RecenterCamera.performed += OnRecenterCameraPerformed;
            playerInputActions.CharacterControls.RecenterCamera.canceled += OnRecenterCameraStop;
            
            playerInputActions.CharacterControls.Look.started += OnLookInput;
            playerInputActions.CharacterControls.Look.canceled += OnLookInput;
            playerInputActions.CharacterControls.Look.performed += OnLookInput;

            playerInputActions.CharacterControls.TimerDisplay.canceled += OnDisplayTimer;
            playerInputActions.CharacterControls.TimerReset.canceled += OnTimerReset;
            playerInputActions.CharacterControls.TimerStartStop.canceled += OnTimerStartStop;

            playerInputActions.CharacterControls.ControlsDetection.started += GetFirstKeyPressed;
            playerInputActions.UI.ControlsDetection.started += GetFirstKeyPressed;

            //set level loader callbacks
            playerInputActions.LeveLoaderUI.Resume.started += OnResumePressed;

            playerInputActions.CharacterControls.Move.started += CheckSwitchInputDevice;

            playerInputActions.UI.Pause.canceled += OnPausePressed;
            // playerInputActions.Drilling.Drill.performed += OnDrillPerformed;
            // playerInputActions.Drilling.Drill.canceled += OnDrillStop;

            //Critter Talk Callbacks
            playerInputActions.CritterTalkingControls.CancelInteraction.started += CancelCritterInteract;
            playerInputActions.CritterTalkingControls.Interact.started += ClearPlayerInput;

            //Slideshow Skip Callbacks
            playerInputActions.SlideShowControls.Skip.started += StartHoldSkip;
            playerInputActions.SlideShowControls.Skip.canceled += StopHoldSkip;

            // InputSystem.onDeviceChange += OnDeviceChange;


            // Debug.Log("INITIAL Controller: " + GlobalSettings.Instance.controller);
            // Debug.Log("INITIAL CONTROLLERS #: " + Input.GetJoystickNames().Length);

        }

        private void Start()
        {
            // GlobalSettings.Instance.SetController(GetControllerType());
        }
        /// <summary>
        /// The inputs will actually work from any device without this logic. This is a place in the code to trigger
        /// changes that need to happen in our scripts in response to different input being used, like adjusting UI
        /// or camera sensitivity.
        /// </summary>
        public void CheckSwitchInputDevice(InputAction.CallbackContext ctx)
        {
            if (ctx.control.device == Keyboard.current)
            {
                SetToKeyboard();
            }
            else if (ctx.control.device == Gamepad.current)
            {
                SetToGamepad();
            }
            else
            {
                Debug.Log("Unknown input source, not changing settings");
            }
        }

        // https://forum.unity.com/threads/how-to-detect-what-controller-youre-using.654904/
        public string GetControllerType()
        {
            string[] joystickNames = Input.GetJoystickNames();

            if(joystickNames.Length == 0) { return "KEYBOARD"; }
            if(joystickNames.Length > 2) { return "MULTIPLE"; }
            foreach (string name in joystickNames)
            {
                if (name.ToLower().Contains("xbox")) { return "XBOX"; }
                else if (name.ToLower().Contains("playstation")) { return "PLAYSTATION"; }
                // else if (name.ToLower().Contains("")         // Other controllers we wanna support? Switch pro? ADD HERE
                else { return "OTHER"; }
            }
            return "OTHER";
        }
        public void SetDevice()
        {

        }

        public string GetControlScheme()
        {
            return playerInputComponent.currentControlScheme;
        }
        // https://forum.unity.com/threads/how-do-i-in-script-detect-if-a-controller-is-plugged-in-runtime.1172651/
/*        public void OnControlsChanged()
        {
            if(!GlobalSettings.Instance) { return; }
            // Debug.Log($"Control Scheme Changed To: {playerInputComponent.currentControlScheme}");
            switch(playerInputComponent.currentControlScheme)
            {
                case "Keyboard":
                    controlScheme = "KEYBOARD";
                    GlobalSettings.Instance.SetController("KEYBOARD");
                    break;
                case "Playstation Controller":
                    controlScheme = "PLAYSTATION";
                    GlobalSettings.Instance.SetController("PLAYSTATION");
                    break;
                case "Switch Pro Controller":
                    controlScheme = "OTHER";
                    GlobalSettings.Instance.SetController("OTHER");
                    break;
                case "Xbox Controller":
                    controlScheme = "XBOX";
                    GlobalSettings.Instance.SetController("XBOX");
                    break;
            }
        }*/


        private void Update()
        {
            //if(GlobalSettings.Instance.playerControllerSetting == "AUTO" && GlobalSettings.Instance.displayedController != controlScheme) { OnControlsChanged(); }
        }
        private void SetToKeyboard()
        {
            // playerCamera.ActivateMouseSensitivity();
        }

        private void SetToGamepad()
        {
            // playerCamera.ActivateGamepadSensitivity();
        }

        public void UpdateInputs()
        {
            currentInputs.MoveInput = ConvertInputToWorldSpace(new Vector3(currentMovementInputRaw.x, 0, currentMovementInputRaw.y));
            currentInputs.MoveInputRaw = currentMovementInputRaw;
            currentInputs.IsMovementPressed = currentInputs.MoveInput.magnitude > 0.01f;
        }
        


        // https://forum.unity.com/threads/detect-if-gamepad-button-was-pressed.1206931/
/*        private void LateUpdate()       // Detect last input is keyboard or gamepad
        {
            if(Gamepad.current != null)
            {
                var gamepad = Gamepad.current.lastUpdateTime;
                var keyboard = Keyboard.current.lastUpdateTime;
                controller = (keyboard > gamepad) ? "KEYBOARD" : "GAMEPAD";
                PauseManager.Instance.SetControls();
                HUDManager.Instance.SetControls();
            }

        }*/




        /// <summary>
        ///  Takes an input vector and converts it to world space by accounting for the current orientation of the camera
        /// </summary>
        /// <param name="vectorToRotate"></param>
        /// <returns></returns>
        public Vector3 ConvertInputToWorldSpace(Vector3 vectorToRotate)
        {
            // store the Y value of the original vector to rotate 
            float currentYValue = vectorToRotate.y;

            // get the forward and right directional vectors of the camera
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;

            // remove the Y values to ignore upward/downward camera angles
            cameraForward.y = 0;
            cameraRight.y = 0;

            // re-normalize both vectors so they each have a magnitude of 1
            cameraForward = cameraForward.normalized;
            cameraRight = cameraRight.normalized;

            // rotate the X and Z VectorToRotate values to camera space
            Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
            Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

            // the sum of both products is the Vector3 in camera space and set Y value
            Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
            vectorRotatedToCameraSpace.y = currentYValue;
            return vectorRotatedToCameraSpace;
        }

        private void OnMovementInput(InputAction.CallbackContext context)
        {
            currentMovementInputRaw = context.ReadValue<Vector2>();
        }
        
        private void OnLookInput(InputAction.CallbackContext context)
        {
            CameraStateMachine.Instance.SetLookValue(context.ReadValue<Vector2>());
        }

        private void OnJumpStart(InputAction.CallbackContext context)
        {
            currentInputs.JumpDown = context.ReadValueAsButton();
        }

        private void OnJumpStop(InputAction.CallbackContext context)
        {
            currentInputs.JumpDown = context.ReadValueAsButton();
        }

        private void OnDrillPerformed(InputAction.CallbackContext context)
        {
            float abilityInput = context.ReadValue<float>();
            currentInputs.DrillDown = abilityInput > ShoulderDownThreshhold;
            drillixirBar.DrillPressed(currentInputs.DrillDown);
        }

        private void OnDrillStop(InputAction.CallbackContext context)
        {
            float abilityInput = context.ReadValue<float>();
            currentInputs.DrillDown = abilityInput > ShoulderDownThreshhold;
        }
        
        private void OnTargetedDashPerformed(InputAction.CallbackContext context)
        {
            float abilityInput = context.ReadValue<float>();
            currentInputs.TargetedDashDown = abilityInput > ShoulderDownThreshhold;
        }
        
        private void OnTargetedDashStop(InputAction.CallbackContext context)
        {
            float abilityInput = context.ReadValue<float>();
            currentInputs.TargetedDashDown = abilityInput > ShoulderDownThreshhold;
        }

        private void OnInteractStart(InputAction.CallbackContext context)
        {
            currentInputs.InteractDown = context.ReadValueAsButton();
        }

        private void OnInteractStop(InputAction.CallbackContext context)
        {
            currentInputs.InteractDown = context.ReadValueAsButton();
        }
        
        private void OnSurgeJumpStart(InputAction.CallbackContext context)
        {
            currentInputs.SurgeJumpDown = context.ReadValueAsButton();
        }

        private void OnSurgeJumpStop(InputAction.CallbackContext context)
        {
            currentInputs.SurgeJumpDown = context.ReadValueAsButton();
        }
        
        private void OnRecenterCameraPerformed(InputAction.CallbackContext context)
        {
            CameraStateMachine.Instance.OnCameraResetStart(context);
        }

        private void OnRecenterCameraStop(InputAction.CallbackContext context)
        {
            CameraStateMachine.Instance.OnCameraResetEnd(context);
        }

        private void OnEnable()
        {
            // if there is a main menu, don't do this please.
            // i am sorry little one for writing this code - John (Evil Thanos 10 PM John)
            if (!FindObjectOfType<MainMenu>())
            {
                EnableCharacterControls();
            }
            
            // Resume += OnResume;
            Pause += OnPause;
        }

        private void OnDisable()
        {
            // Resume -= OnResume;
            SwitchCurrentInputState(PlayerInputState.Nothing);
            Pause -= OnPause;
        }

        private void OnDestroy()
        {
            playerInputActions.UI.Pause.canceled -= OnPausePressed;
            // InputSystem.onDeviceChange -= OnDeviceChange;

        }

        //Input map switcher
        public delegate void PauseDelegate();

        public PauseDelegate Pause;

        public delegate void ResumeDelegate();

        public ResumeDelegate Resume;

        public void EnableCharacterControls()       // Public for UI
        {
            SwitchCurrentInputState(PlayerInputState.Character);
        }

        public void EnableUIControls()      // Public for UI
        {
            SwitchCurrentInputState(PlayerInputState.UI);
        }

        public void EnableCritterInteractControls()
        {
            SwitchCurrentInputState(PlayerInputState.CritterInteract);
        }
        public void SwitchCurrentInputState(PlayerInputState state)
        {
#if DEBUG
            if (Constants.DebugPlayerInputStateChanges)
            {
                Debug.Log("PlayerInput Switching from: " + currInputState.ToString() + " to " + state.ToString());
            }
#endif
            if (currInputState == state)
            {
                return;
            }
            
            switch (currInputState)
            {
                case PlayerInputState.Character:
                    playerInputActions.CharacterControls.Disable();
                    break;
                case PlayerInputState.CritterInteract:
                    playerInputActions.CritterTalkingControls.Disable();
                    break;
                case PlayerInputState.UI:
                    playerInputActions.UI.Disable();
                    break;
                case PlayerInputState.SlideShowControls:
                    playerInputActions.SlideShowControls.Disable();
                    break;
            }
            
            currInputState = state;
            
            switch (currInputState)
            {
                case PlayerInputState.Character:
                    playerInputActions.CharacterControls.Enable();
                    break;
                case PlayerInputState.CritterInteract:
                    playerInputActions.CritterTalkingControls.Enable();
                    break;
                case PlayerInputState.UI:
                    playerInputActions.UI.Enable();
                    break;
                case PlayerInputState.SlideShowControls:
                    playerInputActions.SlideShowControls.Enable();
                    break;
            }
        }
        private void EnableLevelLoaderUIControls()
        {
            playerInputActions.LeveLoaderUI.Enable();
        }

        private void DisableLevelLoaderUIControls()
        {
            playerInputActions.LeveLoaderUI.Disable();
        }

        private void OnPausePressed(InputAction.CallbackContext context)
        {
            if(!UIManager.Instance.canPauseGame) { return; }
            if(UIManager.Instance.WinScreenActive()) { return; }    // Don't do pause if win interface is active

            OnPause();
        }

        private void OnResumePressed(InputAction.CallbackContext context)
        {
            Resume.Invoke();
        }
        private void OnPause()
        {
            //Switch to levelLoad Input Map
            //SwitchInputMap(InputMap.LevelLoaderUI);   
            if (UIManager.Instance.isPaused) {
                UIManager.Instance.UnpauseGame();
                if (!SettingsUI.Instance.settingsActive) { 
                    EnableCharacterControls();
                }
            }
            else {
                UIManager.Instance.PauseGame();
                EnableUIControls();
            }
        }

        private void OnResume()
        {
            //switch to charactercontrol input
            // SwitchInputMap((InputMap.Character));
            UIManager.Instance.UnpauseGame();
        }

        private void OnDisplayTimer(InputAction.CallbackContext context)
        {
            Debug.Log("F4 Pressed");
            UIManager.Instance.DisplayTimer();
        }
        private void OnTimerReset(InputAction.CallbackContext context)
        {
            Debug.Log("F6 Pressed");
            UIManager.Instance.RestartTime();
        }
        private void OnTimerStartStop(InputAction.CallbackContext context)
        {
            // Debug.Log("F5 Pressed");
            // UIManager.Instance.StartStopTime();
        }

        private void ClearPlayerInput(InputAction.CallbackContext context)
        {
            currentInputs.MoveInput = Vector3.zero;
            if (CritterContinueInteraction != null)
            {
                CritterContinueInteraction.Invoke();
            }

        }
        private void CancelCritterInteract(InputAction.CallbackContext context)
        {
            if (CancelCritterInteraction != null)
            {
                CancelCritterInteraction.Invoke();
            }
        }

        private void StartHoldSkip(InputAction.CallbackContext context)
        {
            if (StartHoldingSkip != null)
            {
                StartHoldingSkip.Invoke();
            }
        }
        
        private void StopHoldSkip(InputAction.CallbackContext context)
        {
            if (StopHoldingSkip != null)
            {
                StopHoldingSkip.Invoke();
            }
        }

        private void GetFirstKeyPressed(InputAction.CallbackContext context)
        {
            if(!GlobalSettings.Instance.firstKeyPressed)
            {
                GlobalSettings.Instance.FirstKeyPressed();
            }
        }
    }
}
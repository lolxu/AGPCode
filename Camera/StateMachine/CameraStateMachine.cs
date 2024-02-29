using System;
using System.Collections;
using System.ComponentModel;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine.Subroutines;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace __OasisBlitz.Camera.StateMachine
{
    public class CameraStateMachine : MonoBehaviour
    {
        /// <summary>
        /// If KeyPoint, the camera is focusing on a specific point in the world.
        /// If Velocity, the camera is focusing towards a point in front of the player based on their velocity.
        /// </summary>
        public enum AssistMode
        {
            KeyPoint,
            Velocity
        }
        
        public CameraBaseState CurrentState { get; set; }
        
        private CameraStateFactory states;

        // The camera can observe the player state machine to make decisions.
        // It should NOT be modifying anything, only reading.
        public PlayerStateMachine playerStateMachine;
        
        public RigChanger rigChanger;
        public FreelookAdjuster freelookAdjuster;
        public GameObject CameraSurface;
        public SandCameraCollider sandCameraCollider;
        
        [Header("Rigs")]
        public CameraRigValues surfaceRig;
        public CameraRigValues diveSmallPenetrableRig;
        public CameraRigValues diveLargePenetrableRig;
        public CameraRigValues noDiveLargePenetrableRig;
        public CameraRigValues fullLookAtTargetRig;

        [Header("Layer Masks")] 
        public LayerMask surfaceMask;
        public LayerMask smallPenetrableMask;
        public LayerMask largePenetrableMask;

        [Header("Camera Reference")] 
        public CinemachineCamera freeLookCam;
        public CinemachineCamera cinematicsCam;

        [Header("Restart Related")]
        private CinemachineOrbitalFollow _orbitalFollow;
        private CinemachineInputAxisController _inputAxisController;

        [Header("Cinematics Camera Routine")] 
        public CinematicsCameraSubroutine cinematicsCamRoutine;
        public bool isLoadRestart = false;

        [Header("Target Camera Extras")] 
        [Description("This transform can be positioned via code to set the look target, it should be a child of camera " +
                     "setup but not a direct child of the player")]
        public Transform targetLookTransform;
        
        [Header("Camera Assist")]
        public float AssistDelay = 1f;
        public float AssistDelayAfterLowVelocity = 0.5f;
        private float AssistTimer = 0f;
        private float VelocitySpecificAssistTimer = 0f;
        public AssistMode assistMode = AssistMode.Velocity;
        
        /// <summary>
        /// If assisting is true, the camera will perform automated rotations to frame things better.
        /// Inputs will disable assisting, and it will then enable again after a short delay, or after inputting camera
        /// reset.
        /// </summary>
        private bool assisting;

        private bool velocityAssistAllowed;

        public static CameraStateMachine Instance;

        private bool RequiresNewResetCameraPress = false;

        private Vector2 _lookInput = Vector2.zero;
        
        
        // moving camera tweens:
        private Tween HorizontalCameraTween = null;
        private Tween VerticalCameraTween = null;
        
        [Header("Dive Camera Settings")]
        public float DiveCameraVelocity = 30f;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            
            surfaceRig.InitArray();
            diveSmallPenetrableRig.InitArray();
            diveLargePenetrableRig.InitArray();
            noDiveLargePenetrableRig.InitArray();
            fullLookAtTargetRig.InitArray();
            
            // setup state
            states = new CameraStateFactory(this);
            CurrentState = states.SurfaceDefault();
            CurrentState.EnterState();

            _orbitalFollow = freeLookCam.GetComponent<CinemachineOrbitalFollow>();
            _inputAxisController = freeLookCam.GetComponent<CinemachineInputAxisController>();
            ResetHorizontalAxis();
            ResetVerticalAxis();

            SceneManager.sceneLoaded += SwitchToCinematicsCameraOnSceneLoaded;
        }

        private void SwitchToCinematicsCameraOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CurrentState.ExitState();
            CurrentState = states.LevelPanCamera();
            CurrentState.EnterState();
            Debug.Log("Cinematics on scene loaded");
            if (cinematicsCamRoutine)
            {
                StartCoroutine(cinematicsCamRoutine.CinematicsCameraRoutine(0));
            }
            else
            {
                Debug.LogWarning("Cinematics Routine not set");
            }
            
        }

        public void SwitchToCinematicsCamera(int type)
        {
            CurrentState.ExitState();
            CurrentState = states.LevelPanCamera();
            CurrentState.EnterState();
            StartCoroutine(cinematicsCamRoutine.CinematicsCameraRoutine(type));
        }

        public void ResetVerticalAxis()
        {
            if (_orbitalFollow != null)
            {
                _orbitalFollow.VerticalAxis.Value = _orbitalFollow.VerticalAxis.Center;
            }
        }

        public void ResetHorizontalAxis()
        {
            if (_orbitalFollow != null)
            {
                // get diff in horizontal rotation between the rotator and the positionindicator
                
                _orbitalFollow.HorizontalAxis.Value = playerStateMachine.ModelRotator.transform.eulerAngles.y;
            }
        }
        public void SetHorizontalAxis(float yaw)
        {
            if (_orbitalFollow != null)
            {
                // get diff in horizontal rotation between the rotator and the positionindicator
                
                _orbitalFollow.HorizontalAxis.Value = yaw;
            }
        }

        /// <summary>
        /// Check if the player velocity is high enough to automatically rotate the camera -- only considers lateral velocity
        /// </summary>
        /// <returns></returns>
        private bool IsVelocityHighEnoughForAssist()
        {
            Vector3 velocity = playerStateMachine.PlayerPhysics.Velocity;
            velocity.y = 0;
            return velocity.magnitude > freelookAdjuster.minVelocityToLookAtTarget;
        }

        private void UpdateAssistTimer()
        {
            
            if (AssistTimer > 0)
            {
                AssistTimer -= Time.deltaTime;
            }
            else
            {
                assisting = true;
            }
            
            if (IsPlayerMovingCamera())
            {
                assisting = false;
                AssistTimer = AssistDelay;
            }
        }

        private void UpdateVelocityAssistTimer()
        {
            if (VelocitySpecificAssistTimer > 0)
            {
                VelocitySpecificAssistTimer -= Time.deltaTime;
            }
            else
            {
                velocityAssistAllowed = true;
            }
            
            if (!IsVelocityHighEnoughForAssist())
            {
                velocityAssistAllowed = false;
                VelocitySpecificAssistTimer = AssistDelayAfterLowVelocity;
            }
        }

        private void UpdateAssist()
        {
            UpdateAssistTimer();
            UpdateVelocityAssistTimer();

            if (assisting)
            {
                if (assistMode == AssistMode.KeyPoint)
                {
                    freelookAdjuster.UpdateLookdirection();
                }
                else if (assistMode == AssistMode.Velocity && velocityAssistAllowed)
                {
                    if (IsVelocityHighEnoughForAssist())
                    {
                        freelookAdjuster.SetCurrentLookPoint(playerStateMachine.transform.position +
                                                             playerStateMachine.PlayerPhysics.Velocity.normalized *
                                                             10f);
                        freelookAdjuster.UpdateLookdirection();
                    }
                }
            }
        }

        public void UpdateStates()
        {
            CurrentState.UpdateStates();
            UpdateAssist();
        }

        public void SetLookValue(Vector2 LookInput)
        {
            _lookInput = LookInput;
        }

        private bool IsPlayerMovingCamera()
        {
            return _lookInput.magnitude > .7f;
        }

        public void OnCameraResetStart(InputAction.CallbackContext context)
        {
            // check if player is already using input:
            
            // if not and able, reset camera
            if (!RequiresNewResetCameraPress)
            {
                ResetCamera();
            }
            
            RequiresNewResetCameraPress = true;
        }

        public void OnCameraResetEnd(InputAction.CallbackContext context)
        {
            RequiresNewResetCameraPress = false;
        }
        
        public void ResetCamera()
        {
            // todo: only reset camera if no right stick input
            
            // reset camera
            //ResetHorizontalAxis();
            // ResetVerticalAxis();

            Vector2 NewCamValues = new Vector2();
            NewCamValues.x = playerStateMachine.ModelRotator.transform.eulerAngles.y;
            NewCamValues.y = _orbitalFollow.VerticalAxis.Center;
            
            AlignCameraSmoothly(NewCamValues, .25f);
        }

        public Vector2 CamValuesFromDirection(Vector3 direction)
        {
            // TODO: Determine freelook parameters based on the desired viewing direction
            return Vector2.zero;
        }

        /*
         * Aligns the surface camera to the desired "forward vector" over time time.
         * Note, Forward is a vector 2 with
         *  - x = the new euler angle, in world space, of the horizontal (yaw) of the camera
         *  - y = the new %, from 0-1, between the camera's bottom and top of it's orbit
         * todo: find a more intuitive way of expressing y in Forward
         * This method uses closest-path interpolation to get to it's desired target rotation
         */
        public void AlignCameraSmoothly(Vector2 Forward, float time, bool killOnMove = false)
        {
            // kill existing camera tweens:
            HorizontalCameraTween?.Kill();
            VerticalCameraTween?.Kill();
            
            // what is the target horizontal axis value? determined by caller.
            float targetHorizontal = Forward.x;

            targetHorizontal %= 360;
            targetHorizontal = targetHorizontal > 180 ? targetHorizontal - 360 : targetHorizontal;
            
            // find closest "equivalent"
            float alternative = targetHorizontal < 0 ? targetHorizontal + 360 : targetHorizontal - 360;
            if (Mathf.Abs(alternative - _orbitalFollow.HorizontalAxis.Value)
                < Mathf.Abs(targetHorizontal - _orbitalFollow.HorizontalAxis.Value))
            {
                targetHorizontal = alternative;
            }
            
            // todo: convert this number to a closest equal representation 0-360.
            HorizontalCameraTween = DOTween.To(() => _orbitalFollow.HorizontalAxis.Value,
                x => _orbitalFollow.HorizontalAxis.Value = x, targetHorizontal, time).SetEase(
                Ease.InOutSine
            );
            
            VerticalCameraTween = DOTween.To(() => _orbitalFollow.VerticalAxis.Value,
                x => _orbitalFollow.VerticalAxis.Value = x, Forward.y, time).SetEase(
                Ease.InOutSine
            );
        }

        public void SetToLookAtTarget(Vector3 targetPosition)
        {
            // Whenever this is enabled, force enable assisting
            AssistTimer = -0.1f;
            assisting = true;
            
            assistMode = AssistMode.KeyPoint;
            
            freelookAdjuster.SetCurrentLookPoint(targetPosition);
        }

        public void SetToLookAtVelocity()
        {
            // Upon setting to this, also instantly enable it
            VelocitySpecificAssistTimer = -0.1f;
            velocityAssistAllowed = true;
            
            assistMode = AssistMode.Velocity;
        }
        
        private void Update()
        {
            if (IsPlayerMovingCamera())
            {
                HorizontalCameraTween?.Kill();
                VerticalCameraTween?.Kill();
            }
        }

        /// <summary>
        /// Function to skip cinematics
        /// </summary>
        public void StopCameraCinematics()
        {
            cinematicsCamRoutine.StopCameraPan();
        }
        
    }
}

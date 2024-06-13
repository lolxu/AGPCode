using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.__Scripts.Player.Environment.Fruits;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Enemy;
using __OasisBlitz.Player.Animation;
using __OasisBlitz.Player.Gauntlets;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace __OasisBlitz.Player.StateMachine
{
    [RequireComponent(typeof(DrillChecker))]
    [RequireComponent(typeof(PlayerPhysics))]
    [RequireComponent(typeof(PlayerAudio))]
    [RequireComponent(typeof(PlayerFeedbacks))]
    public class PlayerStateMachine : MonoBehaviour
    {
        // ************************************ CORE COMPONENTS ************************************
        public CharacterController CharacterController { get; set; }
        public DrillChecker DrillChecker { get; set; }
        public BanditAnimationController BanditAnimationController;
        public PlayerAudio PlayerAudio { get; private set; }
        public PlayerPhysics PlayerPhysics;
        public PlayerFeedbacks PlayerFeedbacks { get; private set; }
        public GauntletManager GauntletManager { get; private set; }

        public LevelNames LevelNames;
        
        // ************* ASSIGNED IN INSPECTOR *******************
        public ModelRotator ModelRotator;
        public TrailController TrailController;
        public BlastAbility BlastAbility;
        public TargetedDash TargetedDash;
        public DrillBehavior Drill;
        public ModelSquasher ModelSquasher;

        public CameraStateMachine cameraStateMachine;

        public DustSlideTrail DustTrail;

        public LevelSettings defaultSettings;

        // drilling variables
        private float drillCooldown = 0.5f;

        // state variables
        private PlayerStateFactory states;

        public Vector3 LastMovedWorldDirection { get; private set; }

        public bool Drilling { get; set; }

        public bool DrillReleased { get; set; } = true;

        public bool DrillLocked { get; private set; } = false;

        public GameObject DrillMesh;
        public GameObject DrillMeshInstance { get; private set; } = null;

        public bool IsSubmerged { get; set; }

        public bool IsSliding { get; set; }

        public bool DeathByBarrier { get; private set; } 
        
        public float TimeTillDeathFadeToBlack;

        public bool IsInLargeTerrain
        {
            get => DrillChecker.IsInsideLargePenetrable;
        }

        public BaseState CurrentState { get; set; }

        public bool IsDead { get; set; } = false;

        [field: SerializeField]
        public float InitialJumpVelocity { get; set; } = 35.0f;
        
        [field: SerializeField]
        public float SlideVerticalJumpVelocity { get; set; } = 35.0f;
        
        [field: SerializeField]
        public float SlideForwardJumpVelocity { get; set; } = 35.0f;
        
        [field: SerializeField]
        public float MaxJumpVelocity { get; set; } = 35.0f;
        

        [field: SerializeField]
        public float InitialDrillJumpVelocity { get; set; } = 35.0f;

        public float OrgInitialJumpVelocity { get; } = 25.0f;

        public bool IsMovementPressed { get; private set; }

        public bool DrillRequested { get; private set; }
        public bool RequireNewJumpPress { get; set; } = false;
        public bool RequireNewTargetedDashPress { get; set; } = false;
        public bool RequireNewDrillPressOrEndGrounded { get; set; } = false;

        public bool IsJumping { set; get; } = false;

        public bool JumpRequested { get; private set; } = false;
        
        public bool TargetedDashRequested { get; private set; } = false;

        public bool PressingDrill = false;
        
        public Vector3 MovementInput { get; private set; } = Vector3.zero;
        public Vector2 MovementInputRaw { get; private set; } = Vector2.zero;

        public bool JumpPressedLastFrame { get; private set; } = false;
        
        public bool RecenterCameraRequested { get; set; } = false;
        public bool RequiresNewRecenterCameraPress { get; set; } = false;

        public bool InteractKeyRequested { get; private set; } = false;

        public bool RequireNewInteractKeyPress { get; set; } = false;

        public float WalkSpeed { get; } = 100.0f;

        public float DrillSpeed { get; } = 100.0f;

        public float DrillingTimescale = 0.8f;

        public bool ToggleDrill = true;
        
        public bool ToggleJump = true;

        public bool ToggleTargetedDash = true;

        public bool ToggleWalk = true;

        public bool ToggleSlide = true;

        public bool ToggleWalkOnlyStopsDrill = false;

        public bool bForceDrillDown = false;

        public bool bInvincible = false;

        public bool bForceDrillAboveGravity = false;

        [HideInInspector] public bool OnSlipperySurface = false;
        
        private bool GrappleDownLastFrame = false;

        public bool InWaterTrigger = false;

        public bool IsCelebrating = false;

        /// <summary>
        /// The current movement input from the player, in world space.
        /// (That is, already transformed by the camera's rotation.)
        /// </summary>
        public Vector3 CurrentMovementInputWorld { get; private set; }

        // Awake is called earlier than Start in Unity's event life cycle
        void Awake()
        {
            Drilling = false;

            // initially set reference variables
            CharacterController = GetComponent<CharacterController>();
            DrillChecker = GetComponent<DrillChecker>();
            PlayerAudio = GetComponent<PlayerAudio>();
            PlayerFeedbacks = GetComponent<PlayerFeedbacks>();
            PlayerPhysics = GetComponent<PlayerPhysics>();
            GauntletManager = GetComponent<GauntletManager>();

            // DrillMeshInstance = Instantiate(DrillMesh);
            // DrillMeshInstance.SetActive(false);
            
            // setup state
            states = new PlayerStateFactory(this);
            CurrentState = states.Grounded();
            CurrentState.EnterState();
        }

        private void InitializeAbilities(Scene arg0, LoadSceneMode loadSceneMode)
        {
            LevelSettingsManager perLevelSettings = FindObjectOfType<LevelSettingsManager>();
            if (perLevelSettings == null
                || perLevelSettings.settings == null)
            {
                defaultSettings.InitializePlayerAbilities(this);
            }

            // Overriding blast things
            // BounceAbility.Instance.BounceEnabled = XMLFileManager.Instance.GetBlastStatus();
            // BLAST IS ALWAYS ENABLED
            BounceAbility.Instance.BounceEnabled = true;
        }

        private IEnumerator YieldToInit()
        {
            
            yield return null;
            
            
        }

        private void Start()
        {
            // set the player input callbacks
            // callback for footstep audio
        }

        private void OnEnable()
        {
            //BanditAnimationController.proceduralRun.OnFootPlant += () => PlayerAudio.PlayFootstep(PlayerPhysics.CurrentOnSurfaceType);
            SceneManager.sceneLoaded += InitializeAbilities;
        }

        private void OnDisable()
        {
            // BanditAnimationController.proceduralRun.OnFootPlant -= () => PlayerAudio.PlayFootstep(PlayerPhysics.CurrentOnSurfaceType);
            SceneManager.sceneLoaded -= InitializeAbilities;
        }

        /// <summary>
        /// This is called in Update so as to not miss any inputs
        /// </summary>
        /// <param name="inputs"></param>
        private void SetInputs(PlayerInput.PlayerCharacterInputs inputs)
        {
            if (ToggleWalk)
            {
                IsMovementPressed = inputs.IsMovementPressed;
                MovementInput = inputs.MoveInput;
                MovementInputRaw = inputs.MoveInputRaw;
            }
            // A little bit of jumping input logic
            if (inputs.JumpDown && ToggleJump)
            {
                JumpRequested = true;
            }
            if (!JumpRequested)
            {
                RequireNewJumpPress = false;
            }
            
            

            if (inputs.TargetedDashDown 
                && ToggleTargetedDash
                && !GrappleDownLastFrame)
            {
                TargetedDashRequested = true;
            }
            if (!inputs.TargetedDashDown)
            {
                RequireNewTargetedDashPress = false;
            }

            GrappleDownLastFrame = inputs.TargetedDashDown;

            if (inputs.SurgeJumpDown)
            {
                SurgeJump.Instance.SurgeJumpRequested = true;
            }

            if (!inputs.SurgeJumpDown)
            {
                SurgeJump.Instance.RequiresNewSurgeJump = false;
            }
            
            if (inputs.RecenterCameraDown)
            {
                RecenterCameraRequested = true;
            }

            if (!inputs.RecenterCameraDown)
            {
                RequiresNewRecenterCameraPress = false;
            }

            if (inputs.DrillDown || bForceDrillDown)
            {
                DrillRequested = true;
                if (!ToggleWalkOnlyStopsDrill)
                {
                    RequireNewDrillPressOrEndGrounded = false;
                }
            }
            else
            {
                RequireNewDrillPressOrEndGrounded = false;
            }

            // player is PRESSING the drill button, not necessarily forced to drill, used for tutorial
            PressingDrill = inputs.DrillDown;

            if (inputs.InteractDown)
            {
                // FruitsManager.Instance.RequestInteractFruit = true;
                InteractKeyRequested = true;
            }

            if (!inputs.InteractDown)
            {
                // FruitsManager.Instance.RequireNewInteractFruit = false;
                RequireNewInteractKeyPress = false;
            }

        }

        public void UpdateStates(PlayerInput.PlayerCharacterInputs inputs)
        {
            SetInputs(inputs);
            CurrentState.UpdateStates();
            ResetInputs();
        }


        /// <summary>
        /// We want to reset certain instantanious inputs, such as jump, every time the state machine updates.
        /// </summary>
        private void ResetInputs()
        {
            JumpRequested = false;
            TargetedDashRequested = false;
            DrillRequested = false;
            RecenterCameraRequested = false;
            MovementInput = Vector3.zero;
            MovementInputRaw = Vector2.zero;
            InteractKeyRequested = false;
            // FruitsManager.Instance.RequestInteractFruit = false;
        }

        public void HandleCollision(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            CurrentState.HandleCollision(ref coll, hitNormal, hitPoint);
        }

        public void HandleGroundHit(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            CurrentState.HandleGroundHit(ref coll, hitNormal, hitPoint);
        }

        public bool ShouldObeyGroundSnap()
        {
            return CurrentState.ShouldObeyGroundSnap();
        }

        public void InstantKill()
        {
            if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            if (bInvincible)
            {
                return;
            }
            
            Debug.Log("Instant kill");
            if (!IsDead)
            {
                CurrentState.ExitStates();
                CurrentState = states.Dead();
                CurrentState.EnterState();
            }
            
        }
        public void InstantKillByDeathBarrier()
        {
            if (DebugCommandsManager.Instance.godModeStatus()) { return; }       // If God Mode is on -- do not kill
            if (bInvincible)
            {
                return; 
            }
            
            if (!IsDead)
            {
                DeathByBarrier = true;
                CurrentState.ExitStates();
                CurrentState = states.Dead();
                CurrentState.EnterState();
                StartCoroutine(DeathByBarrierWait());
            }
            
        }
        
        private IEnumerator DeathByBarrierWait()
        {
            yield return new WaitForSeconds(TimeTillDeathFadeToBlack);
            DeathByBarrier = false;
        }

        /// <summary>
        /// Disables All Character Related Movements
        /// </summary>
        public void DisableCharacterMovements()
        {
            ToggleDrill = false;
            ToggleWalk = false;
            ToggleJump = false;
        }
        
        public void EnableCharacterMovements()
        {
            ToggleDrill = true;
            ToggleWalk = true;
            ToggleJump = true;
        }

        public void ForceEnterDrillState()
        {
            Debug.Log("Force enter Drill State");
            ToggleDrill = true;
            bForceDrillDown = true;
            bInvincible = true;
            bForceDrillAboveGravity = true;
            if (SceneManager.GetActiveScene().name == LevelNames.WinningSceneName)
            {
                XMLFileManager.Instance.SaveEndingCutsceneNeedToView();
            }
            AudioManager.instance.PlayOneShot(FMODEvents.instance.levelEndDrillDown);
            StartCoroutine(UnsetForceDrill());
        }

        private IEnumerator UnsetForceDrill()
        {
            yield return new WaitForSeconds(1.0f);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.resetDrill);
            bForceDrillDown = false;
            bForceDrillAboveGravity = false;
        }

        public void PlayLeftWalkParticles()
        {
            if (!InWaterTrigger)
            {
                FeelEnvironmentalManager.Instance
                    .PlayWalkFeedback(ModelRotator.leftFootTransform.position, 1.0f);
            }
        }

        public void PlayRightWalkParticles()
        {
            if (!InWaterTrigger)
            {
                FeelEnvironmentalManager.Instance
                    .PlayWalkFeedback(ModelRotator.rightFootTransform.position, 1.0f);
            }
        }
    }
}
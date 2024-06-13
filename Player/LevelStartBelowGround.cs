using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Camera.StateMachine.Subroutines;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelStartBelowGround : MonoBehaviour
{
    public Action OnEmergeFromGround;
    
    private PlayerStateMachine _ctx;

    public Transform InitialStartPoint;
    public MMF_Player _shakeplayer;
    public ParticleSystem _drillParticles;
    public float TimeToHoldDownDrill = 5.0f;
    public float shakeAmount = 3.0f;
    public CinemachineCamera _startCamera;
    
    public DrillLockedBelowSound drillLockedBelowSound;

    public DrillOutAfterPeriod _DrillOutAfterPeriod;
    
    public bool lockInputOnEmerge;
    public bool instantCameraTransition;
    public float StartForwardsVelocity = 60.0f;
    public float StartVerticalVelocity = 10f;
    
    private MainMenu mainMenu;
    Sequence FadeInText;
    private CinematicsCameraSubroutine subroutine;
    private bool bCanExit = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        _ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        _startCamera.gameObject.SetActive(false);
        
    }

    private void Start()
    {
        CameraStateMachine.Instance.OnCinematicsOver += OnCinematicsOver;
        
    }

    public void OnCinematicsOver()
    {
        
        StartFadeInTween();
        LockPlayerBelowGround();
        
        // lock player to position
        
        // make it possible to drill out
        DOVirtual.DelayedCall(.4f, ()=> bCanExit = true, false);
        // enable camera
        _startCamera.gameObject.SetActive(true);
    }
    
    public void StartFadeInTween()
    {
        FadeInText?.Kill();
        
        FadeInText = DOTween.Sequence();
        FadeInText.AppendInterval(2.0f);
        FadeInText.Append(_DrillOutAfterPeriod._canvasGroup.DOFade(1.0f, 2.0f));

        FadeInText.OnKill(() => {
            _DrillOutAfterPeriod._canvasGroup.DOFade(0.0f, 0.5f);
        });
    }

    private void OnDisable()
    {
        CameraStateMachine.Instance.OnCinematicsOver -= OnCinematicsOver;
    }
    
    /// <summary>
    /// If a scene has several possible start points, it can call this function to set the point and camera that it desires.
    /// If there's only one, it can also just be assigned in the inspector.
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="startCamera"></param>
    public void SetStartPositionAndCamera(Transform startPoint, CinemachineCamera startCamera)
    {
        InitialStartPoint = startPoint;
        _startCamera = startCamera;
    }

    private bool bEmergedFromGround = false;

    private void EmergeFromGround()
    {
        if (lockInputOnEmerge)
        {
            PlayerInput pInput = _ctx.GetComponent<PlayerInput>();
            pInput.EnableCritterInteractControls();
        }
        
        //stop drill sound
        drillLockedBelowSound.StopTutorialDrillSound();
        
        // escaped the hold down drill! release player into the game
        _startCamera.gameObject.SetActive(false);
        // pInput.EnableCharacterControls();
        _ctx.bForceDrillDown = false;
        // _ctx.PlayerPhysics.SetVelocity(Vector3.zero);
        _ctx.PlayerPhysics.SetVelocity(InitialStartPoint.forward * StartForwardsVelocity + InitialStartPoint.up * StartVerticalVelocity);
        
        _ctx.PlayerAudio.bDrillSoundDisabled = false;
        _ctx.PlayerAudio.StartDrill();

        // start timer 
        UIManager.Instance.SetTimer();
        // Destroy(gameObject);
    }

    public void LockPlayerBelowGround()
    {
        _DrillOutAfterPeriod._canvasGroup.alpha = 0.0f;
        StartCoroutine(LockPlayerBelowGroundRoutine());
    }
    
    private IEnumerator LockPlayerBelowGroundRoutine()
    {
        _ctx.GetComponent<PlayerAudio>().bDrillSoundDisabled = true;


        Vector3 Lockpos = InitialStartPoint.position;
        MMF_CinemachineImpulse _shakesource = _shakeplayer.GetFeedbackOfType<MMF_CinemachineImpulse>();
        // pInput.DisableCharacterControls();
        _ctx.bForceDrillDown = true;
        var emission = _drillParticles.emission;
        emission.rateOverTime = 0.0f;

        float timer = 0.0f;
        while (timer <= TimeToHoldDownDrill)
        {
            float lerpDownSpeed = 15f;
            if (_ctx.PressingDrill
                && bCanExit)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer -= Time.deltaTime;
                timer = Mathf.Max(timer, 0.0f);
            }
            
            // lerp amount of particles as percent reached to timer
            float pct = timer / TimeToHoldDownDrill;
            emission.rateOverTime = Mathf.Lerp(0.0f, 30.0f, pct);
            
            // shake the camera based on hold down percent
            float shakePct = timer / TimeToHoldDownDrill;
            _shakesource.Velocity = shakeAmount * shakePct * Vector3.one;
            _shakeplayer.PlayFeedbacks();
            
            _ctx.CharacterController.SetPosition(Lockpos);
            
            //Set drill sound intensity
            drillLockedBelowSound.SetTutorialDrillVolume(pct);
                
            yield return null;
        }

        if (FadeInText != null)
        {
            FadeInText.Kill(false);
        }

        emission.rateOverTime = 0.0f;
        
        EmergeFromGround();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using Sirenix.Utilities;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

namespace __OasisBlitz.Camera.StateMachine.Subroutines
{
    public class CinematicsCameraSubroutine : MonoBehaviour
    {
        [Header("Pan Camera Settings")]
        public CameraStateMachine Ctx;
        private SplineContainer currentDolly;
        private SplineContainer plantCollectionDolly;
        
        private CinematicsType currentCinematics;

        public static Action DrillDownOver;
        
        private void Awake()
        {
            SceneManager.sceneLoaded += HideBandit;
        }

        private void HideBandit(Scene arg0, LoadSceneMode arg1)
        {
            //Hide Bandit Model
            Ctx.playerStateMachine.ModelRotator.HideBandit();
        }

        /// <summary>
        /// Function to play camera cinematics
        /// </summary>
        /// <param name="type"> Use the enum bruh </param>
        /// <returns></returns>
        public IEnumerator CinematicsCameraRoutine(CinematicsType type)
        {
            yield return null;
            currentCinematics = type;
            
            var startDollies = GameObject.FindGameObjectsWithTag("StartLevelDolly");

            Debug.Log($"Playing Animation type: {type}");
            CinemachineSplineDolly cineSpline = Ctx.currentActiveCinematicsCam.GetComponent<CinemachineSplineDolly>();

            switch (type)
            {
                case CinematicsType.StartPan:
                    if (startDollies.Length > 0)
                    {
                        Ctx.currentActiveCinematicsCam.Target.LookAtTarget = null;
                        Ctx.currentActiveCinematicsCam.Target.TrackingTarget = null;
                        // Play Cinematics
                        StartCoroutine(PlayStartingCinematics(startDollies, 2.5f));
                    }
                    else
                    {
                        StartCoroutine(StartPlayerActions(true));
                        // Debug.LogError("Start level Cinematics not set");
                    }
                    break;
                
                case CinematicsType.PlantPan:
                    cineSpline.Spline = null;
                    // Ctx.currentActiveCinematicsCam.transform.localPosition = Ctx.freeLookCam.transform.localPosition;
                    Transform playerTransform = Ctx.playerStateMachine.ModelRotator.transform;
                    Transform camTransform = Ctx.currentActiveCinematicsCam.transform;
                    camTransform.position = playerTransform.position;
                    camTransform.position -= playerTransform.forward * 5.0f;
                    camTransform.position += new Vector3(0.0f, 1.0f, 0.0f);
                    // Vector3 pos = camTransform.position;
                    // pos.y = 2.0f;
                    // camTransform.position = pos;
                    
                    Ctx.currentActiveCinematicsCam.Target.TrackingTarget =
                        Ctx.playerStateMachine.gameObject.transform;
                    Ctx.currentActiveCinematicsCam.Target.LookAtTarget =
                        Ctx.playerStateMachine.gameObject.transform;
                    
                    cineSpline.CameraRotation = CinemachineSplineDolly.RotationMode.Default;
                    
                    // Setting player to celebrating
                    Ctx.playerStateMachine.IsCelebrating = true;
                    
                    //Debug.Log("Playing plant collection");
                    Ctx.currentActiveCinematicsCam.gameObject.SetActive(true);
                    break;
                
                case CinematicsType.DeathPan:
                    cineSpline.Spline = null;
                    Ctx.currentActiveCinematicsCam.transform.localPosition = Ctx.freeLookCam.transform.localPosition;
                    Ctx.currentActiveCinematicsCam.Target.TrackingTarget = Ctx.playerStateMachine.gameObject.transform;
                    Ctx.currentActiveCinematicsCam.Target.LookAtTarget = Ctx.playerStateMachine.gameObject.transform;
                    Ctx.currentActiveCinematicsCam.gameObject.SetActive(true);

                    StartCoroutine(BackToNormal());
                    break;
                default:
                    break;
            }
        }

        private IEnumerator BackToNormal()
        {
            yield return new WaitForSeconds(2.0f);

            StartCoroutine(StartPlayerActions(true));
        }

        private IEnumerator PlayStartingCinematics(GameObject[] startDollies, float timeForCinematics)
        {
            // For multiple cinematics cameras and lookats
            GameObject[] lookAtTargets = GameObject.FindGameObjectsWithTag("SetPiece");
            
            Array.Sort(lookAtTargets, (a, b) => a.name.CompareTo(b.name));
            Array.Sort(startDollies, (a, b) => a.name.CompareTo(b.name));
            
            Debug.Log("Set Piece Count: " + lookAtTargets.Length);
            
            for (int i = 0; i < startDollies.Length; i++)
            {
                GameObject dolly = startDollies[i];
                currentDolly = dolly.GetComponent<SplineContainer>();
                CinemachineSplineDolly currentCSD = Ctx.currentActiveCinematicsCam.GetComponent<CinemachineSplineDolly>();
                currentCSD.Spline = currentDolly;
                
                GameObject curLookTarget = null;
                if (i < lookAtTargets.Length)
                {
                    curLookTarget = lookAtTargets[i];
                    Ctx.currentActiveCinematicsCam.Target.LookAtTarget = curLookTarget.transform;
                    Ctx.currentActiveCinematicsCam.Target.TrackingTarget = curLookTarget.transform;
                    currentCSD.CameraRotation = CinemachineSplineDolly.RotationMode.Default;
                }
                else
                {
                    Debug.Log("No Look At Target");
                    Ctx.currentActiveCinematicsCam.Target.LookAtTarget = null;
                    Ctx.currentActiveCinematicsCam.Target.TrackingTarget = null;
                    currentCSD.CameraRotation = CinemachineSplineDolly.RotationMode.SplineNoRoll;
                }
                
                Ctx.currentActiveCinematicsCam.gameObject.SetActive(true);
                
                if (Ctx.currentActiveCinematicsCam && !Ctx.isLoadRestart)
                {
                    Debug.Log("Trying to play cinematics");
                    currentCSD.CameraPosition = 1.0f;
                    if (currentCSD.Spline != null)
                    {
                        Ctx.currentActiveCinematicsCam.gameObject.SetActive(true);
                    
                        float curTime = 0.0f;
                        while (curTime < timeForCinematics - 0.1f)
                        {
                            yield return null;
                            curTime += Time.deltaTime;
                            // Debug.Log("Time: " + curTime);
                            // Debug.Log(cinematicsCam_A.gameObject.GetComponent<CinemachineSplineDolly>().CameraPosition);
                            currentCSD.CameraPosition -= (1.0f / timeForCinematics) * Time.deltaTime;
                        }
                    }
                }

                if (i != startDollies.Length - 1)
                {
                    // Do camera cuts here
                    Ctx.currentActiveCinematicsCam.gameObject.SetActive(false);
                    Ctx.currentActiveCinematicsCamIndex =
                        (Ctx.currentActiveCinematicsCamIndex + 1) % Ctx.cinematicsCameras.Count;
                    Ctx.currentActiveCinematicsCam = Ctx.cinematicsCameras[Ctx.currentActiveCinematicsCamIndex];
                }
                else
                {
                    Ctx.currentActiveCinematicsCam.gameObject.SetActive(false);
                }
            }
            
            if (CameraStateMachine.Instance.OnCinematicsOver != null)
            {
                CameraStateMachine.Instance.OnCinematicsOver();
            }

            StartCoroutine(StartPlayerActions(false));
        }

        public void StopPlantCollectionCinematics()
        {
            Ctx.playerStateMachine.IsCelebrating = false;
            StartCoroutine(DrillDownRoutine());
        }

        private IEnumerator DrillDownRoutine()
        {
            //Debug.Log("Drill Down");
            PlayerStateMachine psm = GameObject.FindWithTag("Player").GetComponent<PlayerStateMachine>();
            if (psm != null)
            {
                psm.ForceEnterDrillState();
            }
            yield return new WaitForSeconds(1.0f);
            //Call end of drill down action
            DrillDownOver?.Invoke();
            //Return control to player
            PlayerInput playerInput = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
            playerInput.EnableCharacterControls();
            HUDManager.Instance.ShowHideTimerPanel();
        }

        public void StopCameraPan(bool needsCut)
        {
            Debug.Log("Stopping cinematics");
            if (Ctx.currentActiveCinematicsCam.isActiveAndEnabled)
            {
                StopAllCoroutines();
                
                StartCoroutine(StartPlayerActions(needsCut));
                if (currentCinematics == CinematicsType.PlantPan)
                {
                    HUDManager.Instance.ShowHideTimerPanel();
                }

                if (currentCinematics == CinematicsType.StartPan)
                {
                    if (CameraStateMachine.Instance.OnCinematicsOver != null)
                    {
                        CameraStateMachine.Instance.OnCinematicsOver();
                    }
                }
            }
        }

        private IEnumerator StartPlayerActions(bool needsCut)
        {
            if (Ctx.currentActiveCinematicsCam.isActiveAndEnabled)
            {
                Ctx.currentActiveCinematicsCam.gameObject.SetActive(false);
            }
            
            //Reveal Bandit Model
            if (needsCut)
            {
                Ctx.CameraSurface.GetComponent<CinemachineBrain>().enabled = false;
                Ctx.CameraSurface.GetComponent<CinemachineBrain>().enabled = true;
            }
            
            Ctx.playerStateMachine.ModelRotator.RevealBandit();
            Ctx.freeLookCam.gameObject.SetActive(true);

            if (needsCut)
            {
                yield return null;
                yield return null;
                Ctx.ResetCamera();
            }
            
            GameObject startObj = GameObject.FindGameObjectWithTag("StartPoint");
            if (startObj != null)
            {
                startObj.GetComponent<StartPoint>().DisableStartingPlatform();
            }
            
            Ctx.playerStateMachine.ToggleDrill = true;
            Ctx.playerStateMachine.ToggleSlide = true;
            Ctx.playerStateMachine.ToggleWalk = true;
            Ctx.playerStateMachine.ToggleJump = true;
            Ctx.playerStateMachine.bInvincible = false;
            
            HUDManager.Instance.ToggleAdaptiveHud(GlobalSettings.Instance.controlsHUD);
        }
    }
}

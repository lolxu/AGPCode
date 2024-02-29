using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Player.StateMachine;
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
        private SplineContainer levelStartDolly;
        private SplineContainer plantCollectionDolly;

        private Animator panAnim;
        private CinemachineSplineDolly myDolly;

        private void Awake()
        {
            SceneManager.sceneLoaded += HideBandit;
        }

        private void HideBandit(Scene arg0, LoadSceneMode arg1)
        {
            foreach (var characterComp in Ctx.playerStateMachine.ModelComponents)
            {
                if (characterComp)
                {
                    characterComp.SetActive(false);
                }
            }
        }

        private void Start()
        {
            if (Ctx.cinematicsCam)
            {
                // Debug.Log("Here");
                myDolly = Ctx.cinematicsCam.gameObject.GetComponent<CinemachineSplineDolly>();
                panAnim = Ctx.cinematicsCam.gameObject.GetComponent<Animator>();
            }
        }

        /// <summary>
        /// Function to play camera cinematics
        /// </summary>
        /// <param name="type"> 0 = Level Start Camera Dolly; 1 = Plant Collection Camera Dolly</param>
        /// <returns></returns>
        public IEnumerator CinematicsCameraRoutine(int type)
        {
            yield return null;

            levelStartDolly = null;
            plantCollectionDolly = null;
            var startDolly = GameObject.FindWithTag("StartLevelDolly");
            var plantDolly = GameObject.FindWithTag("PlantDolly");

            Debug.Log($"Playing Animation type: {type}");
            
            if (startDolly)
            {
                levelStartDolly = startDolly.GetComponent<SplineContainer>();
            }

            if (plantDolly)
            {
                plantCollectionDolly = plantDolly.GetComponent<SplineContainer>();
            }


            string animationClipName = SceneManager.GetActiveScene().name;

            if (levelStartDolly || plantCollectionDolly)
            {
                switch (type)
                {
                    case 0:
                        myDolly.Spline = levelStartDolly;
                        Ctx.cinematicsCam.GetComponent<CinemachineCamera>().Target.LookAtTarget = null;
                        Ctx.cinematicsCam.GetComponent<CinemachineCamera>().Target.TrackingTarget = null;
                        myDolly.CameraRotation = CinemachineSplineDolly.RotationMode.SplineNoRoll;
                        break;
                    case 1:
                        myDolly.Spline = plantCollectionDolly;
                        Ctx.cinematicsCam.GetComponent<CinemachineCamera>().Target.TrackingTarget =
                            Ctx.playerStateMachine.gameObject.transform;
                        Ctx.cinematicsCam.GetComponent<CinemachineCamera>().Target.LookAtTarget =
                            Ctx.playerStateMachine.gameObject.transform;
                        myDolly.CameraRotation = CinemachineSplineDolly.RotationMode.Default;
                        animationClipName += "_Plants";
                        break;
                }

                if (Ctx.cinematicsCam && !Ctx.isLoadRestart)
                {
                    Debug.Log("Trying tp play animation");
                    if (panAnim && myDolly.Spline != null)
                    {
                        Ctx.cinematicsCam.gameObject.SetActive(true);
                        panAnim.Play(animationClipName);
                        Debug.Log(animationClipName);
                        // Wait until camera pan finishes
                        while (panAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
                        {
                            yield return null;
                        }
                    }
                }
                switch (type)
                {
                    case 0:
                        StartCoroutine(StartPlayerActions());
                        break;
                    case 1:
                        HUDManager.Instance.ShowHideTimerPanel();
                        break;
                }
            }
            else
            {
                StartCoroutine(StartPlayerActions());
                Debug.LogError("Plant collection or Start level Cinematics not set");
            }
        }

        public void StopCameraPan()
        {
            if (Ctx.cinematicsCam.isActiveAndEnabled)
            {
                Debug.Log("Force stop cinematics");
                StopCoroutine(CinematicsCameraRoutine(0));
                panAnim.StopPlayback();
                StartCoroutine(StartPlayerActions());
                if (myDolly.Spline == plantCollectionDolly)
                {
                    HUDManager.Instance.ShowHideTimerPanel();
                }
            }
        }

        private IEnumerator StartPlayerActions()
        {
            if (Ctx.cinematicsCam && !Ctx.isLoadRestart)
            {
                if (Ctx.cinematicsCam)
                {
                    Ctx.cinematicsCam.gameObject.SetActive(false);
                }
            }

            foreach (var characterComp in Ctx.playerStateMachine.ModelComponents)
            {
                characterComp.SetActive(true);
            }
            
            Ctx.freeLookCam.gameObject.SetActive(true);
            
            GameObject startPoint = GameObject.FindGameObjectWithTag("StartPoint");
            if (startPoint)
            {
                startPoint.GetComponent<StartPoint>().DisableStartingPlatform();
            }
            
            yield return null;
            yield return null;
            while (Ctx.CameraSurface.GetComponent<CinemachineBrain>().IsBlending)
            {
                yield return null;
            }
            // Start music
            // AudioManager.instance.InitializeMusic(FMODEvents.instance.musicMainTheme);
            // XMLFileManager.Instance.Load();
            // if (!SceneManager.GetActiveScene().name.Contains("Burrow") && !SceneManager.GetActiveScene().name.Contains("Onboard"))
            // {
            //     UIManager.Instance.RestartTime();
            //     UIManager.Instance.StartTime();  // Logic issue. Needs change
            //     if (UIManager.Instance.gameObject.GetComponent<Timer>().personalBest > 0.0f)
            //     {
            //         UIManager.Instance.DisplayTimer();
            //     }
            //     else
            //     {
            //         UIManager.Instance.HideTimer();
            //     }
            // }
            //Start adding back the fog
            // TODO: Uncomment this for the fog thing during pan
            // RuntimeEnvironmentLighting.Instance.ResetFogValue();
            Ctx.playerStateMachine.ToggleDrill = true;
            Ctx.playerStateMachine.ToggleSlide = true;
            Ctx.playerStateMachine.ToggleWalk = true;
            Ctx.playerStateMachine.ToggleJump = true;
            
            Ctx.CurrentState.SwitchState(Ctx.CurrentState.Factory.SurfaceDefault());
        }
    }
}

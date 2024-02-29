using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment.Cannon;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.StateMachine;
using Unity.Cinemachine;
using UnityEngine;

namespace __OasisBlitz.Player.Environment.Cannon
{
    public class BurrowManager : MonoBehaviour
    {
        public static BurrowManager Instance;
        
        [Header("Cannons In Level Sequence")]
        [SerializeField] private LevelCannonObjects _cannonObject;
        [SerializeField] private int totalLevels = 3;

        [Header("Burrow Cameras")] 
        public CinemachineCamera plantCamera;
        public CinemachineCamera cannonCamera;
        public CinemachineCamera burrowCamera;

        private PlayerStateMachine ctx;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private IEnumerator Start()
        {
            yield return null;
            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
            // ctx.ToggleDrill = false;
            // ctx.ToggleJump = false;
            
            // Initializing cannons
            XMLFileManager.Instance.Load();

            // TODO this should be tied up to the level select thing in burrow
            // for (int i = 0; i < totalLevels; i++)
            // {
            //     bool isUnlocked = CollectableManager.Instance.CheckIsSaved(i);
            //     _cannonObject.SetAvailable(isUnlocked);
            // }

        }

        /// <summary>
        /// For cinematics in burrow
        /// </summary>
        /// <param name="target"> The targeted object </param>
        /// <param name="type"> "Plant" for plant cam, "Cannon" for cannon cam </param>
        /// <param name="plantIndex"> Default = -1, but otherwise put plant index </param>
        /// <returns></returns>
        public IEnumerator ActivateBurrowCinematicsCamera(GameObject target, string type, int plantIndex = -1)
        {
            if (plantCamera && cannonCamera)
            {
                // Switch to plant camera
                Debug.Log(target);
                burrowCamera.gameObject.SetActive(false);
                GameObject cineCam = null;
                switch (type)
                {
                    case "Plant":
                        cineCam = plantCamera.gameObject;
                        break;
                    case "Cannon":
                        cineCam = cannonCamera.gameObject;
                        target = _cannonObject.gameObject;
                        break;
                }

                if (cineCam != null) cineCam.SetActive(true);

                plantCamera.LookAt = target.transform;
                yield return null;
                yield return null;
                while (CameraStateMachine.Instance.CameraSurface.GetComponent<CinemachineBrain>().IsBlending)
                {
                    yield return null;
                }

                if (type == "Cannon")
                {
                    CannonParticleEffect(target, plantIndex);
                }

                yield return new WaitForSeconds(2.0f);

                if (cineCam != null) cineCam.SetActive(false);

                burrowCamera.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Camera not setup correctly for burrow");
            }
        }

        private void CannonParticleEffect(GameObject cannon, int index)
        {
            FeelEnvironmentalManager.Instance.PlayBurrowCannonFeedback(cannon.transform.position, 2.0f, index);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace __OasisBlitz.Utility
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        [SerializeField] private string m_burrowSceneName = "Burrow";
        public float m_transitionDuration;
        public float m_deathTransitionDuration;
        public bool TeleportRequested { get; private set; } = false;

        [SerializeField] private GameObject LoadScreenCanvas;
        [SerializeField] private ScreenWipe wipe;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if(!GameObject.Find("LoadScreenCanvas")) { Instantiate(LoadScreenCanvas); }
            wipe = GameObject.Find("ScreenWipe").GetComponent<ScreenWipe>();
            //Debug.Log($"{GameObject.Find("LoadScreenCanvas")}\t{wipe}");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void LoadBurrowAsync()
        {
            GameMetadataTracker.Instance.SetPreviousSceneName(SceneManager.GetActiveScene().name);
            wipe.WipeRight(() =>
            {
                StartCoroutine(LoadScene(m_burrowSceneName, false));
            });
            TeleportRequested = false;

        }

        public void LoadAnySceneAsync(string sceneName, bool needsDestroyEssentials)
        {
            wipe.WipeRight(() => {
                StartCoroutine(LoadScene(sceneName, needsDestroyEssentials));
            }); 
            TeleportRequested = false;
        }

        IEnumerator LoadScene(string sceneName, bool needsDestroyEssentials)
        {
            // Set the current Scene to be able to unload it later
            Scene currentScene = SceneManager.GetActiveScene();

            // Setting is restart for camera state machine
            if (sceneName != SceneManager.GetActiveScene().name)
            {
                CameraStateMachine.Instance.isLoadRestart = false;
            }

            GlobalSettings.Instance.SetControlsPrevScene();

            // The Application loads the Scene in the background at the same time as the current Scene.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            // Wait until the last operation fully loads to return anything
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            wipe.ClearRight(() =>
            {
                if(sceneName.ToLower().Contains("burrow"))
                {
                    UIManager.Instance.canPauseGame = true; // Loading into burrow in particular, can't pause after a level
                }
            });
            // AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentScene);
            if (needsDestroyEssentials)
            {
                Destroy(GameObject.FindGameObjectWithTag("Essentials"));
            }
        }

        public void RequestTeleport()
        {
            TeleportRequested = true;
        }

        public void FinishRequestTeleport()
        {
            TeleportRequested = false;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            wipe.ClearRight();
            Image sceneTransition = HUDManager.Instance.GetSceneTransitionImage();
            if (sceneTransition.color.a >= 0.0f)
            {
                sceneTransition.DOFade(0.0f, m_transitionDuration).SetEase(Ease.InOutSine);
            }
        }
    }
}
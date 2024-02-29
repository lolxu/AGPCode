using System;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using __OasisBlitz.Utility;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public EventSystem eventSystem;

    [SerializeField] private PlayerInput pInput;       // __OasisBlitz.Player -- enable and disable the gameObject to enable/disable player character controls
    [SerializeField] private PlayerStateMachine playerSM;   // To check if drilling -- to start/stop drilling sound when pausing
    [SerializeField] private PlayerAudio playerAudio;       // STOP walking sound when pausing      -- IsSubmerged?

    [SerializeField] private Canvas LoadScreen;
    [SerializeField] private Canvas HUD;
    [SerializeField] private Canvas PauseMenu;
    [SerializeField] private Canvas WinScreen;
    [SerializeField] private TextMeshProUGUI winSubtext;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private TextMeshProUGUI timerText;     // In HUD
    [SerializeField] private Timer timer;

    [SerializeField] private GameObject SettingsObject;     // Exists from Main Menu, but spawn if it does not exist just in case

    private Tween pauseTween;
    [SerializeField] private GameObject PauseObject;
    [SerializeField] private float tweenDuration;

    public bool isPaused { get; private set; }
    
    // Boolean for forcing game to be able to pause or not
    public bool canPauseGame { private get; set; } = true;


    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        //LoadScreen.gameObject.SetActive(false);
        //PauseMenu.gameObject.SetActive(false);
        HUD.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        Time.timeScale = 1.0f;
        if(!GameObject.Find("Settings")) { Instantiate(SettingsObject); }
        if(!LoadScreen) { LoadScreen = GameObject.Find("LoadScreenCanvas").GetComponent<Canvas>(); }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetTimer()
    {
        if (SceneManager.GetActiveScene().name.Contains("MainMenu") ||
            SceneManager.GetActiveScene().name.Contains("Burrow") ||
            SceneManager.GetActiveScene().name.Contains("Slideshow") ||
            SceneManager.GetActiveScene().name.Contains("Onboard"))
        {
            HideTimer();
        }
        else
        {
            // Do not show timer for first time runs through a level)
            XMLFileManager.Instance.Load();
            if (XMLFileManager.Instance.LookupPBTime(SceneManager.GetActiveScene().name) >= 0)
            {
                DisplayTimer();
            }
            else
            {
                HideTimer();
            }
            timer.StartTime();
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canPauseGame = true;
    }

    private void Update()
    {
        EditorOnlyAltRelease();
        
        timerText.text = timer.GetTime();
        // Debug.Log($"WALKING: {playerAudio.walking}");
    }

    private void EditorOnlyAltRelease()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        #endif
    }
    private void SlidePauseIn()
    {
        PauseMenu.gameObject.SetActive(true);
        //PauseObject.gameObject.transform.localPosition = new Vector2(0, 0);
        if (pauseTween != null) { pauseTween.Kill(false); }
        pauseTween = PauseObject.transform.DOLocalMove(new Vector2(0, 0), tweenDuration)
            .SetUpdate(true);

    }
    private void SlidePauseOut()
    {
        //PauseObject.gameObject.transform.localPosition = new Vector2(960, 0);
        if (pauseTween != null) { pauseTween.Kill(false); }
        pauseTween = PauseObject.transform.DOLocalMove(new Vector2(-800, 0), tweenDuration)
            .SetUpdate(true)
            .OnComplete(()=> {
                PauseMenu.gameObject.SetActive(false);
                FinishUnpause();
                }
            );
        
    }
    public void PauseGame()
    {
        // Debug.Log("Game Paused");
        // pInput.enabled = false;         // Done in PlayerInput.cs now

        if (canPauseGame)
        {
            playerAudio.StopWalkFromPause();
            playerAudio.StopSlideFromPause();
            if (playerSM.IsSubmerged)
            {
                playerAudio.StopDrill();
            }

            // if(playerAudio.walking) { playerAudio.StopWalk(); }
            Time.timeScale = 0.0f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            cinemachineCamera.enabled = false;
            HUD.gameObject.SetActive(false);
            isPaused = true;
            HapticsManager.Instance.StopHapticsFromPause();
            SlidePauseIn();
            //PauseMenu.gameObject.SetActive(true);
        }
    }
    public void UnpauseGame()
    {
        // Debug.Log("Game Unpaused");
        // pInput.enabled = true;             // Done in PlayerInput.cs now
        // playerAudio.StartWalk();

        GlobalSettings.Instance.SavePlayerPrefs();
        
        playerAudio.StartWalkFromPause();
        playerAudio.StartSlideFromPause();
        if (playerSM.IsSubmerged) { playerAudio.StartDrill(); }
        // if (playerAudio.walking) { playerAudio.StartWalk(); }

        SlidePauseOut();

    }
    private void FinishUnpause()
    {
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cinemachineCamera.enabled = true;
        HUD.gameObject.SetActive(true);
        if (SettingsUI.Instance.settingsActive) { SettingsUI.Instance.HideSettings(); }
        isPaused = false;
        //PauseMenu.gameObject.SetActive(false);
    }
    public bool WinScreenActive()
    {
        return WinScreen.gameObject.activeInHierarchy ? true : false;
    }
    public void Win(string subtext = "Level completed")
    {
        Time.timeScale = 0.0f;
        pInput.DisableCharacterControls();

        playerAudio.StopWalk();
        if (playerSM.IsSubmerged) { playerAudio.StopDrill(); }
        if (GlobalSettings.Instance.controller == "KEYBOARD")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        // cinemachineCamera.enabled = false;
        HUD.gameObject.SetActive(false);
        WinScreen.gameObject.SetActive(true);
        winSubtext.text = subtext;
    }

    public void DisplayTimer()      // Display if hidden, hide if displayed
    {
        // Show timer
        timerText.gameObject.SetActive(true);
    }

    public void HideTimer()
    {
        timerText.gameObject.SetActive(false);
    }
    
    public void StartTime()
    {
        // if (timer.running) { timer.StopTime(); }
        // else
        // {
        //     if (!isPaused) { timer.StartTime(); }
        // }
        if (!timer.running)
        {
            if (!isPaused)
            {
                timer.StartTime();
            }
        }
    }

    public void StopTime()
    {
        if (timer.running)
        {
            timer.StopTime();
        }
    }

    public float GetTime()
    {
        return timer.runTime;
    }
    
    public void RestartTime()
    {
        timer.StopTime();
        timer.RestartTime();
    }

    public void StartLoadingScene(string sceneName)
    {
        // LoadScreen.gameObject.SetActive(true);
        // LoadScreen.gameObject.GetComponent<LoadingScreen>().LoadScene(sceneName);
        LevelManager.Instance.LoadAnySceneAsync(sceneName);
    }
}

using System;
using __OasisBlitz.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Player;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Main menu will have:
    //  - Start
    //  - Exit

    // For Later:
    //  - Settings
    //  - Acknowledgements/Credits?
    //  - New game?
    //  - Load file?

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private CanvasGroup Main, _Start, Settings, Credits;
    [SerializeField] private Canvas LoadScreenCanvas;     // Canvas to set active/inactive
    [SerializeField] private LoadingScreen loadScreen;  // Script to access functions

    [SerializeField] private MainMenuSettings mms;
    // Buttons
    [SerializeField] private Button StartButton, SettingsButton, CreditsButton, ExitButton,
        ControlsButton, CloseCreditsButton;

    [SerializeField] private Button ContinueButton, NewGameButton, BackButton;
    [SerializeField] private TextMeshProUGUI ContinueText;
    public Material SpeedLineMaterial;

    [SerializeField] private LevelNames levelNames;

    
    // called when "Continue" or "Start Game" (depending on start scene is pressed).
    public Action OnStartGamePressed;

    /*
     * The main menu is destroyed when you start the game, (after it fades itself out and enables character controls)
     * But, it is created in a fork scene, that decides if you should start in burrow or onboarding
     * 
     */
    private void Start()
    {
        FirstLevelGateway isFirstLevel = FirstLevelGateway.Instance;
        if (isFirstLevel)
        {
            FirstLevelGateway.Instance = null;
            Destroy(isFirstLevel.gameObject);
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;    
            
            PlayerInput pInput = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
            pInput.EnableUIControls();
            UIManager.Instance.canPauseGame = false;
        }
        else
        {
            if (OnStartGamePressed != null)
            {
                OnStartGamePressed();
            }
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        // if we have save data, open start interface
        if (XMLFileManager.Instance.GetNumPlantsCollected() != 0)
        {
            OpenStartInterface();
        }
        // otherwise, just "start" the game from onboarding
        else
        {
            ContinueGame();
            HUDManager.Instance.ToggleAdaptiveHud(GlobalSettings.Instance.controlsHUD);
        }
    }

    public void OpenStartInterface()
    {
        _Start.gameObject.transform.localPosition = new Vector3(-264.0f, 0.0f, 0.0f);
        _Start.gameObject.SetActive(true);
        SetStartNavigation();
        Main.gameObject.SetActive(false);
    }

    public void CloseStartInterface()
    {
        Main.gameObject.SetActive(true);
        StartButton.Select();
        _Start.gameObject.SetActive(false);
    }

    private void SetStartNavigation()
    {
        if (XMLFileManager.Instance.SaveExists())
        {
            ContinueButton.interactable = true;
            ContinueButton.image.color = new Color(ContinueButton.image.color.r, ContinueButton.image.color.g,
                ContinueButton.image.color.b, 1.0f);
            ContinueText.color = new Color(ContinueText.color.r, ContinueText.color.g, ContinueText.color.b, 1.0f);
            // // Navigation with all buttons accessible
            // Navigation cNav = new Navigation
            // {
            //     mode = Navigation.Mode.Explicit,
            //     selectOnDown = NewGameButton,
            //     selectOnUp = BackButton,
            //     selectOnLeft = null,
            //     selectOnRight = null
            // };
            // ContinueButton.navigation = cNav;
            Navigation ngNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = BackButton,
                selectOnUp = ContinueButton,
                selectOnLeft = null,
                selectOnRight = null
            };
            NewGameButton.navigation = ngNav;
            Navigation bNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = ContinueButton,
                selectOnUp = NewGameButton,
                selectOnLeft = null,
                selectOnRight = null
            };
            BackButton.navigation = bNav;
            ContinueButton.Select();
        }
        else
        {
            ContinueButton.interactable = false;
            ContinueButton.image.color = new Color(ContinueButton.image.color.r, ContinueButton.image.color.g,
                ContinueButton.image.color.b, 0.5f);
            ContinueText.color = new Color(ContinueText.color.r, ContinueText.color.g, ContinueText.color.b, 0.5f);

            // // Navigation with all buttons accessible
            // Navigation cNav = new Navigation
            // {
            //     mode = Navigation.Mode.Explicit,
            //     selectOnDown = NewGameButton,
            //     selectOnUp = BackButton,
            //     selectOnLeft = null,
            //     selectOnRight = null
            // };
            // ContinueButton.navigation = cNav;
            Navigation ngNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = BackButton,
                selectOnUp = BackButton,
                selectOnLeft = null,
                selectOnRight = null
            };
            NewGameButton.navigation = ngNav;
            Navigation bNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = NewGameButton,
                selectOnUp = NewGameButton,
                selectOnLeft = null,
                selectOnRight = null
            };
            BackButton.navigation = bNav;
            NewGameButton.Select();
        }
    }

    private void DeleteGhostData(string sceneName)
    {
        string fullReplayName = GhostRecorder.GetRecordingName(sceneName);
        if (File.Exists(fullReplayName))
        {
            File.Delete(fullReplayName);
        }
    }
    
    public void StartNewGame()
    {
        XMLFileManager.Instance.NewGame();
        
        // clear replay data
        DeleteGhostData(levelNames.Level1Name);
        DeleteGhostData(levelNames.Level2Name);
        
        SceneManager.LoadSceneAsync(levelNames.MainMenuSceneName);
    }

    

    public void ContinueGame()
    {
        if (OnStartGamePressed != null)
        {
            OnStartGamePressed();
        }
    }

    public void FadeOutAndDestroyMainMenu(float duration)
    {
        // Debug.Log("Not Implemented Destroy MainMenu!!!");
        // todo: make this an ienumerator or something to actually check "isdestroyed" or if this thing is even done
        // with the animation
        CanvasGroup mainMenuCanvasGroup = GetComponent<CanvasGroup>();
        if (mainMenuCanvasGroup)
        {
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.DOFade(0.0f, duration).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
   
    public void OpenSettings()
    {
        SettingsUI.Instance.ShowSettings();
        Main.gameObject.SetActive(false);

    }
    public void OpenCredits()
    {
        Destroy(GameObject.FindWithTag("Essentials"));
        SceneManager.LoadScene(levelNames.CreditsName);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();     // Eventually some save funcitonality
#endif
    }


    public void CloseSettings()
    {
        Main.gameObject.SetActive(true);
        SettingsButton.Select();
    }
    public void CloseCredits()
    {
        Credits.gameObject.SetActive(false);
        Main.gameObject.SetActive(true);
        CreditsButton.Select();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(HUDManager.Instance) { HUDManager.Instance.ToggleAdaptiveHud(false); }
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    /*
     * todo: make the main menu something that is... spawned in after the scene as part of some sort of
     * singleton that gets destroyed on load. If it isn't there, it destroys itself on awake or something,
     * and is included in onboarding and burrow.
     */
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        StartCoroutine(WaitOneFrame());
        if(SceneManager.GetActiveScene().name.ToLower().Contains("onboard"))
        {
            if(HUDManager.Instance)
            {
                HUDManager.Instance.ToggleAdaptiveHud(false);
            }
        }
    }

    private IEnumerator WaitOneFrame()
    {
        yield return null;
        SpeedLineMaterial.SetColor("_Colour", Color.clear);
        // LoadScreenCanvas.gameObject.SetActive(false);
        CloseCredits();
        CloseSettings();
        StartButton.Select();
        if (loadScreen == null)
        {
            loadScreen = GameObject.FindObjectOfType<LoadingScreen>();
        }
        // Reset time scale if pulling up Main Menu from in game (otherwise credits tween will not work)
        if (Time.timeScale != 1) { Time.timeScale = 1.0f; }
    }
}

using __OasisBlitz.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using __OasisBlitz.Utility;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    
    [SerializeField] private Canvas LoadScreenCanvas;
    [SerializeField] private LoadingScreen loadScreen;
    [SerializeField] private PlayerInput pInput;
    [SerializeField] private GameObject border, dim;
    [SerializeField] private GameObject levelSelectInterface;
    [SerializeField] private GameObject SettingsInterface;
    [SerializeField] private GameObject bounceKeyboardDisplay, bounceControllerDisplay, bouncePromptDisplay;

    // Main Pause Interface
    [SerializeField] private GameObject MainPauseInterface;
    [SerializeField] private float outLoc, inLoc;     // Location out of screen, in screen
    private Tween pauseTween;

    // Primary buttons
    [SerializeField] private Button ResumeButton, RestartButton, BurrowButton, SettingsButton, MainMenuButton;

    [SerializeField] private TextMeshProUGUI BurrowButtonText;
    // Level selection buttons
    [SerializeField] private LevelSelect levels;

    // CONTROLLER DISPLAYS
    [SerializeField] private GameObject keyboardControls, controllerControls;

    [SerializeField] private Image resumeHotkey;
    [SerializeField] private Sprite resKeyboardP, resKeyboardEsc, resGeneric, resXbox;

    // Specific to controller
    [SerializeField] private Image jumpControllerKey, sJumpControllerKey, fruitControllerKey;
    [SerializeField] private Sprite jumpGeneric, jumpXbox,
                                    sJumpGeneric, sJumpXbox,
                                    fruitGeneric, fruitXbox;

    [SerializeField] private Image selectUI;
    [SerializeField] private Sprite selectGeneric, selectXbox;

    // Controls Display
    [SerializeField] private GameObject ControlsDisplay;
    [SerializeField] private Button ControlsButton, ExitControlsButton;
    [SerializeField] private GameObject ControllerDisplay, MouseKeyboardDisplay;
    [SerializeField] private float tweenFrom, tweenTo, tweenDuration;

    [SerializeField] private LevelNames _levelNames;
    
    private Tween controlsTween;

    // Start is called before the first frame update
    void Start()
    {
        // SetControls();
        if (!SettingsInterface)
        {
            // Settings instantiated in UIManager if DNE
            SettingsInterface = GameObject.Find("SettingsInterface");
        }
        GlobalSettings.ControlsChangedEvent += SwitchControlsDisplay;
        SwitchControlsDisplay();        // Initialize the correct controls display

        if (BounceAbility.Instance.BounceEnabled)
        {
            bouncePromptDisplay.SetActive(true);
            bounceKeyboardDisplay.SetActive(true);
            bounceControllerDisplay.SetActive(true);
        }
        else
        {
            bouncePromptDisplay.SetActive(false);
            bounceKeyboardDisplay.SetActive(false);
            bounceControllerDisplay.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        GlobalSettings.ControlsChangedEvent -= SwitchControlsDisplay;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

public void SetControls()
    {
        if (GlobalSettings.Instance.displayedController == "KEYBOARD")
        {
            // ENABLE Keyboard display
            keyboardControls.SetActive(true);
            controllerControls.SetActive(false);
#if UNITY_EDITOR
            resumeHotkey.sprite = resKeyboardP;
#else
        resumeHotkey.sprite = resKeyboardEsc;
#endif
        }
        else
        {
            controllerControls.SetActive(true);
            keyboardControls.SetActive(false);
            // ENABLE Controller display

/*            if (GlobalSettings.Instance.displayedController == "OTHER" || GlobalSettings.Instance.displayedController == "PLAYSTATION")
            {
                // Switch Generic specific images
                resumeHotkey.sprite = resGeneric;
                jumpControllerKey.sprite = jumpGeneric;
                sJumpControllerKey.sprite = sJumpGeneric;
                fruitControllerKey.sprite = fruitGeneric;
                selectUI.sprite = selectGeneric;
            }*/
            if (GlobalSettings.Instance.displayedController == "XBOX" || GlobalSettings.Instance.displayedController == "OTHER" || GlobalSettings.Instance.displayedController == "PLAYSTATION")
            {
                // Switch Xbox specific images
                resumeHotkey.sprite = resXbox;
                jumpControllerKey.sprite = jumpXbox;
                sJumpControllerKey.sprite = sJumpXbox;
                fruitControllerKey.sprite = fruitXbox;
                selectUI.sprite = selectXbox;
            }
        }

    }

    public void Resume()
    {
        if (ControlsDisplay.activeInHierarchy)
        {
            TweenControlsOut();
        }
        pInput.EnableCharacterControls();
        UIManager.Instance.UnpauseGame();
        HapticsManager.Instance.ResumeHapticsFromPause();
    }

    public void LevelSelection()
    {
        // levels.buttons[0].Select();      SELECTED IN LevelSelect.cs instead
        MainPauseInterface.SetActive(false);
        border.SetActive(true);
        // border2.SetActive(true);
        levelSelectInterface.SetActive(true);
        //dim.SetActive(true);
    }

    // public void CloseLevelSelection()
    // {
    //     MainPauseInterface.SetActive(true);
    //     LvlSelectButton.Select();
    //     border.SetActive(false);
    //     // border2.SetActive(false);
    //     levelSelectInterface.SetActive(false);
    //     //dim.SetActive(false);
    // }
    public void LoadLevel(string level)
    {
        if (ControlsDisplay.activeInHierarchy)
        {
            TweenControlsOut();
        }
        //loadScreen.LoadScene(level);

        if (level.Contains("Burrow"))
        {
            GameMetadataTracker.Instance.ResetAllCheckpointForLevel(SceneManager.GetActiveScene().name);
        }
        LevelManager.Instance.LoadAnySceneAsync(level, false);
        UIManager.Instance.UnpauseGame();
        pInput.EnableCharacterControls();
    }

    public void RestartLevel()
    {
        if (ControlsDisplay.activeInHierarchy)
        {
            TweenControlsOut();
        }
        string currentScene = SceneManager.GetActiveScene().name;
        GameMetadataTracker.Instance.ResetAllCheckpointForLevel(currentScene);
        
        StopAllCoroutines();
        
        // Disable timer
        UIManager.Instance.StopTime();
        UIManager.Instance.HideTimer();

        CameraStateMachine.Instance.isLoadRestart = true;
        CameraStateMachine.Instance.StopCameraCinematics(true);

        LoadLevel(currentScene);
        
        
    }


    public void ReturnToTitle()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.unPause, UIManager.Instance.transform.position);
        if (ControlsDisplay.activeInHierarchy)
        {
            TweenControlsOut();
        }
        //LoadScreenCanvas.gameObject.SetActive(true);
        GameMetadataTracker.Instance.ResetAllCheckpointForLevel(SceneManager.GetActiveScene().name);
        // Destroy(GameObject.FindGameObjectWithTag("Essentials"));
        // Destroy(GameMetadataTracker.Instance.gameObject);
        // loadScreen.LoadScene("MainMenu");
        pInput.EnableCharacterControls();
        
        SceneManager.LoadSceneAsync(_levelNames.MainMenuSceneName);
    }
    

    public void Settings()
    {
        if (ControlsDisplay.activeInHierarchy)
        {
            TweenControlsOut(FinishOpeningSettings);
        }
        else
        {
            FinishOpeningSettings();
        }
    }
    private void TweenMainPauseIn(Action DoAfter = null)
    {
        if(pauseTween != null) { pauseTween.Kill(); }
        pauseTween = MainPauseInterface.transform.DOLocalMoveX(inLoc, tweenDuration)
            .OnComplete(() =>
            {
                if(DoAfter != null)
                {
                    DoAfter();
                }
            })
            .SetUpdate(true);
    }
    private void TweenMainPauseOut(Action DoAfter = null)
    {
        if (pauseTween != null) { pauseTween.Kill(); }
        pauseTween = MainPauseInterface.transform.DOLocalMoveX(outLoc, tweenDuration)
            .OnComplete(() =>
            {
                if (DoAfter != null)
                {
                    DoAfter();
                }
            })
            .SetUpdate(true);
    }
    public void FinishOpeningSettings()
    {
        MainPauseInterface.transform.localPosition = new Vector2(inLoc, MainPauseInterface.transform.localPosition.y);
        TweenMainPauseOut(() =>
        {
            MainPauseInterface.SetActive(false);
        });
        SettingsUI.Instance.ShowSettings();

    }
    public void CloseSettings()
    {
        MainPauseInterface.transform.localPosition = new Vector2(outLoc, MainPauseInterface.transform.localPosition.y);
        MainPauseInterface.SetActive(true);
        TweenMainPauseIn(() =>
        {
            SettingsButton.Select();
        });
    }

    public bool IsControlsDisplayActive()
    {
        return ControlsDisplay.activeInHierarchy;
    }
    public void TweenControlsIn()
    {
        ControlsDisplay.SetActive(true);
        if(controlsTween != null) { controlsTween.Kill(false); }
        controlsTween = ControlsDisplay.transform.DOLocalMoveX(tweenTo, tweenDuration)
            .OnComplete(() =>
            {
                ExitControlsButton.Select();
            })
            .SetUpdate(true);
        SetPauseNavigation(ExitControlsButton);
    }
    public void TweenControlsOut()
    {
        if (controlsTween != null) { controlsTween.Kill(false); }
        controlsTween = ControlsDisplay.transform.DOLocalMoveX(tweenFrom, tweenDuration)
            .OnComplete(() =>
            {
                ControlsButton.Select();
                ControlsDisplay.SetActive(false);
            })
            .SetUpdate(true);
        SetPauseNavigation();

    }
    // Overload to add funcitonality after
    public void TweenControlsOut(Action callback)
    {
        if (controlsTween != null) { controlsTween.Kill(false); }
        controlsTween = ControlsDisplay.transform.DOLocalMoveX(tweenFrom, tweenDuration)
            .OnComplete(() =>
            {
                ControlsButton.Select();
                ControlsDisplay.SetActive(false);
                callback();
            })
            .SetUpdate(true);
        SetPauseNavigation();
    }
    private void SwitchControlsDisplay()
    {
        if (GlobalSettings.Instance)
        {
            switch (GlobalSettings.Instance.displayedController)
            {
                case "KEYBOARD":
                    MouseKeyboardDisplay.SetActive(true);
                    ControllerDisplay.SetActive(false);
                    break;
                case "XBOX":
                case "PLAYSTATION":
                case "OTHER":
                    MouseKeyboardDisplay.SetActive(false);
                    ControllerDisplay.SetActive(true);
                    break;
            }
        }
    }

    private void SetPauseNavigation(Button right = null)       // For all buttons in the main pause interface
    {
        // ResumeButton, RestartButton, BurrowButton, SettingsButton, MainMenuButton, ControlsButton

        Navigation rNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = RestartButton,
            selectOnUp = ControlsButton,
            selectOnLeft = null,
            selectOnRight = right
        };
        ResumeButton.navigation = rNav;
        Navigation reNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = (SceneManager.GetActiveScene().name.ToLower().Contains("burrow") || SceneManager.GetActiveScene().name.ToLower().Contains("onboard")) ? SettingsButton : BurrowButton,
            selectOnUp = ResumeButton,
            selectOnLeft = null,
            selectOnRight = right
        };
        RestartButton.navigation = reNav;
        if (SceneManager.GetActiveScene().name.ToLower().Contains("burrow") ||
            SceneManager.GetActiveScene().name.ToLower().Contains("onboard"))
        {
            Navigation bNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown =  SettingsButton,
                selectOnUp = RestartButton,
                selectOnLeft = null,
                selectOnRight = right
            };
            BurrowButton.navigation = bNav;
        }
        Navigation sNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown =  MainMenuButton,
            selectOnUp = (SceneManager.GetActiveScene().name.ToLower().Contains("burrow") || SceneManager.GetActiveScene().name.ToLower().Contains("onboard")) ? RestartButton : BurrowButton,
            selectOnLeft = null,
            selectOnRight = right
        };
        SettingsButton.navigation = sNav;
        Navigation mNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown =  ControlsButton,
            selectOnUp = SettingsButton,
            selectOnLeft = null,
            selectOnRight = right
        };
        MainMenuButton.navigation = mNav;
        Navigation cNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown =  ResumeButton,
            selectOnUp = MainMenuButton,
            selectOnLeft = null,
            selectOnRight = right
        };
        ControlsButton.navigation = cNav;
    }
    public void Exit()
    {
        // XMLFileManager.Instance.SaveLevelStatus();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();     // Eventually some save funcitonality
#endif
    }
    
    private void SetBurrowButtonInteractable()
    {
        if (SceneManager.GetActiveScene().name.ToLower().Contains("burrow") || SceneManager.GetActiveScene().name.ToLower().Contains("onboard"))
        {
            BurrowButton.interactable = false;
            // Semi-transparent to show it is not interactable
            BurrowButton.image.color = new Color(BurrowButton.image.color.r, BurrowButton.image.color.g, BurrowButton.image.color.b, 0.5f);
            BurrowButtonText.color = new Color(BurrowButtonText.color.r, BurrowButtonText.color.g,
                BurrowButtonText.color.b, 0.5f);
            // Set Navigation
            Navigation restartNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = SettingsButton,
                selectOnUp = ResumeButton,
                selectOnRight = null
            };
            RestartButton.navigation = restartNav;
            // Navigation burrowNav = new Navigation
            // {
            //     mode = Navigation.Mode.Explicit,
            //     selectOnDown = null,
            //     selectOnUp = null,
            //     selectOnRight = null
            // };
            Navigation settingsNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = MainMenuButton,
                selectOnUp = RestartButton,
                selectOnRight = null
            };
            SettingsButton.navigation = settingsNav;
        }
        else
        {
            BurrowButton.interactable = true;
            // Opaque because interactable
            BurrowButton.image.color = new Color(BurrowButton.image.color.r, BurrowButton.image.color.g, BurrowButton.image.color.b, 1.0f);
            BurrowButtonText.color = new Color(BurrowButtonText.color.r, BurrowButtonText.color.g,
                BurrowButtonText.color.b, 1.0f);
            // Set navigation
            Navigation restartNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = BurrowButton,
                selectOnUp = ResumeButton,
                selectOnRight = null
            };
            RestartButton.navigation = restartNav;
            // Navigation burrowNav = new Navigation
            // {
            //     mode = Navigation.Mode.Explicit,
            //     selectOnDown = SettingsButton,
            //     selectOnUp = RestartButton,
            //     selectOnRight = null
            // };
            Navigation settingsNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = MainMenuButton,
                selectOnUp = BurrowButton,
                selectOnRight = null
            };
            SettingsButton.navigation = settingsNav;
        }
    }
    private void OnEnable()
    {
        // CloseLevelSelection();      // Hide all secondary/tertiary menus when opening pause
        if (SettingsUI.Instance.settingsActive) { CloseSettings(); }
        SetControls();
        SetBurrowButtonInteractable();
        MainPauseInterface.transform.localPosition = new Vector2(inLoc, MainPauseInterface.transform.localPosition.y);
        ResumeButton.Select();
    }
    private void OnDisable()        // Hide all secondary menus (LevelSelect, Settings, etc) whenever unpaused
    {

    }
}

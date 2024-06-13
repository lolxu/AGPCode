using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI Instance;

    [SerializeField] private Canvas settingsCanvas;
    public bool settingsActive { get; private set; }
    [Header("Main Settings Interface")] 
    [SerializeField] private Button ControlsButton;
    [SerializeField] private Button DisplayButton;
    [SerializeField] private Button SoundButton;
    [SerializeField] private Button CloseSettingsButton;
    [SerializeField] private GameObject ControlsButtonHighlight;
    [SerializeField] private GameObject DisplayButtonHighlight;
    [SerializeField] private GameObject SoundButtonHighlight;
    //[SerializeField] private GameObject SectionBorder;

    [Header("Settings Sections")]
    [SerializeField] private GameObject MainsectionRunes;
    [SerializeField] private GameObject SubsectionRunes;
    [SerializeField] private GameObject ControlsSettingsInterface;
    [SerializeField] private GameObject DisplaySettingsInterface;
    [SerializeField] private GameObject SoundSettingsInterface;

    [Header("First Buttons (to select on opening)")]
    [SerializeField] private Button ControlsFirstButton;
    [SerializeField] private Button DisplayFirstButton;
    [SerializeField] private Button SoundFirstButton;

    [Header("Settings Highlights")] 
    // Display Settings
    [SerializeField] private GameObject SelectedButtonType;
    [SerializeField] private GameObject SelectedControlsHUD;
    [SerializeField] private GameObject SelectedFullscreen;
    [SerializeField] private GameObject SelectedResolution;
    // Controls Settings
    [SerializeField] private GameObject SelectedInvertedX;
    [SerializeField] private GameObject SelectedInvertedY;

    [Header("Tweening")]
    [SerializeField] private SettingsTweens settingsTweens;

    public void ShowSettings()
    {
        settingsCanvas.gameObject.SetActive(true);
        settingsTweens.SlideSettingsIn(() =>
        {
            settingsActive = true;
        });
    }
    public void SlideSettingsOut(Action DoAfter = null)  // For use in PauseManager
    {
        settingsTweens.SlideSettingsOut(() =>
        {
            settingsActive = false;
            GlobalSettings.Instance.SavePlayerPrefs();
            settingsCanvas.gameObject.SetActive(false);
            if(DoAfter != null)
            {
                DoAfter();
            }
        });
    }
    public void HideSettings()
    {
        settingsActive = false;
        // HideOpenSettings("all");
        settingsTweens.SlideSettingsOut(() =>
        {
            MainMenu mainMenu = FindObjectOfType<MainMenu>();
            if (mainMenu != null)
            {
                mainMenu.CloseSettings();
            }
            else
            {
                if (PauseManager.Instance) { PauseManager.Instance.CloseSettings(); }
            }
            GlobalSettings.Instance.SavePlayerPrefs();
            settingsCanvas.gameObject.SetActive(false);
        });
    }

    public void HideOpenSettings(string callType)
    {
        switch(callType)
        {
            case "Controls":
                if (DisplaySettingsInterface.activeInHierarchy) { HideDisplayTween(); }
                if (SoundSettingsInterface.activeInHierarchy) { HideSoundTween(); }
                break;
            case "Display":
                if (ControlsSettingsInterface.activeInHierarchy) { HideControlsTween(); }
                if (SoundSettingsInterface.activeInHierarchy) { HideSoundTween(); }
                break;
            case "Sound":
                if (ControlsSettingsInterface.activeInHierarchy) { HideControlsTween(); }
                if (DisplaySettingsInterface.activeInHierarchy) { HideDisplayTween(); }
                break;
            default:
                if (ControlsSettingsInterface.activeInHierarchy) { HideControlsTween(); }
                if (DisplaySettingsInterface.activeInHierarchy) { HideDisplayTween(); }
                if (SoundSettingsInterface.activeInHierarchy) { HideSoundTween(); }
                break;
        }
    }

    // NO LONGER TOGLGLES SICNE ONE PANEL MSUT BE OPEN ALWASY
    public void ToggleControlsSettings()
    {
        if (!ControlsSettingsInterface.activeInHierarchy) { ShowControlsTween(); }
    }
    public void ToggleDisplaySettings()
    {
        if (!DisplaySettingsInterface.activeInHierarchy) { ShowDisplayTween(); }
    }
    public void ToggleSoundSettings()
    {
        if (!SoundSettingsInterface.activeInHierarchy) { ShowSoundTween(); }
    }

    public void ShowControlsTween()
    {
        ControlsButtonHighlight.SetActive(true);
        HideOpenSettings("Controls");
        settingsTweens.SlideControlsIn();
    }
    public void HideControlsTween()
    {
        settingsTweens.SlideControlsOut(HideControlsSettings);
        ControlsButtonHighlight.SetActive(false);
    }
    public void ShowControlsSettings()
    {
        HideOpenSettings("Controls");
        ControlsSettingsInterface.SetActive(true);
    }
    public void HideControlsSettings()
    {
        //SubsectionRunes.SetActive(false);
        //ControlsButtonHighlight.SetActive(false);
        ControlsSettingsInterface.SetActive(false);
        if (settingsActive) { ControlsButton.Select(); }
        SetSettingsNavigations(null);
    }
    public void ShowDisplayTween()
    {
        DisplayButtonHighlight.SetActive(true);
        HideOpenSettings("Display");
        settingsTweens.SlideDisplayIn();
    }
    public void HideDisplayTween()
    {
        DisplayButtonHighlight.SetActive(false);
        settingsTweens.SlideDisplayOut(HideDisplaySettings);
    }
    public void ShowDisplaySettings()
    {
        HideOpenSettings("Display");
        //SubsectionRunes.SetActive(true);
        //DisplayButtonHighlight.SetActive(true);
        DisplaySettingsInterface.SetActive(true);
        //DisplayFirstButton.Select();
    }
    public void HideDisplaySettings()
    {
        //SubsectionRunes.SetActive(false);
        //DisplayButtonHighlight.SetActive(false);
        DisplaySettingsInterface.SetActive(false);
        if (settingsActive) { DisplayButton.Select(); }
        SetSettingsNavigations(null);
    }
    public void ShowSoundTween()
    {
        SoundButtonHighlight.SetActive(true);
        HideOpenSettings("Sound");
        settingsTweens.SlideSoundIn();
    }
    public void HideSoundTween()
    {
        SoundButtonHighlight.SetActive(false);
        settingsTweens.SlideSoundOut(HideSoundSettings);
    }
    public void ShowSoundSettings()
    {
        HideOpenSettings("Sound");
        //SubsectionRunes.SetActive(true);
        //SoundButtonHighlight.SetActive(true);
        SoundSettingsInterface.SetActive(true);
        //SoundFirstButton.Select();
    }
    public void HideSoundSettings()
    {
        //SubsectionRunes.SetActive(false);
        //SoundButtonHighlight.SetActive(false);
        SoundSettingsInterface.SetActive(false);
        if (settingsActive) { SoundButton.Select(); }
        SetSettingsNavigations(null);
    }
    public void SetSettingsNavigations(Button right)
    {
        // Controls
        Navigation cNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = DisplayButton,
            selectOnUp = CloseSettingsButton,
            selectOnRight = right
        };
        ControlsButton.navigation = cNav;
        // Display
        var dNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = SoundButton,
            selectOnUp = ControlsButton,
            selectOnRight = right
        };
        DisplayButton.navigation = dNav;
        // Sound
        var sNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = CloseSettingsButton,
            selectOnUp = DisplayButton,
            selectOnRight = right
        };
        SoundButton.navigation = sNav;
        // Close button
        var clNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = ControlsButton,
            selectOnUp = SoundButton,
            selectOnRight = right
        };
        CloseSettingsButton.navigation = clNav;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    } 

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        if(Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
        if(settingsCanvas.worldCamera == null)
        {
            SetSceneCamera();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void SetSceneCamera()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Camera mainMenuCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            settingsCanvas.worldCamera = mainMenuCamera;
            settingsCanvas.planeDistance = 100;
        }
        else if (SceneManager.GetActiveScene().name.Contains("Slideshow"))
        {

        }
        else
        {
            Camera levelCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            settingsCanvas.worldCamera = levelCamera;
            settingsCanvas.planeDistance = 100;
        }
        
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Debug.Log($"Scene Loaded: {scene.name}");
        if(scene.name == "Slideshow") { return; }
        SetSceneCamera();
    }
}

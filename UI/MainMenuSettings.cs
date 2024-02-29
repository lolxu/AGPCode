using __OasisBlitz.Player;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSettings : MonoBehaviour
{
    [SerializeField] private CanvasGroup Settings;

    [SerializeField] private Button SettingsButton;     // To open the settings interface

    [SerializeField] private GameObject border, dim;
    [SerializeField] private GameObject DisplaySettings, SoundSettings, MouseKeyboardSettings, ControllerSettings;             // Settings game objects that contain each category's respective settings
    
    // Settings buttons -- To open up the different settings interfaces
    [SerializeField] private Button DisplayButton, SoundButton, MouseKeyboardButton, ControlsButton;
    [SerializeField] private Button CloseSettingsButton, CloseDisplayButton, CloseControlsButton, CloseSoundButton;
    [SerializeField] private Button AutoButton, MasterVolMinButton, MSMinButton, CSMinButton;
    // Highlights for Display, Sound, Controls Buttons
    [SerializeField] private GameObject DisplayButtonHighlight, SoundButtonHighlight, ControlsButtonHighlight;
    // Selected button highlights (for Display interface)
    [SerializeField] private Image SelectedButtonType, SelectedControlsHUD, SelectedFullscreen, SelectedResolution, SelectedInvertedX, SelectedInvertedY;

    // In MainMenu.cs
    /*    public void OpenSettings()
        {
            Settings.gameObject.SetActive(true);
            DisplayButton.Select();
        }
        public void CloseSettings()
        {
            Settings.gameObject.SetActive(false);
            SettingsButton.Select();
        }
    */
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
    public void CloseActiveSettingsInterface()
    {
        if (DisplaySettings.activeInHierarchy) { CloseDisplay(); }
        if (ControllerSettings.activeInHierarchy) { CloseController(); }
        if (SoundSettings.activeInHierarchy) { CloseSound(); }
    }
    public void ToggleDisplaySettingsInterface()
    {
        if (DisplaySettings.activeInHierarchy)
        {
            CloseActiveSettingsInterface();
        }
        else
        {
            CloseActiveSettingsInterface();
            OpenDisplay();
            SetSettingsNavigations(AutoButton);
        }
    }
    public void OpenDisplay()
    {
        dim.SetActive(true);
        border.SetActive(true);
        DisplaySettings.SetActive(true);
        AutoButton.Select();
        DisplayButtonHighlight.SetActive(true);
        
        MSelectedButtonType(GlobalSettings.Instance.playerControllerSetting);
        MSelectedFullscreen();
        MSelectedControlsHUD();
        MSelectedResolution();
    }
    public void CloseDisplay()
    {
        DisplayButtonHighlight.SetActive(false);
        dim.SetActive(false);
        border.SetActive(false);
        DisplaySettings.SetActive(false);
        DisplayButton.Select();
    }
    public void MSelectedButtonType(string button)
    {
        switch (button)
        {
            case "AUTO":
                SelectedButtonType.transform.localPosition = new Vector2(133.2f, 0);
                break;
            case "KEYBOARD":
                SelectedButtonType.transform.localPosition = new Vector2(255, 0);
                break;
            case "XBOX":
                SelectedButtonType.transform.localPosition = new Vector2(378, 0);
                break;
            case "OTHER":
                SelectedButtonType.transform.localPosition = new Vector2(423, 0);
                break;
        }
    }
    public void ChangeSelectedButtonType(string type)       // AUTO, KEYBOARD, XBOX, OTHER
    {
        GlobalSettings.Instance.SetPlayerControllerSetting(type);
    }

    public void MSelectedControlsHUD()
    {
        if (GlobalSettings.Instance.controlsHUD)
        {
            SelectedControlsHUD.gameObject.SetActive(true);
        }
        else
        {
            SelectedControlsHUD.gameObject.SetActive(false);
        }
    }
    public void SetControlsHUD()
    {
        if (GlobalSettings.Instance.controlsHUD)
        {
            GlobalSettings.Instance.DisplayControlsHUD(false);
        }
        else
        {
            GlobalSettings.Instance.DisplayControlsHUD(true);
        }
        MSelectedControlsHUD();
    }
    public void MSelectedFullscreen()
    {
        if (GlobalSettings.Instance.fullscreen)
        {
            SelectedFullscreen.gameObject.SetActive(true);
        }
        else
        {
            SelectedFullscreen.gameObject.SetActive(false);
        }
    }
    public void SetFullscreen()
    {
        if (GlobalSettings.Instance.fullscreen)     // Active, click again = remove full screen
        {
            GlobalSettings.Instance.SetFullscreen(false);
        }
        else
        {
            GlobalSettings.Instance.SetFullscreen(true);
        }
        MSelectedFullscreen();
        GlobalSettings.Instance.UpdateDisplayResolution();
    }
    public void MSelectedResolution()
    {
        int[] res = GlobalSettings.Instance.GetResolutionValues();
        if (res[0] == 1280 && res[1] == 720)
        {
            SelectedResolution.transform.localPosition = new Vector2(213.2f, 0);
        }
        if (res[0] == 1920 && res[1] == 1080)
        {
            SelectedResolution.transform.localPosition = new Vector2(213.2f, -30);
        }
        if (res[0] == 2560 && res[1] == 1440)
        {
            SelectedResolution.transform.localPosition = new Vector2(213.2f, -60);
        }
        if (res[0] == 3840 && res[1] == 2160)
        {
            SelectedResolution.transform.localPosition = new Vector2(213.2f, -90);
        }
    }
    public void ChangeResolution(string button)
    {
        switch (button)
        {
            case "1280x720":
                GlobalSettings.Instance.SetResolutionValues(1280, 720);
                GlobalSettings.Instance.UpdateDisplayResolution();
                MSelectedResolution();
                break;
            case "1920x1080":
                GlobalSettings.Instance.SetResolutionValues(1920, 1080);
                GlobalSettings.Instance.UpdateDisplayResolution();
                MSelectedResolution();
                break;
            case "2560x1440":
                GlobalSettings.Instance.SetResolutionValues(2560, 1440);
                GlobalSettings.Instance.UpdateDisplayResolution();
                MSelectedResolution();
                break;
            case "3840x2160":
                GlobalSettings.Instance.SetResolutionValues(3840, 2160);
                GlobalSettings.Instance.UpdateDisplayResolution();
                MSelectedResolution();
                break;
        }
    }

    public void ToggleSoundSettingsInterface()
    {
        if (SoundSettings.activeInHierarchy)
        {
            CloseActiveSettingsInterface();
        }
        else
        {
            CloseActiveSettingsInterface();
            OpenSound();
            SetSettingsNavigations(MasterVolMinButton);
        }
    }
    public void OpenSound()
    {
        SoundSettings.SetActive(true);
        MasterVolMinButton.Select();
        dim.SetActive(true);
        border.SetActive(true);
        SoundButtonHighlight.SetActive(true);
    }
    public void CloseSound()
    {
        SoundButtonHighlight.SetActive(false);
        SoundSettings.SetActive(false);
        SoundButton.Select();
        dim.SetActive(false);
        border.SetActive(false);
    }
/*    public void OpenMouseKeyboard()
    {
        MouseKeyboardSettings.SetActive(true);
        MSMinButton.Select();
        dim.SetActive(true);
        border.SetActive(true);
    }
    public void CloseMouseKeyboard()
    {
        MouseKeyboardSettings.SetActive(false);
        MouseKeyboardButton.Select();
        dim.SetActive(false);
        border.SetActive(false);
    }*/
    public void ToggleControlSettingsInterface()
    {
        if (ControllerSettings.activeInHierarchy)
        {
            CloseActiveSettingsInterface();
        }
        else
        {
            CloseActiveSettingsInterface();
            OpenController();
            SetSettingsNavigations(MSMinButton);
        }
    }
    public void OpenController()
    {
        ControllerSettings.SetActive(true);
        MSMinButton.Select();
        dim.SetActive(true);
        border.SetActive(true);
        ControlsButtonHighlight.SetActive(true);
        if(MovementSettings.Instance.invertedX == -1) { SelectedInvertedX.gameObject.SetActive(true); }
        else { SelectedInvertedX.gameObject.SetActive(false); }
        if (MovementSettings.Instance.invertedY == -1) { SelectedInvertedY.gameObject.SetActive(true); }
        else { SelectedInvertedY.gameObject.SetActive(false); }
    }
    public void CloseController()
    {
        ControlsButtonHighlight.SetActive(false);
        ControllerSettings.SetActive(false);
        ControlsButton.Select();
        dim.SetActive(false);
        border.SetActive(false);
    }

    private void OnEnable()
    {
        //CloseMouseKeyboard();
        CloseActiveSettingsInterface();

        dim.SetActive(false);
    }
    private void OnDisable()        // Hide all secondary menus (LevelSelect, Settings, etc) whenever unpaused
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject AutoButton, KeyboardButton, GamepadButton;
    [SerializeField] private GameObject res1280Button, res1920Button, res2560Button, res3840Button;

    [SerializeField] private GameObject SelectedControllerType, ControlsHUDActive, FullscreenActive, SelectedResolution;

    public void SelectControllerType(string type)       // AUTO, KEYBOARD, XBOX
    {
        switch(type)
        {
            case "AUTO":
                //SelectedControllerType.transform.localPosition = new Vector2(154.7f, -0.3f);
                SelectedControllerType.transform.localPosition = AutoButton.transform.localPosition;
                break;
            case "KEYBOARD":
                //SelectedControllerType.transform.localPosition = new Vector2(276.2f, -0.3f);
                SelectedControllerType.transform.localPosition = KeyboardButton.transform.localPosition;
                break;
            case "XBOX":
                //SelectedControllerType.transform.localPosition = new Vector2(399.6f, -0.3f);
                SelectedControllerType.transform.localPosition = GamepadButton.transform.localPosition;
                break;
        }
        GlobalSettings.Instance.SetPlayerControllerSetting(type);   
    }
    public void ToggleControlsHUD()
    {
        if(GlobalSettings.Instance.controlsHUD)
        {
            GlobalSettings.Instance.DisplayControlsHUD(false);
            ControlsHUDActive.SetActive(false);
            AudioManager.instance.ui_checkBoxState = false;
        }
        else
        {
            GlobalSettings.Instance.DisplayControlsHUD(true);
            ControlsHUDActive.SetActive(true);
            AudioManager.instance.ui_checkBoxState = true;
        }
    }
    public void ToggleFullscreen()
    {
        if(GlobalSettings.Instance.fullscreen)
        {
            GlobalSettings.Instance.SetFullscreen(false);
            FullscreenActive.SetActive(false);
            AudioManager.instance.ui_checkBoxState = false;
        }
        else
        {
            GlobalSettings.Instance.SetFullscreen(true);
            FullscreenActive.SetActive(true);
            AudioManager.instance.ui_checkBoxState = true;
        }
        GlobalSettings.Instance.UpdateDisplayResolution();
    }

    public void SelectResolution(string resolution)
    {
        switch(resolution)
        {
            case "1280x720":
                GlobalSettings.Instance.SetResolutionValues(1280, 720);
                GlobalSettings.Instance.UpdateDisplayResolution();
                //SelectedResolution.transform.localPosition = new Vector2(221.9f, 8.1f);
                SelectedResolution.transform.localPosition = res1280Button.transform.localPosition;
                break;
            case "1920x1080":
                GlobalSettings.Instance.SetResolutionValues(1920, 1080);
                GlobalSettings.Instance.UpdateDisplayResolution();
                //SelectedResolution.transform.localPosition = new Vector2(221.9f, -24.4f);
                SelectedResolution.transform.localPosition = res1920Button.transform.localPosition;
                break;
            case "2560x1440":
                GlobalSettings.Instance.SetResolutionValues(2560, 1440);
                GlobalSettings.Instance.UpdateDisplayResolution();
                //SelectedResolution.transform.localPosition = new Vector2(221.9f, -58.1f);
                SelectedResolution.transform.localPosition = res2560Button.transform.localPosition;
                break;
            case "3840x2160":
                GlobalSettings.Instance.SetResolutionValues(3840, 2160);
                GlobalSettings.Instance.UpdateDisplayResolution();
                //SelectedResolution.transform.localPosition = new Vector2(221.9f, -91.5f);
                SelectedResolution.transform.localPosition = res3840Button.transform.localPosition;
                break;
        }
    }

    private void OnEnable()
    {
        // Initialize all placements for the selected fields
        switch(GlobalSettings.Instance.resolution[0])
        {
            case 1280:
                //SelectedResolution.transform.localPosition = new Vector2(221.9f, 8.1f);
                SelectedResolution.transform.localPosition = res1280Button.transform.localPosition;
                break;
            case 1920:
                //SelectedResolution.transform.localPosition = new Vector2(221.9f, -24.4f);
                SelectedResolution.transform.localPosition = res1920Button.transform.localPosition;
                break;
            case 2560:
                //SelectedResolution.transform.localPosition = new Vector2(221.9f, -58.1f);
                SelectedResolution.transform.localPosition = res2560Button.transform.localPosition;
                break;
            case 3840:
                //SelectedResolution.transform.localPosition = new Vector2(221.9f, -91.5f);
                SelectedResolution.transform.localPosition = res3840Button.transform.localPosition;
                break;
        }

        switch (GlobalSettings.Instance.playerControllerSetting)
        {
            case "AUTO":
                //SelectedControllerType.transform.localPosition = new Vector2(154.7f, -0.3f);
                SelectedControllerType.transform.localPosition = AutoButton.transform.localPosition;
                break;
            case "KEYBOARD":
                //SelectedControllerType.transform.localPosition = new Vector2(276.2f, -0.3f);
                SelectedControllerType.transform.localPosition = KeyboardButton.transform.localPosition;
                break;
            case "XBOX":
                //SelectedControllerType.transform.localPosition = new Vector2(399.6f, -0.3f);
                SelectedControllerType.transform.localPosition = GamepadButton.transform.localPosition;
                break;
        }

        ControlsHUDActive.SetActive(GlobalSettings.Instance.controlsHUD);
        FullscreenActive.SetActive(GlobalSettings.Instance.fullscreen);
    }
    private void OnDisable()
    {
        GlobalSettings.Instance.SavePlayerPrefs();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

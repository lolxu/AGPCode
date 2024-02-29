using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject SelectedControllerType, ControlsHUDActive, FullscreenActive, SelectedResolution;

    public void SelectControllerType(string type)       // AUTO, KEYBOARD, XBOX
    {
        switch(type)
        {
            case "AUTO":
                SelectedControllerType.transform.localPosition = new Vector2(154.7f, -0.3f);
                break;
            case "KEYBOARD":
                SelectedControllerType.transform.localPosition = new Vector2(276.2f, -0.3f);
                break;
            case "XBOX":
                SelectedControllerType.transform.localPosition = new Vector2(399.6f, -0.3f);
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
        }
        else
        {
            GlobalSettings.Instance.DisplayControlsHUD(true);
            ControlsHUDActive.SetActive(true);

        }
    }
    public void ToggleFullscreen()
    {
        if(GlobalSettings.Instance.fullscreen)
        {
            GlobalSettings.Instance.SetFullscreen(false);
            FullscreenActive.SetActive(false);
        }
        else
        {
            GlobalSettings.Instance.SetFullscreen(true);
            FullscreenActive.SetActive(true);
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
                SelectedResolution.transform.localPosition = new Vector2(236.7551f, 15);
                break;
            case "1920x1080":
                GlobalSettings.Instance.SetResolutionValues(1920, 1080);
                GlobalSettings.Instance.UpdateDisplayResolution();
                SelectedResolution.transform.localPosition = new Vector2(236.7551f, -15);
                break;
            case "2560x1440":
                GlobalSettings.Instance.SetResolutionValues(2560, 1440);
                GlobalSettings.Instance.UpdateDisplayResolution();
                SelectedResolution.transform.localPosition = new Vector2(236.7551f, -45);
                break;
            case "3840x2160":
                GlobalSettings.Instance.SetResolutionValues(3840, 2160);
                GlobalSettings.Instance.UpdateDisplayResolution();
                SelectedResolution.transform.localPosition = new Vector2(236.7551f, -75);
                break;
        }
    }

    private void OnEnable()
    {
        // Initialize all placements for the selected fields
        if (GlobalSettings.Instance.resolution[0] == 1280) { SelectedResolution.transform.localPosition = new Vector2(236.7551f, 15); } 
        else if (GlobalSettings.Instance.resolution[0] == 1920) { SelectedResolution.transform.localPosition = new Vector2(236.7551f, -15); }
        else if (GlobalSettings.Instance.resolution[0] == 2560) { SelectedResolution.transform.localPosition = new Vector2(236.7551f, -45); }
        else if (GlobalSettings.Instance.resolution[0] == 3840) { SelectedResolution.transform.localPosition = new Vector2(236.7551f, -75); }

        switch (GlobalSettings.Instance.playerControllerSetting)
        {
            case "AUTO":
                SelectedControllerType.transform.localPosition = new Vector2(154.7f, -0.3f);
                break;
            case "KEYBOARD":
                SelectedControllerType.transform.localPosition = new Vector2(276.2f, -0.3f);
                break;
            case "XBOX":
                SelectedControllerType.transform.localPosition = new Vector2(399.6f, -0.3f);
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

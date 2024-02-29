using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalSettings : MonoBehaviour
{

    // PLAYER PREFS SAVED HERE:
    // - int   "Fullscreen"     as bool
    // - int    "Resolution_X", "Resolution_Y"
    // - int    "Controls HUD"  as bool


    public static GlobalSettings Instance;

    public string displayedController { get; private set; }     // The type that will be displayed
    public string controller { get; private set; }              // Controller type detected in PlayerInput
    public string playerControllerSetting { get; private set; }     // The setting that will FORCE a controller type, selected by the player

    public int[] resolution { get; private set; } = new int[2];
    public bool fullscreen { get; private set; }
    public bool controlsHUD { get; private set; }

    [SerializeField] private PlayerInput pInput;
    public string controlScheme { get; private set; }
    private string controlsSchemePrevScene;

    [SerializeField] private CanvasScaler HUDCanvas, PauseCanvas, WinCanvas;

    [SerializeField] private AdaptiveButtonsHUD adaptiveButtons;

    private int bypassFirstTwoControlsChanged = 0;


    public static Action ControlsChangedEvent;
    public void OnControlsChanged()
    {
        if(bypassFirstTwoControlsChanged < 2) {     // Cuz pInput keeps defaulting to Keyboard AFTER I reset it...
            bypassFirstTwoControlsChanged++;
            return;
        }
        switch (pInput.currentControlScheme)
        {
            case "Keyboard":
                controlScheme = "KEYBOARD";
                SetController("KEYBOARD");
                break;
            case "Playstation Controller":
                controlScheme = "PLAYSTATION";
                SetController("PLAYSTATION");
                break;
            case "Switch Pro Controller":
                controlScheme = "OTHER";
                SetController("OTHER");
                break;
            case "Xbox Controller":
                controlScheme = "XBOX";
                SetController("XBOX");
                break;
        }
        if(ControlsChangedEvent != null) { ControlsChangedEvent(); }
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "MainMenu") { bypassFirstTwoControlsChanged = 2; }
        switch (controlsSchemePrevScene)
        {
            case "KEYBOARD":
                pInput.SwitchCurrentControlScheme("Keyboard");
                break;
            case "PLAYSTATION":
            case "OTHER":
            case "XBOX":
                pInput.SwitchCurrentControlScheme("Xbox Controller");
                break;
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        controlsSchemePrevScene = controlScheme;
        bypassFirstTwoControlsChanged = 0;
    }

    public void SetPlayerControllerSetting(string _option)
    {
        playerControllerSetting = _option;
        SetDisplayedController();
    }

    public void SetDisplayedController()
    {
        if(playerControllerSetting == "AUTO")
        {
            displayedController = controller;
        }
        else
        {
            switch(playerControllerSetting)
            {
                case "KEYBOARD":
                    displayedController = playerControllerSetting;
                    break;
                case "XBOX":
                    displayedController = playerControllerSetting;
                    break;
                case "PLAYSTATION":
                case "OTHER":
                case "MULTIPLE":
                    displayedController = "XBOX";       // Just do XBOX for now
                    break;
            }
        }
        if(SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (PauseCanvas == null) {
                var canvasScalers = FindObjectsOfType<CanvasScaler>(true);

                foreach(CanvasScaler cs in canvasScalers)
                {
                    // Debug.Log(cs.name);
                    if(cs.name == "PauseCanvas")
                    {
                        PauseCanvas = cs.GetComponent<CanvasScaler>();
                    }
                }           
            }

            if (PauseManager.Instance && PauseCanvas.gameObject.activeInHierarchy)
            {
                // Debug.Log("Setting Controls");
                PauseManager.Instance.SetControls();
            }

            if (UIManager.Instance &&
                UIManager.Instance.isPaused)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        // Debug.Log("Displayed Controller set to: " + displayedController);
    }
    public void SetController(string _controller)
    {
        controller = _controller;
        SetDisplayedController();
    }

    public int[] GetResolutionValues()
    {
        return resolution;
    }
    public void SetResolutionValues(int w, int h)
    {
        resolution[0] = w;
        resolution[1] = h;
    }
    public void SetFullscreen(bool status)
    {
        // Debug.Log("Fullscreen set to " + status.ToString());
        fullscreen = status;
    }
    public void DisplayControlsHUD(bool status)
    {
        controlsHUD = status;
        PlayerPrefs.SetInt("Controls HUD", controlsHUD ? 1 : 0);
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (!adaptiveButtons)
            {
                adaptiveButtons = FindObjectsOfType<AdaptiveButtonsHUD>(true)[0];
            }
            //Debug.Log($"Level Contains Burrow: {SceneManager.GetActiveScene().name.Contains("Burrow")}\tLevel Contains Onboard: {SceneManager.GetActiveScene().name.Contains("Onboard")}");
            if (SceneManager.GetActiveScene().name.Contains("Burrow") || SceneManager.GetActiveScene().name.Contains("Onboard"))
            {
                return;
            }
            //Debug.Log($"Adaptive Buttons Toggled");
            adaptiveButtons.AdaptiveButtons.SetActive(controlsHUD);
        }
    }

    public void UpdateDisplayResolution()
    {
        Screen.SetResolution(resolution[0], resolution[1], fullscreen);

        PlayerPrefs.SetInt("Resolution_X", resolution[0]);
        PlayerPrefs.SetInt("Resolution_Y", resolution[1]);
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.Save();     // Done automatically usually but call when closing pause or settings just to be safe
    }
    public void LoadGlobalPlayerPrefs()
    {
       
        // Refactored a bit to set the initial values in PlayerPrefs
        // TODO: Create a default resolution if none is found
        // resolution[0] = PlayerPrefs.GetInt("Resolution_X");
        // resolution[1] = PlayerPrefs.GetInt("Resolution_Y");
        // resolution = resolution[0] == 0 || resolution[1] == 0 ? new int[] { 1920, 1080 } : resolution;
        // PlayerPrefs.SetInt("Resolution_X", resolution[0]);
        // PlayerPrefs.SetInt("Resolution_Y", resolution[1]);
        
        // Resolution defaulted to 1920 x 1080 if does not exist
        if (!PlayerPrefs.HasKey("Resolution_X"))
        {
            resolution[0] = 1920;
            PlayerPrefs.SetInt("Resolution_X", resolution[0]);
            // Debug.Log("Initialized Resolution_X to " + resolution[0].ToString());
        }
        else { resolution[0] = PlayerPrefs.GetInt("Resolution_X"); }

        if (!PlayerPrefs.HasKey("Resolution_Y"))
        {
            resolution[1] = 1080;
            PlayerPrefs.SetInt("Resolution_Y", resolution[1]);
            // Debug.Log("Initialized Resolution_Y to " + resolution[1].ToString());

        }
        else { resolution[1] = PlayerPrefs.GetInt("Resolution_Y"); }
        
        // Fullscreen defaulted to true if does not exist
        if (!PlayerPrefs.HasKey("Fullscreen"))
        {
            fullscreen = true;
            PlayerPrefs.SetInt("Fullscreen", 1);
            // Debug.Log("Initialized Fullscreen to " + fullscreen);
        }
        else { fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1 ? true : false; }
        
        // Controls HUD defaulted to true if does not exist
        if (!PlayerPrefs.HasKey("Controls HUD"))
        {
            controlsHUD = true;
            PlayerPrefs.SetInt("Controls HUD", 1);
            // Debug.Log("Initialized Controls HUD to" + controlsHUD);
        }
        else { controlsHUD = PlayerPrefs.GetInt("Controls HUD") == 1 ? true : false; }

        SavePlayerPrefs();

        // Debug.Log($"LOADED Resolution: {resolution[0]}, {resolution[1]}\tFullscreen: {fullscreen}");
    }
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        LoadGlobalPlayerPrefs();
        playerControllerSetting = "AUTO";
        displayedController = "AUTO";
        controller = "AUTO";
        UpdateDisplayResolution();

    }
    // Start is called before the first frame update
    void Start()
    {
        //SetController("KEYBOARD");
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // Update is called once per frame
    void Update()
    {
        //if (playerControllerSetting == "AUTO" && displayedController != controlScheme) { OnControlsChanged(); }
    }
}

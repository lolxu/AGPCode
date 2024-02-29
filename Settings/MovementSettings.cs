using __OasisBlitz.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementSettings : MonoBehaviour
{
    // PLAYER PREFS SAVED HERE
    // - float  "Mouse Multiplier"
    // - float  "Mouse Sensitivity"
    // - float  "Controller Multiplier"
    // - float  "Controller Sensitivity"
    // - int    "Inverted X" -- 1 for normal, -1 for inverted
    // - int    "Inverted Y" -- 1 for normal, -1 for inverted
    public static MovementSettings Instance;

    [SerializeField] private __OasisBlitz.Player.PlayerInput pInput;
    [SerializeField] private InputActionAsset inputActions;
    public float controllerLookMultiplier { get; private set; }
    public float mouseLookMultiplier { get; private set; }

    private float iControllerLook = 150, iMouseLook = 0.1f;  // i for "initial" values of the scale vector 2's
    public float controllerSensitivity { get; private set; }
    public float mouseSensitivity { get; private set; }      // Scale factors added to the initial values

    public int invertedX { get; private set; }
    public int invertedY { get; private set; }

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

        LoadMovementPlayerPrefs();
    }

    private void LoadMovementPlayerPrefs()
    {

        // SetMouseSensitivity() and SetControllerSensitivity() put values in PlayerPrefs already
        if(!PlayerPrefs.HasKey("Mouse Sensitivity")) { SetMouseSensitivity(0.1f); }
        else { SetMouseSensitivity(PlayerPrefs.GetFloat("Mouse Sensitivity")); }
        
        if(!PlayerPrefs.HasKey("Controller Sensitivity")) { SetControllerSensitivity(150); }
        else { SetControllerSensitivity(PlayerPrefs.GetFloat("Controller Sensitivity")); }

        
        if (!PlayerPrefs.HasKey("Mouse Multiplier"))
        {
            mouseLookMultiplier = 0;
            PlayerPrefs.SetFloat("Mouse Multiplier", mouseLookMultiplier);
        }
        else { mouseLookMultiplier = PlayerPrefs.GetFloat("Mouse Multiplier");}

        if (!PlayerPrefs.HasKey("Controller Multiplier"))
        {
            controllerLookMultiplier = 0;
            PlayerPrefs.SetFloat("Controller Multiplier", controllerLookMultiplier);
        }
        else { controllerLookMultiplier = PlayerPrefs.GetFloat("Controller Multiplier"); }

        if (!PlayerPrefs.HasKey("Inverted X"))
        {
            invertedX = 1;
            PlayerPrefs.SetInt("Inverted X", 1);
        }
        else { invertedX = PlayerPrefs.GetInt("Inverted X"); }

        if (!PlayerPrefs.HasKey("Inverted Y"))
        {
            invertedY = 1;
            PlayerPrefs.SetInt("Inverted Y", 1);
        }
        else { invertedY = PlayerPrefs.GetInt("Inverted Y"); }

        PlayerPrefs.Save();
        // Debug.Log($"LOADED Mouse Sensitivity: {mouseSensitivity}\tMouse Multiplier: {mouseLookMultiplier}\tController Sensitivity: {controllerSensitivity}\tController Multiplier: {controllerLookMultiplier}");
    }



    // To be used by buttons in the settings
    public bool InvertX()
    {
        if (invertedX == 1) { invertedX = -1; }
        else { invertedX = 1; }
        // Mouse
        inputActions["Look"].ApplyBindingOverride(0, new InputBinding { overrideProcessors = $"DeltaTimeScale,ScaleVector2(x={mouseSensitivity * invertedX},y={mouseSensitivity * invertedY})" });
        // Controller
        inputActions["Look"].ApplyBindingOverride(1, new InputBinding { overrideProcessors = $"ScaleVector2(x={controllerSensitivity * invertedX},y={controllerSensitivity * invertedY})" });
        PlayerPrefs.SetInt("Inverted X", invertedX);
        return invertedX == -1;
    }
    public bool InvertY()
    {
        if (invertedY == 1) { invertedY = -1; }
        else { invertedY = 1; }
        // Mouse
        inputActions["Look"].ApplyBindingOverride(0, new InputBinding { overrideProcessors = $"DeltaTimeScale,ScaleVector2(x={mouseSensitivity * invertedX},y={mouseSensitivity * invertedY})" });
        // Controller
        inputActions["Look"].ApplyBindingOverride(1, new InputBinding { overrideProcessors = $"ScaleVector2(x={controllerSensitivity * invertedX},y={controllerSensitivity * invertedY})" });
        PlayerPrefs.SetInt("Inverted Y", invertedY);
        return invertedY == -1;
    }
    public bool SetMouseLookMultiplier(float value)
    {
        if(value < -0.5f || value > 0.5f) { return false; }
        mouseLookMultiplier = value;
        SetMouseSensitivity(iMouseLook * (1 + mouseLookMultiplier * 2));
        inputActions["Look"].ApplyBindingOverride(0, new InputBinding { overrideProcessors = $"DeltaTimeScale,ScaleVector2(x={mouseSensitivity * invertedX},y={mouseSensitivity * invertedY})" });
        PlayerPrefs.SetFloat("Mouse Multiplier", mouseLookMultiplier);
        return true;
    }
    public bool AddMouseLookMultiplier()
    {
        if (mouseLookMultiplier > 0.4f) { return false; }      // MAX 0.5
        else
        {
            mouseLookMultiplier += 0.1f;
            SetMouseSensitivity(iMouseLook * (1 + mouseLookMultiplier*2));
            inputActions["Look"].ApplyBindingOverride(0, new InputBinding { overrideProcessors = $"DeltaTimeScale,ScaleVector2(x={mouseSensitivity*invertedX},y={mouseSensitivity*invertedY})" });
            // Debug.Log(inputActions["Look"].bindings[0].path + "\t" + inputActions["Look"].bindings[0].overrideProcessors);

            PlayerPrefs.SetFloat("Mouse Multiplier", mouseLookMultiplier);
            // pInput.ChangeMouseSensitivity(sf);
            return true;
        }
    }
    public bool SubtractMouseLookMultiplier()
    {
        if (mouseLookMultiplier < -0.4f) { return false; }      // MAX 0.5
        else
        {
            mouseLookMultiplier -= 0.1f;
            SetMouseSensitivity(iMouseLook * (1 + mouseLookMultiplier));
            inputActions["Look"].ApplyBindingOverride(0, new InputBinding { overrideProcessors = $"DeltaTimeScale,ScaleVector2(x={mouseSensitivity*invertedX},y={mouseSensitivity*invertedY})" });
            // Debug.Log(inputActions["Look"].bindings[0].path + "\t" + inputActions["Look"].bindings[0].overrideProcessors);

            PlayerPrefs.SetFloat("Mouse Multiplier", mouseLookMultiplier);
            // pInput.ChangeMouseSensitivity(sf);
            return true;
        }
    }
    public void SetMouseSensitivity(float ms)
    {
        mouseSensitivity = ms;
        PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSensitivity);
        // Debug.Log($"Set Mouse Sensitivity to {PlayerPrefs.GetFloat("Mouse Sensitivity")}");
    }
    public bool SetControllerLookMultiplier(float value)
    {
        if (value <= -0.5f || value >= 0.5f) { return false; }
        controllerLookMultiplier = value;
        SetControllerSensitivity(iControllerLook * (1 + controllerLookMultiplier * 2));
        inputActions["Look"].ApplyBindingOverride(1, new InputBinding { overrideProcessors = $"ScaleVector2(x={controllerSensitivity * invertedX},y={controllerSensitivity * invertedY})" });
        PlayerPrefs.SetFloat("Controller Multiplier", controllerLookMultiplier);
        return true;
    }
    public bool AddControllerLookMultiplier()
    {
        if (controllerLookMultiplier > 0.4f) { return false; }      // MAX 0.5
        else
        {
            controllerLookMultiplier += 0.1f;
            SetControllerSensitivity(iControllerLook * (1 + controllerLookMultiplier*2));
            inputActions["Look"].ApplyBindingOverride(1, new InputBinding { overrideProcessors = $"ScaleVector2(x={controllerSensitivity*invertedX},y={controllerSensitivity*invertedY})" });
            // Debug.Log(inputActions["Look"].bindings[1].path + "\t" + inputActions["Look"].bindings[1].overrideProcessors);

            PlayerPrefs.SetFloat("Controller Multiplier", controllerLookMultiplier);

            // pInput.ChangeControllerSensitivity(sf);
            return true;
        }
    }
    public bool SubtractControllerLookMultiplier()
    {
        if (controllerLookMultiplier < -0.4f) { return false; }      // MAX 0.5
        else
        {
            controllerLookMultiplier -= 0.1f;
            SetControllerSensitivity(iControllerLook * (1 + controllerLookMultiplier));
            inputActions["Look"].ApplyBindingOverride(1, new InputBinding { overrideProcessors = $"ScaleVector2(x={controllerSensitivity*invertedX},y={controllerSensitivity*invertedY})" });
            // Debug.Log(inputActions["Look"].bindings[1].path + "\t" + inputActions["Look"].bindings[1].overrideProcessors);

            PlayerPrefs.SetFloat("Controller Multiplier", controllerLookMultiplier);
            // pInput.ChangeControllerSensitivity(sf);
            return true;
        }
    }
    public void SetControllerSensitivity(float cs)
    {
        controllerSensitivity = cs;
        PlayerPrefs.SetFloat("Controller Sensitivity", controllerSensitivity);
    }
    private void Start()
    {
        if(invertedX == 0) { invertedX = 1; }
        if(invertedY == 0) { invertedY = 1; }

        // PlayerPrefs loaded before this in Awake function so should be okay to use sensitivity floats
        inputActions["Look"].ApplyBindingOverride(0, new InputBinding { overrideProcessors = $"DeltaTimeScale,ScaleVector2(x={mouseSensitivity*invertedX},y={mouseSensitivity*invertedY})" });
        inputActions["Look"].ApplyBindingOverride(1, new InputBinding { overrideProcessors = $"ScaleVector2(x={controllerSensitivity*invertedX},y={controllerSensitivity*invertedY})" });
    }
}

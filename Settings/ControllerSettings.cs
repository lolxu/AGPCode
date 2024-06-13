using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControllerSettings : MonoBehaviour
{

    [SerializeField] private MouseKeyboardSettings mks;

    [SerializeField] private Button csMinus, csPlus;        // Add/subtract controller sensitivity buttons

    [SerializeField] private GameObject circleDisplay;      // 0, 0 is middle   -- [-135, 135]
    [SerializeField] private Slider fill;
    [SerializeField] private GameObject hapticsEnabledDisplay;
    private bool HapticsEnabled;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Haptics"))
        {
            HapticsEnabled = true;
            PlayerPrefs.SetInt("Haptics", 1);
        }
        else
        {
            HapticsEnabled = PlayerPrefs.GetInt("Haptics") == 1 ? true : false;
        }
    }
    public void ToggleHaptics()
    {
        if(!HapticsManager.Instance)        // Main Menu
        {
            // No Haptics Manager, just toggle and HapticsManager will load the setting when initialized
            if(HapticsEnabled)
            {
                HapticsEnabled = false;
                AudioManager.instance.ui_checkBoxState = false;
                SetHapticsDisplay();
            }
            else
            {
                HapticsEnabled = true;
                AudioManager.instance.ui_checkBoxState = true;
                SetHapticsDisplay();
            }
            PlayerPrefs.SetInt("Haptics", HapticsEnabled == true ? 1 : 0);
        }
        else        // Outside of MainMenu
        {
            if (HapticsManager.Instance.HapticsEnabled)
            {
                HapticsManager.Instance.HapticsEnabled = false;
                AudioManager.instance.ui_checkBoxState = false;
                SetHapticsDisplay();
            }
            else
            {
                HapticsManager.Instance.HapticsEnabled = true;
                AudioManager.instance.ui_checkBoxState = true;
                SetHapticsDisplay();
            }
            PlayerPrefs.SetInt("Haptics", HapticsManager.Instance.HapticsEnabled ? 1 : 0);
        }
        
    }
    public void SetHapticsDisplay()
    {
        if (!HapticsManager.Instance) { hapticsEnabledDisplay.SetActive(HapticsEnabled); }
        else { hapticsEnabledDisplay.SetActive(HapticsManager.Instance.HapticsEnabled); }
    }
    public void IncControllerSensitivity()
    {
        MovementSettings.Instance.AddControllerLookMultiplier();
    }
    public void DecControllerSensitivity()
    {
        MovementSettings.Instance.SubtractControllerLookMultiplier();
    }

    private void OnEnable()
    {
        mks.SetInvertDisplays();
        SetHapticsDisplay();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;
using UnityEngine.SceneManagement;

public class HapticsManager : MonoBehaviour
{
    // PLAYER PREFS:
    // - "Haptics"

    public static HapticsManager Instance;
    
    //To check if your device has basic haptic capabilities, check this property:
    //bool hapticsSupported = DeviceCapabilities.isVersionSupported;
    //To check if the device has advanced haptic capabilities, we need to know if it meets the necessary requirements with:
    //if (DeviceCapabilities.meetsAdvancedRequirements == true)



    // Start is called before the first frame update
    [SerializeField] private HapticPatterns.PresetType Test;
    [SerializeField] private HapticPatterns.PresetType KillEnemyPreset;
    [SerializeField] private HapticPatterns.PresetType PlayerDeathPreset;
    [SerializeField] private HapticPatterns.PresetType EnterDrillBelowImpactPreset;
    
    // refers to the strength of the vibration
    [Range(0.0f, 1.0f)]
    [SerializeField] private float DrillHapticAmplitude;
    //refers to the pitch or tone of the haptic effect. The frequency becomes higher as the value increases
    [Range(0.0f, 1.0f)]
    [SerializeField] private float DrillHapticFrequency;

    private bool Below;

    public bool HapticsEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Initialize();
    }

    private void ReInitialize(Scene scene, LoadSceneMode mode)
    {
        Initialize();
    }

    private void Initialize()
    {
        Below = false;
    }

    void Start()
    {
        //StartCoroutine(BUZZ());
        if (!PlayerPrefs.HasKey("Haptics"))
        {
            HapticsEnabled = true;
            PlayerPrefs.SetInt("Haptics", 1);
        }
        else
        {
            HapticsEnabled = PlayerPrefs.GetInt("Haptics") == 1 ? true : false;     // If edited in Main Menu (ControllerSettings.cs -> ToggleHaptics) should update here?
        }
    }

    private IEnumerator BUZZ()
    {
        while (true)
        {
            HapticPatterns.PlayPreset(Test);
            yield return new WaitForSeconds(1.0f);
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ReInitialize;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ReInitialize;
    }

    public void PlayKillEnemyHaptic()
    {
        if(!HapticsEnabled) { return; }
        HapticPatterns.PlayPreset(KillEnemyPreset);
    }
    
    public void PlayPlayerDeathHaptic()
    {
        if (!HapticsEnabled) { return; }
        HapticPatterns.PlayPreset(PlayerDeathPreset);
    }
    
    public void PlayEnterDrillBelowHaptic()
    {
        if (!HapticsEnabled) { return; }
        HapticPatterns.PlayPreset(EnterDrillBelowImpactPreset);
    }

    public void StartDrillHaptic()
    {
        if (!HapticsEnabled) { return; }
        HapticPatterns.PlayConstant(DrillHapticAmplitude, DrillHapticFrequency, 9999.0f);
        Below = true;
    }
    
    public void StopDrillHaptic()
    {
        if (!HapticsEnabled) { return; }
        HapticController.Stop();
        Below = false;
    }

    public void ResumeHapticsFromPause()
    {
        if (Below)
        {
            StartDrillHaptic();
        }
    }
    public void StopHapticsFromPause()
    {
        HapticController.Stop();
    }
}

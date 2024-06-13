using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class DrillLockedBelowSound : MonoBehaviour
{
    [SerializeField] private EventReference lockedBelowDrillSound;
    
    private EventInstance lockedBelowDrillInstance;
    
    private void OnSceneChange()
    {
        InitializeDrillSound();
    }

    private void OnEnable()
    {
        AudioManager.onAudioSceneChange += OnSceneChange;
    }

    private void OnDisable()
    {
        AudioManager.onAudioSceneChange -= OnSceneChange;
    }
    
    private void InitializeDrillSound()
    {
        lockedBelowDrillInstance = AudioManager.instance.CreateEventInstance(lockedBelowDrillSound, transform);
        lockedBelowDrillInstance.setVolume(0.0f);
        lockedBelowDrillInstance.start();
    }

    public void SetTutorialDrillVolume(float normalizedVolume)
    {
        if (lockedBelowDrillInstance.isValid())
        {
            lockedBelowDrillInstance.setVolume(normalizedVolume);
        }
    }
    
    public void StopTutorialDrillSound()
    {
        if (lockedBelowDrillInstance.isValid())
        {
            lockedBelowDrillInstance.stop(STOP_MODE.IMMEDIATE);
        }
    }
}

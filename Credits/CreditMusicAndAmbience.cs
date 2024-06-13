using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class CreditMusicAndAmbience : MonoBehaviour
{
    [SerializeField] private EventReference LevelMusic;
    [SerializeField] private EventReference LevelAmbience;
    [SerializeField] private EventReference LevelScatter;
    [SerializeField] private Transform listener;
    [FormerlySerializedAs("StartImmediately")] [SerializeField] private bool StartMusicImmediately = true;
    [SerializeField] private bool StartAmbienceImmediately = true;
    
    private EventInstance _ambienceEventInstance;
    public EventInstance _musicEventInstance;
    private EventInstance _scatterEventInstance;
    
    private void OnSceneChange()
    {
        InitializeMusic();
        InitializeAmbience();
        if (StartAmbienceImmediately)
        {
            StartAmbience();
        }
        if (StartMusicImmediately)
        {
            StartMusic();
        }
    }

    private void Start()
    {
        OnSceneChange();
    }

    private void OnDisable()
    {
        _musicEventInstance.stop(STOP_MODE.IMMEDIATE);
        _ambienceEventInstance.stop(STOP_MODE.IMMEDIATE);
    }

    public void InitializeMusic()
    {
        _musicEventInstance = RuntimeManager.CreateInstance(LevelMusic);
    }
    
    public void InitializeAmbience()
    {
        _ambienceEventInstance = RuntimeManager.CreateInstance(LevelAmbience);
        _scatterEventInstance = RuntimeManager.CreateInstance(LevelScatter);
    }

    public void SetPauseAmbience(bool isPaused)
    {
        _ambienceEventInstance.setPaused(isPaused);
        _scatterEventInstance.setPaused(isPaused);
    }

    public void SetPauseMusic(bool isPaused)
    {
        _musicEventInstance.setPaused(isPaused);
    }

    public void StartMusic()
    {
        _musicEventInstance.start();
    }
    
    public void StartAmbience()
    {
        _ambienceEventInstance.start();
        _scatterEventInstance.start();
    }
    
    public void StopMusic()
    {
        _musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    
    public void StopAmbience()
    {
        _ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _scatterEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class GameplayMusic : MonoBehaviour
{
    [SerializeField] private EventReference LevelMusic;
    [SerializeField] private EventReference LevelAmbience;
    [SerializeField] private EventReference LevelScatter;
    [SerializeField] private Transform listener;
    [FormerlySerializedAs("StartImmediately")] [SerializeField] private bool StartMusicImmediately = true;
    [SerializeField] private bool StartAmbienceImmediately = true;
    
    private EventInstance _ambienceEventInstance;
    private EventInstance _musicEventInstance;
    private EventInstance _scatterEventInstance;
    
    private void OnSceneChange()
    {
        GameObject cam = GameObject.FindWithTag("PlayerCamera");
        if(cam != null)
        {
            listener = cam.transform;
        }
        else
        {
            listener = transform;
            Debug.LogWarning("Player camera not found for level scatter sound");
        }
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

    private void OnEnable()
    {
        AudioManager.onAudioSceneChange += OnSceneChange;
    }

    private void OnDisable()
    {
        AudioManager.onAudioSceneChange -= OnSceneChange;
    }

    public void InitializeMusic()
    {
        _musicEventInstance = AudioManager.instance.CreateEventInstance(LevelMusic, transform);
    }
    
    public void InitializeAmbience()
    {
        _ambienceEventInstance = AudioManager.instance.CreateEventInstance(LevelAmbience, transform);
        _scatterEventInstance = AudioManager.instance.CreateEventInstance(LevelScatter, listener);
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
        if (SceneManager.GetActiveScene().name.Contains("Burrow"))
        {
            RuntimeManager.StudioSystem.setParameterByNameWithLabel("ReverbSpace", "Burrow");
        }
        else
        {
            RuntimeManager.StudioSystem.setParameterByNameWithLabel("ReverbSpace", "Level");
        }
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

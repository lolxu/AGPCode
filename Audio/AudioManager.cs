using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Runtime.InteropServices;
using System;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<EventInstance> _eventInstances;
    [SerializeField] private List<StudioEventEmitter> _eventEmitters;

    private EventInstance _ambienceEventInstance;
    private EventInstance _musicEventInstance;
    private EventInstance _voiceEventInstance;
    private EventDescription _voiceEventDescription;

    private EventReference _currentMusicRef;
    public bool allowPlayerSounds { private get; set; }

    public static AudioManager instance { get; private set; }

    public float musicCheckPoint = 0;
    private float musicCheckPointSet;

    private EVENT_CALLBACK beatCallback;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene");
            Destroy(gameObject);
        }
        instance = this;
        allowPlayerSounds = true;
        _eventInstances = new List<EventInstance>();
        _eventEmitters = new List<StudioEventEmitter>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleMusicAndAmbienceOnSceneChange;
    }

    private void OnDisable()
    {
        StopMusic();
        StopAmbience();
        CleanUp();
        SceneManager.sceneLoaded -= HandleMusicAndAmbienceOnSceneChange;
    }

    private void HandleMusicAndAmbienceOnSceneChange(Scene scene, LoadSceneMode mode)
    {
        StopMusic();
        StopAmbience();
        CleanUp();

        switch(scene.name)
        {
            case "MainMenu":
                InitializeMusic(FMODEvents.instance.musicMainTheme);
                break;
            case "Burrow-New-Smaller":
                InitializeMusic(FMODEvents.instance.musicBurrowTheme);
                break;
            case "Level 0 - Onboard Smaller":
                allowPlayerSounds = false;
                InitializeAmbience(FMODEvents.instance.desertAmbience);
                _currentMusicRef = FMODEvents.instance.musicLevel1;
                break;
            case "Level 1 - Pillars Art Test":
            case "Level 2 - Serpent 021324":
            case "Level 3 - Temple - P1":
            case "Level 3 - Temple - P2":
                InitializeAmbience(FMODEvents.instance.desertAmbience);
                InitializeMusic(FMODEvents.instance.musicLevel1);
                break;
        }

        //if (scene.name == "BurrowWithCam")
        //{
        //    InitializeMusic(FMODEvents.instance.musicBurrowTheme);
        //}
        //else
        //{
        //    InitializeAmbience(FMODEvents.instance.desertAmbience);
            
        //    InitializeMusic(FMODEvents.instance.musicMainTheme);
        //}
    }
    

    public void StopAmbience()
    {
        if (_ambienceEventInstance.isValid())
        {
            _ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void PlayMusic()
    {
        if (_musicEventInstance.isValid())
        {
            StopMusic();
        }
        InitializeMusic(_currentMusicRef);
    }
    public void StopMusic()
    {
        if (_musicEventInstance.isValid())
        {
            _musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        _ambienceEventInstance = CreateEventInstance(ambienceEventReference, this.transform);
        _ambienceEventInstance.start();
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicCheckPointSet = musicCheckPoint;
        _musicEventInstance = CreateEventInstance(musicEventReference, this.transform);
        // beatCallback += BeatCallback;
        // _musicEventInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        _musicEventInstance.start();
    }

    public void SetParameter(EventInstance instance, string parameterName, float parameterValue)
    {
        instance.setParameterByName(parameterName, parameterValue);
    }

    //Use this function to turn global Low-Pass Filter on/off (0 = off, 1 = on)
    public void UnderGroundLPF(bool state)
    {
        RuntimeManager.StudioSystem.setParameterByName("UnderGroundLPF", state ? 1 : 0);
    }

    public void PauseSound(EventInstance instance,bool pauseState) 
    {
        instance.setPaused(pauseState);
    }

    //Create an FMOD event instance on the gameObject to play sound
    public EventInstance CreateEventInstance(EventReference eventReference, Transform transform)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        _eventInstances.Add(eventInstance);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, transform);
        return eventInstance;
    }

    //Create an FMOD emitter componenet and attach to the gameObject to emitt sound
    public StudioEventEmitter InitializeStudioEventEmitter(EventReference eventReference, GameObject emitterGO)
    {
        StudioEventEmitter emitter = emitterGO.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        _eventEmitters.Add(emitter);
        return emitter;
    }

    //Play a one off instance of a sound
    public void PlayOneShot(EventReference sound, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(sound, pos);
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public void PlayEvent(EventInstance eventInstance)
    {
        eventInstance.start();
    }

    //For Dialogue Bubble, 0 for start, 1 for next line
    public void DialogueAudio(bool next)
    {
        PlayOneShot(next ? FMODEvents.instance.dialogueNext : FMODEvents.instance.dialogueStart);
    }

    //FMOD callback function, this should be call every beat
    private FMOD.RESULT BeatCallback(EVENT_CALLBACK_TYPE type, IntPtr instance, IntPtr parameterPtr)
    {

        if (type == EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            TIMELINE_BEAT_PROPERTIES beatProperties =
                (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));

            float bpm = beatProperties.tempo;
            int bar = beatProperties.bar;
            int beat = beatProperties.beat;
            int position = beatProperties.position;

            //Debug.Log($"CheckPoint: {musicCheckPointSet}, Bar: {bar}, Beat: {beat}, Position: {position}");

            if (musicCheckPointSet != musicCheckPoint && beat == 1 && bar%4 == 1)
            {
                
                musicCheckPointSet = musicCheckPoint;
                
                SetParameter(_musicEventInstance, "MusicCheckpoint3", musicCheckPointSet);
                //Debug.Log("Set MusicCheckPoint " + musicCheckPointSet + currentPoint);
            }
        }
       
        return FMOD.RESULT.OK;
    }

    private void CleanUp()
    {
        //stop and release any created instances
        Debug.Log("Clean Up FMOD Events");
        foreach (EventInstance eventInstance in _eventInstances)
        {
            if (!eventInstance.isValid()) return;
            
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        CleanUpEmitters();
        beatCallback -= BeatCallback;

    }

    public void CleanUpEmitters()
    {
        //stop all of the event emitters, if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in _eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}

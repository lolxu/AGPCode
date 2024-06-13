using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Runtime.InteropServices;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public enum CritterEmotion
    {
        Upbeat, //Clover, Juno
        Suprised, //Clover, Miles
        Thinking, //Clover
        Frustrated, //Clover. Juno
        Calm, //Clover, Miles
        Teaching, //Juno
        Inspired, //Juno
    }
    
    [SerializeField] private List<EventInstance> _eventInstances;
    [SerializeField] private List<EventInstance> _eventInstancesAlwaysActive;
    [SerializeField] private List<StudioEventEmitter> _eventEmitters;
    private Dictionary<string, VCA> _vca;
    private Dictionary<string, string> _vcaPath;
    private EventInstance _voiceEventInstance;
    private EventDescription _voiceEventDescription;

    //subscribe to this whenever you need to initialize sound on scene change
    public static Action onAudioSceneChange;

    private EventReference _currentMusicRef;

    public static AudioManager instance { get; private set; }

    public float musicCheckPoint = 0;
    private float musicCheckPointSet;

    private VCA vca_Ambience, vca_Music, vca_SFX, vca_UI;

    private EVENT_CALLBACK beatCallback;


    //UI
    public bool ui_checkBoxState = true;
    public int ui_sliderVal = 5;

    private void Awake()
    {
        if (instance != null)
        {
            // Debug.LogError("Found more than one Audio Manager in the scene");
            Destroy(gameObject);
            return;
        }
        instance = this;
        _eventInstances = new List<EventInstance>();
        _eventEmitters = new List<StudioEventEmitter>();
        _vca = new Dictionary<string, VCA>();
        _vcaPath = new Dictionary<string, string>();
        InitVCA();
    }

    public void SetParameter(EventInstance instance, string parameterName, float parameterValue)
    {
        instance.setParameterByName(parameterName, parameterValue);
    }

    //Available channels: Ambience, Music, SFX, UI
    public void SetVolume(string channel, float volume)
    {
        if (SetVCA(channel))
            _vca[channel].setVolume(volume);
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
    
    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        _eventInstances.Add(eventInstance);
        return eventInstance;
    }
    
    public EventInstance CreateEventInstanceDDOL(EventReference eventReference, Transform transform)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        _eventInstancesAlwaysActive.Add(eventInstance);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, transform);
        return eventInstance;
    }
    
    public EventInstance CreateEventInstanceDDOL(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        _eventInstancesAlwaysActive.Add(eventInstance);
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
    public void PlayOneShot(EventReference sound, GameObject obj)
    {
        RuntimeManager.PlayOneShotAttached(sound, obj);
    }
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

    /*
     * ====================================[WRAPPED BEHAVIOURS]===========================================
     */

    //UI

    public void PlaySlider()
    {
        if (ui_sliderVal == 0) PlaySliderMin();
        else if (ui_sliderVal == 10) PlaySliderMax();
        else
        {
            if (!slider.isValid()) slider = instance.CreateEventInstance(FMODEvents.instance.sliderSet, this.gameObject.transform);
            instance.SetParameter(slider, "UIsliderValue", ui_sliderVal);
            Debug.Log("[AUDIO MANAGER] UI Slider: " + ui_sliderVal);
            PlaySliderValue();
        }
    }

    public void PlayCheckBox()
    {
        if (ui_checkBoxState) PlayCheckBoxOn();
        else PlayCheckBoxOff();
    }

    public void CritterDialogueStart(Critter.CritterName critterName, AudioManager.CritterEmotion emotion, Transform t)
    {
        RuntimeManager.PlayOneShot(FMODEvents.instance.dialogueStart);
        CritterDialogueNext(critterName, emotion, t);
    }

    public void CritterDialogueNext(Critter.CritterName critterName, AudioManager.CritterEmotion emotion, Transform t)
    {
        string label = emotion.ToString();
        if (RuntimeManager.StudioSystem.setParameterByNameWithLabel("CritterEmotion", label) == FMOD.RESULT.OK)
        {
            switch (critterName)
            {
                case Critter.CritterName.Clover:
                    RuntimeManager.PlayOneShotAttached(FMODEvents.instance.cloverDialogue, t.gameObject);
                    break;
                case Critter.CritterName.Juno:
                    RuntimeManager.PlayOneShotAttached(FMODEvents.instance.junoDialogue, t.gameObject);
                    break;
                case Critter.CritterName.Miles:
                    RuntimeManager.PlayOneShotAttached(FMODEvents.instance.milesDialogue, t.gameObject);
                    break;
            }
        }
        else
        {
            Debug.LogWarning("[AUDIO MANAGER] Critter Emotion Set Falied");
            RuntimeManager.PlayOneShotAttached(FMODEvents.instance.dialogueNext, t.gameObject);
        }
    }

    public void CritterDialogueStop()
    {
        RuntimeManager.PlayOneShot(FMODEvents.instance.dialogueEnd);
    }


    /*
     * ====================================[PRIVATE MEMBERS]=====================================
     */

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleMusicAndAmbienceOnSceneChange;
    }

    private void OnDisable()
    {
        CleanUp();
        SceneManager.sceneLoaded -= HandleMusicAndAmbienceOnSceneChange;
    }

    private void HandleMusicAndAmbienceOnSceneChange(Scene scene, LoadSceneMode mode)
    {
        CleanUp();
        onAudioSceneChange?.Invoke();
    }

    private void InitVCA()
    {
        _vca.Add("Ambience", vca_Ambience);
        _vca.Add("Music", vca_Music);
        _vca.Add("SFX", vca_SFX);
        _vca.Add("UI", vca_UI);
        _vcaPath.Add("Ambience", "vca:/Ambience");
        _vcaPath.Add("Music", "vca:/Music");
        _vcaPath.Add("SFX", "vca:/SoundEffect");
        _vcaPath.Add("UI", "vca:/UI");
    }

    private bool SetVCA(string channel)
    {
        if (!_vca[channel].isValid())
        {
            VCA v;
            v = RuntimeManager.GetVCA(_vcaPath[channel]);
            if (v.isValid())
            {
                _vca[channel] = v;
                return true;
            }
            else
            {
                Debug.LogError("[AudioManager] VCA invalid");
                return false;
            }
        }
        else return true;
    }


    private EventInstance slider;

    private void PlaySliderMax()
    {
        instance.PlayOneShot(FMODEvents.instance.sliderMax);
    }

    private void PlaySliderMin()
    {
        instance.PlayOneShot(FMODEvents.instance.sliderMin);
    }

    private void PlaySliderValue()
    {
        instance.PlayEvent(slider);
    }

    private void PlayCheckBoxOn()
    {
        instance.PlayOneShot(FMODEvents.instance.checkBoxOn);
    }

    private void PlayCheckBoxOff()
    {
        instance.PlayOneShot(FMODEvents.instance.checkBoxOff);
    }

    private void CleanUp()
    {
        if (_eventInstances == null)
        {
            return;
        }
        //stop and release any created instances
        //Debug.Log("Clean Up FMOD Events");
        foreach (EventInstance eventInstance in _eventInstances)
        {
            if (!eventInstance.isValid()) return;
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        CleanUpEmitters();
        _eventInstances.Clear();
    }
    
    private void CleanUpAlwaysActive()
    {
        //stop and release any created instances
        //Debug.Log("Clean Up FMOD Events");
        foreach (EventInstance eventInstance in _eventInstancesAlwaysActive)
        {
            if (!eventInstance.isValid()) return;
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        _eventInstancesAlwaysActive.Clear();
    }

    public void CleanUpEmitters()
    {
        //stop all of the event emitters, if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in _eventEmitters)
        {
            emitter.Stop();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FMODUnity;
using UnityEngine;
using EventInstance = FMOD.Studio.EventInstance;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class BurrowAudio : MonoBehaviour
{
    private EventInstance waterAmbience;
    private List<EventInstance> grassAmbience = new List<EventInstance>();

    [SerializeField] private Transform waterTransform;
    [SerializeField] private List<Transform> grassTransforms;

    private void OnEnable()
    {
        waterAmbience = RuntimeManager.CreateInstance(FMODEvents.instance.burrowWater);
        waterAmbience.start();
        RuntimeManager.AttachInstanceToGameObject(waterAmbience, waterTransform);
        for (int i = 0; i < grassTransforms.Count; i++)
        {
            EventInstance newEvent = RuntimeManager.CreateInstance(FMODEvents.instance.burrowGrass);
            newEvent.start();
            RuntimeManager.AttachInstanceToGameObject(newEvent, grassTransforms[i]);
            grassAmbience.Add(newEvent);
        }
    }

    private void Start()
    {
        int plantsCollected = XMLFileManager.Instance.GetNumPlantsCollected();
        if (plantsCollected <= 3 && plantsCollected <= grassTransforms.Count)
        {
            for (int i = 0; i < plantsCollected; i++)
            {
                EventInstance newEvent = RuntimeManager.CreateInstance(FMODEvents.instance.burrowGrass);
                newEvent.start();
                RuntimeManager.AttachInstanceToGameObject(newEvent, grassTransforms[i]);
                grassAmbience.Add(newEvent);
            }
        }
        else
        {
            for (int i = 0; i < grassTransforms.Count; i++)
            {
                EventInstance newEvent = RuntimeManager.CreateInstance(FMODEvents.instance.burrowGrass);
                newEvent.start();
                RuntimeManager.AttachInstanceToGameObject(newEvent, grassTransforms[i]);
                grassAmbience.Add(newEvent);
            }
        }
    }

    private void OnDisable()
    {
        waterAmbience.stop(STOP_MODE.IMMEDIATE);
        waterAmbience.release();
        for (int i = 0; i < grassAmbience.Count; i++)
        {
            if (grassAmbience[i].isValid())
            {
                grassAmbience[i].stop(STOP_MODE.IMMEDIATE);
                grassAmbience[i].release();
            }
        }
    }
}

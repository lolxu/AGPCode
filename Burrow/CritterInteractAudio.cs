using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Yarn.Unity;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class CritterInteractAudio : MonoBehaviour
{
    public enum CritterEmotion
    {
        Upbeat, //0 Clover, Juno
        Suprised, //1 Clover, Miles
        Thinking, //2 Clover
        Frustrated, //3 Clover. Juno
        Calm, //4 Clover, Miles
        Teaching, //5 Juno
        Inspired, //6 Juno
        Random //7
    }
    
    private FMOD.Studio.EventInstance interactAudio;
    private Critter.CritterName currName;
    private Transform critterTrans;
    private CritterEmotion currEmotion;
    private PARAMETER_ID globalCritterEmotionParameterId;
    private HashSet<CritterEmotion> CloverEmotions = new HashSet<CritterEmotion>();
    private HashSet<CritterEmotion> JunoEmotions = new HashSet<CritterEmotion>();
    private HashSet<CritterEmotion> MilesEmotions = new HashSet<CritterEmotion>();


    private void Awake()
    {
        FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("CritterEmotion", out PARAMETER_DESCRIPTION critterEmotionParameterDescription);
        globalCritterEmotionParameterId = critterEmotionParameterDescription.id;
        CloverEmotions.Add(CritterEmotion.Upbeat);
        CloverEmotions.Add(CritterEmotion.Suprised);
        CloverEmotions.Add(CritterEmotion.Thinking);
        CloverEmotions.Add(CritterEmotion.Frustrated);
        CloverEmotions.Add(CritterEmotion.Calm);
        JunoEmotions.Add(CritterEmotion.Upbeat);
        JunoEmotions.Add(CritterEmotion.Frustrated);
        JunoEmotions.Add(CritterEmotion.Teaching);
        JunoEmotions.Add(CritterEmotion.Inspired);
        MilesEmotions.Add(CritterEmotion.Suprised);
        MilesEmotions.Add(CritterEmotion.Calm);
    }

    public void StartAudio(Critter.CritterName name, Transform critterTransform) //TODO: Add CritterEmotion into the input
    {
        //Debug.Log("StartAudio");
        currName = name;
        critterTrans = critterTransform;
        RuntimeManager.PlayOneShot(FMODEvents.instance.dialogueStart);
        //DismissLineAudio();
    }

    [YarnCommand("Emotion")]
    public void SetEmotion(int emotion)//TODO: HAVE THIS CALL THE AUDIO ONLY AND CALL IT BEFORE EVERY LINE INSTEAD OF THE DISMISSAL CALLING IT
    {
        currEmotion = (CritterEmotion)emotion;
        if (currEmotion != CritterEmotion.Random)
        {
            if (EmotionSupported())
            {
                //Debug.Log("Critter emotion: " + currEmotion.ToString());
                RuntimeManager.StudioSystem.setParameterByID(globalCritterEmotionParameterId, emotion);
                //Debug.Log("Critter emotion Parameter set ");
            }
            else
            {
                //Debug.LogWarning("Emotion: " + currEmotion.ToString() + " Not supported for critter: " + currName.ToString() + " setting emotion to random");
                currEmotion = CritterEmotion.Random;
            }
        }
        DismissLineAudio();
    }

    private bool EmotionSupported()
    {
        switch (currName)
        {
            case Critter.CritterName.Clover:
                return CloverEmotions.Contains(currEmotion);
                break;
            case Critter.CritterName.Juno:
                return JunoEmotions.Contains(currEmotion);
                break;
            case Critter.CritterName.Miles:
                return MilesEmotions.Contains(currEmotion);
                break;
        }
        return false;
    }
    
    private bool EmotionSupported(CritterEmotion emotion)
    {
        switch (currName)
        {
            case Critter.CritterName.Clover:
                return CloverEmotions.Contains(emotion);
                break;
            case Critter.CritterName.Juno:
                return JunoEmotions.Contains(emotion);
                break;
            case Critter.CritterName.Miles:
                return MilesEmotions.Contains(emotion);
                break;
        }
        return false;
    }
    public void DismissLineAudio()
    {
        //Debug.Log("Critter emotion audio call: " + currEmotion.ToString());
        if (currEmotion == CritterEmotion.Random)
        {
            int emotion = Random.Range(0, (int)currEmotion);
            while (!EmotionSupported((CritterEmotion)emotion))
            {
                emotion++;
                emotion %= (int)CritterEmotion.Random;
            }
            RuntimeManager.StudioSystem.setParameterByID(globalCritterEmotionParameterId, emotion);
        }
        switch (currName)
        {
            case Critter.CritterName.Clover:
                RuntimeManager.PlayOneShotAttached(FMODEvents.instance.cloverDialogue, critterTrans.gameObject);
                break;
            case Critter.CritterName.Juno:
                RuntimeManager.PlayOneShotAttached(FMODEvents.instance.junoDialogue, critterTrans.gameObject);
                break;
            case Critter.CritterName.Miles:
                RuntimeManager.PlayOneShotAttached(FMODEvents.instance.milesDialogue, critterTrans.gameObject);
                break;
        }
    }
    
    public void EndAudio()
    {
        //Debug.Log("EndAudio");
        RuntimeManager.PlayOneShot(FMODEvents.instance.dialogueEnd);
    }
}

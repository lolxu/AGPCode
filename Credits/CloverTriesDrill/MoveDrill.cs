using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveDrill : MonoBehaviour
{
    [SerializeField] private Transform junoModelTransform;
    [SerializeField] private Transform junoBodyTransform;
    [SerializeField] private Vector3 alignedWithJuno;
    [SerializeField] private Animator junoAnimator;
    [SerializeField] private Animator cloverAnimator;
    [SerializeField] private float timeBeforeLaunch;
    [SerializeField] private float tweenTime = 1f;
    [SerializeField] private ParticleSystem sandBurst;
    [SerializeField] private float TimeFromLaunchToBurst;
    private FMOD.Studio.EventInstance drillEventInstance;
    [SerializeField] private CreditMusicAndAmbience music;
    [SerializeField] private Transform cameraTransform;
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.01f);
        music._musicEventInstance.setVolume(0.5f);
        RuntimeManager.StudioSystem.setParameterByName("CritterEmotion", 2);
        RuntimeManager.PlayOneShotAttached(FMODEvents.instance.cloverDialogue, cloverAnimator.gameObject);
        yield return new WaitForSeconds(timeBeforeLaunch);
        LaunchDrill();
    }
    
    private void LaunchDrill()
    {
        //play drill sound
        drillEventInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.drill, transform);
        StartDrill();
        cloverAnimator.SetTrigger("LetGo");
        RuntimeManager.StudioSystem.setParameterByName("CritterEmotion", 0);
        RuntimeManager.PlayOneShotAttached(FMODEvents.instance.cloverDialogue, cameraTransform.gameObject);
        transform.DOMove(transform.position + transform.forward * 41.95f, tweenTime).OnComplete(BringBackDrill).SetEase(Ease.OutQuad);
        StartCoroutine(playBurst());
        StartCoroutine(controlVolume());
    }
    
    private IEnumerator controlVolume()
    {
        float minVolume = 0.1f;
        float maxVolume = 1.0f;
        float time = 0f;
        while (time < tweenTime)
        {
            drillEventInstance.setVolume(Mathf.Lerp(maxVolume, minVolume, time/tweenTime));
            yield return null;
            time += Time.deltaTime;
        }
        time = 0f;
        while (time < tweenTime)
        {
            drillEventInstance.setVolume(Mathf.Lerp(minVolume, maxVolume, time/tweenTime));
            yield return null;
            time += Time.deltaTime;
        }
        time = 0f;
        while (time < tweenTime)
        {
            drillEventInstance.setVolume(Mathf.Lerp(maxVolume, minVolume, time/tweenTime));
            yield return null;
            time += Time.deltaTime;
        }
    }
    
    private IEnumerator playBurst()
    {
        yield return new WaitForSeconds(TimeFromLaunchToBurst);
        RuntimeManager.PlayOneShotAttached(FMODEvents.instance.drillSandEnter, cloverAnimator.gameObject);
        sandBurst.Play();
    }
    
    public void BringBackDrill()
    {
        //align with Juno
        transform.position = alignedWithJuno;
        Vector3 junoPos = junoBodyTransform.position;
        junoPos.y = alignedWithJuno.y;
        //Debug.Log("Juno dist: " + Vector3.Distance(junoPos, alignedWithJuno));
        transform.rotation = Quaternion.LookRotation(junoPos - transform.position);
        // launch back and hit Juno
        transform.DOMove(junoPos, tweenTime).OnComplete(hit).SetEase(Ease.InQuad);
    }
    
    private void hit()
    {
        //TODO: play sound
        RuntimeManager.PlayOneShotAttached(CreditsAudio.instance.punch, cameraTransform.gameObject);
        RuntimeManager.StudioSystem.setParameterByName("CritterEmotion", 3);
        RuntimeManager.PlayOneShotAttached(FMODEvents.instance.junoDialogue, cameraTransform.gameObject);
        junoAnimator.SetTrigger("hitByDrill");
        Vector3 junoPos = junoBodyTransform.position;
        junoPos.y = alignedWithJuno.y;
        //start moving Juno
        transform.DOMove(junoPos - junoBodyTransform.forward * 41.95f, tweenTime).SetEase(Ease.Linear);
        junoModelTransform.DOMove(junoPos - junoBodyTransform.forward * 41.95f, tweenTime).SetEase(Ease.Linear).OnComplete(GoBackToCredits);
         //rotate Juno
         //junoModelTransform.localRotation = Quaternion.Euler(65f, junoBodyTransform.rotation.eulerAngles.y, 0f);
    }
    
    private void StartDrill()
    {
        drillEventInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.drill, cameraTransform);
    
        // Start
        drillEventInstance.start();
    }
    
    private void GoBackToCredits()
    {
        music._musicEventInstance.setVolume(1.0f);
        StopDrill();
        CreditsManager.Instance.AdvanceCredits();
    }
    private void StopDrill()
    {
        drillEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
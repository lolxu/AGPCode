using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class DrillLanding : MonoBehaviour
{
    [SerializeField] private float timeBeforeLaunch;
    [SerializeField] private float tweenTime = 1f;
    [SerializeField] private CreditMusicAndAmbience music;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform landingPos;
    [SerializeField] private float timeAfterLand;
    [SerializeField] private ParticleSystem burst;
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.01f);
        music._musicEventInstance.setVolume(0.5f);
        yield return new WaitForSeconds(timeBeforeLaunch);
        LaunchDrill();
    }
    
    private void LaunchDrill()
    {
        transform.rotation = Quaternion.LookRotation(landingPos.position - transform.position);
        transform.DOMove(landingPos.position, tweenTime).OnComplete(PlayBurst).SetEase(Ease.InQuad);
    }

    public void PlayBurst()
    {
        burst.Play();
        RuntimeManager.PlayOneShotAttached(FMODEvents.instance.drillSandEnter, cameraTransform.gameObject);
        StartCoroutine(waitToGoBackToCredits());
    }

    private IEnumerator waitToGoBackToCredits()
    {
        yield return new WaitForSeconds(timeAfterLand);
        GoBackToCredits();
    }


    private void GoBackToCredits()
    {
        music._musicEventInstance.setVolume(1.0f);
        CreditsManager.Instance.AdvanceCredits();
    }
}

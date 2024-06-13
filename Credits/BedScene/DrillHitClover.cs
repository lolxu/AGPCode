using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class DrillHitClover : MonoBehaviour
{
    [SerializeField] private float timeBeforeLaunch;
    [SerializeField] private float tweenTime = 1f;
    [SerializeField] private CreditMusicAndAmbience music;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform hitPos;
    [SerializeField] private GameObject bedClover;
    [SerializeField] private GameObject drillClover;
    private float dist;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.01f);
        music._musicEventInstance.setVolume(0.5f);
        //have Juno FaceDrill
        //JunoTransform.rotation = Quaternion.LookRotation(startPos.position - JunoTransform.position);
        yield return new WaitForSeconds(timeBeforeLaunch);
        LaunchDrill();
    }
    
    private void LaunchDrill()
    {
        dist = Vector3.Distance(hitPos.position, transform.position);
        transform.rotation = Quaternion.LookRotation(hitPos.position - transform.position);
        transform.DOMove(hitPos.position, tweenTime).OnComplete(HitClover).SetEase(Ease.InQuad);
    }

    public void HitClover()
    {
        RuntimeManager.PlayOneShotAttached(CreditsAudio.instance.punch, cameraTransform.gameObject);
        RuntimeManager.PlayOneShotAttached(CreditsAudio.instance.cloverAhh, cameraTransform.gameObject);
        music._musicEventInstance.setVolume(0.0f);
        bedClover.SetActive(false);
        drillClover.SetActive(true);
        transform.DOMove(hitPos.position + hitPos.right * dist, tweenTime).OnComplete(GoBackToCredits).SetEase(Ease.OutQuad);
    }
    
    private void GoBackToCredits()
    {
        //music._musicEventInstance.setVolume(1.0f);
        CreditsManager.Instance.AdvanceCredits();
    }
}

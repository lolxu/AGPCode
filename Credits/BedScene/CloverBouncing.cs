using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class CloverBouncing : MonoBehaviour
{
    private int currPose = -1;
    private int maxPoses = 7;
    private float startY;
    [SerializeField] private float moveUp;
    [SerializeField] private float tweenTime;
    [SerializeField] private Animator clover;
    [SerializeField] private Transform cameraTransform;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.01f);
        startY = transform.localPosition.y;
        BounceUp();
    }
    private void BounceUp()
    {
        if (currPose != -1)
        {
            currPose++;
            currPose %= maxPoses;
            clover.SetTrigger(currPose.ToString());
            RuntimeManager.PlayOneShotAttached(FMODEvents.instance.bounce, cameraTransform.gameObject);
        }
        else
        {
            currPose++;
        }
        transform.DOLocalMoveY(startY + moveUp, tweenTime).SetEase(Ease.OutQuad).OnComplete(FallDown);
    }
    private void FallDown()
    {
        transform.DOLocalMoveY(startY, tweenTime).SetEase(Ease.InQuad).OnComplete(BounceUp);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}

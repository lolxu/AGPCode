using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Serialization;

public class ShieldCollision : MonoBehaviour
{
    public BounceKnightStateMachine machine;
    //Kill Collider
    [SerializeField] private Collider shieldCollider;
    //InitialFall
    [SerializeField] private float fallDistance;
    [SerializeField] private float fallTime;
    //SecondFall
    [SerializeField] private float AnimationTime;
    [SerializeField] private float YMove;
    [SerializeField] private float ZMove;
    [SerializeField] private float LocalRotate;
    private bool isDetached = false;
    public bool IsDetached
    {
        get { return isDetached; }
    }

    //Spikes
    [SerializeField] private float spikeAnimationTime;
    [SerializeField] private Transform spikesTransform;
    [SerializeField] private BounceKnightSounds bounceKnightSounds;
    private Vector3 originalLocalSpikeLocation;

    private Sequence tipOverShield;
    private Sequence ejectSpikes;
    private Sequence retractSpikes;

    private void Awake()
    {
        tipOverShield = DOTween.Sequence();
        originalLocalSpikeLocation = spikesTransform.localPosition;
    }

    public void EjectSpikes()
    {
        bounceKnightSounds.PlayShieldSpikeSound();
        ejectSpikes.Append(spikesTransform.DOScale(Vector3.one, spikeAnimationTime).SetEase(Ease.OutExpo));
        ejectSpikes.Join(spikesTransform.DOLocalMoveZ(originalLocalSpikeLocation.z + 0.5f, spikeAnimationTime).SetEase(Ease.OutExpo).OnComplete(RetractSpikes));
    }

    private void RetractSpikes()
    {
        bounceKnightSounds.PlayShieldSpikeSound();
        ejectSpikes.Append(spikesTransform.DOScale(Vector3.zero, spikeAnimationTime).SetEase(Ease.InExpo));
        ejectSpikes.Join(spikesTransform.DOLocalMoveZ(originalLocalSpikeLocation.z - 0.5f, spikeAnimationTime).SetEase(Ease.InExpo));
    }

    public void DropShieldAnimation()
    {
        //set tag to prevent shield from killing player
        shieldCollider.enabled = false;
        tipOverShield
            .Append(transform.DOLocalMoveY(transform.localPosition.y - fallDistance, fallTime).SetEase(Ease.InQuad))
            .Join(transform.DOLocalMove(transform.localPosition + new Vector3(0.0f, YMove, ZMove), AnimationTime))
            .Join(transform.DORotate(transform.localEulerAngles + new Vector3(LocalRotate, 0.0f, 0.0f), AnimationTime).OnComplete(ReactivateCollider));
    }
    private void ReactivateCollider()
    {
        isDetached = true;
        shieldCollider.enabled = true;
    }

}

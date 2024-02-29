using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class CircularShooter : Swarmer
{
    [SerializeField] private List<Transform> rocks = new List<Transform>();
    [SerializeField] private List<Vector3> rockProjectileVector = new List<Vector3>();
    [SerializeField] private List<Vector3> rockLocalStartPosVector = new List<Vector3>();
    private List<Vector3> rockStartLocalScale = new List<Vector3>();
    [SerializeField] private float LaunchSpeed = 1.0f;
    [SerializeField] private Transform centerReference;
    [SerializeField] private Transform rocksCenter;
    private float launchDist;
    [SerializeField] private ParticleSystem ringParticle;
    [SerializeField] private ParticleSystem dustParticle;
    [SerializeField] private float lineWidth = 10.0f;
    [SerializeField] private float lineYOffset = 1.0f;
    private Vector3 RingOffset;
    [SerializeField] private float hitBoxWidth = 2.5f;
    [SerializeField] private float hitBoxHeightTop = 2.5f;

    private Sequence rockSpin;
    private bool hasAttacked = false;

    public override void OnAwake()
    {
        base.OnAwake();
        Vector3 rockPos;
        RingOffset = new Vector3(0.0f, lineYOffset, 0.0f);
        //shoot away from local center (0, 0, 0) in our case
        for (int i = 0; i < rocks.Count; i++)
        {
            rockPos = rocks[i].localPosition;
            rockLocalStartPosVector.Add(rockPos);
            rockStartLocalScale.Add(rocks[i].localScale);
            
            rockPos += RingOffset;
            rockPos.y = 0;
            rockProjectileVector.Add(rockPos.normalized);

            Vector3 curRot = rocks[i].rotation.eulerAngles;
            int direction = Random.Range(-1, 2);
            if (direction == 0) direction = 1;
            rocks[i].DORotate(curRot + new Vector3(0.0f, 360.0f, 360.0f) * direction, Random.Range(1.75f, 3.5f), RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }
        launchDist = (centerReference.position - rocks[0].position).magnitude;
        var vel = ringParticle.velocityOverLifetime;
        var particleMain = ringParticle.main;
        particleMain.startLifetime = attackTime;
        vel.radial = LaunchSpeed;
        
        Vector3 centerRot = rocksCenter.eulerAngles;
        rockSpin = DOTween.Sequence();
        rockSpin.Append(rocksCenter.DORotate(centerRot + new Vector3(0.0f, 360.0f, 0.0f), Random.Range(5.5f, 8.5f),
                RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear));
        
        RespawnManager.OnReset += ClearRing;
    }

    private void OnDestroy()
    {
        RespawnManager.OnReset -= ClearRing;
    }

    private void ClearRing()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        hasAttacked = false;
        ResetOnEnable();
    }

    public override void HandleEnterAttack()
    {
        base.HandleEnterAttack();

        Sequence rockPullBack = DOTween.Sequence();
        // Start shrinking the rocks
        for (int i = 0; i < rocks.Count; i++)
        {
            var pullBackPos = rocks[i].localPosition - rockProjectileVector[i] * 3.5f;
            rockPullBack.Join( rocks[i].DOLocalMove(pullBackPos, 0.25f).SetEase(Ease.OutSine));
            rocks[i].DOScale(Vector3.zero, attackTime).SetEase(Ease.InExpo);
        }

        rockPullBack.OnComplete(() =>
        {
            hasAttacked = true;
            if (!ringParticle.isEmitting)
            {
                ringParticle.Play();
            }
            rockSpin.Kill();
        });
    }

    public override void HandleAttack()
    {
        if (hasAttacked)
        {
            for (int i = 0; i < rocks.Count; i++)
            {
                rocks[i].localPosition += rockProjectileVector[i] * Time.deltaTime * LaunchSpeed;
            }
            launchDist += Time.deltaTime * LaunchSpeed;
            //check if ring hits player
            //are we within the distnace and local y range of the rocks and bandit is not submerged
            if (Mathf.Abs((centerReference.position - playerTransform.position).magnitude - launchDist) < hitBoxWidth
                && Mathf.Abs(centerReference.InverseTransformPoint(playerTransform.position).y -
                             centerReference.localPosition.y) < hitBoxHeightTop
                && !playerStateMachine.IsSubmerged)
            {
                playerStateMachine.InstantKill();
            }
        }
    }

    public override void HandleExitAttack()
    {
        if (hasAttacked)
        {
            hasAttacked = false;

            // Debug.Log("Exit attack");
            Sequence initRocks = DOTween.Sequence();

            initRocks.OnComplete(() =>
            {
                Vector3 centerRot = rocksCenter.eulerAngles;
                rockSpin = DOTween.Sequence();
                rockSpin.Append(rocksCenter.DORotate(centerRot + new Vector3(0.0f, 360.0f, 0.0f), Random.Range(5.5f, 8.5f),
                        RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear));
                
                launchDist = (centerReference.position - rocks[0].position).magnitude;
            });
            
            dustParticle.Play();
            
            for (int i = 0; i < rocks.Count; i++)
            {
                rocks[i].localPosition = rockLocalStartPosVector[i] - new Vector3(0.0f, 3.0f, 0.0f);
                initRocks.Join(rocks[i].DOScale(rockStartLocalScale[i], 0.35f).SetEase(Ease.InOutSine));
                initRocks.Join(rocks[i].DOLocalMoveY(rocks[i].localPosition.y + 3.0f, 1.25f)).SetEase(Ease.InExpo);
            }
        }
    }
    
    public override void ResetOnEnable()
    {
        base.ResetOnEnable();
        for (int i = 0; i < rocks.Count; i++)
        {
            rocks[i].gameObject.SetActive(true);
            
            Vector3 curRot = rocks[i].rotation.eulerAngles;
            int direction = Random.Range(-1, 2);
            if (direction == 0) direction = 1;
            rocks[i].DORotate(curRot + new Vector3(0.0f, 360.0f, 360.0f) * direction, Random.Range(1.75f, 3.5f), RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            
            rocks[i].localScale = rockStartLocalScale[i];
            rocks[i].localPosition = rockLocalStartPosVector[i];
        }
        launchDist = (centerReference.position - rocks[0].position).magnitude;
    }

    public override void HandleEnterDeath()
    {
        base.HandleEnterDeath();
        for (int i = 0; i < rocks.Count; i++)
        {
            rocks[i].gameObject.SetActive(false);
        }
        // lineRenderer.gameObject.SetActive(false);
    }

    public override void HandleAlertMove()
    {
        base.HandleAlertMove();
        //kill player if they just touch the rocks
        //check if ring hits player
        //are we within the distnace and local y range of the rocks and bandit is not submerged
        if (Mathf.Abs((centerReference.position - playerTransform.position).magnitude - launchDist) < hitBoxWidth
            && Mathf.Abs(centerReference.InverseTransformPoint(playerTransform.position).y - centerReference.localPosition.y) < hitBoxHeightTop
            && !playerStateMachine.IsSubmerged) 
        {
            playerStateMachine.InstantKill();
        }
    }

    public override void HandleIdle()
    {
        base.HandleIdle();
        //kill player if they just touch the rocks
        //check if ring hits player
        //are we within the distnace and local y range of the rocks and bandit is not submerged
        if (Mathf.Abs((centerReference.position - playerTransform.position).magnitude - launchDist) < hitBoxWidth
            && Mathf.Abs(centerReference.InverseTransformPoint(playerTransform.position).y - centerReference.localPosition.y) < hitBoxHeightTop
            && !playerStateMachine.IsSubmerged) 
        {
            playerStateMachine.InstantKill();
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawWireSphere(GetPlayerViewPosition(), launchDist + hitBoxWidth);
    //     Gizmos.DrawWireSphere(GetPlayerViewPosition(), launchDist - hitBoxWidth);
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawLine(centerReference.TransformPoint(new Vector3(0.0f, hitBoxHeightTop, 0.0f)), centerReference.TransformPoint(new Vector3(0.0f, hitBoxHeightTop, launchDist)));
    //     Gizmos.DrawLine(centerReference.TransformPoint(new Vector3(0.0f, 0.0f, 0.0f)), centerReference.TransformPoint(new Vector3(0.0f, 0.0f, launchDist)));
    // }
}

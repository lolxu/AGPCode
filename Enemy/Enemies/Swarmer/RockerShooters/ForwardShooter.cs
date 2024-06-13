using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ForwardShooter : Swarmer
{
    [SerializeField] private List<FSRockBehavior> rocks = new List<FSRockBehavior>();
    [SerializeField] private List<Transform> visualRocks = new List<Transform>();

    private List<Vector3> orgRockScale;
    
    [SerializeField] private ParticleSystem _telegraphParticles;
    [SerializeField] private float ShotsPerSec = 2.0f;
    [SerializeField] private float launchSpeed = 1.0f;
    private float attackTimeElapsed = 0.0f;
    private float timePerShot = 0.0f;

    private bool hasAttacked = false;
    
    public override void OnAwake()
    {
        base.OnAwake();
        _telegraphParticles.Stop();
        orgRockScale = new List<Vector3>();
        
        for (int i = 0; i < rocks.Count; i++)
        {
            rocks[i].SetShootAtTargetLaunchSpeed(launchSpeed);
            orgRockScale.Add(rocks[i].transform.localScale);
        }
        timePerShot = 1.0f/ShotsPerSec;

        for (int i = 0; i < visualRocks.Count; i++)
        {
            Vector3 curRot = visualRocks[i].eulerAngles;
            int direction = Random.Range(-1, 2);
            if (direction == 0) direction = 1;
            visualRocks[i].DORotate(curRot + new Vector3(0.0f, 360.0f, 360.0f) * direction, Random.Range(1.75f, 3.5f), RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }

        RespawnManager.OnReset += ResetVisuals;
    }

    private void OnDestroy()
    {
        RespawnManager.OnReset -= ResetVisuals;
    }

    private void ResetVisuals()
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
            var pullBackPos = rocks[i].transform.localPosition - rocks[i].transform.InverseTransformVector(playerTransform.position - transform.position).normalized * 3.5f;
            rockPullBack.Join( rocks[i].transform.DOLocalMove(pullBackPos, 0.35f).SetEase(Ease.InExpo));
            
        }

        rockPullBack.OnComplete(() =>
        {
            for (int i = 0; i < rocks.Count; i++)
            {
                rocks[i].gameObject.GetComponent<ParticleSystem>().Play();
            }
            hasAttacked = true;
        });
    }

    public override void HandleAttack()
    {
        base.HandleAttack();
        if (hasAttacked)
        {
            attackTimeElapsed += Time.deltaTime;
            for (int i = 0; i < Mathf.Min((attackTimeElapsed/timePerShot), rocks.Count); i++)
            {
                rocks[i].transform.localPosition += rocks[i].transform.InverseTransformVector(playerTransform.position - transform.position).normalized * Time.deltaTime * launchSpeed;
            }

            if (attackTimeElapsed >= attackTime - 1.0f
                && !_telegraphParticles.isPlaying)
            {
                for (int i = 0; i < Mathf.Min((attackTimeElapsed/timePerShot), rocks.Count); i++)
                {
                    rocks[i].transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutExpo);
                    rocks[i].gameObject.GetComponent<ParticleSystem>().Stop();
                }
                _telegraphParticles.Play();
            }
        }
    }

    public override void HandleExitAttack()
    {
        if (hasAttacked)
        {
            hasAttacked = false;
            Sequence rockInit = DOTween.Sequence();
            for (int i = 0; i < rocks.Count; i++)
            {
                rocks[i].transform.localScale = orgRockScale[i];
                rocks[i].ResetRock();
                rocks[i].transform.localPosition -= new Vector3(0.0f, 3.0f, 0.0f);
                rockInit.Join(rocks[i].transform.DOLocalMoveY(rocks[i].transform.localPosition.y + 3.0f, 1.15f)
                    .SetEase(Ease.OutExpo));
            }

            attackTimeElapsed = 0.0f;
        }
    }
    
    public override void ResetOnEnable()
    {
        base.ResetOnEnable();
        for (int i = 0; i < rocks.Count; i++)
        {
            rocks[i].gameObject.SetActive(true);
            rocks[i].ResetRock();
        }
        attackTimeElapsed = 0.0f;
        _telegraphParticles.Stop();
    }

    public override void HandleEnterDeath()
    {
        base.HandleEnterDeath();
        for (int i = 0; i < rocks.Count; i++)
        {
            rocks[i].gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        // Debug.DrawLine(transform.position, (playerTransform.position - transform.position).normalized * 10.0f + transform.position, Color.magenta);
    }
}

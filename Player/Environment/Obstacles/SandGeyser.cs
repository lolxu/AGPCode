using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using UnityEngine;

public class SandGeyser : MonoBehaviour
{
    public float downTime = 3.0f;
    public float startOffset = 0.0f;
    public float activeTime = 1.0f;
    public float transitionTime = .4f;

    public Transform sandGeyserParent;
    public ParticleSystem _particles;
    public MeshCollider _meshCollider;
    
    private PlayerStateMachine ctx;
    private bool bIsGeyserActive = false;
    
    private Coroutine geyserCoroutine = null;
    private void Awake()
    {
        ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
    }

    private void Start()
    {
        geyserCoroutine = StartCoroutine(RunSandGeyser());
    }

    private void OnEnable()
    {
        RespawnManager.OnReset += ResetGeyser;
    }

    private void OnDisable()
    {
        RespawnManager.OnReset -= ResetGeyser;
    }

    public void ResetGeyser()
    {
        if (geyserCoroutine != null) 
        {
            StopCoroutine(geyserCoroutine);
        }
        
        geyserCoroutine = StartCoroutine(RunSandGeyser());
    }
    
    private void Update()
    {
        if (ctx && bIsGeyserActive &&
            _meshCollider.bounds.Contains(ctx.transform.position))
        {
            ctx.InstantKill();
        }
    }

    public IEnumerator RunSandGeyser()
    {
        sandGeyserParent.localScale = new Vector3(1, 0, 1);
        sandGeyserParent.gameObject.SetActive(false);
        bIsGeyserActive = false;
        _particles.Stop();
        yield return new WaitForSeconds(startOffset);
        _particles.Play();
        while (true)
        {
            // start particles, wait for downtime
            yield return new WaitForSeconds(downTime / 2.0f);
            _particles.Play();
            yield return new WaitForSeconds(downTime / 2.0f);
            // animate and scale geyser collider
            _particles.Stop();
            // wait for transitionTime and animate a little
            sandGeyserParent.gameObject.SetActive(true);
            bIsGeyserActive = true;
            sandGeyserParent.DOScale(Vector3.one, transitionTime).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(transitionTime);
            
            // have it shake a lil (later)
            
            yield return new WaitForSeconds(activeTime);

            // scale down geyser collider and resume particles
            sandGeyserParent.DOScale(new Vector3(1, 0, 1), transitionTime).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(transitionTime);
            sandGeyserParent.gameObject.SetActive(false);
            bIsGeyserActive = false;
        }
    }
}

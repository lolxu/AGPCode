using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.Mathematics;
using UnityEngine.Serialization;
using Debug = FMOD.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class Bomb : Swarmer
{
    private Color OGColor;
    private Color White = Color.red;
    private int nameID;
    private Vector3 OGScale;
    private Vector3 newScale;
    [SerializeField] private FMODUnity.EventReference loopingSizzle;
    private EventInstance sizzleEvent;
    [SerializeField] private StudioEventEmitter sizzleEmitter;
    private float TwoPI = Mathf.PI * 2.0f;
    private float FlickerRate = 1.0f;
    //Explosion
    [SerializeField] private FMODUnity.EventReference explodeEvent;
    [SerializeField] private Transform explosionTransform;
    [SerializeField] private GameObject BombBody;

    public override void OnAwake()
    {
        base.OnAwake();
        FlickerRate = 2.0f * TimeForWarningAndDisengagingAlert;
        float explosionScale = AlertDistance * 0.5f;
        explosionTransform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);
        NormalDeath = false;
    }

    private void Start()
    {
        sizzleEvent = RuntimeManager.CreateInstance(loopingSizzle);
        sizzleEmitter.EventReference = loopingSizzle;
        sizzleEmitter.OverrideMaxDistance = AlertDistance * 3.2f;
    }

    private void OnDestroy()
    {
        sizzleEvent.release();
        sizzleEvent.stop(STOP_MODE.IMMEDIATE);
        sizzleEmitter.Stop();
    }

    public override void SetSpawnLocation()
    {
        base.SetSpawnLocation();
        //OGColor = _meshRenderer.material.color;
        OGScale = transform.localScale;
        newScale = OGScale * 1.2f;
    }

    public override void ResetOnEnable()
    {
        base.ResetOnEnable();
        BombBody.SetActive(true);
        //_meshRenderer.material.color = OGColor;
        transform.localScale = OGScale;
    }

    public override void ResetOnDisable()
    {
        base.ResetOnDisable();
        //stop explosion from going off
        explosionTransform.gameObject.SetActive(false);
    }

    public override void HandleIdleLOSMove()
    {
        base.HandleIdleLOSMove();
        if (!sizzleEmitter.IsPlaying())
        {
            sizzleEmitter.Play();
        }
        float normalizedCurrTimeForWarningAndDisengagingAlert = CurrTimeForWarningAndDisengagingAlert / TimeForWarningAndDisengagingAlert;
        //sizzleEvent.setParameterByName("SizzleVolume", normalizedCurrTimeForWarningAndDisengagingAlert);
        sizzleEmitter.SetParameter("SizzleVolume", normalizedCurrTimeForWarningAndDisengagingAlert);
        //starts at -1, goes to 0 halfway through goes to white until we hit PI
        //_meshRenderer.material.color = Color.Lerp(OGColor, White, (-Mathf.Cos(normalizedCurrTimeForWarningAndDisengagingAlert * Mathf.PI + FlickerRate * TwoPI * normalizedCurrTimeForWarningAndDisengagingAlert * normalizedCurrTimeForWarningAndDisengagingAlert * normalizedCurrTimeForWarningAndDisengagingAlert) + 1) * 0.5f);
        transform.localScale = Vector3.Lerp(OGScale, newScale, normalizedCurrTimeForWarningAndDisengagingAlert);
    }

    public override void HandleIdleSuspiciousMove()
    {
        base.HandleIdleSuspiciousMove();
        if (!sizzleEmitter.IsPlaying())
        {
            sizzleEmitter.Play();
        }
        float normalizedCurrTimeForWarningAndDisengagingAlert = CurrTimeForWarningAndDisengagingAlert / TimeForWarningAndDisengagingAlert;
        sizzleEmitter.SetParameter("SizzleVolume", normalizedCurrTimeForWarningAndDisengagingAlert);
        //sizzleEvent.setParameterByName("SizzleVolume", normalizedCurrTimeForWarningAndDisengagingAlert);
        //_meshRenderer.material.color = Color.Lerp(OGColor, White, (-Mathf.Cos(normalizedCurrTimeForWarningAndDisengagingAlert * Mathf.PI + FlickerRate * TwoPI * normalizedCurrTimeForWarningAndDisengagingAlert * normalizedCurrTimeForWarningAndDisengagingAlert * normalizedCurrTimeForWarningAndDisengagingAlert) + 1) * 0.5f);
        transform.localScale = Vector3.Lerp(OGScale, newScale, normalizedCurrTimeForWarningAndDisengagingAlert);
    }

    public override void HandleIdleMove()
    {
        base.HandleIdleMove();
        sizzleEvent.stop(STOP_MODE.ALLOWFADEOUT);
        sizzleEmitter.Stop();
    }

    public override void HandleEnterAttack()
    {
        transform.localScale = newScale;
        //_meshRenderer.material.color = White;
        sizzleEvent.stop(STOP_MODE.IMMEDIATE);
        sizzleEmitter.Stop();
        explosionTransform.position = GetPlayerViewPosition();
        TriggerExplosion();
        KillPlayer();
        InstantKill();
    }

    public void PlayExplodeSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(explodeEvent, GetPlayerViewPosition());
    }
    
    public void TriggerExplosion()
    {
        explosionTransform.gameObject.SetActive(true);
        PlayExplodeSound();
    }

    public override void HandleEnterDeath()
    {
        base.HandleEnterDeath();
        //playerStateMachine.DrillixirManager.RefillDrillixir(drillixirRefillAmount);
        sizzleEmitter.Stop();
        BombKill();
    }

    public void BombKill()
    {
        BombDeathFlash();
        _collider.enabled = false;
        _enemyAudio.PlayDeathSound();
    }
    
    private void BombDeathFlash()
    {
        //_meshRenderer.material.color = White;
        StartCoroutine(BombWaitThenDestroy());
    }
    private IEnumerator BombWaitThenDestroy()
    {
        yield return new WaitForSeconds(0.25f);
        // Destroy(gameObject);
        BombBody.SetActive(false);
    }
}

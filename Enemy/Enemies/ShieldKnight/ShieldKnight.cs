using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;

[RequireComponent(typeof(EnemyFacePlayerBehavior))]
public class ShieldKnight : EnemyStateMachine
{
    [SerializeField] private Material _whiteMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Collider _collider;
    private EnemyAudio _enemyAudio;
    private Vector3 spawnLocation;
    
    private EnemyFacePlayerBehavior enemyFacePlayerBehavior;
    
    private Material _orgMaterial;
    
    public override void OnAwake()
    {
        base.OnAwake();
        _enemyAudio = GetComponent<EnemyAudio>();
        enemyFacePlayerBehavior = GetComponent<EnemyFacePlayerBehavior>();
        _orgMaterial = _meshRenderer.material;
    }

    public override void SetSpawnLocation()
    {
        spawnLocation = transform.position;
    }

    public override void ResetOnEnable()
    {
        base.ResetOnEnable();
        transform.position = spawnLocation;
    }

    public override void HandleEnterDeath()
    {
        base.HandleEnterDeath();
        Kill();
    }
    public override void HandleDeath()
    {
        
    }

    public override void HandleAlert()
    {

    }

    public override void HandleAlertMove()//handle enemy move, rotation, and tilt
    {
        enemyFacePlayerBehavior.FacePlayer();
    }

    public override void HandleIdleMove()
    {
    }

    public override void HandleIdleLOSMove()
    {
        enemyFacePlayerBehavior.FacePlayer();
    }
    
    public override void HandleIdleSuspiciousMove()
    {
        
    }
    
    public void Kill()
    {
        DeathFlash();
        _collider.enabled = false;
        _enemyAudio.PlayDeathSound();
    }
    
    private void DeathFlash()
    {
        _meshRenderer.material = _whiteMaterial;
        StartCoroutine(WaitThenDestroy());
    }
    private IEnumerator WaitThenDestroy()
    {
        yield return new WaitForSeconds(0.25f);
        _collider.enabled = true;
        _meshRenderer.material = _orgMaterial;
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }
}

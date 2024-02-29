using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using __OasisBlitz.Utility;
using Lofelt.NiceVibrations;
using UnityEngine.Serialization;

[RequireComponent(typeof(EnemyFacePlayerBehavior))]
public class Swarmer : EnemyStateMachine
{
    //TODO: add function that has enemy look around instead of just starring forward
    [SerializeField] private Material _whiteMaterial;
    [SerializeField] protected MeshRenderer _meshRenderer;
    [SerializeField] protected Collider _collider;
    protected EnemyAudio _enemyAudio;
    private EnemyFacePlayerBehavior enemyFacePlayerBehavior;
    private Vector3 startSpawnLocation;

    private Material _orgMaterial;
    //Set to false if Swarmer Varient has its own death logic
    protected bool NormalDeath = true;
    
    public override void OnAwake()
    {
        base.OnAwake();
        _enemyAudio = GetComponent<EnemyAudio>();
        enemyFacePlayerBehavior = GetComponent<EnemyFacePlayerBehavior>();
        _orgMaterial = _meshRenderer.material;
    }
    

    public override void ResetOnEnable()
    {
        base.ResetOnEnable();
        transform.position = startSpawnLocation;
        _collider.enabled = true;
        _meshRenderer.material = _orgMaterial;
    }

    public override void SetSpawnLocation()
    {
        startSpawnLocation = transform.position;
    }

    public override void HandleEnterDeath()
    {
        base.HandleEnterDeath();
        if (NormalDeath)
        {
            Kill();
        }
    }
    public override void HandleDeath()
    {
    }

    public override void HandleAlert()
    {

    }

    public override void HandleAlertMove() //handle enemy move, rotation, and tilt
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
        // Destroy(gameObject);
        gameObject.SetActive(false);
        
    }
}

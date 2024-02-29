using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.Enemy;
using __OasisBlitz.Enemy.StateMachine;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(EnemyFacePlayerBehavior))]
public class BounceKnightStateMachine : EnemyStateMachine
{

    [SerializeField] private GameObject BounceKnightBody;
    [SerializeField] private Material _whiteMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;
    //Face meshes
    [SerializeField] private List<MeshRenderer> _faceRenderer;
    [SerializeField] private Collider _collider;
    [SerializeField] private EnemyAudio _enemyAudio;
    [SerializeField] private BoxCollider DeathBarrier;
    [SerializeField] private ShieldCollision shieldCollision;

    private EnemyFacePlayerBehavior enemyFacePlayerBehavior;
    
    //Respawn
    [SerializeField] private GameObject BounceKnightPrefab;
    private Vector3 spawnLocation;
    
    public override void OnAwake()
    {
        base.OnAwake();
        playerStateMachine = playerTransform.gameObject.GetComponent<PlayerStateMachine>();
        enemyFacePlayerBehavior = GetComponent<EnemyFacePlayerBehavior>();
    }

    public override void SetSpawnLocation()
    {
        spawnLocation = transform.position;
    }

    public override void ResetOnEnable()
    {
        base.ResetOnEnable();
        //GameObject newBounceKnight = Instantiate(BounceKnightPrefab, null);
        //newBounceKnight.transform.position = spawnLocation;
        //Destroy(gameObject);
    }

    public override void HandleEnterDeath()
    {
        //shield drop
        EnemyRigidBody.constraints = RigidbodyConstraints.FreezePosition;
        shieldCollision.DropShieldAnimation();
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
        //TODO: randomly turn around 
    }

    public override void HandleIdleLOSMove()
    {
        enemyFacePlayerBehavior.FacePlayer();
    }
    
    public override void HandleIdleSuspiciousMove()
    {
        
    }
    public void ForceAttack()
    {
        StartCoroutine(waitAndKill());
    }
    private void KillPlayer()
    {
        //Set spikes
        shieldCollision.EjectSpikes();
        if (DeathBarrier.bounds.Contains(playerTransform.position))
        {
            playerStateMachine.InstantKill();
        }
    }

    private IEnumerator waitAndKill()
    {
        yield return new WaitForSeconds(0.8f);
        KillPlayer();
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
        StartCoroutine(WaitThenDisable());
    }
    private IEnumerator WaitThenDisable()
    {
        yield return new WaitForSeconds(0.25f);
        _meshRenderer.enabled = false;
        for (int i = 0; i < _faceRenderer.Count; i++)
        {
            _faceRenderer[i].enabled = false;
        }
    }

}

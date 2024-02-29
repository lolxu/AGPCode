using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;

[RequireComponent(typeof(EnemyFacePlayerBehavior))]
public class RangedShooter : EnemyStateMachine
{
    [SerializeField] private Material _whiteMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Collider _collider;
    //shooting
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private Transform bulletExitTransform;
    //Audio
    private EnemyAudio _enemyAudio;
    private Vector3 spawnLocation;
    
    private EnemyFacePlayerBehavior enemyFacePlayerBehavior;
    public override void OnAwake()
    {
        base.OnAwake();
        _enemyAudio = GetComponent<EnemyAudio>();
        enemyFacePlayerBehavior = GetComponent<EnemyFacePlayerBehavior>();
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

    public override void HandleEnterAttack()
    {
        Shoot();
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
        Destroy(gameObject);
    }
    
    private void Shoot()
    {
        Vector3 shootDirection = (playerTransform.position - bulletExitTransform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletExitTransform.position + shootDirection * 2f, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity =
            shootDirection * projectileSpeed;
        _enemyAudio.PlayShoot();
    }
}

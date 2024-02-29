using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    [SerializeField] private EnemyStateMachine _enemyStateMachine; 
    private int _layerMaskNonPhysics;
    [SerializeField] private float hitBoxRadius;
    [SerializeField] private float capsuleLength;
    void Awake() 
    {
        _layerMaskNonPhysics = LayerMask.GetMask("Default", "Enemy");
    }
    
    private void FixedUpdate()
    {
        if (!_enemyStateMachine.IsDead)
        {
            CheckIndependentCollisions();
        }
    }
    
    ///<summary>
    /// only checks collisions on the player body
    ///</summary>
    
    public void CheckIndependentCollisions()
    {
        Collider[] colliders = UnityEngine.Physics.OverlapCapsule(transform.position - transform.up * capsuleLength, transform.position + transform.up * capsuleLength, hitBoxRadius, _layerMaskNonPhysics);
                
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("InstantKill"))
            {
                //_playerStateMachine.ImpactInstantKill();
            }else if (colliders[i].CompareTag("DeathBarrier"))
            {
                if (!_enemyStateMachine.IsDead)
                {
                    _enemyStateMachine.InstantKill();
                }
            }else if (colliders[i].CompareTag("Player"))
            {
                //TODO: check if drill and stunn - nah do this from player
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hitBoxRadius);
        Gizmos.DrawLine(transform.position - transform.up * capsuleLength, transform.position + transform.up * capsuleLength);
    }
}

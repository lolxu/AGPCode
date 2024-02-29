using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;

public class HitPartOfNewEnemy : MonoBehaviour
{
    [SerializeField] private EnemyStateMachine enemyStateMachine;
    [SerializeField] private int BodyPart;
    public void CollideWithBody(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
    {
        OnDamageCollideWithEnemyBody(ref coll);
    }
    
    // called by collisions with objects other than player w/out normal data
    public void CollideWithBody(ref Collider coll)
    {
        // OnDamageCollideWithEnemyBody(ref coll);
    }
    
    // Called when valid collider calls Collide With Body on this New Enemy
    // currently only a colliding player in the drilling state or damaging ability can call this,
    // just die for now
    private void OnDamageCollideWithEnemyBody(ref Collider coll)
    {
        enemyStateMachine.InstantKill();
    }

    // Get EnemyStateMachine to check the type of enemy if needed
    public EnemyStateMachine GetEnemyStateMachine()
    {
        return enemyStateMachine;
    }
}

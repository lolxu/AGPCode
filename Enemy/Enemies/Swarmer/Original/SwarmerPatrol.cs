using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPatrolBehavior))]
public class SwarmerPatrol : Swarmer
{
    private EnemyPatrolBehavior enemyPatrolBehavior;
    private Vector3 spawnLocation;
    public override void OnAwake()
    {
        base.OnAwake();
        enemyPatrolBehavior = GetComponent<EnemyPatrolBehavior>();
    }

    public override void SetSpawnLocation()
    {
        spawnLocation = enemyPatrolBehavior.moveAgent.transform.position;
    }

    public override void ResetOnEnable()
    {
        base.ResetOnEnable();
        enemyPatrolBehavior.moveAgent.Warp(spawnLocation);
    }

    public override void HandleAlertMove() //handle enemy move, rotation, and tilt
    {
        if (!enemyPatrolBehavior.moveAgent.isStopped)
        {
            enemyPatrolBehavior.StopAgent();
        }
        enemyPatrolBehavior.FacePlayer();
    }

    public override void HandleIdleMove()
    {
        enemyPatrolBehavior.Patrol();
    }

    public override void HandleIdleSuspiciousMove()
    {
        if (!enemyPatrolBehavior.moveAgent.isStopped)
        {
            enemyPatrolBehavior.StopAgent();
        }
    }

    public override void HandleIdleLOSMove()
    {
        if (!enemyPatrolBehavior.moveAgent.isStopped)
        {
            enemyPatrolBehavior.StopAgent();
        }
        enemyPatrolBehavior.FacePlayer();
    }
}

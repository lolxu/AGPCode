using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core;
using UnityEngine;

[RequireComponent(typeof(EnemyChaseBehavior))]
public class SwarmerPatrolAndFollow : SwarmerPatrol
{
    private EnemyChaseBehavior enemyChaseBehavior;
    private Vector3 startSpawnLocation;
    public override void OnAwake()
    {
        base.OnAwake();
        enemyChaseBehavior = GetComponent<EnemyChaseBehavior>();
    }
    public override void SetSpawnLocation()
    {
        startSpawnLocation = enemyChaseBehavior.moveAgent.transform.position;
    }

    public override void ResetOnEnable()
    {
        base.ResetOnEnable();
        enemyChaseBehavior.moveAgent.Warp(startSpawnLocation);
    }

    public override void HandleAlertMove() //handle enemy move, rotation, and tilt
    {
        enemyChaseBehavior.ChasePlayer();
        enemyChaseBehavior.FacePlayer();
    }

    public override void HandleIdleLOSMove()
    {
        enemyChaseBehavior.ChasePlayer();
        enemyChaseBehavior.FacePlayer();
    }
}

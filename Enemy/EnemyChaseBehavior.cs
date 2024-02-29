using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseBehavior : EnemyPatrolBehavior
{
    private PlayerStateMachine playerStateMachine;
    [SerializeField] private Transform triggerBoxBoundsParent;
    [SerializeField] private List<Bounds> canMoveBounds;
    private float navmeshPointSearchMaxDistance;
    private float chaseSpeed = 25.0f;
    private float angularSpeed = 360.0f;
    protected override void Awake()
    {
        base.Awake();
        navmeshPointSearchMaxDistance = moveAgent.height * 2.0f;
        foreach(Transform child in triggerBoxBoundsParent)
        {
            canMoveBounds.Add(child.GetComponent<BoxCollider>().bounds);
        }
        playerStateMachine = playerTransform.GetComponent<PlayerStateMachine>();
    }
    
    public void ChasePlayer()
    {
        if (moveAgent.autoBraking)
        {
            moveAgent.autoBraking = false;
        }
        moveAgent.speed = chaseSpeed;
        moveAgent.angularSpeed = angularSpeed;
        //check if player is in boxes
        foreach (Bounds box in canMoveBounds)
        {
            if (box.Contains(playerTransform.position))
            {
                if (!playerStateMachine.IsDead)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(playerTransform.position, out hit, navmeshPointSearchMaxDistance,
                            NavMesh.AllAreas))
                    {
                        if (moveAgent.isStopped)
                        {
                            moveAgent.isStopped = false;
                        }

                        moveAgent.SetDestination(hit.position);
                        return;
                    }
                }
            }
        }
        //if there is not a valid chase position, stop the agent otherwise they would go toward their last set patrol point
        if ((moveAgent.destination.magnitude - waypoints[waypointIndex].magnitude) < 1.0f 
            || (moveAgent.destination.magnitude - waypoints[prevWaypointIndex].magnitude) < 1.0f)
        {
            StopAgent();
        }
        //FacePlayer();
    }

    public override void Patrol()
    {
        base.Patrol();
        if (moveAgent.destination != waypointTarget)
        { 
            moveAgent.SetDestination(waypointTarget);
        }
    }
}

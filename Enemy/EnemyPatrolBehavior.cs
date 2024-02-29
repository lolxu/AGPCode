using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Utility;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolBehavior : EnemyFacePlayerBehavior
{
    //Patrol
    [SerializeField] public NavMeshAgent moveAgent;
    private float patrolSpeed = 5.0f;
    private float patrolAngularSpeed = 180.0f;
    private float acceleration = 100.0f;
    [SerializeField] protected Transform waypointsParent;
    [SerializeField] protected List<Vector3> waypoints;
    protected int waypointIndex = 1;
    protected int prevWaypointIndex = 0;
    protected Vector3 waypointTarget;

    protected override void Awake()
    {
        base.Awake();
        moveAgent.acceleration = acceleration;
        moveAgent.isStopped = true;
        foreach (Transform child in waypointsParent)
        {
            waypoints.Add(child.transform.position);
        }
    }
    
    public void StopAgent()
    {
        moveAgent.velocity = Vector3.zero;
        moveAgent.ResetPath();
        moveAgent.isStopped = true;
#if DEBUG
        if (Constants.DebugSwarmNavMesh)
        {
            Debug.Log("Setting isStopped to true: " + moveAgent.isStopped);
        }
#endif
    }
    
    private void ResetDestination()
    {
        moveAgent.isStopped = false;
        waypointTarget = waypoints[prevWaypointIndex];
        moveAgent.SetDestination(waypointTarget);
#if DEBUG
        if (Constants.DebugSwarmNavMesh)
        {
            Debug.Log("New target is waypoint " + prevWaypointIndex + " " + moveAgent.destination);
        }
#endif
    }
    
    private void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex >= waypoints.Count)
        {
            waypointIndex = 0;
        }
        prevWaypointIndex++;
        if (prevWaypointIndex >= waypoints.Count)
        {
            prevWaypointIndex = 0;
        }
    }
    
    private void UpdateDestination()
    {
        waypointTarget = waypoints[waypointIndex];
        moveAgent.SetDestination(waypointTarget);
    }
    
    public virtual void Patrol()
    {
        if (!moveAgent.autoBraking)
        {
            moveAgent.autoBraking = true;
        }
        moveAgent.speed = patrolSpeed;
        moveAgent.angularSpeed = patrolAngularSpeed;
        //check if we are close to waypoint
#if DEBUG
        if (Constants.DebugSwarmNavMesh)
        {
            Debug.Log("Patrol isStopped: " + moveAgent.isStopped);
        }
#endif
        if (moveAgent.isStopped)
        {
            ResetDestination();
#if DEBUG
            if (Constants.DebugSwarmNavMesh)
            {
                Debug.Log("Resetting Agent");
            }
#endif
        }
        if (Vector3.Distance(moveAgent.transform.position, waypointTarget) < moveAgent.baseOffset * 2.0f + 1)
        {
            IterateWaypointIndex();
            UpdateDestination();
        }
    }
}

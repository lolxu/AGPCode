using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Enemy;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TargetedDash : MonoBehaviour
{
    public float dashSlowdownTime;
    public float postSlowdownPauseTime;
    public float dashSpeed;
    public float postDashSpeed;

    public float drillixirCost;
    public float minimumDrillixir;
    
    public GameObject trajectoryLinePrefab;
    private DashTrajectoryLine currentTrajectoryLine;

    public bool forwardDashOnly = false;
    
    [Header("Dash ranges")] 
    public float upperIncomingAngle = 45f;
    public float lowerIncomingAngle = 135f;

    [Header("Dash angles")] 
    public float highDashAngle = 30f;
    public float midDashAngle = 75f;
    public float lowDashAngle = 145f;
    
    public Spring3D dashSpring;

    private Vector3 directionToTarget;

    public float startSpringFreq;
    public float endSpringFreq;
    public float springFreqLerpTime;

    public float dashTargetAngle = 30.0f;

    public bool DashEnabled = true;

    private float incomingAngle = 0f;
    
    public class DashAttemptResult
    {
        public bool DidDash;
        public Vector3 DashVelocity;

        public DashAttemptResult()
        {
            DidDash = false;
            DashVelocity = Vector3.zero;
        }
    }

    public Vector3 TargetPosition()
    {
        if (currentDashTarget != null)
        {
            return currentDashTarget.transform.position;
        }

        return Vector3.zero;
    }

    public Vector3 ClosestPointOnTargetSurface()
    {
        if (currentDashTarget == null) return Vector3.zero;

        Vector3 targetToPlayer = (transform.position - currentDashTarget.transform.position).normalized;
        Vector3 targetSurface = currentDashTarget.transform.position + targetToPlayer * currentDashTarget.drillAttachRadius;
        
        return targetSurface;
        
    }

    public enum DashStage
    {
        SlowDown,
        HighSpeed,
        Finished
    }
    
    [SerializeField] private PlayerStateMachine Ctx;

    private LayerMask targetPointMask;
    public float potentialTargetRadius;
    public float actualDashableRadius;
    private Collider[] queryResult;
    public DashStage currentDashStage { get; private set; }
    public DashTargetPoint currentDashTarget { get; private set; }
    private bool hasHitEnemy;

    private Coroutine currentDashRoutine;

    void Awake()
    {
        targetPointMask = LayerMask.GetMask("EnemyTargetPoint");
        queryResult = new Collider[30];
        SceneManager.sceneLoaded += EnableDash;
    }

    private void EnableDash(Scene scene, LoadSceneMode mode)
    {
        DashEnabled = true;
    }

    void Update()
    {
        DashTargetPoint newDashTarget = GetDashTarget();

        // Handle aim indicator for targeted enemy
        if (newDashTarget != currentDashTarget)
        {
            // This is unsetting the dash target every frame as well...
            if (currentDashTarget != null)
            {
                currentDashTarget.UnsetAsTarget();
            }
            if (newDashTarget != null)
            {
                newDashTarget.SetAsTarget(actualDashableRadius);
            }

            currentDashTarget = newDashTarget;

            // TODO: Uncomment for dash trajectory line
            // if (currentTrajectoryLine != null)
            // {
            //     currentTrajectoryLine.OnDashTargetChanged();
            // }
            
            // Set up line visual towards this dash target
            //currentTrajectoryLine = Instantiate(trajectoryLinePrefab).GetComponent<DashTrajectoryLine>();
        }
        
        // if (currentDashTarget != null)
        // {
        //     currentTrajectoryLine.SetPositions(transform.position, currentDashTarget.transform.position);
        // }

        if (currentDashTarget != null)
        {
            if (Vector3.Distance(currentDashTarget.transform.position, transform.position) < actualDashableRadius)
            {
                // Debug.Log("enemy in dashable range");
                currentDashTarget.SetDashable();
            }
            else
            {
                // Debug.Log("enemy NOT in dashable range");
                currentDashTarget.UnsetDashable();
                currentDashTarget = null;
            }
        }
        else
        {
            HUDManager.Instance.SetDisplayDashPrompt(false);
        }
        
    }
    
    public bool CanPerformDash()
    {
        
        return currentDashTarget != null && Ctx.DrillixirManager.CanDash() && DashEnabled;
    }

    public void PerformDash()
    {
        currentDashTarget = GetDashTarget();
        if (Vector3.Distance(transform.position, currentDashTarget.transform.position) < actualDashableRadius)
        {
            currentDashRoutine = StartCoroutine(DashCoroutine(currentDashTarget));
            // Store the angle at the start of the dash
            incomingAngle = GetDashAngle();
            directionToTarget = (currentDashTarget.transform.position - transform.position).normalized;
            // Empty for now...
            // Ctx.DrillixirManager.ConsumeDashDrillixir();
        }
    }
    
    private IEnumerator DashCoroutine(DashTargetPoint dashTarget)
    {
        hasHitEnemy = false;
        // Aim the camera at the target
        Ctx.cameraStateMachine.SetToLookAtTarget(dashTarget.transform.position);
        
        Vector3 slowdownVelocity = Ctx.PlayerPhysics.Velocity.normalized;
        
        // ********** FIRST STEP: SLOW THE PLAYER OVER TIME ************
        // Goal for playerPhysics during this step: maintain direction of velocity from start of dash, but reduce magnitude to zero
        currentDashStage = DashStage.SlowDown;
        float timeElapsed = 0; 
        
        
        
        // First, tween player velocity to zero over 0.5 seconds
        // Uncomment the following to enable the slowdown before dashing
        // float initialSpeed = Ctx.PlayerPhysics.Velocity.magnitude;
        // float currentSpeed = initialSpeed;
        //
        // while (timeElapsed < dashSlowdownTime)
        // {
        //     currentSpeed = Mathf.Lerp(initialSpeed, 0, timeElapsed / dashSlowdownTime);
        //     Ctx.PlayerPhysics.SetVelocity(slowdownVelocity * currentSpeed);
        //     timeElapsed += Time.deltaTime;
        //     yield return null; // wait for the next frame
        // }
        //
        // currentSpeed = 0; // ensure the final value is exactly zero
        // Ctx.PlayerPhysics.SetVelocity(slowdownVelocity * currentSpeed);
        //
        // yield return new WaitForSeconds(postSlowdownPauseTime);
        
        // ********** SECOND STEP: MOVE AT HIGH VELOCITY STRAIGHT TOWARDS TARGET *********
        currentDashStage = DashStage.HighSpeed;
        
        // Vector3 dashDirection = (dashTarget.transform.position - transform.position).normalized;
        //
        //
        // Vector3 dashVelocity = dashDirection * dashSpeed;
        // Ctx.PlayerPhysics.SetVelocity(dashVelocity);
        // RaycastHit hit;
        // if (Physics.Raycast(transform.position, directionToTarget, out hit))
        // {
        //     if (!hit.transform.gameObject.CompareTag("Enemy"))
        //     {
        //         currentDashStage = DashStage.Finished;
        //         yield break;
        //     }
        // }
        // else
        // {
        //     currentDashStage = DashStage.Finished;
        //     yield break;
        // }

            // Spring based approach
        dashSpring.equilibriumPosition = dashTarget.transform.position;
        dashSpring.position = transform.position;
        dashSpring.velocity = Ctx.PlayerPhysics.Velocity;
        dashSpring.angularFrequency = startSpringFreq;
        
        while (currentDashStage == DashStage.HighSpeed)
        {
            dashSpring.equilibriumPosition = dashTarget.transform.position;
            timeElapsed += Time.deltaTime;
            // Debug.Log("current dash target position: " + dashTarget.transform.position);
            // Debug.Log("Current spring position" + dashSpring.position);
            // dashSpring.equilibriumPosition = dashTarget.transform.position;
            dashSpring.angularFrequency = Mathf.Lerp(startSpringFreq, endSpringFreq, timeElapsed / springFreqLerpTime);
            Ctx.PlayerPhysics.SetVelocity(dashSpring.velocity);
            yield return null;
        }
        
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.magenta;
        // Gizmos.DrawWireSphere(transform.position, potentialTargetRadius);
        // Gizmos.DrawSphere(transform.position, actualDashableRadius);
    }

    private DashTargetPoint GetDashTarget()
    {
        // Target enemies within FOV

        // Overlap sphere non alloc to find enemies
        int numEnemiesInRange = Physics.OverlapSphereNonAlloc(transform.position, potentialTargetRadius, queryResult, targetPointMask);
        
        // Debug.Log(numEnemiesInRange);
        
        // If no enemies in range, return null
        if (numEnemiesInRange == 0)
        {
            return null;
        }

        // Otherwise, return closest enemy
        DashTargetPoint bestTarget = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < numEnemiesInRange; i++)
        {
            DashTargetPoint targetPoint = queryResult[i].GetComponent<DashTargetPoint>();

            // Go to the next iteration if the enemy is null, the target is no longer valid (enemy has been killed), or the enemy is not aligned with player move direction
            // if (targetPoint == null || !targetPoint.isValidTarget || Vector3.Dot(Ctx.PlayerPhysics.Velocity, targetPoint.transform.position - transform.position) < 0)
            if (targetPoint == null || !targetPoint.isValidTarget)
            {
                continue;
            }

            Vector3 targetLocation = targetPoint.transform.position;

            float distance = Vector3.Distance(transform.position, targetLocation);

            // Only look for targets within the acceptable angle
            var camPos = CameraStateMachine.Instance.CameraSurface.transform.position;
            Vector3 toTarget = targetLocation - camPos;
            Vector3 camForward = CameraStateMachine.Instance.CameraSurface.transform.forward;
            float angle =
                Mathf.Acos(Vector3.Dot(toTarget, camForward) / (toTarget.magnitude * camForward.magnitude)) *
                Mathf.Rad2Deg;
            // Debug.Log(angle);

            // Do a raycast check for anything else other than enemies
            RaycastHit hit;
            LayerMask dashLayerMask = Physics.AllLayers & ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
            if (Physics.Raycast(camPos, toTarget, out hit, toTarget.magnitude * 1.5f,
                    dashLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.layer == 8
                    || hit.collider.gameObject.layer == 14
                    || hit.collider.gameObject.layer == 16)
                {
                    if (angle < dashTargetAngle)
                    {
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            bestTarget = targetPoint;
                        }
                    }
                }
            }
        }

        return bestTarget;
        
    }

    private float GetDashAngle()
    {
        // For now, we always use the world space up vector
        Vector3 upVector = Vector3.up;
        
        Vector3 targetToPlayer = transform.position - currentDashTarget.transform.position;
        targetToPlayer.Normalize();
        
        // We want the angle between the up vector and the vector 

        float angle = Vector3.Angle(upVector, targetToPlayer);
        
        // Debug.Log("Dashing with incoming angle: " + angle);
        return angle;
    }

    /// <summary>
    /// Adjusts the vertical angle of the vector, returned vector is normalized
    /// Won't behave well if the vector has very small X and Z components
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    private Vector3 SetVerticalAngleOfVector(Vector3 vec, float verticalAngle)
    {
        vec.y = 0;
        vec.Normalize();

        // We want this to be relative to the up vector, but by default this will be relative to the ground plane
        verticalAngle -= 90f;

        Vector3 rotationAxis = Vector3.Cross(Vector3.up, vec);
        Quaternion rotation = Quaternion.AngleAxis(verticalAngle, rotationAxis);
        
        return (rotation * vec).normalized;

    }

    private Vector3 GetResultantVelocity()
    {
        //  Get vertical difference between player and target
        if (currentDashTarget != null)
        {
            Vector3 velocity = directionToTarget;

            // TODO: Uncomment this for variable dash angle based on incoming angle.
            if (!forwardDashOnly)
            {
                if (incomingAngle < 5f)
                {
                    // For a very low angle, just dash straight down (don't want to turn the player around)
                    velocity = Vector3.down;
                }
                if (incomingAngle < upperIncomingAngle)
                {
                    // Your incoming angle is in the upper range (above enemy), so dash downwards after
                    velocity = SetVerticalAngleOfVector(velocity, lowDashAngle);
                }
                else if (incomingAngle < lowerIncomingAngle)
                {
                    // Your incoming angle is in the middle range (in front of enemy), so dash slightly upwards after
                    velocity = SetVerticalAngleOfVector(velocity, midDashAngle);
                }
                else if (incomingAngle < 175f)
                {
                    // Your incoming angle is in the lower range (below enemy), so dash upwards after
                    velocity = SetVerticalAngleOfVector(velocity, highDashAngle);
                }
                else
                {
                    // For a very high angle, dash straight up (don't want to turn the player around)
                    velocity = Vector3.up;
                }
            }
            else
            {
                velocity = SetVerticalAngleOfVector(velocity, midDashAngle);
            }

            velocity = velocity.normalized * postDashSpeed;
            // Debug.Log("Post dash speed " + velocity.magnitude);
            // Debug.Log("Y component of post dash speed " + velocity.y);
            return velocity;
        }
        
        return Ctx.PlayerPhysics.Velocity;
    }

    public void ImpactEnemy()
    {
        currentDashStage = DashStage.Finished;       
        // Ctx.PlayerPhysics.SetVelocity(Ctx.PlayerPhysics.Velocity.normalized * postDashSpeed);
        Ctx.PlayerPhysics.SetVelocity(GetResultantVelocity());
    }

    public void ImpactOther()
    {
        currentDashStage = DashStage.Finished;       
        // Ctx.PlayerPhysics.SetVelocity(Ctx.PlayerPhysics.Velocity.normalized * postDashSpeed);
        Ctx.PlayerPhysics.SetVelocity(GetResultantVelocity());
    }
    
    //Called to make sure the player resets properly in instances where they experience death without collision and therefore do not exit properly
    public void EndDash()
    {
        currentDashStage = DashStage.Finished;
        if (currentDashRoutine != null)
        {
            StopCoroutine(currentDashRoutine);
        }
    }

}

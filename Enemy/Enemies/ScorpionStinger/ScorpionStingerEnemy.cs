using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines.Interpolators;

public class ScorpionStingerEnemy : EnemyStateMachine
{
    //TODO: have idle movement, look at, and attack always on and move between them
    [SerializeField] private Transform targetLocation;
    [SerializeField] private Transform idleLocation;
    [SerializeField] private Transform lookAtLocation;
    [SerializeField] private Transform attackLocation;
    [Range(0.0f, 1.0f)] [SerializeField] private float idleWeight;
    [Range(0.0f, 1.0f)] [SerializeField] private float attackWeight;
    private Vector2 idleAttack;
    private Vector3 idleStartPos;
    private float idleMoveTime = -1.0f;
    private bool idleMoveDir = false;
    //scale to modify attack time (2 means 0.5 sec and 0.5 means 2 sec)
    private float AttackScale = 4.0f;
    private float AttackCooldownScale = 0.25f;

    [SerializeField] private float idleOffsetScale = 3.0f;
    
    [SerializeField] private float maxRotationSpeedDegreesPerSec = 90.0f;
    public override void OnAwake()
    {
        base.OnAwake();
        SetIdleWeight(1.0f);
        SetAttackWeight(0.0f);
        float scaleMultiplier = transform.parent.localScale.x;
        AlertDistance *= scaleMultiplier;
        MinAttackDistance *= scaleMultiplier;
        MaxAttackDistance *= scaleMultiplier;
        ViewpointVerticalAlignment *= scaleMultiplier;
        //SetLookAtWeight(0.0f);
        idleStartPos = GetRestingPoint();
        idleLocation.position = idleStartPos;
    }
    
    //TODO: have look at movement be an average between the player pos and the center top

    public override void HandleEnterAttack()
    {
        base.HandleEnterAttack();
        //TODO: activate look at - quickly add weight to attack target and decrease weight from others
        attackLocation.position = playerTransform.position;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        //quickly add weight to attack target and decrease weight from others\
        attackTime = 1.0f * (1 / AttackScale) + 1.0f * (1 / AttackCooldownScale);
        float time = 0;
        while (time < 1.0f)
        {
            time += Time.deltaTime * AttackScale;
            SetIdleWeight(Mathf.Lerp(1.0f, 0.0f, time));
            SetAttackWeight(Mathf.Lerp(0.0f, 1.0f, time));
            //SetLookAtWeight(Mathf.Lerp(1.0f, 0.0f, time));
            yield return null;
        }
        time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime * AttackCooldownScale;
            SetIdleWeight(Mathf.Lerp(0.0f, 1.0f, time));
            SetAttackWeight(Mathf.Lerp(1.0f, 0.0f, time));
            //SetLookAtWeight(Mathf.Lerp(0.0f, 1.0f, time));
            yield return null;
        }
    }

    public override void HandleAlertMove()
    {
        //TODO:look at player - - add weight to look target
        SetLookAtTarget();
        //TODO: slerp to look at target
        Vector3 from = idleLocation.position - GetPlayerViewPosition();
        from.y = 0.0f;
        Vector3 to = lookAtLocation.position - GetPlayerViewPosition();
        to.y = 0.0f;
        //TODO: make num equal to the total rotation and maxRotationSpeed be a preset value
        float totalAngle = Vector3.Angle(from, to);
        to = Vector3.SlerpUnclamped(from, to, Mathf.Min(1f, maxRotationSpeedDegreesPerSec/totalAngle * Time.deltaTime));
        to += GetPlayerViewPosition();
        to.y = idleStartPos.y;
        idleLocation.position = to;
        SetIdleWeight(idleWeight + Time.deltaTime);
    }

    private void SetLookAtTarget()
    {
        //TODO: make it in the direction of player in magnitude of minDist - look good if player is
        Vector3 playerPos = playerTransform.position;
        playerPos.y = 0.0f;
        Vector3 enemyPos = GetPlayerViewPosition();
        enemyPos.y = 0.0f;
        Vector3 enemyToPlayer = playerPos - enemyPos;
        enemyToPlayer.Normalize();
        enemyToPlayer = MinAttackDistance * enemyToPlayer;
        //enemyToPlayer.y = idleStartPos.y;
        enemyToPlayer += GetPlayerViewPosition();
        enemyToPlayer.y = idleStartPos.y;
        lookAtLocation.position = enemyToPlayer;
    }

    public override void HandleIdle()
    {
        base.HandleIdle();
        SetIdleWeight(idleWeight + Time.deltaTime);
        //SetIdleTarget();
    }

    private Vector3 GetIdleOffset()
    {
        Vector3 offset;
        if (idleMoveDir)
        {
            idleMoveTime += Time.deltaTime;
            if (idleMoveTime >= 1.0f)
            {
                idleMoveTime = 1.0f;
                idleMoveDir = false;
            }
        }
        else
        {
            idleMoveTime -= Time.deltaTime;
            if (idleMoveTime <= -1.0f)
            {
                idleMoveTime = -1.0f;
                idleMoveDir = true;
            }
        }
        offset = Vector3.up * Mathf.Sin(idleMoveTime) + Vector3.right * Mathf.Cos(idleMoveTime);
        offset *= idleOffsetScale;
        
        return offset;
    }

    private void SetTarget()
    {
        Vector3 newTargetLocation =  (((idleLocation.position + GetIdleOffset()) - GetPlayerViewPosition()) * idleAttack.x) + ((attackLocation.position - GetPlayerViewPosition()) * idleAttack.y);//idleLocation.position * idleLookAttack.y +
        //Get distance from viewpoint to this
        if (newTargetLocation.magnitude > MaxAttackDistance)
        {
            newTargetLocation.Normalize();
            newTargetLocation *= MaxAttackDistance;
        }else if (newTargetLocation.magnitude < MinAttackDistance)
        {
            newTargetLocation.Normalize();
            newTargetLocation *= MinAttackDistance;
        }
        targetLocation.position = GetPlayerViewPosition() + newTargetLocation;
    }
    
    private void Update()
    {
        SetTarget();
    }
    
    private void SetIdleWeight(float newVal)
    {
        idleWeight = newVal;
        idleWeight = Mathf.Clamp(idleWeight, 0.0f, 1.0f);
        idleAttack.x = idleWeight;
        UpdateWeightedNormal();
    }

    private void SetAttackWeight(float newVal)
    {
        attackWeight = newVal;
        attackWeight = Mathf.Clamp(attackWeight, 0.0f, 1.0f);
        idleAttack.y = attackWeight;
        UpdateWeightedNormal();
    }
    
    private void UpdateWeightedNormal()//CALL THIS AFTER EVERY WEIGHT UPDATE
    {
        idleAttack.Normalize();
    }
    
    private Vector3 GetRestingPoint()
    {
        float x = MinAttackDistance;
        float hype = x / Mathf.Cos(Mathf.Deg2Rad * AlertDegreesFromHorizontal);
        Vector3 enemyToOuterRangeUnitTop = transform.forward * (1 - AlertDegreesFromHorizontal/90f) + transform.up * (AlertDegreesFromHorizontal/90f);
        Vector3 point = GetPlayerViewPosition() + hype * enemyToOuterRangeUnitTop;
        return point;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(lookAtLocation.position, 1.0f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(idleLocation.position, 1.0f);
        Gizmos.DrawWireSphere(attackLocation.position, 1.0f);
    }
}

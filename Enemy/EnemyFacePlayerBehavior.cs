using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyFacePlayerBehavior : MonoBehaviour
{
    [SerializeField] protected Transform enemyBodyTransform;
    [SerializeField] protected EnemyStateMachine enemyStateMachine;
    protected Transform playerTransform;
    [SerializeField] private float rotationSpeed = 180f;
    protected virtual void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    public void FacePlayer()
    {
        Vector3 enemyToPlayerInEnemySpace = enemyBodyTransform.InverseTransformVector(playerTransform.position - enemyStateMachine.GetPlayerViewPosition());
        enemyToPlayerInEnemySpace.y = 0.0f;
        Quaternion targetRotation = Quaternion.LookRotation(enemyBodyTransform.TransformVector(enemyToPlayerInEnemySpace), enemyBodyTransform.up);
        targetRotation = Quaternion.RotateTowards(enemyBodyTransform.rotation, targetRotation,  rotationSpeed * Time.deltaTime);
        enemyBodyTransform.rotation = targetRotation;
    }
}

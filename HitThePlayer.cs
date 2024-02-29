using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;
using Debug = FMOD.Debug;

/*
 *Add this script to the rigidbody of all enemies that chase the player 
 */
public class HitThePlayer : MonoBehaviour
{
    [SerializeField] private EnemyStateMachine state;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            state.TestKillPlayer();
            UnityEngine.Debug.Log("FOUND THE PLAYER");
        }

    }
}

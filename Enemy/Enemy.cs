using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

namespace __OasisBlitz.Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachine enemyStateMachine;

        // Update is called once per frame
        void Update()
        {
            enemyStateMachine.UpdateStates();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Enemy.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance;
        
        [Header("Enemy Related Settings")] 
        [SerializeField] private GameObject bounceKnightPrefab;
        private List<GameObject> EleminatedEnemies;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            EleminatedEnemies = new List<GameObject>();
        }

        private void OnEnable()
        {
            RespawnManager.OnReset += ResetAllEnemies;
        }

        private void OnDisable()
        {
            RespawnManager.OnReset -= ResetAllEnemies;
        }

        public void AddEliminatedEnemy(GameObject enemy)
        {
            if (!EleminatedEnemies.Contains(enemy))
            {
                EleminatedEnemies.Add(enemy);
            }
        }
        
        public void ResetAllEnemies()
        {
            // Debug.LogError(EleminatedEnemies.Count);
            for (int i = 0; i < EleminatedEnemies.Count; i++)
            {
                var enemy = EleminatedEnemies[i];
                if (enemy != null)
                {
                    enemy.SetActive(false);
                    enemy.SetActive(true);
                }
            }
            EleminatedEnemies.Clear();
        }
    }
}
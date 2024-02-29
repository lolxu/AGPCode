using System;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Collectables;
using __OasisBlitz.__Scripts.Player.Environment.Cannon;
using __OasisBlitz.Enemy;
using MoreMountains.Tools;
using UnityEngine;

namespace __OasisBlitz.Player.Environment.WinCondition
{
    public class WinConditionManager : MonoBehaviour
    {
        [Header("Settings for a Win Condition")] 
        [SerializeField] private GameObject winEffectedTarget;
        [SerializeField] private List<GameObject> requiredEnemies;
        [SerializeField] private GameObject selectedEnemyIndicator;
        [SerializeField] private List<GameObject> requiredInteractableObjects;

        [Header("Settings for Cannon Unlocks")]
        [SerializeField] private List<CannonToEnemies> cannonUnlocks;
        [SerializeField] private GameObject cannonEnemyIndicator;
        [Serializable]
        public class CannonToEnemies
        {
            public GameObject cannon;
            public List<GameObject> requiredEnemies;
        }

        // [Header("Settings for Critter Unlocks")]
        // [SerializeField] private List<CritterToEnemies> critterUnlocks;
        // [SerializeField] private GameObject critterEnemyIndicator;
        // [Serializable]
        // public class CritterToEnemies
        // {
        //     public GameObject critter;
        //     public List<GameObject> requiredEnemies;
        // }

        [Header("Settings for Plant Unlocks")]
        [SerializeField] private List<PlantToEnemies> plantUnlocks;
        [SerializeField] private GameObject plantEnemyIndicator;
        [Serializable]
        public class PlantToEnemies
        {
            public GameObject plant;
            public List<GameObject> requiredEnemies;
        }
        

        public static WinConditionManager Instance;
        
        private List<Enemy.Enemy> enemiesForWinCondition;
        private Dictionary<GameObject, List<Enemy.Enemy>> cannonEnemyDict;
        private Dictionary<GameObject, List<Enemy.Enemy>> critterEnemyDict;
        private Dictionary<GameObject, List<Enemy.Enemy>> plantEnemyDict;

        private Vector3 indicatorOffset = new Vector3(0.0f, 0.5f, 0.0f);
        private bool finalWinHasTriggered = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        
        private void Start()
        {
            enemiesForWinCondition = new List<Enemy.Enemy>();
            cannonEnemyDict = new Dictionary<GameObject, List<Enemy.Enemy>>();
            critterEnemyDict = new Dictionary<GameObject, List<Enemy.Enemy>>();
            plantEnemyDict = new Dictionary<GameObject, List<Enemy.Enemy>>();
            
            // Setup required enemies for win conditions
            foreach (var enemy in requiredEnemies)
            {
                enemiesForWinCondition.Add(enemy.GetComponent<Enemy.Enemy>());
                ApplyEnemyIndicator(enemy);
            }
            finalWinHasTriggered = false;
            
            // Setup required enemies for all cannon unlocks
            foreach (var cannonEnemies in cannonUnlocks)
            {
                List<GameObject> enemies = cannonEnemies.requiredEnemies;
                CannonTrigger cannon = cannonEnemies.cannon.GetComponent<CannonTrigger>();
                if (enemies.Count > 0)
                {
                    cannon.isUnlocked = false;
                }
                
                cannonEnemyDict.Add(cannonEnemies.cannon, new List<Enemy.Enemy>());
                foreach (var enemy in enemies)
                {
                    ApplyEnemyIndicator(enemy);
                    cannonEnemyDict[cannonEnemies.cannon].Add(enemy.GetComponent<Enemy.Enemy>());
                }
                
                GameObject cannonIndicator = Instantiate(cannonEnemyIndicator, 
                    cannonEnemies.cannon.transform.position, 
                    Quaternion.identity, 
                    cannonEnemies.cannon.transform);
                cannonIndicator.transform.localScale *= 2.0f;
                cannonIndicator.transform.parent = cannonEnemies.cannon.transform;
            }
            
            // Setup required enemies for all critter unlocks
            // foreach (var critterEnemies in critterUnlocks)
            // {
            //     List<GameObject> enemies = critterEnemies.requiredEnemies;
            //     CollectableObject collectableObject = critterEnemies.critter.GetComponent<CollectableObject>();
            //     if (!CollectableManager.Instance.CheckIsSaved(collectableObject.colletctableIndex))
            //     {
            //         if (enemies.Count > 0)
            //         {
            //             collectableObject.canBeSaved = false;
            //         }
            //     
            //         critterEnemyDict.Add(critterEnemies.critter, new List<Enemy.Enemy>());
            //         foreach (var enemy in enemies)
            //         {
            //             ApplyEnemyIndicator(enemy);
            //             critterEnemyDict[critterEnemies.critter].Add(enemy.GetComponent<Enemy.Enemy>());
            //         }
            //     }
            // }
            
            // Setup required enemies for all plant unlocks
            foreach (var plantEnemies in plantUnlocks)
            {
                List<GameObject> enemies = plantEnemies.requiredEnemies;
                CollectableObject collectableObject = plantEnemies.plant.GetComponent<CollectableObject>();
                if (!CollectableManager.Instance.CheckIsSaved(collectableObject.colletctableIndex))
                {
                    if (enemies.Count > 0)
                    {
                        collectableObject.isCollectable = false;
                    }
                
                    plantEnemyDict.Add(plantEnemies.plant, new List<Enemy.Enemy>());
                    foreach (var enemy in enemies)
                    {
                        ApplyEnemyIndicator(enemy);
                        plantEnemyDict[plantEnemies.plant].Add(enemy.GetComponent<Enemy.Enemy>());
                    }
                }
            }

        }

        private void ApplyEnemyIndicator(GameObject enemy)
        {
            // Locate body to apply the indicator
            Transform parentTransform = enemy.transform.Find("Body");
            if (!parentTransform)
            {
                parentTransform = enemy.transform.Find("AgentMoveable/Body");
                if (!parentTransform)
                {
                    parentTransform = enemy.transform;
                }
            }
            GameObject indicator = Instantiate(cannonEnemyIndicator, parentTransform.position, Quaternion.identity, enemy.transform);
            indicator.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
            indicator.transform.parent = parentTransform;
        }

        private void Update() // Very inefficient, needs to be changed later
        {
            CheckEnemiesRespawns();
            CheckEnemiesForWin();
            CheckEnemiesForUnlocks();
        }

        /*
         * TODO
         * These functions needs to be refactored
         * 
         */
        private void CheckEnemiesForWin()
        {
            int inactiveCnt = 0;
            foreach (var enemy in enemiesForWinCondition)
            {
                if (!enemy.gameObject.activeInHierarchy)
                {
                    inactiveCnt++;
                }
            }
            
            if (enemiesForWinCondition.Count == inactiveCnt)
            {
                // Activate something
                TriggerWin();
            }
        }

        private void CheckEnemiesForUnlocks()
        {
            foreach (var cannonEnemies in cannonEnemyDict)
            {
                CheckForEnemies(cannonEnemies);
            }
            
            // foreach (var critterEnemies in critterEnemyDict)
            // {
            //     CheckForEnemies(critterEnemies);
            // }

            foreach (var plantEnemies in plantEnemyDict)
            {
                CheckForEnemies(plantEnemies);
            }
        }

        private void CheckForEnemies(KeyValuePair<GameObject, List<Enemy.Enemy>> pair)
        {
            List<Enemy.Enemy> enemies = pair.Value;
            int inactiveCnt = 0;
            foreach (var enemy in enemies)
            {
                if (!enemy.gameObject.activeInHierarchy)
                {
                    inactiveCnt++;
                }
            }
            if (pair.Value.Count == inactiveCnt)
            {
                UnlockObjects(pair.Key);
            }
        }
        
        private void CheckEnemiesRespawns()
        {
            // TODO make this better
            
            foreach (var cannonEnemies in cannonUnlocks)
            {
                List<GameObject> enemies = cannonEnemies.requiredEnemies;
                GameObject cannonObj = cannonEnemies.cannon;
                if (cannonObj.transform.childCount <= 1)
                {
                    foreach (var enemy in enemies)
                    {
                        if (enemy.gameObject.activeInHierarchy)
                        {
                            // Debug.Log("Reset cannon");
                            LockObjects(cannonObj);
                            break;
                        }
                    }
                }
            }
            
            // foreach (var critterEnemies in critterEnemyDict)
            // {
            //     List<Enemy.Enemy> enemies = critterEnemies.Value;
            //     GameObject critterObj = critterEnemies.Key;
            //     if (critterObj.GetComponent<CollectableObject>().canBeSaved)
            //     {
            //         foreach (var enemy in enemies)
            //         {
            //             if (enemy.gameObject.activeInHierarchy)
            //             {
            //                 LockObjects(critterObj);
            //                 break;
            //             }
            //         }
            //     }
            // }
            
            foreach (var plantEnemies in plantEnemyDict)
            {
                List<Enemy.Enemy> enemies = plantEnemies.Value;
                GameObject critterObj = plantEnemies.Key;
                if (critterObj.GetComponent<CollectableObject>().isCollectable)
                {
                    foreach (var enemy in enemies)
                    {
                        if (enemy.gameObject.activeInHierarchy)
                        {
                            LockObjects(critterObj);
                            break;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Function to force a trigger win 	
        /// </summary>
        private void TriggerWin()
        {
            if (!finalWinHasTriggered && winEffectedTarget != null)
            {
                winEffectedTarget.GetComponent<WinObject>().ActivateObject();
                finalWinHasTriggered = true;
            }
        }

        /// <summary>
        /// Function to unlock a cannon
        /// </summary>
        private void UnlockObjects(GameObject obj)
        {
            if (obj.CompareTag("Cannon"))
            {
                CannonTrigger cannon = obj.GetComponent<CannonTrigger>();
                cannon.isUnlocked = true;

                if (obj.transform.childCount > 1)
                {
                    Destroy(obj.transform.GetChild(1).gameObject);
                }
            }
            else if (obj.CompareTag("Collectable"))
            {
                CollectableObject collectableObject = obj.GetComponent<CollectableObject>();
                if (!collectableObject.isCollectable)
                {
                    collectableObject.isCollectable = true;
                }
            }
            
        }

        /// <summary>
        /// This is probably going to be deprecated soon...
        /// </summary>
        /// <param name="obj"></param>
        private void LockObjects(GameObject obj)
        {
            if (obj.CompareTag("Cannon"))
            {
                CannonTrigger cannon = obj.GetComponent<CannonTrigger>();
                cannon.isUnlocked = false;

                GameObject cannonIndicator = Instantiate(cannonEnemyIndicator,
                    obj.transform.position,
                    Quaternion.identity,
                    obj.transform);
                cannonIndicator.transform.localScale *= 2.0f;
                cannonIndicator.transform.SetParent(obj.transform);
            }
            else if (obj.CompareTag("Collectable"))
            {
                CollectableObject collectableObject = obj.GetComponent<CollectableObject>();
                collectableObject.isCollectable = false;
                CollectableManager.Instance.ChangeCollectableStatus(collectableObject, false, false);
            }
        }

        /// <summary>
        /// Function to force the has triggered state to change for the win condition manager  	
        /// </summary>
        /// <param name="state"> The bool you want to set hasTriggered to </param>
        public void ForceChangeHasTriggered(bool state)
        {
            finalWinHasTriggered = state;
        }
    }
}
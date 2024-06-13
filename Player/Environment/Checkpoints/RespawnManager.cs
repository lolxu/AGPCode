using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __OasisBlitz.__Scripts.Enemy.Enemies;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

namespace __OasisBlitz.__Scripts.Player.Environment.Checkpoints
{
    public class RespawnManager : MonoBehaviour
    {
        public static RespawnManager Instance;
        
        // events
        public static Action OnReset;
        public static Action OnInitialSpawn;

        private List<GameObject> listOfCurrentCheckpoints; 
        private PlayerStateMachine ctx;
        private GameObject currentCheckpoint;
        private Dictionary<String, CheckPoint> mapOfCurrentCheckpoints;
        
        [SerializeField] private LevelNames _levelNames;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        IEnumerator Start()
        {
            yield return null;
            
            mapOfCurrentCheckpoints = new Dictionary<String, CheckPoint>();
            listOfCurrentCheckpoints = new List<GameObject>();

            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();

                currentCheckpoint = null;

                GameMetadataTracker.Instance.InitializeCheckpointsForScene(SceneManager.GetActiveScene().name);
            
                InitializeCheckpoints();

                // Set player to the latest checkpoint position if there is one
                KeyValuePair<Vector3, Vector3> spawnPos = Respawn();
            
                Debug.Log("Setup Respawn Manager");
            
                if (spawnPos.Key != Vector3.zero)
                {
                    ctx.CurrentState.RespawnCharacter(spawnPos);
                    if (OnInitialSpawn != null)
                    {
                        OnInitialSpawn();
                    }
                }
                else
                {
                    Debug.LogError("No last positions");
                }
            }
        }

        /*
         *  This function initializes checkpoints to have the states stored in the metadata tracker (Pretty Unoptimized)
         */
        private void InitializeCheckpoints()
        {
            Dictionary<String, bool> allCheckpoints = GameMetadataTracker.Instance.GetAllCheckpoints(SceneManager.GetActiveScene().name);

            bool isFirstTime = !allCheckpoints.Any();

            foreach (var obj in GameObject.FindGameObjectsWithTag("RespawnPoint"))
            {
                mapOfCurrentCheckpoints.Add(obj.name, obj.GetComponent<CheckPoint>());

                // Add to the storage if it's the first time loading this scene
                if (isFirstTime)
                {
                    GameMetadataTracker.Instance.AddToCheckpoints(SceneManager.GetActiveScene().name, obj);
                }
            }

            // REFACTORED -- NO NEED TO PRESERVE CHECKPOINT STATUS
            // if (!isFirstTime)
            // {
            //     // Initialize all checkpoints materials
            //     foreach (var pair in allCheckpoints)
            //     {
            //         String chkptName = pair.Key;
            //         bool isActivated = pair.Value;
            //         if (mapOfCurrentCheckpoints.TryGetValue(chkptName, out var checkpoint))
            //         {
            //             if (isActivated)
            //             {
            //                 // Debug.Log("Activating checkpoint: " + chkptName);
            //                 checkpoint.SetActivated();
            //             }
            //         }
            //     }
            //
            //     // Deal with destructible checkpoints and initialize their states
            //     foreach (var pair in allDestructibleCheckpoints)
            //     {
            //         String chkptName = pair.Key;
            //         bool isDestroyed = pair.Value;
            //
            //         if (allDestructibleCheckpoints.ContainsKey(chkptName))
            //         {
            //             if (isDestroyed)
            //             {
            //                 mapOfCurrentCheckpoints[chkptName].gameObject.GetComponent<DestructibleCheckpoint>()
            //                     .DestroyCheckpoint(false);
            //             }
            //         }
            //     }
            // }
            
            // Sort the checkpoint list
            listOfCurrentCheckpoints = GameObject.FindGameObjectsWithTag("RespawnPoint").OrderBy(obj => obj.name).ToList();
        }

        public void SetSpawnPoint(GameObject obj)
        {
            // Debug.LogError(mapOfCurrentCheckpoints.Count);
            if (mapOfCurrentCheckpoints == null)
            {
                mapOfCurrentCheckpoints = new Dictionary<String, CheckPoint>();
                listOfCurrentCheckpoints = new List<GameObject>();
            }
            
            if (mapOfCurrentCheckpoints.Count == 0)
            {
                // Force a initialization here because sometimes weird things happen lmao
                InitializeCheckpoints();
            }
            
            if (currentCheckpoint != null)
            {
                currentCheckpoint.GetComponent<CheckPoint>().DisableCurrentCheckpoint();
            }
            
            currentCheckpoint = obj;
            GameMetadataTracker.Instance.SetLastLocationInLevel(SceneManager.GetActiveScene().name, obj);
            // Debug.Log("Spawn Point Updated!");
            currentCheckpoint.GetComponent<CheckPoint>().EnableCurrentCheckpoint();
        }
        
        // This is deprecated
        public Vector3 GetSpawnPoint()
        {
            Debug.LogError(mapOfCurrentCheckpoints.Count);
            if (currentCheckpoint != null)
            {
                return currentCheckpoint.transform.position;
            }

            return Vector3.zero;
        }

        public void TeleportToNextCheckpoint(GameObject chkpt)
        {
            for(int i = 0; i < listOfCurrentCheckpoints.Count; i++)
            {
                if (listOfCurrentCheckpoints[i] == chkpt)
                {
                    if (i + 1 < listOfCurrentCheckpoints.Count)
                    {
                        ctx.CharacterController.SetPosition(listOfCurrentCheckpoints[i + 1].transform.position);
                    } // otherwise do nothing
                    break;
                }
            }
        }
        
        public void TeleportToPreviousCheckpoint(GameObject chkpt)
        {
            for(int i = 0; i < listOfCurrentCheckpoints.Count; i++)
            {
                if (listOfCurrentCheckpoints[i] == chkpt)
                {
                    if (i - 1 >= 0)
                    {
                        ctx.CharacterController.SetPosition(listOfCurrentCheckpoints[i - 1].transform.position);
                    } // otherwise do nothing
                    break;
                }
            }
        }

        public void TeleportToCheckpoint(CheckPoint checkpoint)
        {
            // ctx.CharacterController.SetPosition(checkpoint.gameObject.transform.position);
            
            var chkptTransform = checkpoint.gameObject.transform;
            Vector3 lastCheckpointPos = chkptTransform.position;
            Vector3 lastCheckpointForward = chkptTransform.forward;
            var spawnPos = new KeyValuePair<Vector3, Vector3>(lastCheckpointPos, lastCheckpointForward);
            ctx.CurrentState.RespawnCharacter(spawnPos);
        }

        private GameObject FindLatestAvailableCheckpoint()
        {
            ctx.IsDead = false; // Revert death state no matter what

            string lastCheckpointName = GameMetadataTracker.Instance.GetLastLocationInLevel(SceneManager.GetActiveScene().name);
            
            // This will be used when in the same level or transitioning between level
            if (lastCheckpointName != "noCheckpoint")
            {
                foreach (var chkpt in mapOfCurrentCheckpoints)
                {
                    // Debug.LogError(chkpt);
                    if (lastCheckpointName == chkpt.Key)
                    {
                        currentCheckpoint = chkpt.Value.gameObject;
                        currentCheckpoint.GetComponent<CheckPoint>().EnableCurrentCheckpoint();
                        // Debug.LogError(currentCheckpoint.name);
                        return currentCheckpoint;
                    }
                    
                }
            }
            Debug.Log("Didn't find latest checkpoint");
            // Respawning at start point

            return GameObject.FindGameObjectWithTag("StartPoint");
        }
        
        public KeyValuePair<Vector3, Vector3> Respawn()
        {
            // NEW RESPAWN STUFF - Based on collision and game data
            // Debug.LogError(mapOfCurrentCheckpoints.Count);
            OnReset.Invoke();
            
            GameObject latestCheckpoint = FindLatestAvailableCheckpoint();
            
            // Debug.Log(latestCheckpoint);
            if (latestCheckpoint != null)
            {
                var chkptTransform = latestCheckpoint.gameObject.transform;
                Vector3 lastCheckpointPos = chkptTransform.position;
                Vector3 lastCheckpointForward = chkptTransform.forward;

                // Debug.Log(lastCheckpointPos);

                // Debug.Log("Going to return values");
                return new KeyValuePair<Vector3, Vector3>(lastCheckpointPos, lastCheckpointForward);
            }
            
            Debug.Log("No Checkpoints for respawn"); 

            return new KeyValuePair<Vector3, Vector3>(Vector3.zero, Vector3.right);
        }
    }
}
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using __OasisBlitz.__Scripts.Player.Environment.Fruits;

namespace __OasisBlitz.__Scripts.Player.Environment.Checkpoints
{
    public class GameMetadataTracker : MonoBehaviour
    {
        public static GameMetadataTracker Instance;
        
        // Checkpoint Tracker Data
        private Dictionary<string, Dictionary<String, bool>> _allCheckpointMetaData = new Dictionary<string, Dictionary<string, bool>>();
        private Dictionary<string, Dictionary<String, bool>> _destructibleCheckpointsMetaData = new Dictionary<string, Dictionary<string, bool>>();
        private Dictionary<string, List<string>> _lastLocationInLevel = new Dictionary<string, List<string>>();
        private List<string> _allSceneNames = new List<string>();
        private string _previousSceneName = "";
        private bool _startPointActivated = false;

        // Fruit Tracker Data - Deprecated
        // private String _currentActivatedFruit = "nothing";
        // private int _nonPersistentFruitCount = 0;
        // private Dictionary<string, bool> _fruitCollectStatus = new Dictionary<string, bool>();

        // Vitalizer Tracker Data
        private int _vitalizerCount = 0;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        /*
         *
         * Checkpoint Section
         * 
         */
        public void InitializeCheckpointsForScene(String sceneName)
        {
            if (!_allCheckpointMetaData.ContainsKey(sceneName)) // Only add new checkpoints if the scene's data is not set
            {
                _allCheckpointMetaData.Add(sceneName, new Dictionary<String, bool>());
                _destructibleCheckpointsMetaData.Add(sceneName, new Dictionary<String, bool>());
                _lastLocationInLevel.Add(sceneName, new List<string>());
                _allSceneNames.Add(sceneName);
                _startPointActivated = false;
            }
        }

        public Dictionary<String, bool> GetAllCheckpoints(String sceneName)
        {
            if (_allCheckpointMetaData.TryGetValue(sceneName, out var checkpoints))
            {
                return checkpoints;
            }
            return new Dictionary<string, bool>();
        }

        // REMOVED -- No More Destructible Checkpoints
        // public Dictionary<String, bool> GetAllDestructibleCheckpoints(String sceneName)
        // {
        //     return _destructibleCheckpointsMetaData[sceneName];
        // }

        public string GetLastLocationInLevel(String sceneName)
        {
            // Debug.Log(sceneName);
            if (_lastLocationInLevel.ContainsKey(sceneName))
            {
                if (_lastLocationInLevel[sceneName].Any())
                {
                    // Debug.LogError(_lastLocationInLevel[sceneName][_lastLocationInLevel[sceneName].Count - 1]);
                    return _lastLocationInLevel[sceneName][_lastLocationInLevel[sceneName].Count - 1];
                }
            }
            return "noCheckpoint";
        }

        public void SetLastLocationInLevel(String sceneName, GameObject checkpoint)
        {
            String chkptName = checkpoint.name;
            
            if (_lastLocationInLevel.TryGetValue(sceneName, out var value))
            {
                value.Add(chkptName);
            }

            if (_allCheckpointMetaData[sceneName].ContainsKey(chkptName))
            {
                _allCheckpointMetaData[sceneName][chkptName] = true;
            }
        }

        public void ResetAllCheckpointForLevel(String sceneName)
        {
            // if (_allCheckpointMetaData.TryGetValue(sceneName, out var value))
            // {
            //     value.Clear();
            //     _lastLocationInLevel[sceneName].Clear();
            //     _destructibleCheckpointsMetaData[sceneName].Clear();
            // }
            _lastLocationInLevel.Clear();
            _destructibleCheckpointsMetaData.Clear();
            _allCheckpointMetaData.Clear();
        }
        
        public void AddToCheckpoints(String sceneName, GameObject checkpoint)
        {
            // Debug.Log("Adding checkpoint");
            String chkptName = checkpoint.name;
            if (checkpoint.GetComponent<DestructibleCheckpoint>())
            {
                bool foundDestroyed = _destructibleCheckpointsMetaData[sceneName].ContainsKey(chkptName);

                if (!foundDestroyed)
                {
                    _destructibleCheckpointsMetaData[sceneName].Add(chkptName, checkpoint.GetComponent<DestructibleCheckpoint>().CheckIsDestroyed());
                }
            }

            bool foundAll = _allCheckpointMetaData[sceneName].ContainsKey(chkptName);
            if (!foundAll)
            {
                _allCheckpointMetaData[sceneName].Add(chkptName, checkpoint.GetComponent<CheckPoint>().isActivated);
            }
        }

        public void SetPreviousSceneName(String sceneName)
        {
            _previousSceneName = sceneName;
        }

        public String GetPreviousSceneName()
        {
            return _previousSceneName;
        }

        /*
         * REMOVED -- A function for updating destructible checkpoints
         */
        // public void ModifyDestructibleCheckpoint(String sceneName, GameObject checkpoint, bool isDestroyed)
        // {
        //     String chkptName = checkpoint.name;
        //     if (_destructibleCheckpointsMetaData.TryGetValue(sceneName, out var checkpoints))
        //     {
        //         // _lastLocationInLevel[sceneName].Pop();
        //         if (isDestroyed)
        //         {
        //             _lastLocationInLevel[sceneName].RemoveAll(chkname => chkname == chkptName);
        //         }
        //         
        //         bool found = checkpoints.ContainsKey(chkptName);
        //         if (found)
        //         {
        //             if (isDestroyed)
        //             {
        //                 _allCheckpointMetaData[sceneName][chkptName] = false;
        //             }
        //         }
        //         
        //         if (!found)
        //         {
        //             checkpoints.Add(chkptName, isDestroyed);
        //             _allCheckpointMetaData[sceneName].Add(chkptName, false);
        //         }
        //     }
        // }

        /*
         *
         * Fruit Section - Deprecated
         * 
         */
        
        // Store the currently activated fruit
        // public void StoreActivatedFruit(String fruitName)
        // {
        //     _currentActivatedFruit = fruitName;
        // }
        //
        // public String GetActivatedFruit()
        // {
        //     return _currentActivatedFruit;
        // }
        //
        // public void ResetActivatedFruit()
        // {
        //     _currentActivatedFruit = "";
        // }
        //
        // public void SetFruitCollectStatus(String fruitName, bool status)
        // {
        //     _fruitCollectStatus[fruitName] = status;
        // }
        //
        // public bool LookupFruitCollectStatus(string fruitName)
        // {
        //     if (_fruitCollectStatus.TryGetValue(fruitName, out var status))
        //     {
        //         return status;
        //     }
        //     else
        //     {
        //         // Debug.LogError("No fruit found!");
        //         return false;
        //     }
        // }
        //
        // public Dictionary<string, bool> GetAllFruitCollectStatus()
        // {
        //     return _fruitCollectStatus;
        // }
        //
        // // This is not set in stone...
        // public void SetNonPersistentFruitCount(int amount)
        // {
        //     _nonPersistentFruitCount = amount;
        // }
        //
        // public int GetNonPersistentFruitCount()
        // {
        //     return _nonPersistentFruitCount;
        // }
        
        /*
         *
         * Vitalizer Section
         * 
         */
        
        public void StoreVitalizerCount(int amount)
        {
            _vitalizerCount = amount;
        }

        public int GetVitalizerCount()
        {
            return _vitalizerCount;
        }
    }
}
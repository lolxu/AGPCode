using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.__Scripts.Player
{
    public class DontDestroySceneEssentials : MonoBehaviour
    {

        public static DontDestroySceneEssentials Instance = null;

        private PlayerStateMachine ctx;
    
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        }

        IEnumerator Start()
        {
            yield return null;
            String sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != "Burrow")
            {
                if (GameMetadataTracker.Instance.GetAllCheckpoints(sceneName).Count == 0)
                {
                    ctx.CharacterController.SetPosition(GameObject.FindGameObjectWithTag("StartPoint").transform.position);
                }
                // Otherwise, the respawn manager should handle this
                // Debug.Log("Respawn should manage");
            }
            else
            {
                ctx.CharacterController.SetPosition(GameObject.FindGameObjectWithTag("StartPoint").transform.position);
            }
        }
    }
}


using System;
using System.Collections;
using System.Globalization;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Utility;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace __OasisBlitz.__Scripts.Player.Environment.Checkpoints
{
    public class PermanentCheckpoint : CheckPoint
    {

        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject collidePrompt;
        [SerializeField] private GameObject burrowPrompt;
        [SerializeField] private GameObject KeyboardHotkey;
        [SerializeField] private GameObject ControllerHotkeys;
        private string hotkeyType;
        
        

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!isActivated)
                {
                    // Debug.Log("Collide checkpoints");
                    isActivated = true;
                    SetActivated();
                    collidePrompt.SetActive(false);
                    burrowPrompt.SetActive(true);
                }
                else
                {
                    // When reloading into the level from burrow, should reset the prompts
                    if(collidePrompt.activeInHierarchy)
                    {
                        collidePrompt.SetActive(false);
                        burrowPrompt.SetActive(true);
                    }
                    if(myCurrentCheckpointHint.IsActive() && !burrowPrompt.activeInHierarchy)
                    {
                        burrowPrompt.SetActive(true);
                    }
                }
                if (!myCurrentCheckpointHint.enabled)
                {
                    // FeelEnvironmentalManager.Instance.checkpointFeedback.PlayFeedbacks(other.gameObject.transform.position);
                }
                
                RespawnManager.Instance.SetSpawnPoint(gameObject);
                InLevelMetrics.Instance?.LogEvent(this, MetricAction.ActivateCheckpoint);

                /* We are not drilling into checkpoints anymore
                if (ctx.Drilling)
                {
                    GameMetadataTracker.Instance.SetPreviousSceneName(SceneManager.GetActiveScene().name);
                    LevelManager.Instance.LoadBurrowAsync();
                }
                */
            }
        }

        private void Update() 
        {
            
            // myCurrentCheckpointHint.transform.LookAt(2 * gameObject.transform.position - myCameraTransform.position);
            // myHint.transform.LookAt(2 * gameObject.transform.position - myCameraTransform.position);
            // myHint.transform.rotation = Quaternion.Euler(0.0f, myHint.transform.rotation.eulerAngles.y, 0.0f);
            // myCurrentCheckpointHint.transform.rotation = Quaternion.Euler(0.0f, myHint.transform.rotation.eulerAngles.y, 0.0f);
            //
            // canvas.transform.LookAt(2 * gameObject.transform.position - myCameraTransform.position);
            // canvas.transform.rotation = Quaternion.Euler(0.0f, canvas.transform.rotation.eulerAngles.y, 0.0f);
            //
            // if (hotkeyType != GlobalSettings.Instance.displayedController)
            // {
            //     hotkeyType = GlobalSettings.Instance.displayedController;
            //     switch (hotkeyType)
            //     {
            //         case "KEYBOARD":
            //             KeyboardHotkey.SetActive(true);
            //             ControllerHotkeys.SetActive(false);
            //             break;
            //         case "XBOX":
            //         case "PLAYSTATION":
            //         case "OTHER":
            //             KeyboardHotkey.SetActive(false);
            //             ControllerHotkeys.SetActive(true);
            //             break;
            //
            //     }
            // }
            //
            // if (Vector3.Distance(gameObject.transform.position, ctx.gameObject.transform.position) < 3.5f)
            // {
            //     
            //     if (DebugCommandsManager.Instance.GetDebugMode())
            //     {
            //         if (Input.GetKey(KeyCode.E) && isActivated) // Teleport to Next chkpt
            //         {
            //             pressedTimer += Time.deltaTime;
            //             if (pressedTimer > 0.5f)
            //             {
            //                 RespawnManager.Instance.TeleportToNextCheckpoint(gameObject);
            //                 pressedTimer = 0.0f;
            //             }
            //         }
            //         if (Input.GetKey(KeyCode.Q) && isActivated) // Teleport to previous chkpt
            //         {
            //             pressedTimer += Time.deltaTime;
            //             if (pressedTimer > 0.5f)
            //             {
            //                 RespawnManager.Instance.TeleportToPreviousCheckpoint(gameObject);
            //                 pressedTimer = 0.0f;
            //             }
            //         }
            //
            //         if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Q))
            //         {
            //             pressedTimer = 0.0f;
            //         }
            //     }
            //     
            //     // Check if teleport is requested
            //     if (LevelManager.Instance.TeleportRequested)
            //     {
            //         LevelManager.Instance.LoadBurrowAsync();
            //         HUDManager.Instance.SetCanInteract(false);
            //     }
            // }
        }
    }
}
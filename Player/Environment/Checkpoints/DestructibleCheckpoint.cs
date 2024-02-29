using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.__Scripts.Player.Environment.Vitalizer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace __OasisBlitz.__Scripts.Player.Environment.Checkpoints
{
    public class DestructibleCheckpoint : CheckPoint
    {
        [SerializeField] private List<GameObject> lootList;
        [SerializeField] private int checkpointRepairCost = 5;
        private bool isDestroyed = false;

        /*
         *  Call this to reactivate this destructible checkpoint
         */
        // public void ReactivateCheckpoint()
        // {
        //     isActivated = false;
        //     isDestroyed = false;
        //     myRend.enabled = true;
        //     myCol.enabled = true;
        //     myRend.material = orgMaterial;
        //     GameMetadataTracker.Instance.ModifyDestructibleCheckpoint(SceneManager.GetActiveScene().name, gameObject, isDestroyed);
        // }

        public bool CheckIsDestroyed()
        {
            return isDestroyed;
        }

        // public void DestroyCheckpoint(bool shouldModifyData)
        // {
        //     // Disable all components
        //     isActivated = false;
        //     myRend.enabled = false;
        //     myCol.enabled = false;
        //     isDestroyed = true;
        //     DisableCurrentCheckpoint();
        //     if (shouldModifyData)
        //     {
        //         GameMetadataTracker.Instance.ModifyDestructibleCheckpoint(SceneManager.GetActiveScene().name, gameObject, isDestroyed);
        //         // Spawn Loot
        //         StartCoroutine(SpawnLoot(gameObject.transform.position));
        //     }
        // }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // Drilling into the checkpoint breaks it
                /*if (ctx.Drilling)
                {
                    DestroyCheckpoint(true);
                }
                else
                {
                    
                }*/
                
                if (!isActivated)
                {
                    // Debug.Log("Collide checkpoints");
                    isActivated = true;
                    SetActivated();
                }
                
                if (!myCurrentCheckpointHint.enabled)
                {
                    FeelEnvironmentalManager.Instance.checkpointFeedback.PlayFeedbacks(other.gameObject.transform.position);
                }
                
                RespawnManager.Instance.SetSpawnPoint(gameObject);
            }
        }
        
        private void Update()
        {
            myHint.transform.LookAt(2 * gameObject.transform.position - myCameraTransform.position);
            myCurrentCheckpointHint.transform.LookAt(2 * gameObject.transform.position - myCameraTransform.position);
            myHint.transform.rotation = Quaternion.Euler(0.0f, myHint.transform.rotation.eulerAngles.y, 0.0f);
            myCurrentCheckpointHint.transform.rotation = Quaternion.Euler(0.0f, myHint.transform.rotation.eulerAngles.y, 0.0f);
            if (Vector3.Distance(gameObject.transform.position, ctx.gameObject.transform.position) < 3.5f)
            {
                if (Input.GetKeyDown(KeyCode.X) && VitalizerManager.Instance.VitalizerPieceCount >= checkpointRepairCost) // Temporary
                {
                    VitalizerManager.Instance.ChangeVitalizerCountBy(-checkpointRepairCost);
                    GameMetadataTracker.Instance.StoreVitalizerCount(VitalizerManager.Instance.VitalizerPieceCount);
                    // ReactivateCheckpoint();
                }

                if (DebugCommandsManager.Instance.GetDebugMode())
                {
                    if (Input.GetKey(KeyCode.E) && isActivated) // Teleport to Next chkpt
                    {
                        pressedTimer += Time.deltaTime;
                        if (pressedTimer > 0.5f)
                        {
                            RespawnManager.Instance.TeleportToNextCheckpoint(gameObject);
                            pressedTimer = 0.0f;
                        }
                    }
                    if (Input.GetKey(KeyCode.Q) && isActivated) // Teleport to previous chkpt
                    {
                        pressedTimer += Time.deltaTime;
                        if (pressedTimer > 0.5f)
                        {
                            RespawnManager.Instance.TeleportToPreviousCheckpoint(gameObject);
                            pressedTimer = 0.0f;
                        }
                    }

                    if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Q))
                    {
                        pressedTimer = 0.0f;
                    }
                }
                
            }
        }
        
        IEnumerator SpawnLoot(Vector3 spawnPos)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject obj in lootList)
            {
                GameObject instantiatedObj = Instantiate(obj, spawnPos, obj.transform.rotation);
                if (instantiatedObj.GetComponent<Rigidbody>())
                {
                    instantiatedObj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(1.0f, 2.0f), Random.Range(1.0f, 2.0f), Random.Range(1.0f, 2.0f)) );
                }

                // TODO make this to use object pooling later if there will be performance issues
                IPooledObject pooledObject = instantiatedObj.GetComponent<IPooledObject>();
                if (pooledObject != null)
                {
                    pooledObject.OnObjectAllocate();
                }
            }
        }
    }
}
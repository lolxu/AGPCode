using System;
using System.Collections;
using System.Numerics;
using System.Xml.Xsl;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player;
using __OasisBlitz.Player.Environment.Cannon;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace __OasisBlitz.__Scripts.Collectables
{
    public class Plant : CollectableObject
    {
        public bool isPlaced { get; set; } = false;
        [SerializeField] private MeshRenderer _renderer;
        public GameObject plantVisualPrefab;
        
        private bool isInBurrow;
        private bool isCollected = false;

        public Action PlantCollected;

        private void Awake()
        {
             isInBurrow = SceneManager.GetActiveScene().name.Contains("Burrow");    
        }
        
        // [SerializeField] private bool transitionLevelPlant = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) // && !SceneManager.GetActiveScene().name.Contains("Burrow"))
            {
                UIManager.Instance.canPauseGame = false;
                if (isInBurrow)
                {
                    // Do nothing
                }
                else
                {
                    if (!isCollected)
                    {
                        if (PlantCollected != null)
                        {
                            PlantCollected();
                        }
                        
                        //Play Audio
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.flowerCollected, transform.position);
                        
                        // Stop Time
                        UIManager.Instance.StopTime();


                        //Freeze Character movement and velocity
                        GameObject player = GameObject.FindWithTag("Player");
                        PlayerInput playerInput = player.GetComponent<PlayerInput>();
                        playerInput.EnableUIControls();

                        PlayerStateMachine psm = player.GetComponent<PlayerStateMachine>();
                        psm.PlayerPhysics.SetVelocity(Vector3.zero);

                        FeelEnvironmentalManager.Instance.PlayPlantCollectFeedback(transform.position, 1.25f);
                        CollectSequence();

                        InLevelMetrics.Instance?.LogEvent(MetricAction.GetPlant);
                        isCollected = true;
                    }
                }
            }
        }
        
        protected override void OnStart()
        {
            // transform.DOLocalRotate(new Vector3(0.0f, 360.0f, 0.0f), 1.5f, RotateMode.FastBeyond360)
            //     .SetEase(Ease.InOutSine)
            //     .SetLoops(-1, LoopType.Yoyo);
            
            // Checking for placed or unplaced
            if (isInBurrow)
            {
                if (isPlaced)
                {
                    // Spawn the extra plants for this plant
                    BurrowManager.Instance.burrowPlantManager.SpawnExtraPlantsForPlant(colletctableIndex, false);
                }
                else
                {
                    // scale transform down so it can pop up when Bandit emerges from the ground
                    // transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
                    transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
                    transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
                    BurrowManager.Instance.burrowPlantManager.SetUnplacedPlant(this);
                }
            }
        }

        protected override void OnCollected()
        {
            GameMetadataTracker.Instance.ResetAllCheckpointForLevel(SceneManager.GetActiveScene().name);

            // CollectableManager.Instance.OnPlantCollected.Invoke();

            //Debug.Log("Collected");
            
            // Play Collect animation stuff first
            CameraStateMachine.Instance.SwitchToCinematicsCamera(CinematicsType.PlantPan);
            Instantiate(plantVisualPrefab, transform.position, Quaternion.identity);
        }

        protected override void OnInteract()
        {
            FeelEnvironmentalManager.Instance.PlayPlantCollectFeedback(transform.position, 1.5f);
        }

        protected override void OnPlaced()
        {
            // Placing the plant by interacting
            if (!isPlaced)
            {
                CollectableManager.Instance.ChangeCollectableStatus(this, true, true);
                isPlaced = true;
                
                // StartCoroutine(BurrowManager.Instance.ActivateBurrowCinematicsCamera(gameObject, "Cannon", colletctableIndex));
                transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBounce);
                transform.DOMoveY(transform.position.y + 0.1f, 0.2f).SetEase(Ease.OutBounce);
                BurrowManager.Instance.burrowPlantManager.SpawnExtraPlantsForPlant(colletctableIndex, true);
            }
        }
    }
}
using System.Collections;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.Environment.Cannon;
using __OasisBlitz.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.__Scripts.Collectables
{
    public class Plant : CollectableObject
    {
        public bool isPlaced { get; set; } = false;
        [SerializeField] private GameObject Arrow;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Material unplacedMaterial;
        [SerializeField] private Material placedMaterial;

        [SerializeField] private bool transitionLevelPlant = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) // && !SceneManager.GetActiveScene().name.Contains("Burrow"))
            {
                if (SceneManager.GetActiveScene().name.Contains("Burrow"))
                {
                    StartInteractSequence();
                }
                else
                {
                    FeelEnvironmentalManager.Instance.PlayPlantCollectFeedback(transform.position, 1.25f);
                    CollectSequence();
                }
                
            }
        }
        
        protected override void OnStart()
        {
            // transform.DOLocalRotate(new Vector3(0.0f, 360.0f, 0.0f), 1.5f, RotateMode.FastBeyond360)
            //     .SetEase(Ease.InOutSine)
            //     .SetLoops(-1, LoopType.Yoyo);
            
            // Checking for placed or unplaced
            if (SceneManager.GetActiveScene().name.Contains("Burrow"))
            {
                if (isPlaced)
                {
                    _renderer.material = placedMaterial;
                    Arrow.SetActive(false);
                }
                else
                {
                    _renderer.material = unplacedMaterial;
                    // Plant cinematics
                    // StartCoroutine(BurrowManager.Instance.ActivateBurrowCinematicsCamera(gameObject, "Plant"));
                    Arrow.SetActive(true);
                }
            }
        }

        protected override void OnCollected()
        {
            GameMetadataTracker.Instance.ResetAllCheckpointForLevel(SceneManager.GetActiveScene().name);

            // CollectableManager.Instance.OnPlantCollected.Invoke();
            if (!transitionLevelPlant)
            {
                Debug.Log("Collected");

                //Play Audio
                AudioManager.instance.PlayOneShot(FMODEvents.instance.flowerCollected);
            
                // Stop Time
                UIManager.Instance.StopTime();

                // Play Collect animation stuff first
                CameraStateMachine.Instance.SwitchToCinematicsCamera(1);
            }
            else
            {
                LevelManager.Instance.LoadAnySceneAsync("Level 3 - Temple - P2");
            }
            
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
                Arrow.SetActive(false);
                _renderer.material = placedMaterial;
                CollectableManager.Instance.ChangeCollectableStatus(this, true, true);
                isPlaced = true;
                
                StartCoroutine(BurrowManager.Instance.ActivateBurrowCinematicsCamera(gameObject, "Cannon", colletctableIndex));
            }
        }
    }
}
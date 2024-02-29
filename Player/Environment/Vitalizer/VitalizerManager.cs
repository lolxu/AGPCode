using System;
using System.Collections;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.__Scripts.Player.Environment.Vitalizer
{
    public class VitalizerManager : MonoBehaviour
    {
        public int VitalizerPieceCount { set; get; } = 0;
        public static VitalizerManager Instance;

        private PlayerStateMachine ctx;

        [SerializeField] private VitalizerCountHUD vcHUD;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private IEnumerator Start()
        {
            yield return null;
            
            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
            vcHUD = GameObject.Find("HUDCanvas").GetComponent<VitalizerCountHUD>();

            VitalizerPieceCount = GameMetadataTracker.Instance.GetVitalizerCount();
            // Debug.Log("Number of vitalizers: " + VitalizerPieceCount);
        }

        public void ChangeVitalizerCountBy(int amount)
        {
            VitalizerPieceCount += amount;
            vcHUD.UpdateCount(VitalizerPieceCount);
        }

        public void NearVitalizerCollision(GameObject vitObj, bool shouldDestroy)
        {
            VitalizerPieceCount += 1;
            vcHUD.UpdateCount(VitalizerPieceCount);
            // Debug.Log("Vitalizer Piece: " + VitalizerPieceCount);
            vitObj.transform.DOKill();
            
            FeelEnvironmentalManager.Instance.PlayPlantCollectFeedback(vitObj.transform.position, 1.0f);
            ctx.PlayerAudio.PlayVitalizerCollect();
            
            GameMetadataTracker.Instance.StoreVitalizerCount(VitalizerPieceCount);
            ObjectPooler.Instance.Deallocate("Vitalizer", vitObj);
            if (shouldDestroy)
            {
                Destroy(vitObj);
            }
        }
    }
}
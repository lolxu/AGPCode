using System;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace __OasisBlitz.__Scripts.Player.Environment.Checkpoints
{
    public class CheckPoint : MonoBehaviour
    {

        [SerializeField] protected TextMeshPro myHint;
        [SerializeField] protected bool toggleHint;
        [SerializeField] protected Material activatedMaterial;
        [SerializeField] protected TextMeshPro myCurrentCheckpointHint;
        [SerializeField] protected FlagAnimation _flag;
        public int musicIntensity;
        public bool isActivated { get; set; } = false;
        protected PlayerStateMachine ctx;
        protected Transform myCameraTransform;
        protected MeshRenderer myRend;
        protected Collider myCol;
        protected float pressedTimer;
        protected Material orgMaterial;

        private void Awake()
        {
            myRend = GetComponent<MeshRenderer>();
            myCol = GetComponent<SphereCollider>();

            // transform.localScale = Vector3.one;
        }
        
        private void Start()
        {
            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
            myCameraTransform = GameObject.FindGameObjectWithTag("PlayerCamera").transform;
            
            if (myCurrentCheckpointHint)
            {
                myCurrentCheckpointHint.enabled = false;
            }
            
            orgMaterial = myRend.material;

            if (!toggleHint)
            {
                myHint.renderer.enabled = false;
            }
        }
        
        public void SetActivated()
        {
            isActivated = true;

            // TODO: Enable this for dynamic audio
            // AudioManager.instance.musicCheckPoint = musicIntensity;
            // AudioManager.instance.PlayOneShot(FMODEvents.instance.checkPointFlag, transform.position);
            myRend.material = activatedMaterial;
            // ctx.DrillixirManager.FullRefillDrillixir();
            // _flag.gameObject.SetActive(true);
            // _flag.SetFlagActive(true);
        }

        public void EnableCurrentCheckpoint()
        {
            myCurrentCheckpointHint.enabled = true;
        }

        public void DisableCurrentCheckpoint()
        {
            myCurrentCheckpointHint.enabled = false;
        }

        /// Editor Gizmo Draw Respawn Direction
        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 2.5f, Color.green);
        }
    }
}
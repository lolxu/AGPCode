using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.Enemy.StateMachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace __OasisBlitz.__Scripts.Enemy.Enemies.Flashing
{
    [RequireComponent(typeof(EnemyFacePlayerBehavior))]
    public class HardeningEnemy : EnemyStateMachine
    {
        [SerializeField] private Material _whiteMaterial;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;

        [Header("Hardening Enemy Specific Settings")] 
        [SerializeField] private float _flashTime = 0.0f;
        [SerializeField] private Material _impenetrableMaterial;
        [SerializeField] private bool canHaveDetection = false;
        [SerializeField] private List<GameObject> bodyParts;

        public float bounceMagnitude = 35.0f;

        private Color _orgMaterialColor;
        // private Color _impenetrableMaterialColor;

        private bool detectedPlayer = false;
        
        public bool _canBePenetrated { get; private set; } = true;
        private bool _isVulnerable = true;
        private float _timer = 0.0f;
        
        private EnemyAudio _enemyAudio;
        private Vector3 startSpawnLocation;
    
        private EnemyFacePlayerBehavior enemyFacePlayerBehavior;
        private Material _orgMaterial;

        public override void OnAwake()
        {
            base.OnAwake();
            _enemyAudio = GetComponent<EnemyAudio>();
            enemyFacePlayerBehavior = GetComponent<EnemyFacePlayerBehavior>();
            _orgMaterial = _meshRenderer.material;
            _orgMaterialColor = _orgMaterial.color;
            // _impenetrableMaterialColor = _impenetrableMaterial.color;
        }
        
        public override void SetSpawnLocation()
        {
            startSpawnLocation = transform.position;
            _orgMaterialColor = _orgMaterial.color;
            // _impenetrableMaterialColor = _impenetrableMaterial.color;
        }

        public override void ResetOnEnable()
        {
            base.ResetOnEnable();
            transform.DOKill(true);
            
            transform.position = startSpawnLocation;
            _meshRenderer.material = _orgMaterial;
            _meshRenderer.material.color = _orgMaterialColor;

            _canBePenetrated = true;
            _isVulnerable = true;
            ChangeHardeningStatus();
            
            // Debug.LogError(_meshRenderer.material.color);
            detectedPlayer = false;

            CurrTimeForWarningAndDisengagingAlert = 0.0f;
        }
        
        public override void HandleEnterDeath()
        {
            Kill();
        }
        public override void HandleDeath()
        {
        
        }

        public override void HandleEnterAlert()
        {
            // Debug.Log("Enter Alert");
            if (canHaveDetection && !detectedPlayer)
            {
                detectedPlayer = true;
                ChangePenetrableStatus();
            }
        }

        public override void HandleAlertMove()//handle enemy move, rotation, and tilt
        {
            enemyFacePlayerBehavior.FacePlayer();
        }
        
        public override void HandleEnterIdle()
        {
            // ChangePenetrableStatus();
            // Debug.Log("Enter Idle");
            if (canHaveDetection && detectedPlayer)
            {
                // Debug.Log("Penetrable");
                detectedPlayer = false;
                ChangePenetrableStatus();
            }
        }

        public override void HandleIdleLOSMove()
        {
            // Debug.Log("LOS IDLE");
            enemyFacePlayerBehavior.FacePlayer();
        }

        public override void HandleIdle()
        {
            base.HandleIdle();
            // Debug.Log("Idle");
        }

        public override void HandleEnterAttack()
        {
            // Debug.Log("Entered Attack");
        }

        public override void HandleIdleSuspiciousMove()
        {
            // Debug.Log("Suspicious Move");
        }
    
        public void Kill()
        {
            transform.DOKill(true);
            detectedPlayer = false;
            DeathFlash();
            _collider.enabled = false;
            _enemyAudio.PlayDeathSound();
        }
    
        private void DeathFlash()
        {
            _meshRenderer.material = _whiteMaterial;
            StartCoroutine(WaitThenDisable());
        }
        private IEnumerator WaitThenDisable()
        {
            yield return new WaitForSeconds(0.25f);
            _collider.enabled = true;
            _meshRenderer.material = _orgMaterial;
            _meshRenderer.material.color = _orgMaterialColor;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!canHaveDetection)
            {
                _timer += Time.deltaTime;
                if (_timer > _flashTime)
                {
                    _timer = 0.0f;
                    ChangePenetrableStatus();
                }
            }
        }

        private void ChangePenetrableStatus()
        {
            _isVulnerable = !_isVulnerable;
            
            // Makes the flashing stage to be vulnerable
            if (!_canBePenetrated)
            {
                _canBePenetrated = true;
            }
            
            if (!_isVulnerable)
            {
                _meshRenderer.material.DOColor(Color.white, 0.25f)
                    .SetLoops(3, LoopType.Yoyo)
                    .OnComplete(ChangeHardeningStatus);
            }
            else
            {
                _meshRenderer.material.DOColor(Color.white, "_BaseColor",0.25f)
                    .SetLoops(3, LoopType.Yoyo)
                    .OnComplete(ChangeHardeningStatus);
            }
        }

        private void ChangeHardeningStatus()
        {
            if (!_isVulnerable)
            {
                _canBePenetrated = false;
                _orgMaterial.color = _orgMaterialColor;
                _meshRenderer.material = _impenetrableMaterial;

                foreach (var part in bodyParts)
                {
                    var rend = part.GetComponent<MeshRenderer>();
                    rend.material = _impenetrableMaterial;
                }
                // _meshRenderer.material.color = _impenetrableMaterialColor;
            }
            else
            {
                // _impenetrableMaterial.color = _impenetrableMaterialColor;
                _meshRenderer.material = _orgMaterial;
                _meshRenderer.material.color = _orgMaterialColor;
                
                foreach (var part in bodyParts)
                {
                    var rend = part.GetComponent<MeshRenderer>();
                    rend.material = _orgMaterial;
                }
            }
        }
    }
}
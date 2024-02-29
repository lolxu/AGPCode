using System;
using System.Collections;
using System.Security.Cryptography;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.Enemy.StateMachine;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Enemy.Enemies.Hammer
{
    [RequireComponent(typeof(EnemyFacePlayerBehavior))]
    public class Hammer : EnemyStateMachine
    {
        [SerializeField] private Material _whiteMaterial;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;

        [Header("Hammer Enemy Specific Settings")] 
        [SerializeField] private float _hammerDownTime = 0.0f;
        [SerializeField] private float _hammerTimeout = 0.15f;
        [SerializeField] private float _hammerDamageRadius = 1.5f;
        [SerializeField] private float _hammerAOEDisplacement = 5.0f;
        [SerializeField] private GameObject _hammerAOEObject;

        private GameObject aoeEffect;
        private bool _hammerDone = true;
        private bool _startedAttack = false;

        public bool _canBePenetrated { get; private set; } = true;
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
        }
        
        public override void SetSpawnLocation()
        {
            startSpawnLocation = transform.position;
        }

        public override void ResetOnEnable()
        {
            transform.position = startSpawnLocation;
            CurrTimeForWarningAndDisengagingAlert = 0.0f;

            _hammerDone = true;
            _startedAttack = false;
            
            _meshRenderer.material = _orgMaterial;
            
            if (aoeEffect != null)
            {
                Destroy(aoeEffect);
            }
        }
        
        public override void HandleEnterDeath()
        {
            Kill();
        }
        public override void HandleDeath()
        {
        
        }

        public override void HandleAlert()
        {
            
        }

        public override void HandleAlertMove()//handle enemy move, rotation, and tilt
        {
            enemyFacePlayerBehavior.FacePlayer();
        }

        private void CastDamageSphere()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position - new Vector3(0.0f, _hammerAOEDisplacement - 1.5f, 0.0f), _hammerDamageRadius);
            foreach (var col in cols)
            {
                if (col.CompareTag("Player"))
                {
                    PlayerStateMachine ctx = col.gameObject.GetComponent<PlayerStateMachine>();
                    ctx.InstantKill();
                }
            }
        }

        IEnumerator WaitToContinueHammering()
        {
            yield return new WaitForSeconds(_hammerTimeout);
            _startedAttack = false;
            if (aoeEffect != null)
            {
                Destroy(aoeEffect);
            }
            transform.DOMoveY(startSpawnLocation.y, _hammerDownTime).OnComplete(() =>
            {
                _hammerDone = true;
            });
        }
        
        public override void HandleIdleMove()
        {
        }

        public override void HandleIdleLOSMove()
        {
            enemyFacePlayerBehavior.FacePlayer();
        }

        public override void HandleEnterAttack()
        {
            
        }

        public override void HandleIdleSuspiciousMove()
        {
            
        }
    
        public void Kill()
        {
            if (aoeEffect != null)
            {
                Destroy(aoeEffect);
            }
            transform.DOKill();
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
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position - new Vector3(0.0f, _hammerAOEDisplacement - 1.5f, 0.0f), _hammerDamageRadius);
        }

        private void Update()
        {
            // Hammer Time
            if (_hammerDone)
            {
                _hammerDone = false;
                transform.DOMoveY(0.0f, _hammerDownTime)
                    .OnComplete(() =>
                    {
                        if (aoeEffect == null)
                        {
                            aoeEffect = Instantiate(_hammerAOEObject,
                                transform.position - new Vector3(0.0f, _hammerAOEDisplacement - 1.5f, 0.0f),
                                Quaternion.identity);
                            aoeEffect.transform.SetParent(transform);
                            aoeEffect.transform.localScale *= 20.0f;
                        }
                        
                        _startedAttack = true;

                        StartCoroutine(WaitToContinueHammering());
                    });
            }

            if (_startedAttack && _collider.enabled)
            {
                CastDamageSphere();
            }
        }
    }
}
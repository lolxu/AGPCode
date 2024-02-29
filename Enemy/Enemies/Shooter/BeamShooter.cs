using System.Collections;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.Enemy.StateMachine;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Enemy.Enemies.Shooter
{
    public class BeamShooter : EnemyStateMachine
    {
        [SerializeField] private Material _whiteMaterial;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;
        

        //shooting
        [SerializeField] private Laser _laser;
        [SerializeField] private Transform _laserStart;
        [SerializeField] private float _laserLingerTime = 0.25f;
        [SerializeField] private float _laserAttackTimeout = 1.0f;
        [SerializeField] private float _laserContinueAttackDuration = 30.0f;
        [SerializeField] private float _attackTelegraphDuration = 0.1f;
        private bool canShoot = true;
        private bool isAttacking = false;
        
        //Audio
        private EnemyAudio _enemyAudio;
        private Vector3 spawnLocation;

        private EnemyFacePlayerBehavior enemyFacePlayerBehavior;
        private Material _orgMaterial;

        public override void OnAwake()
        {
            base.OnAwake();
            _enemyAudio = GetComponent<EnemyAudio>();
            enemyFacePlayerBehavior = GetComponent<EnemyFacePlayerBehavior>();
            // _laser.DisableBeam();
            _orgMaterial = _meshRenderer.material;
        }

        public override void SetSpawnLocation()
        {
            spawnLocation = transform.position;
        }

        public override void ResetOnEnable()
        {
            base.ResetOnEnable();
            transform.position = spawnLocation;
            _laser.Deactivate();
            StopCoroutine(ShootSequence());
            StopCoroutine(ContinueAttackTimer());
            canShoot = true;
            isAttacking = false;
            _collider.enabled = true;
            _meshRenderer.material = _orgMaterial;
        }

        public override void HandleEnterDeath()
        {
            base.HandleEnterDeath();
            Kill();
        }

        public override void HandleDeath()
        {
        }

        public override void HandleEnterAlert()
        {
            isAttacking = true;
            _laser.canFollowPlayerY = true;
        }

        public override void HandleExitAlert()
        {
            
        }

        public override void HandleAlertMove() //handle enemy move, rotation, and tilt
        {
            enemyFacePlayerBehavior.FacePlayer();
            
            if (canShoot && isAttacking)
            {
                StartCoroutine(ShootSequence());
            }
        }

        public override void HandleIdleLOSMove()
        {
            enemyFacePlayerBehavior.FacePlayer();
            if (canShoot && isAttacking)
            {
                StartCoroutine(ShootSequence());
            }
        }

        public override void HandleIdleSuspiciousMove()
        {
            if (canShoot && isAttacking)
            {
                StartCoroutine(ShootSequence());
            }
        }

        public override void HandleIdle()
        {
            _laser.EnableBeam();
            _laser.canFollowPlayerY = false;
            if (!isAttacking)
            {
                // _laser.DisableBeam();
                /*_laser.Deactivate();*/
            }
            else
            {
                if (canShoot)
                {
                    StartCoroutine(ShootSequence());
                }
            }
        }

        public override void HandleEnterIdle()
        {
            StopCoroutine(ContinueAttackTimer());
            if (gameObject.activeInHierarchy && isAttacking)
            {
                StartCoroutine(ContinueAttackTimer());
            }
        }

        /*public override void HandleAttack()
        {
            enemyFacePlayerBehavior.FacePlayer();
        }*/

        public void Kill()
        {
            DeathFlash();
            _collider.enabled = false;
            _enemyAudio.PlayDeathSound();
        }

        private void DeathFlash()
        {
            _meshRenderer.material = _whiteMaterial;
            StartCoroutine(WaitThenDestroy());
        }

        private IEnumerator WaitThenDestroy()
        {
            yield return new WaitForSeconds(0.25f);
            gameObject.SetActive(false);
        }

        private IEnumerator ShootSequence()
        {
            canShoot = false;
            transform.DOScale(transform.localScale * 1.5f, _attackTelegraphDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(2, LoopType.Yoyo);
            yield return new WaitForSeconds(_attackTelegraphDuration * 2);
            _laser.Activate();
            _enemyAudio.PlayShoot();
            yield return new WaitForSeconds(_laserLingerTime);
            _laser.Deactivate();
            yield return new WaitForSeconds(_laserAttackTimeout);
            canShoot = true;
        }

        private IEnumerator ContinueAttackTimer()
        {
            yield return new WaitForSeconds(_laserContinueAttackDuration);
            isAttacking = false;
        }
    }
}
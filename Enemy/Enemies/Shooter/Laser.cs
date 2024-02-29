using System;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace __OasisBlitz.__Scripts.Enemy.Enemies.Shooter
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private LineRenderer beam;
        [SerializeField] private GameObject beamParticles;
        [SerializeField] private Transform laserStart;
        [SerializeField] private float maxLength;

        private Transform _playerTransform;
        private Vector3 shootDirection;
        public bool canKill = false;
        public bool canFollowPlayerY = false;
        
        private void Awake()
        {
            
        }

        private void Start()
        {
            _playerTransform = GameObject.FindWithTag("Player").transform;
            beam.enabled = false;
        }

        public void EnableBeam()
        {
            beam.enabled = true;
            ParticleSystem beamPS = beamParticles.GetComponent<ParticleSystem>();
            if (beamPS.isStopped)
            {
                // Debug.Log("Start laser");
                beamPS.Play();
            }
        }

        public void DisableBeam()
        {
            beam.enabled = false;
            ParticleSystem beamPS = beamParticles.GetComponent<ParticleSystem>();
            if (beamPS.isEmitting)
            {
                // Debug.Log("Stop laser");
                beamPS.Stop( true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
        
        public void Activate()
        {
            //beam.enabled = true;
            beam.widthMultiplier = Mathf.Clamp(beam.widthMultiplier*10, 0.1f, 1.0f);
            canKill = true;
            // If we want to make laser more avoidable, we can use this
            // shootDirection = direction;
        }

        public void Deactivate()
        {
            beam.widthMultiplier = Mathf.Clamp(beam.widthMultiplier/10, 0.1f, 1.0f);
            canKill = false;
            // beam.enabled = false;
            /*beam.SetPosition(0, laserStart.position);
            beam.SetPosition(1, laserStart.position);*/
        }

        private void Update()
        {
            var startPoint = laserStart.position;
            
            shootDirection = (_playerTransform.position - startPoint).normalized;
            // Vector3 laserDirection = transform.forward;
            shootDirection.x = transform.forward.x;
            shootDirection.z = transform.forward.z;

            if (!canFollowPlayerY)
            {
                shootDirection.y = transform.forward.y;
            }

            Ray ray = new Ray(startPoint, shootDirection);
            // Use the following if we want to make laser more avoidable
            // Ray ray = new Ray(startPoint, shootDirection);
            
            bool cast = Physics.Raycast(ray, out RaycastHit hit, maxLength);
            Vector3 hitPosition = cast ? hit.point : laserStart.position + shootDirection * maxLength;

            beam.SetPosition(0, laserStart.position);
            beam.SetPosition(1, hitPosition);

            if (cast && canKill)
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    var playerStateMachine = _playerTransform.GetComponent<PlayerStateMachine>();
                    if (!playerStateMachine.IsDead)
                    {
                        playerStateMachine.InstantKill(); 
                    }
                }
            }
        }
    }
}
using System;
using System.Collections;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Enemy.old
{
    public class EnemyProjectile : MonoBehaviour
    {
        private Transform _target;
        
        private Rigidbody _rigidbody;

        [SerializeField] private float trackingIntensity;
        
        [SerializeField] private float lifeSpan;
        
        void Awake()
        {
            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _rigidbody = GetComponent<Rigidbody>();
            LifeSpan();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == Constants.PlayerLayer)
            {
                try
                {
                    other.gameObject.GetComponent<PlayerStateMachine>().InstantKill();
                }
                catch (Exception e)
                {

                }
            }
            Destroy(gameObject);
        }

        void FixedUpdate()
        {
            // Redirect rigidbody slightly towards the target 
            Vector3 targetDirection = (_target.position - transform.position).normalized;
            Vector3 newVelocity = Vector3.RotateTowards(_rigidbody.velocity, targetDirection, trackingIntensity, 0.0f);
            _rigidbody.velocity = newVelocity;
            
        }

        private IEnumerator LifeSpan()
        {
            yield return new WaitForSeconds(lifeSpan);
            Destroy(gameObject);
        }
    }
    
}
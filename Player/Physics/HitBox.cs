using __OasisBlitz.Enemy;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

namespace __OasisBlitz.Player.Physics
{
    public class HitBox : MonoBehaviour
    {
        [SerializeField] private PlayerStateMachine _playerStateMachine;
        [SerializeField] private Transform _playerTransform;
        private Vector3 _prevPlayerPos;
        private int _layerMaskNonPhysics;
        [SerializeField] private float hitBoxRadius;
        private Collider[] hitboxCheckResults;

        void Awake()
        {
            _layerMaskNonPhysics = LayerMask.GetMask("Default", "Enemy");
            hitboxCheckResults = new Collider[30];
        }

        private void FixedUpdate()
        {
            if (!_playerStateMachine.IsDead)
            {
                // CheckPhysicsIndependentCollisions();
            }
        }

        ///<summary>
        /// Check special collisions that might interfere with player health or character state independent of physics
        /// Note: any changes to player physics velocity or acceleration done here might not be accurately displayed
        /// A collision belongs here if the collision is still needed when the player is "stuck"
        /// PlayerPhysics.stuck should only be called if the player dies
        ///</summary>
        public void CheckPhysicsIndependentCollisions()
        {
            int numCollisions = UnityEngine.Physics.OverlapSphereNonAlloc(transform.position, hitBoxRadius, hitboxCheckResults, _layerMaskNonPhysics);
            
            Debug.Log("Num collisions: " + numCollisions);
            
            for (int i = 0; i < numCollisions; i++)
            {
                if (hitboxCheckResults[i].CompareTag("Enemy"))
                {
                    // Debug.Log("Enemy!");
                    // _playerStateMachine.ImpactEnemy(colliders[i].gameObject.transform.parent.GetComponent<BasicEnemyController>());
                }
                else if (hitboxCheckResults[i].CompareTag("InstantKill"))
                {
                    _playerStateMachine.InstantKill();
                }else if (hitboxCheckResults[i].CompareTag("DeathBarrier"))
                {
                    if (!_playerStateMachine.IsDead)
                    {
                        _playerStateMachine.InstantKill();
                    }
                }
            }
        }
        
    }
}
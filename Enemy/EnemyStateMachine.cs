using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UIElements;
using CharacterController = UnityEngine.CharacterController;

namespace __OasisBlitz.Enemy.StateMachine
{
    public class EnemyStateMachine : MonoBehaviour
    {
        // ************************************ CORE COMPONENTS ************************************

        public Transform playerTransform;
        private LayerMask notEnemyLayerMask;
        [SerializeField] protected List<EnemyStateFactory.EnemyStates> EnemyStatesList;
        public Rigidbody EnemyRigidBody;
        public string EnemyType;
        public DashTargetPoint DashTargetPoint;
        public PlayerStateMachine playerStateMachine;

        // ************* ASSIGNED IN INSPECTOR *******************
        public TextMeshPro stateDebuggingText;
        public TextMeshPro AlertLevelIndicator;
        public SpriteRenderer AlertLevelIndicatorBackground;
        private Color RedTransparent = new Color(1.0f, 0.0f, 0.0f, 0.0f);
        private Color Red = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        private float BackgroundFlashTime = 0.35f;

        // state variables
        private EnemyStateFactory states;

        public Vector3 LastMovedWorldDirection { get; private set; }

        public EnemyBaseState CurrentState { get; set; }

        public bool IsDead { get; set; } = false;
        
        public float MinAttackDistance = 0.0f;
        
        public float MaxAttackDistance = 5.0f;
        
        public float TimeBetweenAttack = 2.0f;
        
        public float AlertDistance = 50.0f;
        
        public float attackTime = 1.0f;

        public float currAttackTime = 0.0f;
        
        private float drillixirRefillAmount = 6.0f;

        [Range(0f, 90f)]
        public float AlertDegreesFromHorizontal;
        
        public float TimeForWarningAndDisengagingAlert = 1.0f;
        [HideInInspector] public float CurrTimeForWarningAndDisengagingAlert = 0.0f;
        //if the behaviour is the same for LOS and Alert move, set this bool
        //this cause the state to switch to alert but still have suspicious enemy after LOS breaks
        public bool LOSAndAlertEnactSameBehavior = false;

        public bool DestroyOnDeath { get; set; } = true;

        public float ViewpointVerticalAlignment = 0f;

        public Vector3 GetPlayerViewPosition()
        {
            return  EnemyRigidBody.transform.position + ViewpointVerticalAlignment * transform.up;
        }
        
        private void Awake()
        {
            //might cause issues on frames where player is dead
            playerTransform = GameObject.FindWithTag("Player").transform;
            playerStateMachine = playerTransform.GetComponent<PlayerStateMachine>();
            // setup state
            states = new EnemyStateFactory(this, EnemyStatesList);
            CurrentState = states.Idle();
            CurrentState.EnterState();
            notEnemyLayerMask = LayerMask.GetMask("Default", "Penetrable", "Player", "Water", "LargePenetrable");
            OnAwake();
            SetSpawnLocation();
        }

        public virtual void OnAwake()
        {
            
        }

        private void OnEnable()
        {
            ResetOnEnable();
        }

        private void OnDisable()
        {
            ResetOnDisable();
        }

        public void UpdateStates()
        {
            CurrentState.UpdateStates();
            //UpdateStateDebuggingText();
            if (AlertLevelIndicator.text != "")
            {
                //AlertIconFacePlayer();
            }
        }

        public virtual void ResetOnEnable()
        {
            CurrTimeForWarningAndDisengagingAlert = 0.0f;
        }

        public virtual void ResetOnDisable()
        {
            CurrentState.ExitStates();
            CurrentState = states.Idle();
            CurrentState.EnterState();
            //CurrTimeForWarningAndDisengagingAlert = 0.0f;
        }
        
        public virtual void SetSpawnLocation()
        {
            Debug.LogError("ENEMY DOES NOT IMPLEMENT SET SPAWN LOCATION - MUST ALSO IMPLEMENT RESENT ON ENABLE");
        }

        private void UpdateStateDebuggingText()
        {
            string text = CurrentState.StateName();
            stateDebuggingText.SetText(text);
        }
        
        //since OnCollisionEnter triggers for all colliders on the enemy, this will only handle collisions that are not body part dependent
        private void OnCollisionEnter(Collision other)
        {
            Collider coll = other.collider;
            Vector3 normal = Vector3.zero;
            Vector3 hitPoint = Vector3.zero;
            float divideBy = 1 / other.contacts.Length;
            foreach (ContactPoint cp in other.contacts) 
            {
                normal += cp.normal * divideBy;
                hitPoint += cp.point * divideBy;
            }

            CurrentState.HandleCollision(ref coll, normal, hitPoint);
        }

        public void HandleCollision(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            CurrentState.HandleCollision(ref coll, hitNormal, hitPoint);
        }

        public void TestKillPlayer()
        {
            if (playerStateMachine.CurrentState.StateName() != "Drill")
            {
                //Debug.Log("what state? " + playerStateMachine.CurrentState.StateName());
                playerStateMachine.InstantKill();
            }
        }

        public void KillPlayer()
        {
            playerStateMachine.InstantKill();
        }

        public bool ShouldObeyGroundSnap()
        {
            return CurrentState.ShouldObeyGroundSnap();
        }

        public void InstantKill()
        {
            if (!IsDead)
            {
                // refresh blast on killing enemy
                BounceAbility.Instance.RefreshBounce();
                
                CurrentState.ExitStates();
                CurrentState = states.Dead();
                CurrentState.EnterState();
            }
        }

        private Vector3 enemyToPlayer;
        private Vector3 enemyToPlayerOnForward;
        private Vector3 enemyToPlayerOnRight;
        private Vector3 projectionsAdded;
        public bool hasLOS()
        {
            if (!playerTransform)
            {
                Debug.Log("enemy does not have reference to player");
                return false;
            }
            enemyToPlayer = playerTransform.position - GetPlayerViewPosition();
            float distanceToPlayer = enemyToPlayer.magnitude;
            if (distanceToPlayer < AlertDistance)//are we within alert distance
            {
                //testing for LOS
                enemyToPlayerOnForward = Vector3.Dot(enemyToPlayer, transform.forward) * transform.forward;
                enemyToPlayerOnRight = Vector3.Dot(enemyToPlayer, transform.right) * transform.right;
                projectionsAdded = enemyToPlayerOnForward + enemyToPlayerOnRight;
                float angle = Mathf.Abs(Vector3.Angle(projectionsAdded, enemyToPlayer));
                if (angle > AlertDegreesFromHorizontal)
                {
                    return false;
                }
                RaycastHit hit;
                if (Physics.Raycast(GetPlayerViewPosition(), enemyToPlayer.normalized, out hit, distanceToPlayer, notEnemyLayerMask))//is there a collider in the way
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public float DistanceToPlayer()
        {
            Vector3 enemyToPlayer = playerTransform.position - GetPlayerViewPosition();
            return enemyToPlayer.magnitude;
        }

        public bool WithinAttackDistance()
        {
            float distance = DistanceToPlayer();
            if (distance <= MaxAttackDistance)
            {
                if (distance >= MinAttackDistance)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void HandleEnterDeath()
        {
            // currently two different adds, once we decide on one remove one (drillixir amount in enemy atm)
            HapticsManager.Instance.PlayKillEnemyHaptic();
        }
        
        //*****************The following Handle functions are called EVERY FRAME******************************
        public virtual void HandleDeath()
        {
            //Debug.Log("Calling Base HandleDeath");
            
        }
        public virtual void HandleAlert()
        {
            //Debug.Log("Calling Base HandleAlert");
        }
        
        public virtual void HandleExitAlert()
        {
            
        }
        
        public virtual void HandleEnterAttack()
        {
            //Debug.Log("Calling Base HandleEnterAttack");
        }
        
        public virtual void HandleAttack()
        {
           
        }

        public virtual void HandleExitAttack()
        {
           
        }
        
        //Move
        public virtual void HandleAlertMove()//handle enemy move, rotation, and tilt
        {
            //Debug.Log("Calling Base HandleAlertMove");
        }

        public virtual void HandleEnterAlert()
        {
            
        }
        
        //Always called in Idle
        public virtual void HandleIdle()
        {
            //Debug.Log("Calling Base HandleIdle");
        }

        public virtual void HandleEnterIdle()
        {
            
        }
        
        //Idle movement - no suspicion of player being near
        public virtual void HandleIdleMove()
        {
            //Debug.Log("Calling Base HandleIdleMove");
        }
        //In idle state but you have LOS on the player and are suspicious
        public virtual void HandleIdleLOSMove()
        {
            //Debug.Log("Calling Base HandleIdleLOSMove");
        }
        //In idle state but you just saw the player
        public virtual void HandleIdleSuspiciousMove()
        {
            //Debug.Log("Calling Base HandleIdleSuspiciousMove");
        }
        public void SetSuspiciousSymbol()
        {
            // AlertLevelIndicator.color = Color.yellow;
            // AlertLevelIndicator.SetText("?");
        }
        
        public void SetSuspiciousLOSSymbol()
        {
            // AlertLevelIndicator.color = Color.yellow;
            // AlertLevelIndicator.SetText("!");
        }

        public void SetIdleSymbol()
        {
            // AlertLevelIndicator.SetText("");
        }

        public void SetAlertSymbol()
        {
            // AlertLevelIndicator.color = Color.red;
            // AlertLevelIndicator.SetText("!");
            // AlertLevelIndicatorBackground.DOColor(Red, BackgroundFlashTime).SetLoops(-1, LoopType.Yoyo);
        }

        public void DisableAlertSymbolBackground()
        {
            // AlertLevelIndicatorBackground.DOKill();
            // AlertLevelIndicatorBackground.color = RedTransparent;
        }
        
        public void AlertIconFacePlayer()
        {
            // Quaternion targetRotation = Quaternion.LookRotation(playerTransform.position - GetPlayerViewPosition());
            // targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y + 180.0f, 0);
            // AlertLevelIndicator.transform.localRotation = targetRotation;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 enemyToOuterRangeUnitTop = transform.forward * (1 - AlertDegreesFromHorizontal/90f) + transform.up * (AlertDegreesFromHorizontal/90f);
            Gizmos.DrawLine(GetPlayerViewPosition(), GetPlayerViewPosition() + enemyToOuterRangeUnitTop.normalized * AlertDistance);
            //reflect across the horizontal
            Vector3 enemyToOuterRangeUnitBottom = transform.forward * (1 - AlertDegreesFromHorizontal/90f) - transform.up * (AlertDegreesFromHorizontal/90f);
            Gizmos.DrawLine(GetPlayerViewPosition(), GetPlayerViewPosition() + enemyToOuterRangeUnitBottom.normalized * AlertDistance);
            Gizmos.DrawWireSphere(GetPlayerViewPosition(), AlertDistance);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(GetPlayerViewPosition(), GetPlayerViewPosition() + enemyToPlayerOnForward);
            Gizmos.DrawLine(GetPlayerViewPosition(), GetPlayerViewPosition() + enemyToPlayerOnRight);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(GetPlayerViewPosition(), GetPlayerViewPosition() + enemyToPlayer);
            Gizmos.DrawLine(GetPlayerViewPosition(), GetPlayerViewPosition() + projectionsAdded);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(GetPlayerViewPosition() + transform.forward * MinAttackDistance, GetPlayerViewPosition() + transform.forward * MaxAttackDistance);
            //Gizmos.DrawLine(GetPlayerViewPosition() + enemyToOuterRangeUnitTop * AlertDistance, GetPlayerViewPosition() + enemyToOuterRangeUnitBottom * AlertDistance);
            // Gizmos.color = Color.blue;
            // Gizmos.DrawLine(GetPlayerViewPosition(), GetPlayerViewPosition() + transform.up * AlertDistance);
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawLine(GetPlayerViewPosition(), GetPlayerViewPosition() + transform.forward * AlertDistance);
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(GetPlayerViewPosition(), GetPlayerViewPosition() + transform.right * AlertDistance);
        }

        
    }
}

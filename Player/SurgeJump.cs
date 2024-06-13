using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using UnityEngine.Serialization;

namespace __OasisBlitz.Player
{
    public class SurgeJump : MonoBehaviour
    {
        [SerializeField] private float surgeWindow = 0.0f;
        [SerializeField] private GameObject surgeJumpParticle;
        public float surgeJumpMultiplier = 1.0f;

        [Header("Boost Hitbox Properties")]
        [SerializeField] private float hitboxRadius = 5.0f;
        [SerializeField] private float constantHitRadius = 2f;
        [SerializeField] private LayerMask blastLayerMask;
        [SerializeField] private LayerMask penetrableLayerMask;
        private Vector3 CachedExitPos;  // guaranteed to be valid by DrillBelowState
        [SerializeField] private GameObject surgeJumpHitboxParticle;
        
        public bool SurgeJumpRequested { get; set; } = false;
        public bool RequiresNewSurgeJump { get; set; } = false;

        private bool bHasInputtedJumpSubmerged = false;
        public static SurgeJump Instance;
        private PlayerStateMachine ctx;

        private float SurgeJumpTimer = -1.0f;   // if less than 0, can surge jump
        [SerializeField] private float SurgeJumpCooldown = .3f;

        public void SetCachedExitPos(Vector3 inCachedExitPos)
        {
            CachedExitPos = inCachedExitPos;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        }

        private void FixedUpdate()
        {
            // if (ctx.Drilling)
            // {
            //     OasisDebugDraw.DrawSphere(ctx.transform.position, constantHitRadius, Color.red);
            //     Collider[] hitColliders = UnityEngine.Physics.OverlapSphere(ctx.transform.position, constantHitRadius, blastLayerMask);
            //     for(Int32 i = 0; i < hitColliders.Length; i++)
            //     {
            //         Collider collider = hitColliders[i];
            //         if (collider.CompareTag("NewEnemy"))
            //         {
            //             collider.gameObject.GetComponent<HitPartOfNewEnemy>().CollideWithBody(ref collider);
            //         }
            //     }
            // }   
        }

        private void Update()
        {
            SurgeJumpTimer -= Time.deltaTime;
        }

        public void CheckForSubmergedBoost()
        {
            return;
            StartCoroutine(SubmergedJumpCheck());
        }
        public void StartSurgeJumpWindow()
        {
            return;
            StartCoroutine(SurgeJumpWindow());
        }

        /**
         * Iterates every frame while drilling and submerged,
         * if the spacebar has been pressed in the last surgeWindow seconds,
         * that bHasInputtedJumpSubmerged is true.
         *
         * Begins when submerging
         */
        IEnumerator SubmergedJumpCheck()
        {
            bHasInputtedJumpSubmerged = false;
            float timeLeft = -1.0f;
            while (ctx.IsSubmerged)
            {
                if (SurgeJumpRequested && !RequiresNewSurgeJump)
                {
                    RequiresNewSurgeJump = true;
                    bHasInputtedJumpSubmerged = true;
                    timeLeft = surgeWindow;
                }
                else
                {
                    timeLeft -= Time.deltaTime;
                }
                
                if (timeLeft < 0.0)
                {
                    bHasInputtedJumpSubmerged = false;
                }

                SurgeJumpRequested = false;

                yield return null;
            }
        }

        
        /*
         * Timer for allowing surge jump after exiting the ground.
         * initial check for buffered jump input as well.
         *
         * Begins when exiting submerging.
         */
        IEnumerator SurgeJumpWindow()
        {
            // check for buffered jump input
            if (SurgeJumpTimer <= 0 && bHasInputtedJumpSubmerged)
            {
                ActivateSurgeJump();
            }
            else
            {
                // begin timer for leniency on surge jump above ground.
                float timeLeft = surgeWindow;
                while (timeLeft >= 0.0f)
                {
                    if (SurgeJumpTimer <= 0 && SurgeJumpRequested && !RequiresNewSurgeJump)
                    {
                        RequiresNewSurgeJump = true;
                        ActivateSurgeJump();
                        break;
                    }
                    timeLeft -= Time.deltaTime;
                    SurgeJumpRequested = false;
                    yield return null;
                }
            }
        }

        // todo: this should somewhere where it can be used by other things not just surgejump.
        Vector3 GetExitNormal()
        {
            // invert velocity
            Vector3 CheckDir = -ctx.PlayerPhysics.Velocity;

            // raycast backwards into terrain
            RaycastHit penetrableHit;
            if (UnityEngine.Physics.Raycast(ctx.transform.position, CheckDir, out penetrableHit, 20.0f,
                    penetrableLayerMask))
            {
                // get normal, and return it
                return penetrableHit.normal;
            }
            
            return Vector3.up;
        }

        private void ActivateSurgeJump()
        {
            SurgeJumpTimer = SurgeJumpCooldown;
            
            Transform PlayerTransform = ctx.gameObject.transform;
            ctx.PlayerPhysics.AddForce(ctx.PlayerPhysics.Velocity.normalized * surgeJumpMultiplier);
                
            // Player Feedback support - Feel Package
            ctx.PlayerFeedbacks.playerBoostFeedback.PlayFeedbacks();

            // Surge Jump Hitbox
            // currently tied to speed increase.

            //OasisDebugDraw.DrawSphere(CachedExitPos,
            //    hitboxRadius, Color.red, 5.0f);

            GameObject hitBoxParticles = Instantiate(surgeJumpHitboxParticle, PlayerTransform.position, PlayerTransform.rotation);
            hitBoxParticles.transform.localScale = PlayerTransform.localScale;
            // hitBoxParticles.transform.rotation = PlayerTransform.rotation;  // todo: make this the normal of the exited surf.
            
            hitBoxParticles.transform.rotation = Quaternion.FromToRotation(Vector3.up, GetExitNormal());
            hitBoxParticles.transform.position = CachedExitPos;
            hitBoxParticles.GetComponent<ParticleSystem>().Play();
            
            Collider[] hitColliders = UnityEngine.Physics.OverlapSphere(CachedExitPos, hitboxRadius, blastLayerMask);
            for(Int32 i = 0; i < hitColliders.Length; i++)
            {
                Collider collider = hitColliders[i];
                if (collider.CompareTag("NewEnemy"))
                {
                    collider.gameObject.GetComponent<HitPartOfNewEnemy>().CollideWithBody(ref collider);
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Collectables;
using __OasisBlitz.Player.Gauntlets;
using __OasisBlitz.Player.Physics;
using UnityEngine;
using Animancer;
using Animancer.Units;
using UnityEngine.Serialization;

namespace __OasisBlitz.Player.Animation
{
    [RequireComponent(typeof(ProceduralRun))]
    public class BanditAnimationController : MonoBehaviour
    {
        [SerializeField] private AnimancerComponent _Animancer;
        public ClipTransition idle;
        [SerializeField] private ClipTransition jump;
        [SerializeField] private ClipTransition drill;
        [SerializeField] private ClipTransition blast;
        [SerializeField] private ClipTransition slide;
        [SerializeField] private ClipTransition death;
        [SerializeField] private ClipTransition celebrate;
        [SerializeField] private ModelRotator modelRotator;
        [SerializeField] private FlipAnimation flipAnimation;
        
        [SerializeField] private PlayerPhysics playerPhysics;
        
        private SurveyorWheel surveyorWheel;
        public ProceduralRun proceduralRun;
        private IdlePoseIK idlePoseIK;

        [SerializeField] private Transform modelTransform;

        [SerializeField] private GauntletManager gauntletManager;
        
        private Coroutine currentBlastRoutine;

        private bool creatingDrill;
        
        private bool isGrounded;

        private bool isSliding;
        
        void Awake()
        {
            proceduralRun = GetComponent<ProceduralRun>();
            surveyorWheel = GetComponent<SurveyorWheel>();
            idlePoseIK = GetComponent<IdlePoseIK>();
            CollectableManager.Instance.OnPlantCollected += PlayCelebration;
        }

        void Update()
        {
            if (isGrounded && !isSliding)
            {
                surveyorWheel.UpdateWheel(playerPhysics.Velocity, Time.deltaTime);
                proceduralRun.UpdateProceduralRun(playerPhysics.Velocity.magnitude);
                
                Vector2 curr = new Vector2(playerPhysics.Velocity.x, playerPhysics.Velocity.z);
                if (curr.magnitude <= 0.1f)
                {
                    PlayIdle();
                }
                else
                {
                    PlayWalk();
                }
            }
            else if (isGrounded && isSliding)
            {
                PlaySlide();
            }
            
        }

        void OnEnable()
        {
            PlayIdle();
        }

        public void PlayIdle()
        {
            // TODO: Calling this function every time is a lazy approach that will not scale -- the point is just that
            // we need to allow these other animations to stop the coroutine that the blast ability starts
            // Debug.Log("Idle");
            if (playerPhysics.isStuckTimer <= 0.0f)
            {
                ChangeAnimation();
                // TODO: Enable this for IK
                // idlePoseIK.ApplyAnimatorIK = true;
                _Animancer.Play(idle);
            }
        }
        
        public void PlayDeath()
        {
            ChangeAnimation();
            _Animancer.Play(death);
        }

        public void PlayCelebration()
        {
            ChangeAnimation();
            playerPhysics.SetStuck(4.0f, Vector3.zero); 
            _Animancer.Play(celebrate);
            Debug.Log("Play Celebration");
        }

        public void PlayBall()
        {
            ChangeAnimation();
            _Animancer.Play(blast);
            Debug.Log("Play ball");
        }

        public void PlayDrill()
        {
            ChangeAnimation();
            _Animancer.Play(drill);
        }

        public void PlayWalk()
        {
            ChangeAnimation();
            proceduralRun.ApplyAnimatorIK = true;
            proceduralRun.StartRun();
        }

        public void SetGrounded(bool grounded)
        {
            // TODO: Current challenge is that jump animation is played on a frame when still grounded
            isGrounded = grounded;
        }

        public void SetSliding(bool sliding)
        {
            isSliding = sliding;
        }

        public void PlayJump()
        {
            // Don't allow the jump to interrupt a flip
            if (modelRotator.Flipping) return;
            
            ChangeAnimation();
            _Animancer.Play(jump);
        }

        public void PlaySlide()
        {
            ChangeAnimation();
            _Animancer.Play(slide);           
        }

        public void PlayBlast(Vector3 blastVelocity)
        {
            modelRotator.OnBlast(blastVelocity);
            ChangeAnimation();
            _Animancer.Play(blast);
        }

        public void PlayFlipIntoDrill()
        {
            // Debug.Log("Flip into drill");
            flipAnimation.FlipJumpToDrill();
            creatingDrill = true;
            gauntletManager.ExtendBlades();
        }
        
        public void PlayFlipOutOfDrill()
        {
            // Debug.Log("Flip out of drill");
            flipAnimation.FlipDrillIntoJump();
            creatingDrill = false;
            gauntletManager.UnformDrill();
            // Blades will retract when the flip is done
            gauntletManager.ExtendBlades();
            
        }
        
        public void UpdateFlipProgress(float flipProgress)
        {
            flipAnimation.SetFlipProgress(flipProgress);
            
            if (flipProgress > 0.8f && creatingDrill)
            {
                gauntletManager.FormDrill();
            }
            else if (flipProgress > 0.95f && !creatingDrill)
            {
                gauntletManager.RetractBlades();
            }
        }

        private void ChangeAnimation()
        {
            InterruptBlastRoutine();
            stopIdleKinematics();
            stopRunKinematics();
            ResetModelYPos();
        }

        private void stopIdleKinematics()
        {
            // idlePoseIK.ApplyAnimatorIK = false;
        }

        private void stopRunKinematics()
        {
            // proceduralRun.leftFootWeight = 0.0f;
            // proceduralRun.rightFootWeight = 0.0f;
            // proceduralRun.ApplyAnimatorIK = false;
        }
        private void InterruptBlastRoutine()
        {
            if (currentBlastRoutine != null)
            {
                StopCoroutine(currentBlastRoutine);
            }
            
            // modelRotater.UnpauseRotation();
        }

        private void ResetModelYPos()
        {
            SetModelYPos(0.0f);
        }
        public void SetModelYPos(float weightToUse)
        {
            float val = weightToUse * -0.1f;
            modelTransform.localPosition = new Vector3(0.0f, val, 0.0f);
        }
        
        public void SetModelYRunPos(float weightToUse)
        {
            float val = weightToUse * 0.05f;
            modelTransform.localPosition = new Vector3(0.0f, val, 0.0f);
        }
    }
}
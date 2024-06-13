using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Collectables;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.Player.Gauntlets;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using Animancer;
using Animancer.Units;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace __OasisBlitz.Player.Animation
{
    public class BanditAnimationController : MonoBehaviour
    {
        [SerializeField] private BanditAnimations animations;
        
        [SerializeField] private AnimancerComponent _Animancer;
        
        [SerializeField] private ModelRotator modelRotator;

        [SerializeField] private Transform modelTransform;

        [SerializeField] private DrillBehavior drill;

        [field: SerializeField] public BoardBehavior board { get; private set; }
        
        [SerializeField] private GrappleAnimation grappleAnimation;

        #region AddedForRipples
        [SerializeField] private WaterRipples waterRipples;
        #endregion

        // if this is false, animation events won't be emitted
        // there are safeguards in code for this in the ghost replayer to prevent accidental recording of ghosts.
        public bool bIsSourceBandit = true;
        
        private Coroutine currentBlastRoutine;

        private AnimancerState currentAnimation;
        public BanditAnimations.BanditAnimationTypes currentAnimationType { get; private set; }
        private BanditAnimations.BanditAnimationTypes queuedAnimation;

        // If this is true, then when we set a background animation we will play it immediately
        private bool currentlyPlayingOneshot = false;

        public Action<BanditAnimations.BanditAnimationTypes> OnPlayAnimation;
        
        void Awake()
        {
            queuedAnimation = BanditAnimations.BanditAnimationTypes.nullAnim;
        }

        void OnEnable()
        {
            PlayIdle();
        }

        public void PlayAnimation(BanditAnimations.BanditAnimationTypes anim)
        {
            Debug.Log("Playing: " + anim);
            
            if (OnPlayAnimation != null
                && bIsSourceBandit)
            {
                OnPlayAnimation(anim);
            }
            
            #region AddedForWaterRipples
            if (anim != BanditAnimations.BanditAnimationTypes.Idle &&
                currentAnimationType == BanditAnimations.BanditAnimationTypes.Idle)
            {
                //ending idle
                waterRipples.StopIdleRipples();
            }
            #endregion
            
            currentAnimationType = anim;
            currentAnimation = _Animancer.Play(animations.GetAnimation(anim));
            currentAnimation.Events.OnEnd = () => OnAnimationEnd();
            
            HandleDrillStateOnAnimationStart(anim);
            HandleBoardStateOnAnimationStart(anim);
            
            if (modelRotator)
            {
                HandleModelRotatorOnAnimationStart(anim);
            }
        }

        private void HandleDrillStateOnAnimationStart(BanditAnimations.BanditAnimationTypes anim)
        {
            // Certain animations should always disable the drill when they are played
            if (anim is BanditAnimations.BanditAnimationTypes.Run
                or BanditAnimations.BanditAnimationTypes.Freefall
                or BanditAnimations.BanditAnimationTypes.Slide
                or BanditAnimations.BanditAnimationTypes.Jump
                or BanditAnimations.BanditAnimationTypes.GrappleStart)
            {
                drill.StopDrill();
            }
            // Other animations should always enable the drill when they are played, if it somehow hasn't been enabled yet
            else if (anim is BanditAnimations.BanditAnimationTypes.Drill)
            {
                drill.StartDrill();
            }
            
        }
        
        private void HandleBoardStateOnAnimationStart(BanditAnimations.BanditAnimationTypes anim)
        {
            // Certain animations should always disable the drill when they are played
            if (anim is BanditAnimations.BanditAnimationTypes.Slide)
            {
                board.StartBoard();
            }
            // Other animations should always enable the drill when they are played, if it somehow hasn't been enabled yet
            else
            {
                board.StopBoard();
            }
            
        }

        private void HandleModelRotatorOnAnimationStart(BanditAnimations.BanditAnimationTypes anim)
        {
            if (anim is BanditAnimations.BanditAnimationTypes.Drill
                or BanditAnimations.BanditAnimationTypes.DrillStart
                or BanditAnimations.BanditAnimationTypes.Blast)
            {
                modelRotator.SetUpright(false);
            }
            else
            {
                modelRotator.SetUpright(true);
            }
        }

        private void OnAnimationEnd()
        {
            if (queuedAnimation != BanditAnimations.BanditAnimationTypes.nullAnim)
            {
                PlayAnimation(queuedAnimation);
                queuedAnimation = BanditAnimations.BanditAnimationTypes.nullAnim;
            }
        }
        

        public void PlayFreeFall()
        {
            if (currentAnimationType is BanditAnimations.BanditAnimationTypes.Jump or BanditAnimations.BanditAnimationTypes.GrappleImpact or BanditAnimations.BanditAnimationTypes.DrillEnd)
            {
                queuedAnimation = BanditAnimations.BanditAnimationTypes.Freefall;
            }
            else
            {
                PlayAnimation(BanditAnimations.BanditAnimationTypes.Freefall);
            }
        }

        public void PlayIdle()
        {
            // TODO: Calling this function every time is a lazy approach that will not scale -- the point is just that
            // we need to allow these other animations to stop the coroutine that the blast ability starts
            PlayAnimation(BanditAnimations.BanditAnimationTypes.Idle);
        }

        public void PlayDeath()
        {
        }

        public void PlayCelebration()
        {
            PlayAnimation(BanditAnimations.BanditAnimationTypes.Celebrate);
        }

        public void PlayRun()
        {
            PlayAnimation(BanditAnimations.BanditAnimationTypes.Run);
        }

        public void PlayJump()
        {
            // Don't allow the jump to interrupt a flip
            // if (modelRotator.Flipping) return;
            
            // PlayAnimation(jump, true);
            PlayAnimation(BanditAnimations.BanditAnimationTypes.Jump);
        }

        public void PlaySlide()
        {
            // PlayAnimation(slide);
            PlayAnimation(BanditAnimations.BanditAnimationTypes.Slide);
        }

        public void PlayBlastFromDrill(Vector3 blastVelocity)
        {
            // modelRotator.OnBlast(blastVelocity);
            // ChangeAnimation();
            // PlayAnimation(blast, true);
            currentAnimation.Stop();
            PlayAnimation(BanditAnimations.BanditAnimationTypes.Blast);
            queuedAnimation = BanditAnimations.BanditAnimationTypes.Drill;
        }
        
        public void PlayBlastFromFreefall(Vector3 blastVelocity)
        {
            // modelRotator.OnBlast(blastVelocity);
            // ChangeAnimation();
            // PlayAnimation(blast, true);
            currentAnimation.Stop();
            PlayAnimation(BanditAnimations.BanditAnimationTypes.Blast);
            queuedAnimation = BanditAnimations.BanditAnimationTypes.Freefall;
        }

        public void PlayStartGrapple(Vector3 targetCenter)
        {
            // PlayAnimation(beginGrapple);
            PlayAnimation(BanditAnimations.BanditAnimationTypes.GrappleStart);
            grappleAnimation.PlayGrappleAnimation(targetCenter, 6.0f);
            
        }
        
        public void PlayGrappleImpact()
        {
            // This has priority 1, so the jump animation will be queued up after it when entering airborne state
            // PlayAnimation(grappleImpact, true);
            PlayAnimation(BanditAnimations.BanditAnimationTypes.GrappleImpact);
            grappleAnimation.End();
            
        }

        public void PlayFlipIntoDrill()
        {
            // Debug.Log("Flip into drill");
            // flipAnimation.FlipJumpToDrill();
            // creatingDrill = true;
            // gauntletManager.ExtendBlades();
            
            // Play flip animation and set queued animation to be drill
            PlayAnimation(BanditAnimations.BanditAnimationTypes.DrillStart);
            queuedAnimation = BanditAnimations.BanditAnimationTypes.Drill;

        }
        
        public void PlayFlipOutOfDrill()
        {
            // Just play flip animation, freefall will be set by freefall state
            PlayAnimation(BanditAnimations.BanditAnimationTypes.DrillEnd);
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
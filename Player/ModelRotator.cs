using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.Animation;
using UnityEngine;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Utility;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine.Serialization;

namespace __OasisBlitz.Player
{
    public struct FlipType
    {
        public bool forward;
        public bool passOnce;
        public bool intoDrill;

    }
    
    // No rotation data is stored in the actual transform of the character. Every frame, we reset the rotation of all
    // relevant components, then reconstruct it based on the quaternions that we store in this class.
    public class ModelRotator : MonoBehaviour
    {
        [SerializeField] private PlayerPhysics playerPhysics;
        [SerializeField] private float rotationFactorPerFrame;
        [SerializeField] private BanditAnimationController banditAnimationController;
        
        [Header("Flip Parameters")]
        [SerializeField] private float flipDuration;
        [SerializeField] private float drillPitchOffset;

        /// <summary>
        /// How much to tilt the player when they accelerate
        /// </summary>
        [SerializeField] private float tiltAmount;

        [FormerlySerializedAs("uprightTiltAmount")] [SerializeField] private float slideTiltAmount;

        private Vector3 startPositionLocal;
        
        private const float uprightPitch = 0f;
        
        // Slide State
        private Quaternion currentSlidePitch;
        private float currentSlideRoll;
        [SerializeField] private float slidePitchLerpSpeed = 40f;
        [SerializeField] private float RollLerpSpeed = 40f;
        
        // Rotation state
        private bool grounded;
        private bool upright = true;
        private float currentYawAngle;
        private Quaternion currentYawQuaternion;
        private float currentPitch;
        
        // Blast state
        private bool yawLocked;
        [SerializeField] private float blastFlipDuration;
        [SerializeField] private float blastLockDuration;
        private bool blasting;
        [SerializeField] private float blastPitch;
        private Tweener blastTween;
        private float blastProgress;

        private bool sliding;
        
        // Flipping state
        public bool Flipping { get; private set; } = false;
        private FlipType currentFlipType;
        private float flipProgressTemp = 0;
        private float flipProgress;
        private Tweener flipTween;
        private float flipStartPitch;
        
        // Dash state
        private bool dashing;
        private Vector3 dashTargetPosition;
        
        //lock rotation
        private bool lockRotation = false;

        void Awake()
        {
            startPositionLocal = transform.localPosition;
            currentYawQuaternion = Quaternion.identity;
        }

        void Update()
        {
            if (lockRotation)
            {
                return;
            }
            ResetTransformRotation();

            if (sliding)
            {
                SlideRotations();
            }
            else if (grounded)
            {
                GroundedRotations();
            }
            else
            {
                AerialRotations();
            }

            if (blasting)
            {
                UpdateBlast();
            }
        }

        public void SetGrounded(bool grounded)
        {
            if (grounded)
            {
                if (blastTween != null)
                {
                    blastTween.Complete();
                }
            }

            this.grounded = grounded;
        }

        public void SetSliding(bool sliding)
        {
            this.sliding = sliding;

            currentSlideRoll = CalculateCurrentSlideRollAngle();
        }

        public Quaternion GetCurrentYawQuaternion()
        {
            return currentYawQuaternion;
        }

        public void SetFullDirection(Vector3 lookTarget)
        {
            currentYawQuaternion =
                Quaternion.FromToRotation(Vector3.forward, new Vector3(lookTarget.x, 0, lookTarget.z));
        }
        
        public void SetAndLockYaw(Vector3 lookTarget)
        {
            currentYawQuaternion =
                Quaternion.FromToRotation(Vector3.forward, new Vector3(lookTarget.x, 0, lookTarget.z));
            yawLocked = true;
        }

        public void ReleaseLockedRotation()
        {
            yawLocked = false;
        }
        
        private void ResetTransformRotation()
        {
            transform.localPosition = startPositionLocal;
            transform.localRotation = Quaternion.identity;
        }
        
        
        public void OnStartDrilling()
        {
            if (dashing) return;
            blastTween?.Complete();
            upright = false;
            StartFlip(true, flipDuration);
            banditAnimationController.PlayFlipIntoDrill();
        }

        public void OnStopDrilling()
        {
            if (dashing) return;
            blastTween?.Complete();
            upright = true;
            StartFlip(false, flipDuration);
            banditAnimationController.PlayFlipOutOfDrill();
        }

        public void OnDash(Vector3 targetPosition)
        {
            // TODO: This does nothing right now because I didn't have time to finish it
            // dashTargetPosition = targetPosition;
            // dashing = true;           
            // StartFlip(true, flipDuration/2f);
        }

        public void OnDashComplete()
        {
            dashing = false;
        }

        public void OnBlast(Vector3 blastVelocity)
        {
            // Finish any ongoing flip
            flipTween?.Complete();
            
            // Bandit should yaw away from the lateral blast direction, then pitch until the forwards vector points directly
            // away
            yawLocked = true;
            blasting = true;

            Vector3 lateralVelocity = new Vector3(blastVelocity.x, 0, blastVelocity.z);

            // If lateral direction is nearly zero, do not change yaw
            float lateralVelocityMagnitude = lateralVelocity.magnitude;
            if (lateralVelocityMagnitude > 0.01f)
            {
                currentYawQuaternion = Quaternion.FromToRotation(Vector3.forward, lateralVelocity);
                // currentPitch = Vector3.Angle(Vector3.up, blastVelocity * -1);
                currentPitch = blastPitch;
                // currentPitch = 120f;
                // currentYawQuaternion = Quaternion.identity; 
            }
            else
            {
                currentPitch = 90f;
                // StartFlip(false, 0.8f);
            }
            
            // A dotween tween that interpolates flipProgress from 0 to 1 and sets flipping to false when done
            blastTween = DOTween.To(() => blastProgress, x => blastProgress = x, 1f, blastLockDuration)
                        .OnComplete(() => {
                            blasting = false;  // Set flipping to false when the tween is complete
                            yawLocked = false;
                            blastProgress = 0f;
                            // TODO: Go into drill or into upright based on current state
                            StartFlip(false, blastFlipDuration);
                        });
            
        }

        /// <summary>
        /// Start a tween that interpolates the flipPitch field, which another function will apply to the model
        /// </summary>
        private void StartFlip(bool enterDrill, float duration)
        {
            // Interrupt existing flip if necessary
            if (flipTween != null)
            {
                flipTween.Complete();
            }

            flipStartPitch = currentPitch;

            Flipping = true;

            flipProgress = 0;
            
            
            
            // A dotween tween that interpolates flipProgress from 0 to 1 and sets flipping to false when done
            flipTween = DOTween.To(() => flipProgress, x => flipProgress = x, 1f, duration)
                        .OnComplete(() => {
                            Flipping = false;  // Set flipping to false when the tween is complete
                            flipProgress = 0f;
                        });
            

            // Flip cases:
            // 1. Starting upright, target is down 
            // **** Backflip, no pass
            FlipType intoDrillDown = new FlipType {forward = true, passOnce = false, intoDrill = true};
            // 2. Starting upright, target is up
            // **** Frontflip, also no pass?
            // Switched from the orignal idea, this is also forwards now
            FlipType intoDrillUp = new FlipType {forward = true, passOnce = false, intoDrill = true};
            // 3. Starting down, target is upright
            // **** Frontflip, no pass
            FlipType outOfDrillDown = new FlipType {forward = true, passOnce = false, intoDrill = false};
            // 4. Starting up, target is upright
            // **** Backflip, pass once
            // FlipType outOfDrillUp = new FlipType {forward = false, passOnce = true, intoDrill = false};
            FlipType outOfDrillUp = new FlipType {forward = true, passOnce = false, intoDrill = false};
            // FlipType outOfDrillUp = new FlipType {forward = false, passOnce = true, intoDrill = false};

            if (enterDrill)
            {
                // TODO: To enable flips into drill, delete this line
                if (TargetAerialPitchAngle() > 90f)
                {
                    Flipping = true;
                    currentFlipType = intoDrillDown;
                    // Debug.Log("Into drill down");
                }
                else
                {
                    Flipping = true;
                    currentFlipType = intoDrillUp;
                    // Debug.Log("Into drill up");
                }
            }
            else
            {
                if (currentPitch > 90f)
                {
                    Flipping = true;
                    currentFlipType = outOfDrillDown;
                    // Debug.Log("Out of drill down");
                }
                else
                {
                    Flipping = true;
                    currentFlipType = outOfDrillUp;
                    // Debug.Log("Out of drill up");
                }
            }
           
            // Flip should end when the pitch was previously "behind" the target, and is now "in front of" the target
            // (behind or in front is a notion dependent on the flip direction)
            
        }

        private void UpdateBlast()
        {
        }

        private void UpdateFlip()
        {
            float endPitchTarget;
            
            if (currentFlipType.intoDrill)
            {
                endPitchTarget = TargetAerialPitchAngle() + 360;
            }
            else
            {
                endPitchTarget = uprightPitch + 360;
            }

            // If we're flipping into drilling, we want to end on the target pitch. Otherwise, we end on zero
            // if (currentFlipType.intoDrill) endPitchTarget += TargetAerialPitchAngle();
            float pitchRange;
            
            // Add or subtract 360 from the target pitch if we're passing once
            // if (currentFlipType.passOnce)
            if (!currentFlipType.forward)
            {
                pitchRange = endPitchTarget + flipStartPitch;
            }
            else
            {
                pitchRange = endPitchTarget - flipStartPitch;
            }
            
            // float pitchRange = endPitchTarget - flipStartPitch;

            if (currentFlipType.forward)
            {
                currentPitch = (flipStartPitch + pitchRange * flipProgress) % 360f;
            }
            else
            {
                currentPitch = (flipStartPitch - pitchRange * flipProgress) % 360f;
            }
            
            // Update animation
            banditAnimationController.UpdateFlipProgress(flipProgress);
            

        }

        private Quaternion AerialPitch()
        {
            if (!Flipping)
            {
                // Don't update pitch if we're blasting
                if (!blasting)
                {
                    if (upright)
                    {
                        currentPitch = uprightPitch;
                    }
                    else
                    {
                        currentPitch = TargetAerialPitchAngle();
                    }
                }
            }
            else
            {
                UpdateFlip();
            }
            
            // Vector3 right = transform.TransformDirection(transform.right);
            // return Quaternion.Euler(0, 0, currentPitch);
            // return Quaternion.FromToRotation(Vector3.up, playerPhysics.Velocity);
            return Quaternion.AngleAxis(currentPitch, Vector3.right);

            // Get transform.right in world space

        }

        private float TargetAerialPitchAngle()
        {
            // return Quaternion.FromToRotation(Vector3.up, playerPhysics.Velocity).eulerAngles.y;
            return Vector3.Angle(Vector3.up, playerPhysics.Velocity) + drillPitchOffset;
        }

        private void SlideRotations()
        {
            
            currentSlidePitch = CalculateCurrentSlidePitch();
            
            transform.localRotation = currentSlidePitch * VelocityYaw() * Quaternion.identity;

            currentSlideRoll = CalculateCurrentSlideRollAngle();
            
            Quaternion roll = Quaternion.Euler(currentSlideRoll, 0, 0);
            
            // Calculate roll
            // Vector3 right = transform.TransformDirection(transform.right);
            // Vector3 up = transform.TransformDirection(transform.up);
            //
            // Vector3 projectedAcceleration = Vector3.Project(playerPhysics.Acceleration, right);
            // Quaternion roll = Quaternion.FromToRotation(up, projectedAcceleration);
            
            transform.Rotate(0, 0, currentSlideRoll * -1, Space.Self);


        }

        private Quaternion CalculateCurrentSlidePitch()
        {
            Vector3 groundNormal = playerPhysics.currentGroundNormal;
            Quaternion targetInclinePitch = Quaternion.FromToRotation(Vector3.up, groundNormal);
            
            return Quaternion.Lerp(currentSlidePitch, targetInclinePitch, slidePitchLerpSpeed * Time.deltaTime);
        }
        
        private float CalculateCurrentSlideRollAngle()
        {
            Vector3 accelerationWorldSpace = playerPhysics.Acceleration;
            Vector3 localAcceleration = transform.InverseTransformDirection(accelerationWorldSpace);
            
            // X component of localAcceleration is how much I should lean left or right
            float targetRollAmount = localAcceleration.x * slideTiltAmount;

            return Mathf.Lerp(currentSlideRoll, targetRollAmount, RollLerpSpeed * Time.deltaTime);
        }
        
        private void GroundedRotations()
        {
            transform.localRotation = AccelerationPitchAndRoll() * VelocityYaw() * Quaternion.identity;
        }

        private void AerialRotations()
        {
            // currentPitch = TargetAerialPitchAngle();
            // transform.localRotation = AccelerationPitchAndRoll() * VelocityYaw() * AerialPitch() * Quaternion.identity;
            transform.localRotation = VelocityYaw() * AerialPitch() * Quaternion.identity;
        }

        private Quaternion VelocityYaw()
        {
            Vector3 positionToLookAt = new Vector3(playerPhysics.Velocity.x, 0, playerPhysics.Velocity.z); 
            
            // When dashing, look towards the enemy instead
            if (dashing)
            {
                Vector3 dashLookDirection = dashTargetPosition - transform.position;
                positionToLookAt = new Vector3(dashLookDirection.x, 0, dashLookDirection.z);
            }
            
            if (yawLocked || positionToLookAt.magnitude < 0.01f)
            {
                return currentYawQuaternion;
            }
            
            Quaternion targetYaw = Quaternion.LookRotation(positionToLookAt);
            // Debug.Log("Calling here");
            currentYawQuaternion = Quaternion.Slerp(currentYawQuaternion, targetYaw, rotationFactorPerFrame * Time.deltaTime);
            return currentYawQuaternion;
        }

        private Quaternion AccelerationPitchAndRoll()
        {
            // Vector3 acceleration = playerPhysics.Acceleration;
            
            return Quaternion.Euler(playerPhysics.Acceleration.z * tiltAmount, 0, -playerPhysics.Acceleration.x * tiltAmount);
        }

        public void SetRotation()
        {
            
        }
        
        /// <summary>
        /// Rotate yaw-wise around the center of mass
        /// </summary>
        // private void YawAroundCoM()
        // {
        //     // First, get desired yaw angle
        //     // We want the shortest angle between the velocity and the current rotation
        //     var desiredYawAngle = Vector3.SignedAngle(new Vector3(-1, 0, 0), playerPhysics.Velocity, Vector3.up);
        //     
        //     // Now, apply it around the center of mass
        //     transform.RotateAround(centerOfMass.position, Vector3.up, desiredYawAngle);
        // }
        
    }
}

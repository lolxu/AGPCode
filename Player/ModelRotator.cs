using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Camera.StateMachine;
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
        [SerializeField] private float rotationFactorPerFrame;
        [SerializeField] private PlayerPhysics playerPhysics;
        
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

        //ModelVisability
        [SerializeField] private List<GameObject> ModelComponents;
        [SerializeField] private Material BanditWhite;

        [Header("Feet")] 
        public Transform leftFootTransform;
        public Transform rightFootTransform;

        [Header("Hands")] 
        public Transform leftHandTransform;
        public Transform rightHandTransform;
        
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
            StartCoroutine(WaitToResetCamera());
        }

        private IEnumerator WaitToResetCamera()
        {
            yield return null;
            // Reset Camera look direction
            CameraStateMachine.Instance.ResetVerticalAxis();
            CameraStateMachine.Instance.ResetHorizontalAxis();
            // CameraStateMachine.Instance.ResetCamera();
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


        public void SetUpright(bool upright)
        {
            this.upright = upright;
        }

        public void OnStartBlasting()
        {
            
        }

        public void OnStopBlasting()
        {
            
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

        private Quaternion AerialPitch()
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
        
        //Bandit Visability
        public void HideBandit()
        {
            foreach (var characterComp in ModelComponents)
            {
                if (characterComp)
                {
                    characterComp.SetActive(false);
                }
            }
        }
        
        public void RevealBandit()
        {
            foreach (var characterComp in ModelComponents)
            {
                if (characterComp)
                {
                    characterComp.SetActive(true);
                }
            }
        }

        // Bandit flash
        public void BanditDeathModelSequence()
        {
            StartCoroutine(BanditFlash());
        }
        private IEnumerator BanditFlash()
        {
            List<Material> orgMats = new List<Material>();
            foreach (var characterComp in ModelComponents)
            {
                var meshRenderer = characterComp.GetComponent<SkinnedMeshRenderer>();
                orgMats.Add(meshRenderer.material);
                meshRenderer.material = BanditWhite;
            }
            yield return new WaitForSeconds(0.35f);
            for (int i = 0; i < ModelComponents.Count; i++)
            {
                var meshRenderer = ModelComponents[i].GetComponent<SkinnedMeshRenderer>();
                meshRenderer.material = orgMats[i];
            }
            HideBandit();
        }
        
    }
}

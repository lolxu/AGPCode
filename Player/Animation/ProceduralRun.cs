using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Utility;
using Animancer;
using Animancer.Units;
using UnityEngine;
using UnityEngine.Serialization;
using AnimancerComponent = Animancer.AnimancerComponent;

namespace __OasisBlitz.Player.Animation
{
    public class ProceduralRun : MonoBehaviour
    {
        // **************************** TIPS ON USING THIS CLASS ********************************
        // If you want to modify the animation to add more personality, additional frames can be added to the mixer.
        // The thresholds should still be between 0 and 2, where 0, 1 and 2 represent the points where the feet touch the ground.
        // (at 0 the left foot is touching, at 1 the right foot is touching, at 2 the left foot is touching once again, then cycle).

        // Float ranging from 0 to 1 that blends between walk and run
        public float shouldRun = 1f;
        [SerializeField] private BanditAnimationController animationController;
        public SurveyorWheel surveyorWheel;

        [Description("When below this speed, the character will walk instead of run")]
        public float fullRunSpeed = 2f;
        public float fullWalkSpeed = 1f;

        public float runWheelRadius;
        public float walkWheelRadius;
        
        [Header("Run poses")]
        // Single frame animations that can be blended between
        public AnimationClip leftFootPlant;
        public AnimationClip leftFootBack;
        public AnimationClip rightFootPlant;
        public AnimationClip rightFootBack;

        [Header("Walk poses")] 
        public AnimationClip leftFootPlantWalk;
        public AnimationClip leftFootBackWalk;
        public AnimationClip rightFootPlantWalk;
        public AnimationClip rightFootBackWalk;
        
        //foot weights
        public float leftFootWeight;
        public float rightFootWeight;
        private float MaxWeight = 1.0f;
        private float MinWeight = 0.0f;

        private LinearMixerState runMixer;
        private LinearMixerState walkMixer;
        
        // A higher order mixer that blends between the walk and run mixers
        private LinearMixerState blendMixer;
        
        [SerializeField] private AnimationCurve mixerCurve;
        [SerializeField] private AnimationCurve heightCurve;

        [SerializeField] private AnimationCurve runCurve;
        
        [SerializeField] private AnimancerComponent _Animancer;
        [SerializeField] private PlayerPhysics playerPhysics;
        [SerializeField] private PlayerAudio playerAudio;
        
        private float previousFrameRunStage = 0.0f;
        
        void Awake()
        {
            mixerCurve.preWrapMode = WrapMode.Clamp;
            mixerCurve.postWrapMode = WrapMode.Clamp;

            runMixer = new LinearMixerState();
            
            runMixer.Add(leftFootPlant, 0);
            runMixer.Add(leftFootBack, 0.5f);
            runMixer.Add(rightFootPlant, 1);
            runMixer.Add(rightFootBack, 1.5f);
            runMixer.Add(leftFootPlant, 2);
            
            walkMixer = new LinearMixerState();
            
            walkMixer.Add(leftFootPlantWalk, 0);
            walkMixer.Add(leftFootBackWalk, 0.5f);
            walkMixer.Add(rightFootPlantWalk, 1);
            walkMixer.Add(rightFootBackWalk, 1.5f);
            walkMixer.Add(leftFootPlantWalk, 2);
            
            blendMixer = new LinearMixerState();
            blendMixer.Add(walkMixer, 0);
            blendMixer.Add(runMixer, 1);
            
            // Because of our setup with mixers as still poses, we don't need to synchronize children anyway.
            // If we did, though, this would cause an error because of the way nested mixers work
            // https://forum.unity.com/threads/animancer-less-animator-controller-more-animator-control.566452/page-32
            blendMixer.DontSynchronizeChildren();
            
            // TODO: Remove
            surveyorWheel.SetWheelRadius(walkWheelRadius);
            
            _LeftFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            _RightFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.RightFoot);
            
            // Tell Unity that OnAnimatorIK needs to be called every frame.
            ApplyAnimatorIK = true;
        }

        public void UpdateProceduralRun(float speed)
        {
            // Time.timeScale = 0.10f;
            // A float value that will be between 0 and 1, representing how far along the run cycle we are.
            // To blend between the four animations, we will multiply this value by 4.
            
            if (runMixer != null)
            {
                float runStage = surveyorWheel.RunStage;
                // (For testing) quantize runStage to always be 0.5, 1, 1.5, or 2
                // runStage = Mathf.Round(runStage * 2) / 2;
                runMixer.Parameter = runStage;
                
                // mixer.Parameter = ConvertRunStageToMixerParameter(runStage);
                
            }
            
            if (walkMixer != null)
            {
                float walkStage = surveyorWheel.RunStage;
                walkMixer.Parameter = walkStage;

            }
            
            if (blendMixer != null)
            {
                BlendBetweenWalkAndRun(speed);
            }
            
            // Debug.Log("Previous frame runstage " + previousFrameRunStage);
            // Debug.Log("Current frame runstage " + surveyorWheel.RunStage);
            
            if (previousFrameRunStage < 1.0f && surveyorWheel.RunStage >= 1.0f)
            {
               playerAudio.PlayFootstep();
            }
            // This is a slightly messy way to check for the foot plant that occurs when we wrap around between 2 and 0
            else if (previousFrameRunStage > 1.0f && surveyorWheel.RunStage < 1.0f)
            {
               playerAudio.PlayFootstep();
            }
            
            previousFrameRunStage = surveyorWheel.RunStage;
            

        }

        private void BlendBetweenWalkAndRun(float speed)
        {
            // If speed is greater than fullRunSpeed, we should run (blend towards 1)
            // If speed is less than fullWalkSpeed, we should walk (blend towards 0)
            // Between fullWalkSpeed and fullRunSpeed, we should blend between the two
            float runBlend = Mathf.Clamp01((speed - fullWalkSpeed) / (fullRunSpeed - fullWalkSpeed));
            
            // The radius for the surveyor wheel should be between walkWheelRadius and runWheelRadius
            float wheelRadius = Mathf.Lerp(walkWheelRadius, runWheelRadius, runBlend);
            
            surveyorWheel.SetWheelRadius(wheelRadius);
            
            blendMixer.Parameter = runBlend;

        }

        private float InterpolateValuesOnCurve(float start, float end, float progress, bool forwards)
        {
            if (forwards)
            {
                return start + runCurve.Evaluate(progress) * (end - start);
            }
            else
            {
                return start + ((1 - runCurve.Evaluate(1 - progress)) * (end - start));
            }
        }

        private float ConvertRunStageToMixerParameter(float runStage)
        {
            // Runstage comes in between 0 and 2, but we want to break it up into 4 pieces, so let's multiply by 2
            // (we can divide by 2 at the end)

            if (runStage < 0.5f)
            {
                float progress = (runStage / 0.5f);
                return InterpolateValuesOnCurve(0, 0.5f, progress, true);
            }
            else if (runStage < 1.0f)
            {
                float progress = ((runStage - 0.5f)/ 0.5f);
                return InterpolateValuesOnCurve(0.5f, 1.0f, progress, false);

            }
            else if (runStage < 1.5f)
            {
                float progress = ((runStage - 1.0f)/ 0.5f);
                return InterpolateValuesOnCurve(1.0f, 1.5f, progress, true);
            }
            else
            {
                float progress = ((runStage - 1.5f)/ 0.5f);
                return (InterpolateValuesOnCurve(1.5f, 2.0f, progress, false));
            }
            


            // float curveValue;
            //
            // float positionInCurve = runStage % 1.0f;
            //
            // if (positionInCurve < 0.5f)
            // {
            //     // 0 to 0.5 range
            //     curveValue = runCurve.Evaluate(positionInCurve * 2f);
            // }
            // else
            // {
            //     // 0.5 to 1 range, reverse the curve
            //     curveValue = runCurve.Evaluate(1 - (positionInCurve * 2f));
            // }
            //
            // if (runStage > 1.0f) return curveValue + 1;
            //
            // return curveValue;

            // return (runCurve.Evaluate(runStage / 2)) * 2;
        }

        public void StartRun()
        {
            // _Animancer.Play(runMixer);
            // _Animancer.Play(walkMixer);
            _Animancer.Play(blendMixer);


        }

        /************************************************************************************************************************/
        [SerializeField, Meters] private float _RaycastOriginY = 0.2f;
        [SerializeField, Meters] private float _RaycastEndY = -0.25f;

        /************************************************************************************************************************/

        private Transform _LeftFoot;
        private Transform _RightFoot;

        [SerializeField] private LayerMask canStandOn;

#if DEBUG
        private Vector3 GOALPOSITION;
        private Vector3 GOALPOSITIONR;
        private Vector3 GOALPOSITIONTemp;
        private Vector3 GOALPOSITIONRTempR;
        #endif

        /************************************************************************************************************************/

        /// <summary>Public property for a UI Toggle to enable or disable the IK.</summary>
        public bool ApplyAnimatorIK
        {
            get => _Animancer.Layers[0].ApplyAnimatorIK;
            set => _Animancer.Layers[0].ApplyAnimatorIK = value;
        }

        /************************************************************************************************************************/

        // Note that due to limitations in the Playables API, Unity will always call this method with layerIndex = 0.
        private void OnAnimatorIK(int layerIndex)
        {
//             if (runMixer != null)
//             {
//                 if (runMixer.IsPlaying)
//                 { 
//                     //Determine weight based off of y differences of raycast
//                     float runStage = surveyorWheel.RunStage;
//                     if (runStage < 1)
//                     {
//                         //it is between left plant(0) and right plant (1)
//                         rightFootWeight = mixerCurve.Evaluate(1.0f - runStage);
//                         leftFootWeight = mixerCurve.Evaluate(runStage);
//                     }
//                     else
//                     {
//                         //it is between right plant(1) and left plant (2)
//                         runStage -= 1.0f;
//                         leftFootWeight = mixerCurve.Evaluate(1.0f - runStage);
//                         rightFootWeight = mixerCurve.Evaluate(runStage);
//
//                     }
//                     animationController.SetModelYRunPos(heightCurve.Evaluate(runStage));
//                     UpdateFootIK(_LeftFoot, AvatarIKGoal.LeftFoot, leftFootWeight,
//                         _Animancer.Animator.leftFeetBottomHeight);
//                     UpdateFootIK(_RightFoot, AvatarIKGoal.RightFoot, rightFootWeight,
//                         _Animancer.Animator.rightFeetBottomHeight);
// #if DEBUG
//                     if (Constants.DebugIKRun)
//                     {
//                         Debug.Log("Left Weight: " + leftFootWeight + " Right Weight: " + rightFootWeight);// + " weightToUse: " + weightToUse);
//                     }
// #endif
//                 }
//             }
        }

        /************************************************************************************************************************/

        private void UpdateFootIK(Transform footTransform, AvatarIKGoal goal, float weight, float footBottomHeight)
        {
            if (weight == 0)
                return;
            
            Animator animator = _Animancer.Animator;
            animator.SetIKPositionWeight(goal, weight);

            // Get the local up direction of the foot.
            Quaternion rotation = animator.GetIKRotation(goal);
            Vector3 localUp = rotation * Vector3.up;
            Vector3 localPos = footTransform.position;
            localPos += localUp * _RaycastOriginY;
            
            Vector3 globalUp = Vector3.up;
            
            Vector3 position = footTransform.position;
            position += globalUp * _RaycastOriginY;

            float distance = _RaycastOriginY - _RaycastEndY;

            if (UnityEngine.Physics.Raycast(localPos, -globalUp, out var hit, distance, canStandOn))
            {
                // Use the hit point as the desired position.
                position = hit.point;
                position += globalUp * (footBottomHeight * 1.5f);
                animator.SetIKPosition(goal, position);
#if DEBUG
                if (Constants.DebugIKRun)
                {
                    if (goal == AvatarIKGoal.LeftFoot)
                    {
                        GOALPOSITION = position;
                    }
                    else
                    {
                        GOALPOSITIONR = position;
                    }
                }
#endif

                // Use the hit normal to calculate the desired rotation.
                Vector3 rotAxis = Vector3.Cross(localUp, hit.normal);
                float angle = Vector3.Angle(localUp, hit.normal);
                rotation = Quaternion.AngleAxis(angle, rotAxis) * rotation;
                
                animator.SetIKRotation(goal, rotation);
            }
            else// Otherwise simply stretch the leg out to the end of the ray.
            {
                position += globalUp * (footBottomHeight - distance);
                animator.SetIKPosition(goal, position);
#if DEBUG
                if (Constants.DebugIKRun)
                {
                    if (goal == AvatarIKGoal.LeftFoot)
                    {
                        GOALPOSITION = position;
                    }
                    else
                    {
                        GOALPOSITIONR = position;
                    }
                }
#endif
            }
        }

        private void OnDrawGizmos()
        {
#if DEBUG
            // if (Constants.DebugIKRun)
            // {
            //     Gizmos.color = Color.red;
            //     Animator animator = _Animancer.Animator;
            //     // Get the local up direction of the foot.
            //     Quaternion rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
            //     // Vector3 localUp = rotation * Vector3.up;
            //     //
            //     // Vector3 position = _LeftFoot.position;
            //     // position += localUp * _RaycastOriginY;
            //     Vector3 localUp = Vector3.up;
            //
            //     Vector3 position = _LeftFoot.position;
            //     position += localUp * _RaycastOriginY;
            //
            //     float distance = _RaycastOriginY - _RaycastEndY;
            //
            //     Gizmos.DrawLine(position, position - (localUp * distance));
            //
            //     Gizmos.DrawSphere(_LeftFoot.position, 0.05f);
            //     Gizmos.DrawSphere(_RightFoot.position, 0.05f);
            //     Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
            //     Gizmos.DrawSphere(GOALPOSITION, 0.05f);
            //
            //     Gizmos.color = Color.red;
            //     // Get the local up direction of the foot.
            //     Quaternion rotationR = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
            //     // Vector3 localUpR = rotationR * Vector3.up;
            //     //
            //     // Vector3 positionR = _LeftFoot.position;
            //     // positionR += localUpR * _RaycastOriginY;
            //     
            //     Vector3 localUpR = Vector3.up;
            //
            //     Vector3 positionR = _RightFoot.position;
            //     positionR += localUpR * _RaycastOriginY;
            //
            //     float distanceR = _RaycastOriginY - _RaycastEndY;
            //
            //     Gizmos.DrawLine(positionR, positionR - (localUpR * distanceR));
            //
            //     Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
            //     Gizmos.DrawSphere(GOALPOSITIONR, 0.05f);
            //     Gizmos.DrawLine(position, position - (localUp * _Animancer.Animator.leftFeetBottomHeight));
            //     Gizmos.color = Color.blue;
            //     Gizmos.DrawSphere(GOALPOSITIONTemp, 0.05f);
            //     Gizmos.DrawSphere(GOALPOSITIONRTempR, 0.05f);
            // }
#endif
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.Animation;
using __OasisBlitz.Utility;
using Animancer;
using Animancer.Units;
using UnityEngine;

// public class IdlePoseIK : MonoBehaviour
// {
//         /************************************************************************************************************************/
//
//         [SerializeField] private AnimancerComponent _Animancer;
//         [SerializeField] private BanditAnimationController animationController;
//         [SerializeField, Meters] private float _RaycastOriginY = 0.2f;
//         [SerializeField, Meters] private float _RaycastEndY = -0.25f;
//
//         /************************************************************************************************************************/
//
//         private Transform _LeftFoot;
//         private Transform _RightFoot;
//         
//         private float weightFactor = 3.0f;
//
//         [SerializeField] private LayerMask canStandOn;
//
//         public float weightToUse;
// #if DEBUG
//         private Vector3 GOALPOSITION;
//         private Vector3 GOALPOSITIONR;
//         private Vector3 GOALPOSITIONTemp;
//         private Vector3 GOALPOSITIONRTempR;
// #endif
//
//         /************************************************************************************************************************/
//
//         /// <summary>Public property for a UI Toggle to enable or disable the IK.</summary>
//         public bool ApplyAnimatorIK
//         {
//             get => _Animancer.Layers[0].ApplyAnimatorIK;
//             set => _Animancer.Layers[0].ApplyAnimatorIK = value;
//         }
//
//         /************************************************************************************************************************/
//
//         private void Awake()
//         {
//             _LeftFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
//             _RightFoot = _Animancer.Animator.GetBoneTransform(HumanBodyBones.RightFoot);
//
//             //_FootWeights = new AnimatedFloat(_Animancer, "LeftFootIK", "RightFootIK");
//
//             // Tell Unity that OnAnimatorIK needs to be called every frame.
//             ApplyAnimatorIK = true;
//         }
//
//         /************************************************************************************************************************/
//
//         // Note that due to limitations in the Playables API, Unity will always call this method with layerIndex = 0.
//         private void OnAnimatorIK(int layerIndex)
//         {
//             if (_Animancer.States[animationController.idle].IsPlaying)
//             {
//                 //Determine weight based off of y differences of raycast
//                 float distance = _RaycastOriginY - _RaycastEndY;
//                 //calculate raycast hit points
//                 // Get the local up direction of the foot.
//                 Vector3 localUpL = Vector3.up;
//
//                 Vector3 positionL = _LeftFoot.position;
//                 positionL += localUpL * _RaycastOriginY;
//
//                 float goalYPosL = positionL.y;
// #if DEBUG
//                 if (Constants.DebugIKIdle)
//                 {
//                     GOALPOSITIONTemp = positionL;
//                 }
// #endif
//
//                 if (Physics.Raycast(positionL, -localUpL, out var hitL, canStandOn))
//                 {
//                     goalYPosL = hitL.point.y;
// #if DEBUG
//                     if (Constants.DebugIKIdle)
//                     {
//                         GOALPOSITIONTemp = hitL.point;
//                     }
// #endif
//                 }
//
//                 Vector3 localUpR = Vector3.up;
//
//                 Vector3 positionR = _RightFoot.position;
//                 positionR += localUpR * _RaycastOriginY;
//
//                 float goalYPosR = positionR.y;
// #if DEBUG
//                 if (Constants.DebugIKIdle)
//                 {
//                     GOALPOSITIONRTempR = positionR;
//                 }
// #endif
//
//                 if (Physics.Raycast(positionR, -localUpR, out var hitR, canStandOn))
//                 {
//                     goalYPosR = hitR.point.y;
// #if DEBUG
//                     if (Constants.DebugIKIdle)
//                     {
//                         GOALPOSITIONRTempR = hitR.point;
//                     }
// #endif
//                 }
//
//                 float yDiff = Mathf.Clamp(Mathf.Abs(goalYPosL - goalYPosR), 0.0f, distance);
//                 //Closer yDifferences gives less weight
//                 weightToUse = (yDiff * weightFactor) / distance;
//                 weightToUse = Mathf.Clamp(weightToUse, 0.0f, 1.0f);
// #if DEBUG
//                 if (Constants.DebugIKIdle)
//                 {
//                     Debug.Log("Weight to use: " + weightToUse + " yDiff: " + yDiff);
//                     if (goalYPosR == positionR.y)
//                     {
//                         Debug.Log("R Cast Failed");
//                     }
//                     else if (goalYPosL == positionL.y)
//                     {
//                         Debug.Log("L Cast Failed");
//                     }
//                 }
// #endif
//                 // UpdateFootIK(_LeftFoot, AvatarIKGoal.LeftFoot, weightToUse, _Animancer.Animator.leftFeetBottomHeight);
//                 // UpdateFootIK(_RightFoot, AvatarIKGoal.RightFoot, weightToUse,
//                 //     _Animancer.Animator.rightFeetBottomHeight);
//                 //move model lower on uneven ground to let legs bend or raise model to prevent bend on flat surface
//                 animationController.SetModelYPos(weightToUse);
//                 UpdateFootIK(_LeftFoot, AvatarIKGoal.LeftFoot, 1.0f, _Animancer.Animator.leftFeetBottomHeight);
//                 UpdateFootIK(_RightFoot, AvatarIKGoal.RightFoot, 1.0f, _Animancer.Animator.rightFeetBottomHeight);
//             }
//         }
//
//         /************************************************************************************************************************/
//
//         private void UpdateFootIK(Transform footTransform, AvatarIKGoal goal, float weight, float footBottomHeight)
//         {
//             if (weight == 0)
//                 return;
//             
//             Animator animator = _Animancer.Animator;
//             animator.SetIKPositionWeight(goal, weight);
//
//             // Get the local up direction of the foot.
//             Quaternion rotation = animator.GetIKRotation(goal);
//             Vector3 localUp = Vector3.up;
//
//             Vector3 position = footTransform.position;
//             position += localUp * _RaycastOriginY;
//
//             float distance = _RaycastOriginY - _RaycastEndY;
//
//             if (Physics.Raycast(position, -localUp, out var hit, distance, canStandOn))
//             {
//                 // Use the hit point as the desired position.
//                 position = hit.point;
//                 position += localUp * footBottomHeight;
//                 animator.SetIKPosition(goal, position);
// #if DEBUG
//                 if (Constants.DebugIKIdle)
//                 {
//                     if (goal == AvatarIKGoal.LeftFoot)
//                     {
//                         GOALPOSITION = position;
//                     }
//                     else
//                     {
//                         GOALPOSITIONR = position;
//                     }
//                 }
// #endif
//
//                 // Use the hit normal to calculate the desired rotation.
//                 Vector3 rotAxis = Vector3.Cross(localUp, hit.normal);
//                 float angle = Vector3.Angle(localUp, hit.normal);
//                 rotation = Quaternion.AngleAxis(angle, rotAxis) * rotation;
//                 
//                 animator.SetIKRotation(goal, rotation);
//             }
//             else// Otherwise simply stretch the leg out to the end of the ray.
//             {
//                 position += localUp * (footBottomHeight - distance);
//                 animator.SetIKPosition(goal, position);
// #if DEBUG
//                 if (Constants.DebugIKIdle)
//                 {
//                     if (goal == AvatarIKGoal.LeftFoot)
//                     {
//                         GOALPOSITION = position;
//                     }
//                     else
//                     {
//                         GOALPOSITIONR = position;
//                     }
//                 }
// #endif
//             }
//         }
//
//         private void OnDrawGizmos()
//         {
// #if DEBUG
//             if (Constants.DebugIKIdle)
//             {
//                 Gizmos.color = Color.red;
//                 Animator animator = _Animancer.Animator;
//                 // Get the local up direction of the foot.
//                 Quaternion rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
//                 Vector3 localUp = rotation * Vector3.up;
//
//                 Vector3 position = _LeftFoot.position;
//                 position += localUp * _RaycastOriginY;
//
//                 float distance = _RaycastOriginY - _RaycastEndY;
//
//                 Gizmos.DrawLine(position, position - (localUp * distance));
//
//                 Gizmos.DrawSphere(_LeftFoot.position, 0.05f);
//                 Gizmos.DrawSphere(_RightFoot.position, 0.05f);
//                 Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
//                 Gizmos.DrawSphere(GOALPOSITION, 0.05f);
//
//                 Gizmos.color = Color.red;
//                 // Get the local up direction of the foot.
//                 Quaternion rotationR = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
//                 Vector3 localUpR = rotationR * Vector3.up;
//
//                 Vector3 positionR = _LeftFoot.position;
//                 positionR += localUpR * _RaycastOriginY;
//
//                 float distanceR = _RaycastOriginY - _RaycastEndY;
//
//                 Gizmos.DrawLine(positionR, positionR - (localUpR * distanceR));
//
//                 Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
//                 Gizmos.DrawSphere(GOALPOSITIONR, 0.05f);
//                 Gizmos.DrawLine(position, position - (localUp * _Animancer.Animator.leftFeetBottomHeight));
//                 
//                 Gizmos.color = Color.blue;
//                 Gizmos.DrawSphere(GOALPOSITIONTemp, 0.05f);
//                 Gizmos.DrawSphere(GOALPOSITIONRTempR, 0.05f);
//             }
// #endif
//         }
// }

using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Playables;

public class CobbleBodyPartMover : MonoBehaviour, IMoverController
{
    public PhysicsMover Mover;

    private Transform _transform;

    [SerializeField] private Transform shouldBeParentTransform;

    public bool newWay;

    private Vector3 _localPos;
    private Quaternion _localRot;

    private void Start()
    {
        _transform = transform;
        //_localPos = _transform.localPosition;
        _localRot = _transform.localRotation;
        _transform.position = shouldBeParentTransform.position;
        _transform.rotation = shouldBeParentTransform.rotation;
        Mover.MoverController = this;
    }
    
    // Remember pose before animation
    private Vector3 _positionBeforeAnim;
    private Quaternion _rotationBeforeAnim;

    public void SaveCurrPositionAndRotation()
    {
        _positionBeforeAnim = _transform.position;
        _rotationBeforeAnim = _transform.rotation;
    }
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        if (!newWay)
        {
            // Set our platform's goal pose to the animation's
            //TODO: make the goal rot and pos equal to the starting local pos in global space
            goalPosition = shouldBeParentTransform.position;
            goalRotation = shouldBeParentTransform.rotation;

            // Reset the actual transform pose to where it was before evaluating. 
            _transform.position = _positionBeforeAnim;
            _transform.rotation = _rotationBeforeAnim;
        }else
        {
            // Remember pose before animation
            Vector3 _positionBeforeAnim = _transform.position;
            Quaternion _rotationBeforeAnim = _transform.rotation;

            // Update animation
            AnimancerComponent _Animancer = GetComponent<AnimancerComponent>();
            _Animancer.Evaluate(deltaTime * -1.0f);

            // Set our platform's goal pose to the animation's
            goalPosition = _transform.position;
            goalRotation = _transform.rotation;

            // Reset the actual transform pose to where it was before evaluating. 
            // This is so that the real movement can be handled by the physics mover; not the animation
            _transform.position = _positionBeforeAnim;
            _transform.rotation = _rotationBeforeAnim;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(TransformExtensions.TransformPointUnscaled(_transform, _localPos), 20.0f);
    }
}

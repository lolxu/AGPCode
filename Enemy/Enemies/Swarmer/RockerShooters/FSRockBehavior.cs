using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class FSRockBehavior : MonoBehaviour
{
    [SerializeField] private Vector3 localStartPos;
    private Vector3 zAxis = new Vector3(0.0f, 0.0f, 1.0f);
    private Rigidbody rigidbody;
    private float launchSpeed;

    private void Awake()
    {
        localStartPos = transform.localPosition;
        rigidbody = GetComponent<Rigidbody>();
    }

    public void SetShootAtTargetLaunchSpeed(float newLaunchSpeed)
    {
        this.launchSpeed = newLaunchSpeed;
    }

    public void UpdatePosition(float deltaTime)
    {
        //Debug.Log("Pos: " + transform.position + " Local: " + transform.localPosition + " NewPoint: " + transform.TransformPoint(zAxis * deltaTime * launchSpeed));
        rigidbody.MovePosition(transform.TransformPoint(zAxis * deltaTime * launchSpeed));
    }

    public void ResetRock()
    {
        if (localStartPos != Vector3.zero)//this is to prevent the position reset before Awake
        {
            transform.localPosition = localStartPos;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerStateMachine>().InstantKill();
        }
    }
}
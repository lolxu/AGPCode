using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Camera.StateMachine;
using UnityEngine;
using UnityEngine.Splines;

public class CinematicsTrigger : MonoBehaviour
{
    [SerializeField] private SplineContainer cinematicsDolly;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraStateMachine.Instance.SwitchToCinematicsCamera(CinematicsType.CinematicsPan);
        }
    }
}

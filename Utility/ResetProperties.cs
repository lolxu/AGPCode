using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using UnityEngine;

public class ResetProperties : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector3 _startingPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void ResetObject()
    {
        
    }
}

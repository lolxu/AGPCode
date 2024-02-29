using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

public class ScreenVisualController : MonoBehaviour
{
    public Material SpeedLineMaterial;
    private PlayerStateMachine ctx;

    public float SpeedEnabledAlpha = 35;

    private void Awake()
    {
        ctx = GetComponent<PlayerStateMachine>();
    }

    private void Update()
    {
        Color speedColor = Color.white;
        bool bShouldShowSpeedLines = false;
        Vector3 LateralVelocity = ctx.PlayerPhysics.Velocity;
        LateralVelocity.y = 0;
        bShouldShowSpeedLines = LateralVelocity.magnitude >= ctx.PlayerPhysics.blitzSpeedThreshold * 1.5;
        speedColor.a = bShouldShowSpeedLines ? SpeedEnabledAlpha / 256.0f : 0;
        SpeedLineMaterial.SetColor("_Colour", speedColor);
    }

    private void OnDestroy()
    {
        SpeedLineMaterial.SetColor("_Colour", Color.clear);
    }
}

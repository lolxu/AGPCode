using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Cannon;
using UnityEngine;

public class HideOnGrapple : MonoBehaviour
{
    public DashTargetPoint CannonToGrapple;
    public ShowSpritesByDistance _SpritesByDistance;

    private void OnEnable()
    {
        CannonToGrapple.OnDashedToPoint += _SpritesByDistance.HideSprites;
    }

    private void OnDisable()
    {
        CannonToGrapple.OnDashedToPoint -= _SpritesByDistance.HideSprites;
    }
}

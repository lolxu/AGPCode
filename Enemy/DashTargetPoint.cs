using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using Animancer;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class DashTargetPoint : MonoBehaviour
{
    public bool isValidTarget = true;
    private bool isInRange = false;
    private bool isDashTarget = false;

    public Action OnDashedToPoint;

    public Transform leftGrappleAttachPoint;
    public Transform rightGrappleAttachPoint;

    void OnEnable()
    {
        isValidTarget = true;
    }
    
    public void SetAsTarget(float actualDashableRadius)
    {
        isInRange = true;
    }

    public void UnsetAsTarget()
    {
        isInRange = false;
    }

    public void SetDashable()
    {
        if (!isDashTarget)
        {
            isDashTarget = true;
        }
        
        HUDManager.Instance.SetDisplayDashPrompt(true);
    }

    public void UnsetDashable()
    {
        // Debug.Log("Here");
        if (isDashTarget)
        {
            isDashTarget = false;
        }
    }
}

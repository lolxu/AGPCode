using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.Animation;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

public class WaterDetector : MonoBehaviour
{
    private PlayerStateMachine Ctx;
    private BanditAnimationController _controller;

    private IEnumerator Start()
    {
        yield return null;
        Ctx = FindObjectOfType<PlayerStateMachine>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Ctx.InWaterTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Ctx.InWaterTrigger = false;
        }
    }
}

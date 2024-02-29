using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

public class ElixerEffect : MonoBehaviour
{
    [SerializeField] private PlayerStateMachine ctx;
    private void OnDestroy()
    {
        ctx.DrillixirManager.FullRefillDrillixir();
    }
}

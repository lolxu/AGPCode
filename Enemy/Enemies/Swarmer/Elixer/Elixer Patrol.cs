using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElixerPatrol : SwarmerPatrol
{
    public override void HandleEnterDeath()
    {
        base.HandleEnterDeath();
        playerStateMachine.DrillixirManager.FullRefillDrillixir();
    }
}

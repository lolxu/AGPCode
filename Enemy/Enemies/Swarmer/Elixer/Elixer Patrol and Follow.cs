using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElixerPatrolandFollow : SwarmerPatrolAndFollow
{
    public override void HandleEnterDeath()
    {
        base.HandleEnterDeath();
        playerStateMachine.DrillixirManager.FullRefillDrillixir();
    }
}

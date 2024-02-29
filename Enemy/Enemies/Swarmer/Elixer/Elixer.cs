using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elixer : Swarmer
{
    public override void HandleEnterDeath()
    {
        base.HandleEnterDeath();
        playerStateMachine.DrillixirManager.FullRefillDrillixir();
    }
}

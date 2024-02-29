using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

public class Launch : Swarmer
{
    [SerializeField] private VortexBehavior vortex;
    [SerializeField] private float vortexLifespan;
    public override void HandleEnterDeath()
    {
        vortex.StartVortex(vortexLifespan);
        base.HandleEnterDeath();
    }

}

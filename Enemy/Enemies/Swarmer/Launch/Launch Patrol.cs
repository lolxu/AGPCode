using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPatrol : SwarmerPatrol
{
    [SerializeField] private VortexBehavior vortex;
    [SerializeField] private float vortexLifespan;
    public override void HandleEnterDeath()
    {
        vortex.StartVortex(vortexLifespan);
        base.HandleEnterDeath();
    }
}

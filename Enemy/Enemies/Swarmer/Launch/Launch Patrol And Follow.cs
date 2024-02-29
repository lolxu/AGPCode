using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPatrolAndFollow : SwarmerPatrolAndFollow
{
    [SerializeField] private VortexBehavior vortex;
    [SerializeField] private float vortexLifespan;
    public override void HandleEnterDeath()
    {
        vortex.StartVortex(vortexLifespan);
        base.HandleEnterDeath();
    }
}

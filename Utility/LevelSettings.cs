using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelSettings/Ability Toggles")]
public class LevelSettings : ScriptableObject
{
    public bool isBlastEnabled = true;
    public bool isDashEnabled = true;
    public bool isDrillEnabled = true;

    // will default to burrow if left blank
    public string NextLevelOnCollectFlower = "";

    public void InitializePlayerAbilities(PlayerStateMachine ctx)
    {
        // BounceAbility.Instance.BounceEnabled = isBlastEnabled;
        ctx.TargetedDash.DashEnabled = isDashEnabled;
        ctx.ToggleDrill = isDrillEnabled;
    }
}

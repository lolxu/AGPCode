using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

public class LevelSettingsManager : MonoBehaviour
{
    private PlayerStateMachine ctx;
    public LevelSettings settings;
    private IEnumerator Start()
    {
        yield return null;
        if (settings != null)
        {
            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        
            settings.InitializePlayerAbilities(ctx);
        }
        else
        {
            Debug.LogError("Please assign the settings asset to the level ability manager.");
        }
    }
}

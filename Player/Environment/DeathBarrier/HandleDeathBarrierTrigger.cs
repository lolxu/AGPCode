using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

public class HandleDeathBarrierTrigger : MonoBehaviour
{
    public PlayerStateMachine playerStateMachine { set; private get; }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerStateMachine.InstantKillByDeathBarrier();
        }
    }
}

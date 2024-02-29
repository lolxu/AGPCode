using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableDeathBarriers : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += DisableBarriers;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= DisableBarriers;
    }

    private void DisableBarriers(Scene scene, LoadSceneMode mode)
    {
        GameObject[] DeathBarriers = GameObject.FindGameObjectsWithTag("DeathBarrier");
        GameObject playerObject = GameObject.FindWithTag("Player");
        PlayerStateMachine playerStateMachine = playerObject.GetComponent<PlayerStateMachine>();
        for (int i = 0; i < DeathBarriers.Length; i++)
        {
            MeshRenderer BarrierMesh = DeathBarriers[i].GetComponent<MeshRenderer>();
            if(BarrierMesh != null)
            {
                BarrierMesh.enabled = false;
            }
            HandleDeathBarrierTrigger DeathBarrierTrigger = DeathBarriers[i].GetComponent<HandleDeathBarrierTrigger>();
            if (DeathBarrierTrigger != null)
            {
                DeathBarrierTrigger.playerStateMachine = playerStateMachine;
            }
        }
    }
}

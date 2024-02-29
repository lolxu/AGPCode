using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using UnityEngine;

public class DebugTeleportButton : MonoBehaviour
{
    public CheckPoint checkpoint;
    public void Teleport()
    {
        if (checkpoint == null)
        {
            Debug.LogWarning("DebugTeleportButton: No checkpoint set");
            return;
        }

        RespawnManager.Instance.TeleportToCheckpoint(checkpoint);
        Debug.Log($"DebugTeleportButton: Teleport to {checkpoint.name}");
    }
}

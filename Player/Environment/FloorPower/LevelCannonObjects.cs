using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Cannon;
using UnityEngine;
using UnityEngine.UI;

public class LevelCannonObjects : MonoBehaviour
{
    public MeshRenderer FloorPowerMesh;
    public CannonTrigger LevelCannonTrigger;
    public CannonLevelTransit LevelTransit;
    public string loadSceneName = "";

    // this is constant per cannon:
    [SerializeField] private Material FloorPowerActiveMaterial;
    [SerializeField] private Material FloorPowerInactiveMaterial;

    /**
     * You can go to this level if and only if this is true
     */
    public void SetAvailable(bool isActive)
    {
        FloorPowerMesh.material = isActive ? FloorPowerActiveMaterial : FloorPowerInactiveMaterial;
        LevelCannonTrigger.IsUnlocked = isActive;
    }

    /**
     * You have completed this level if and only if this is true
     */
    public void SetCollected(bool bIsCollected)
    {
        
    }
}

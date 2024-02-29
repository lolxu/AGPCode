using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobbleGiantManager : MonoBehaviour
{
    public static CobbleGiantManager instance;
    [SerializeField] private List<CobbleGiantAnimationController> giantsList;
    public bool freshUpdate = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddGiant(CobbleGiantAnimationController giant)
    {
        giantsList.Add(giant);
    }
    
    public void RemoveGiant(CobbleGiantAnimationController giant)
    {
        giantsList.Remove(giant);
    }

    public void SaveAllGiantPartLocationAndRotation()
    {
        foreach (CobbleGiantAnimationController giant in giantsList)
        {
            if (giant.enabled)
            {
                giant.SaveCurrentPositionAndRotationOfEachPart();
            }
        }
    }
    
    //TODO: make this something a little different to make animations not all in sync or find another way (like making animations take different time or starting the animations at different times)
    public void EvaluateAllGiantAnimationGraphs(float deltaTime)
    {
        foreach (CobbleGiantAnimationController giant in giantsList)
        {
            if (giant.enabled)
            {
                giant.EvaluateAnimGraph(deltaTime);
            }
        }
    }
}

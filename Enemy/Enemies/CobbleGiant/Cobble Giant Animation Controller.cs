using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using __OasisBlitz.Player.Animation;
using Animancer;
using UnityEngine;

public class CobbleGiantAnimationController : MonoBehaviour
{
    [SerializeField] private List<CobbleBodyPartMover> allBodyColliders;
    //fresh update prevents EvaluateAllGiantAnimationGraphs from being called by each body part
    public bool newWay;
    [SerializeField] private AnimancerComponent _Animancer;
    [SerializeField] private AnimationClip idle;
    [SerializeField] private AnimationClip walk;
    [SerializeField] private AnimationClip crawl;
    
    void OnEnable()
    {
        foreach (CobbleBodyPartMover bodyPart in allBodyColliders)
        {
            bodyPart.newWay = newWay;
        }
        _Animancer.Playable.PauseGraph();
        //PlayIdle();
        PlayCrawl();
        //EvaluateAnimGraph(0.0f);
        //CobbleGiantManager.instance.AddGiant(this);
    }

    private void OnDisable()
    {
        //CobbleGiantManager.instance.RemoveGiant(this);
    }

    public void PlayIdle()
    {
        _Animancer.Play(idle);
        Debug.Log("Playing Idle");
    }
    
    public void PlayWalk()
    {
        _Animancer.Play(walk);
        Debug.Log("Playing Walk");
    }
    
    public void PlayCrawl()
    {
        _Animancer.Play(crawl);
        Debug.Log("Playing Crawl");
    }

    public void SaveCurrentPositionAndRotationOfEachPart()
    {
        if (!newWay)
        {
            foreach (CobbleBodyPartMover bodyPart in allBodyColliders)
            {
                bodyPart.SaveCurrPositionAndRotation();
            }
        }
        
    }

    public void EvaluateAnimGraph(float deltaTime)
    {
        if (!newWay)
        {
            //update the animation graph
            _Animancer.Playable.Evaluate(deltaTime);
        }
    }
}

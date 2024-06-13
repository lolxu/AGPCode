using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalCritterAnimations : CritterAnimations
{
    private List<List<string>> additionalCloverAnimation = new List<List<string>>();
    private List<List<string>> additionalCloverAnimationTrigger = new List<List<string>>();

    private void Awake()
    {
        //**bools**//
        //oldManIdle
        additionalCloverAnimation.Add(new List<string>());
        //injuredIdle
        additionalCloverAnimation.Add(new List<string>());
        //sadIdle
        additionalCloverAnimation.Add(new List<string>());
        //offensiveIdle
        additionalCloverAnimation.Add(new List<string>() {"offensiveDance"});
        //fightIdle
        additionalCloverAnimation.Add(new List<string>());
        //happyIdle
        additionalCloverAnimation.Add(new List<string>());
        //lookAroundIdle
        additionalCloverAnimation.Add(new List<string>());
        //defaultIdle
        additionalCloverAnimation.Add(new List<string>());
        
        //**Triggers**//
        //oldManIdle
        additionalCloverAnimationTrigger.Add(new List<string>());
        //injuredIdle
        additionalCloverAnimationTrigger.Add(new List<string>());
        //sadIdle
        additionalCloverAnimationTrigger.Add(new List<string>());
        //offensiveIdle
        additionalCloverAnimationTrigger.Add(new List<string>());
        //fightIdle
        additionalCloverAnimationTrigger.Add(new List<string>() {"punch"});
        //happyIdle
        additionalCloverAnimationTrigger.Add(new List<string>() {"reacting"});
        //lookAroundIdle
        additionalCloverAnimationTrigger.Add(new List<string>() {"surprised"});
        //defaultIdle
        additionalCloverAnimationTrigger.Add(new List<string>() {"talking"});
    }

    protected override void ResetAllAnimations()
    {
        base.ResetAllAnimations();
    }

    public override void SetAdditionalAnimation(int index, bool isSet, bool isTrigger)
    {
        base.SetAdditionalAnimation(index, isSet, isTrigger);
        Debug.Log("narrative: " + (int)currIdle + " : " + index + " : " + isSet + " : " + isTrigger);
        if (isTrigger)
        {
            if (isSet)
            {
                _animator.SetTrigger(additionalCloverAnimationTrigger?[(int)currIdle][index]);
            }
            else
            {
                _animator.ResetTrigger(additionalCloverAnimationTrigger?[(int)currIdle][index]);
            }
        }
        else
        {
            _animator.SetBool(additionalCloverAnimation?[(int)currIdle][index], isSet);
        }
    }

    public override void SwitchIdleState(Idles newState)
    {
        //Remove all additional animations
        if (currIdle != Idles.noIdle)
        {
            for (int i = 0; i < additionalCloverAnimation[(int)currIdle].Count; i++)
            {
                _animator.SetBool(additionalCloverAnimation[(int)currIdle][i], false);
            }
            for (int i = 0; i < additionalCloverAnimation[(int)currIdle].Count; i++)
            {
                _animator.ResetTrigger(additionalCloverAnimation[(int)currIdle][i]);
            }
        }
        base.SwitchIdleState(newState);
    }
}

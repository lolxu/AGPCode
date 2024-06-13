using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Yarn.Unity;

public class CritterAnimations : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private float timeToRevertRootMotion = 1.0f;
    protected Idles currIdle = Idles.noIdle;
    private Coroutine stayGrounded = null;
    public enum Idles
    {
        oldManIdle,//0
        injuredIdle,//1
        sadIdle,//2
        offensiveIdle,//3
        fightingIdle,//4
        happyIdle,//5
        lookAroundIdle,//6
        defaultIdle,//7
        noIdle//8 - This will also play when no other idle bools are set (same as default idle)
    }

    public void SetupInteractAnimations()
    {
        _animator.applyRootMotion = true;
    }

    public void DisableInteractAnimations()
    {
        ResetAllAnimations();
        //move model back into its collider
        _animator.applyRootMotion = false;
        modelTransform.DOKill();
        modelTransform.DOLocalMove(Vector3.zero, timeToRevertRootMotion);
        modelTransform.DOLocalRotate(Vector3.zero, timeToRevertRootMotion);
    }

    protected virtual void ResetAllAnimations()
    {
        //reset all bools for idle animations
        _animator.SetBool(Idles.oldManIdle.ToString(), false);
        _animator.SetBool(Idles.injuredIdle.ToString(), false);
        _animator.SetBool(Idles.sadIdle.ToString(), false);
        _animator.SetBool(Idles.offensiveIdle.ToString(), false);
        _animator.SetBool(Idles.fightingIdle.ToString(), false);
        _animator.SetBool(Idles.happyIdle.ToString(), false);
        _animator.SetBool(Idles.lookAroundIdle.ToString(), false);
        _animator.SetBool(Idles.defaultIdle.ToString(), false);
        SwitchIdleState(Idles.noIdle);
    }

    private void LateUpdate()
    {
        modelTransform.localPosition = new Vector3(modelTransform.localPosition.x, modelTransform.localPosition.y, 0.0f);
    }

    public virtual void SetAdditionalAnimation(int index, bool isSet, bool isTrigger) {}

    public virtual void SwitchIdleState(Idles newState)
    {
        //inherited overrides will stop second level animations
        if (currIdle != Idles.noIdle)
        {
            _animator.SetBool(currIdle.ToString(), false);
        }
        currIdle = newState;
        if (currIdle != Idles.noIdle)
        {
            _animator.SetBool(currIdle.ToString(), true);
        }
    }
}

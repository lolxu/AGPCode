using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FlagAnimation : MonoBehaviour
{
    public MeshRenderer _flagMesh;
    
    // .77 -> -.79
    private float _dissolve = .77f;

    private void Awake()
    {
        _flagMesh.material = new Material(_flagMesh.material);
    }

    public void SetFlagActive(bool bIsActive)
    {
        if (bIsActive)
        {
            _dissolve = .77f;
            _flagMesh.material.SetFloat("_DissolvePercent", _dissolve);
            // hard set scale to be tiny tiny
            transform.localScale = new Vector3(0, 0, 0);
            // hard set flag mesh to be invisible
            
            // tween the flag scale
            transform.DOScale(Vector3.one, .25f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                DOTween.To(() => _dissolve, x =>
                {
                    _dissolve = x;
                    _flagMesh.material.SetFloat("_DissolvePercent", _dissolve);
                }, -.79f, .4f).SetEase(Ease.OutCubic);
            });
            // once tween is over
        }
    }
}

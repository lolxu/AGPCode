using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlantArrow : MonoBehaviour
{
    private Vector3 localPos;
    private Quaternion rot;
    private Vector3 scale;
    [SerializeField] private float duration;
    [SerializeField] private float localMoveDown;
    [Range(0.0f, 1.0f)] [SerializeField] private float scaleXFactor;
    [SerializeField] private Ease easeType;
    private Sequence mySequence;
    
    
    
    private void Awake()
    {
        localPos = transform.localPosition;
        scale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localPosition = localPos;
        transform.localScale = scale;
        transform.DOLocalMoveY(localPos.y - localMoveDown, duration).SetLoops(-1, LoopType.Yoyo).SetEase(easeType);
        transform.DOScaleX(scaleXFactor * scale.x, duration).SetLoops(-1, LoopType.Yoyo).SetEase(easeType);
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}

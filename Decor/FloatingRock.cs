using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class FloatingRock : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.1f;
    [SerializeField] private float duration = 2f;

    // The duration can be randomized by this amount
    private const float durationOffsetRange = 1f;
    
    private TweenerCore<Vector3, Vector3, VectorOptions> bobTween;
    
    // Start is called before the first frame update
    void Start()
    {
        StartFloatingTween();
    }

    private void StartFloatingTween()
    {
        // Randomize offset
        float randomStartDelay = Random.Range(0f, duration);
        
        float realDuration = duration + Random.Range(-durationOffsetRange, durationOffsetRange);
        
        // Begin a DOTWeen tween that will move the rock up and down on a subtle sin wave
        bobTween = transform.DOMoveY(transform.position.y + bobHeight, realDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(randomStartDelay);
    }

    private void StopFloatingTween()
    {
        bobTween.Kill();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    // Start is called before the first frame update
    private Coroutine falling;
    [SerializeField] private float TimeToFall = 5f;
    [SerializeField] private float TimeToWait = 1.5f;
    [SerializeField] private float ShakeInterval = 0.2f;
    [SerializeField] private Ease FallEaseType = Ease.InCubic;
    [SerializeField] private Transform ToFollowTransform;

    // private void Start()
    // {
    //     WaitThenFall();
    // }

    public void WaitThenFall()
    {
        if (falling == null)
        {
            falling = StartCoroutine(WaitThenFallCoroutine());
        }
    }

    private IEnumerator WaitThenFallCoroutine()
    {
        ToFollowTransform.DOLocalMoveX(ToFollowTransform.localPosition.x + 0.05f, ShakeInterval).SetLoops((int)(TimeToWait / ShakeInterval), LoopType.Yoyo);
        yield return new WaitForSeconds(TimeToWait);
        ToFollowTransform.DOMoveY(ToFollowTransform.position.y - 1000.0f, TimeToFall).SetUpdate(UpdateType.Fixed).SetEase(FallEaseType).OnComplete(Fell);
    }

    private void Fell()
    {
        transform.parent.parent.gameObject.SetActive(false);
    }
}

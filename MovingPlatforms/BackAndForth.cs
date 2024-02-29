using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BackAndForth : MonoBehaviour
{
    [Header("Moving platform settings")]
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    [SerializeField] private float duration = 2.0f;
    [SerializeField] private bool canMoveOnItsOwn = true;

    [Header("Set these if you ticked off canMoveOnItsOwn")]
    [SerializeField] private GameObject startTrigger;
    [SerializeField] private GameObject endTrigger;
    // [Tooltip("Set a negative distance value to use the Vector3 value.")]
    // [SerializeField] private float distance;
    private Tween moveTween;
    private void Start()
    {
        // if (distance > 0.01f)
        // {
        //     // compatible with old parameters
        //     // TODO: delete this
        //     moveTween = transform.DOLocalMoveZ(transform.localPosition.z + distance / 2f, duration)
        //         .From(transform.localPosition.z - distance / 2f)
        //         .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad)
        //         .SetUpdate(UpdateType.Fixed).SetAutoKill(false);
        // }
        // else
        // {
        //     moveTween = transform.DOLocalMove(transform.localPosition + distanceVector/2f, duration)
        //         .From(transform.localPosition - distanceVector/2f)
        //         .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad)
        //         .SetUpdate(UpdateType.Fixed).SetAutoKill(false);
        // }
        if (!canMoveOnItsOwn && !startTrigger && !endTrigger)
        {
            Debug.LogError("Moving Platform cannot move on its own, but start and end triggers are not set!");
        }

        transform.localPosition = pointA.transform.localPosition;

        if (canMoveOnItsOwn)
        {
            StartMoving();
        }
    }

    public void StartMoving()
    {
        if (moveTween == null)
        {
            // Using two points to move the platform
            moveTween = transform.DOLocalMove(pointB.transform.localPosition, duration)
                .From(pointA.transform.localPosition)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad)
                .SetUpdate(UpdateType.Fixed)
                .SetAutoKill(false);
        }
        else
        {
            moveTween.Play();
        }
    }

    public void StopMoving()
    {
        moveTween.Pause();
    }
}

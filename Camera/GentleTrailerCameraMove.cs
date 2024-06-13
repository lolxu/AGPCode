using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GentleCameraMove : MonoBehaviour
{
    public float targetX; // Target x position
    public float duration = 2.0f; // Duration of the movement to the target

    void Start()
    {
        // Move to targetX and back to originalX repeatedly
        transform.DOMove(transform.position + transform.right * targetX, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}

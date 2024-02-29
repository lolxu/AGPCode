using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowCameraFollowPoint : MonoBehaviour
{
    private Transform playerOffsetTransform;
    public Vector3 Offset = Vector3.zero;

    private void Awake()
    {
        playerOffsetTransform = GameObject.FindGameObjectWithTag("PositionIndicator").transform;
        
        transform.position = playerOffsetTransform.position + Offset;
    }

    private void LateUpdate()
    {
        // transform.position = Vector3.Lerp( transform.position, playerOffsetTransform.position + Offset,
        // 1);
        if (transform != null)
        {
            transform.position = playerOffsetTransform.position + Offset;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using StylizedGrass;
using UnityEngine;
using UnityEngine.Serialization;

public class GrassBenderFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform transformToFollow;
    [SerializeField] private GrassBender grassBender;
    private Vector3 prevPos;
    private void Update()
    {
        Vector3 forward = transformToFollow.forward;
        forward.y = 0.0f;
        if (prevPos != transformToFollow.position)
        {
            prevPos = transformToFollow.position;
            transform.position = prevPos;
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class MoveSphereDown : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject enemy;
    [SerializeField] private float moveDown;
    [SerializeField] private float time;
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private Animator anim;

    private void Start()
    {
        StartCoroutine(startMove());
        
    }

    private IEnumerator startMove()
    {
        while (!cam.IsLive)
        {
            yield return null;
        }
        enemy.SetActive(true);
        anim.SetBool("aware", true);
        parent.DOMoveY(parent.position.y - moveDown, time).SetEase(Ease.InQuart).OnComplete(StartRunAndJump);
    }

    private void StartRunAndJump()
    {
        //TODO: start run and jump
        //anim.SetBool("aggro", true);
    }
}

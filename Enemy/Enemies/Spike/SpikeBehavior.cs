using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment;
using DG.Tweening;
using UnityEngine;

public class SpikeBehavior : MonoBehaviour
{
    [SerializeField] private Transform Cone;
    [SerializeField] private float ShootUpTime;
    [SerializeField] private float InitialDelay;

    private void OnEnable()
    {
        Cone.DOLocalMoveY(-2.0f, InitialDelay).OnComplete(ShootUp);
    }

    private void ShootUp()
    {
        Cone.DOLocalMoveY(1.92f, ShootUpTime).OnComplete(StayStill);
    }

    private void StayStill()
    {
        Cone.DOLocalMoveY(1.92f, ShootUpTime).OnComplete(MoveBack);
    }

    private void MoveBack()
    {
        Cone.DOLocalMoveY(-2.0f, ShootUpTime).OnComplete(Disable);
    }

    private void Disable()
    {
        ObjectPooler.Instance.Deallocate("DeathBarrier", gameObject);
    }
}

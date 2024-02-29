using System;
using UnityEngine;

namespace __OasisBlitz.__Scripts.MovingPlatforms
{
    public class MovingPlatformTrigger : MonoBehaviour
    {
        [SerializeField] private BackAndForth myMovingObj;
        [SerializeField] private bool isStart = false;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (isStart)
                {
                    myMovingObj.StartMoving();
                }
                else
                {
                    myMovingObj.StopMoving();
                }
            }
        }
    }
}
using System;
using __OasisBlitz.Camera.StateMachine;
using Unity.Cinemachine;
using UnityEngine;

namespace __OasisBlitz.__Scripts.UI
{
    public class LevelSelectTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject levelInterface;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                levelInterface.GetComponent<BurrowLevelInterface>().OpenLevelSelectInterface();
            }
        }
    }
}
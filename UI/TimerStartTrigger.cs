using System;
using System.Collections;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.StateMachine;
using Unity.Cinemachine;
using UnityEngine;

namespace __OasisBlitz.__Scripts.UI
{
    public class TimerStartTrigger : MonoBehaviour
    {
        private IEnumerator OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerStateMachine>().ModelRotator.SetFullDirection(transform.forward);
                
                // Start timer
                yield return null;
                yield return null;
                while (CameraStateMachine.Instance.CameraSurface.GetComponent<CinemachineBrain>().IsBlending)
                {
                    yield return null;
                }
                
                Debug.Log("Starting My Timer!");
                UIManager.Instance.SetTimer();
                CameraStateMachine.Instance.ResetCamera();
                gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 5.0f, Color.green);
        }
    }
}
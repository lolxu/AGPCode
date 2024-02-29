using System;
using System.Collections;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.__Scripts.Player.Environment.Checkpoints
{
    public class StartPoint : MonoBehaviour
    {
        [SerializeField] GameObject startingPlatform;
        private PlayerStateMachine ctx;

        public void DisableStartingPlatform()
        {
            startingPlatform.SetActive(false);
        }

        /// Editor Gizmo Draw Respawn Direction
        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 2.5f, Color.green);
        }
    }
}
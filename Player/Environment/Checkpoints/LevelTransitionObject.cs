using System;
using TMPro;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Player.Environment.Checkpoints
{
    public class LevelTransitionObject : CheckPoint
    {
        [Header("Settings for scenes")]
        [SerializeField] private string loadSceneName = "Level-101";
        private void Update()
        {
            myHint.transform.LookAt(2 * gameObject.transform.position - myCameraTransform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogError("ENTERED");
            if (other.gameObject.CompareTag("Player"))
            {
                // Drilling into the checkpoint breaks it
                // if (ctx.Drilling)
                // {
                    UIManager.Instance.StartLoadingScene(loadSceneName);
                // }
            }
        }
    }
}
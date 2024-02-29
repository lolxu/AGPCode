using System;
using System.Collections;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Collectables
{
    public class CritterSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject critterPrefab;
        [SerializeField] private bool canAlwaysSpawn;
        [SerializeField] private int associatedIndex = 0;
        
        private IEnumerator Start()
        {
            yield return null;

            if (CollectableManager.Instance.CheckIsSaved(associatedIndex) || canAlwaysSpawn)
            {
                GameObject critterInstance = Instantiate(critterPrefab, transform.position, Quaternion.LookRotation(transform.forward));
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5.0f);
        }
    }
}
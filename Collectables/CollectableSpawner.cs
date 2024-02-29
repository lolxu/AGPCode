using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace __OasisBlitz.__Scripts.Collectables
{
    public class CollectableSpawner : MonoBehaviour
    {
        [SerializeField] private CollectableType type;
        [SerializeField] private int collectableIndex;
        private IEnumerator Start()
        {
            yield return null;
            yield return null;

            if (type == CollectableType.PLANT)
            {
                CollectableManager.Instance.RequestForCollectableAppearance(0, collectableIndex, transform.position, transform.rotation);
            }
            else
            {
                CollectableManager.Instance.RequestForCollectableAppearance(1, collectableIndex, transform.position, transform.rotation);
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 5.0f, Color.green);
        }
    }
}
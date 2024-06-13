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

            CollectableManager.Instance.RequestForCollectableAppearance(type, collectableIndex, transform.position, transform.rotation);
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 5.0f, Color.green);
        }
    }
}
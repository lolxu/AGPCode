using System;
using Animancer;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace __OasisBlitz.__Scripts.Player.Environment
{
    public class PickupSpawner : MonoBehaviour
    {
        public GameObject pickup;
        public float distanceToLoad = 200.0f;

        private GameObject _myPlayer;
        private bool _hasSpawned = false;
        private bool _hasDeallocated = false;
        private ObjectPooler _myPool;
        private GameObject _currentAllocatedObject;

        private void Start()
        {
            _myPlayer = GameObject.FindGameObjectWithTag("Player");
            _myPool = ObjectPooler.Instance;
        }

        private void Update()
        {
            Vector3 pos = transform.position;
            if (Vector3.Distance(pos, _myPlayer.transform.position) < distanceToLoad)
            {
                if (!_hasSpawned)
                {
                    _currentAllocatedObject = _myPool.Allocate("Vitalizer", transform.position, pickup.transform.rotation);
                    _hasSpawned = true;
                    _hasDeallocated = false;
                }
            }
            else
            {
                if (!_hasDeallocated && _currentAllocatedObject != null)
                {
                    _hasSpawned = false;
                    _hasDeallocated = true;
                    _myPool.Deallocate("Vitalizer", _currentAllocatedObject);
                }
            }

            if (Vector3.Distance(pos, _myPlayer.transform.position) <= 2.5f)
            {
                Destroy(gameObject);
            }
            
        }
    }
}
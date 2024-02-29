using System;
using System.Collections.Generic;
using System.Linq;
using __OasisBlitz.Player.Environment;
using UnityEngine;
using UnityEngine.Serialization;

namespace __OasisBlitz.__Scripts.Player.Environment
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public String tag;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        public Dictionary<String, List<GameObject>> poolDictionary;
        
#region Singleton
        public static ObjectPooler Instance;
        private void Awake()
        {
            Instance = this;
        }
#endregion
        
        private void Start()
        {
            poolDictionary = new Dictionary<string, List<GameObject>>();

            foreach (Pool pool in pools)
            {
                List<GameObject> objectPool = new List<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);
                    objectPool.Add(obj);
                }
                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        /// <summary>
        /// Function to allocate gameobjects from the pool
        /// </summary>
        /// <param name="tag"> The type of gameobject </param>
        /// <param name="pos"> The position to spawn the object </param>
        /// <param name="rotation"> The rotation to spawn the object </param>
        /// <returns></returns>
        public GameObject Allocate(String tag, Vector3 pos, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogError("Pool with tag " + tag + " does not exist");
                return null;
            }

            GameObject objToSpawn = null;

            foreach (var obj in poolDictionary[tag])
            {
                if (!obj.activeInHierarchy)
                {
                    objToSpawn = obj;
                    break;
                }
            }

            if (objToSpawn != null)
            {
                objToSpawn.SetActive(true);
                // objToSpawn.transform.SetParent(null);
                objToSpawn.transform.position = pos;
                objToSpawn.transform.rotation = rotation;

                IPooledObject pooledObj = objToSpawn.GetComponent<IPooledObject>();
                if (pooledObj != null)
                {
                    pooledObj.OnObjectAllocate();
                }
            }
            return objToSpawn;
        }

        /// <summary>
        /// Function to Deallocate gameobjects from the pool.
        /// </summary>
        /// <param name="tag"> The type of gameobject </param>
        /// <param name="deallocateObject"> The gameobject to deallocate </param>
        public void Deallocate(String tag, GameObject deallocateObject)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogError("Pool with tag " + tag + " does not exist");
                return;
            }
            IPooledObject pooledObj = deallocateObject.GetComponent<IPooledObject>();
            if (pooledObj != null)
            {
                pooledObj.OnObjectDeallocate();
            }
            deallocateObject.SetActive(false); 
        }

        /// <summary>
        /// Deallocate all objects associated with the tag.
        /// </summary>
        /// <param name="tag"> The type of gameobject </param>
        public void DeallocateAll(String tag)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogError("Pool with tag " + tag + " does not exist");
                return;
            }
        }
    }
}
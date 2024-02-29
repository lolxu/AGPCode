using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Player.Environment.Chest
{
    public class Breakables : MonoBehaviour
    {
        [SerializeField] private List<GameObject> lootList;
        public bool bCanBounce = true;
        
        public IEnumerator SpawnLoot(Vector3 spawnPos)
        {
            yield return null;
            foreach (GameObject obj in lootList)
            {
                GameObject instantiatedObj = Instantiate(obj, spawnPos, obj.transform.rotation);
                instantiatedObj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(1.0f, 2.0f), Random.Range(1.0f, 2.0f), Random.Range(1.0f, 2.0f)) );

                // TODO make this to use object pooling later if there will be performance issues
                IPooledObject pooledObject = instantiatedObj.GetComponent<IPooledObject>();
                if (pooledObject != null)
                {
                    pooledObject.OnObjectAllocate();
                }
            }
        }
    }
}
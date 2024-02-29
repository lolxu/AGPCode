using UnityEngine;

namespace __OasisBlitz.__Scripts.Player.Environment
{
    public interface IPooledObject
    {
        void OnObjectAllocate();
        void OnObjectDeallocate();
    }
}
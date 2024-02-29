using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.VFX
{
    public class ShrinkVFX : MonoBehaviour
    {
        [SerializeField] private float lifetime = 1f;
        
        // Start is called before the first frame update
        void Start()
        {
            DOVirtual.DelayedCall(lifetime, () => Destroy(gameObject), false);
        }
    
    }
}

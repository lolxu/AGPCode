using UnityEngine;

namespace __OasisBlitz.Utility
{
    public class rotate : MonoBehaviour
    {
        public float speed = .25f;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0.0f, speed, 0.0f, Space.Self);
        }
    }
}
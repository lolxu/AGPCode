using UnityEngine;

namespace __OasisBlitz.Player
{
    public class TrailController : MonoBehaviour
    {
        private TrailRenderer _trailRenderer;
    
        // Start is called before the first frame update
        void Awake()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }

        // Create two functions, EnableTrail and DisableTrail. EnableTrail will enable the trail, and reset it 
        // so that it starts with zero length. Disabletrail will stop new trail from generating, but not immediately
        // destroy existing trail.
        public void EnableTrail()
        {
            if (_trailRenderer)
            {
                _trailRenderer.enabled = true;
                _trailRenderer.Clear();
            }
        }
    
        public void DisableTrail()
        {
            if (_trailRenderer)
            {
                _trailRenderer.enabled = false;
            }
        }
    }
}

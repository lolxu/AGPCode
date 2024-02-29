using UnityEngine;

namespace __OasisBlitz.Player
{
    public class SnapTrailController : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
    
        [SerializeField] private Vector3[] _positions;
    
        private int _positionIndex;
    
        [SerializeField] private Transform _playerTransform;
    
        [SerializeField] private int _maxPositions;

        private bool _isTrailActive;
    
        void Awake()
        {
            _positions = new Vector3[_maxPositions];
        
        }

        void Update()
        {
            if (_isTrailActive)
            {
                _positions[_positionIndex] = _playerTransform.position;
            
                _lineRenderer.SetPositions(_positions);
            }
        }

        public void StartTrail()
        {
            _positionIndex = 0;
            _isTrailActive = true;
            _lineRenderer.enabled = true;
        }

        public void EndTrail()
        {
            _isTrailActive = false;
            _lineRenderer.enabled = false;
        }
    
    
    }
}

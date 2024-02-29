using __OasisBlitz.Utility;
using UnityEngine;

namespace __OasisBlitz.Player
{
    public class SurfaceMarker : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _playerTransform;

        [SerializeField] private GameObject _visibleMarker;

        [SerializeField] private float _yOffset;
    
        private int _layerMask = (1 << Constants.PenetrableLayer);

        void Awake()
        {
            TrackPlayer();
        }
    
        public void SetVisible(bool visible)
        {
            _visibleMarker.SetActive(visible);
        }
    
        public void TrackPlayer()
        {
            transform.localPosition = new Vector3(0, _yOffset, 0);
        }
    
        public void GetSurfaceAbovePoint()
        {
            // The origin point for the raycast is the X and Z of the player's position
            // with the Y of the camera. Raycast straight down, and set the position of this marker to the player's X
            // and Z, with the Y of the hit point.
            // If nothing is hit (which should not be possible?), disable the marker and return
        
            Vector3 origin = new Vector3(_playerTransform.position.x, _cameraTransform.position.y, _playerTransform.position.z);
            RaycastHit hit;
            UnityEngine.Physics.Raycast(origin, Vector3.down, out hit, 1000, _layerMask);
        
            if (hit.collider != null)
            {
                transform.position = new Vector3(_playerTransform.position.x, hit.point.y + _yOffset, _playerTransform.position.z);
                // transform.position = new Vector3(_playerTransform.position.x, _playerTransform.position.y, _playerTransform.position.z);
            
            }
            else
            {
                gameObject.SetActive(false);
            }


        }
    }
}

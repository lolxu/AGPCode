using UnityEngine;

namespace __OasisBlitz.Player
{
    public class TargetSwitcher : MonoBehaviour
    {
        [SerializeField] private SurfaceMarker _surfaceMarker;
    
        private bool _isTrackingPlayer = true;

        public void UpdatePosition()
        {
            // Only need to manually move if tracking surface. When tracking player,
            // the GameObject is parented so it moves automatically with the player.
            if (!_isTrackingPlayer)
            {
                // _surfaceMarker.GetSurfaceAbovePoint();
            }
        }
    
        public void TrackPlayer()
        {
            _isTrackingPlayer = true;
            _surfaceMarker.SetVisible(false);
            _surfaceMarker.TrackPlayer();
        }
    
        public void TrackSurfaceMarker()
        {
            // _isTrackingPlayer = false;
            // _surfaceMarker.SetVisible(true);
        }
    }
}

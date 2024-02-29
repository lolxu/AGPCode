using __OasisBlitz.Utility;
using UnityEngine;

namespace __OasisBlitz.Player
{
    [RequireComponent(typeof(Spring1D))]
    public class DrillBehavior : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Spring1D _spring;
        [SerializeField] private DrillRotator _drillRotator;

        [SerializeField] private Quaternion _startRotation;
        [SerializeField] private Vector3 _startPosition;

        [SerializeField] private TrailRenderer _trailRenderer;

        private const float _drillDistance = 0.35f;

        private const float _unpreppedSize = 0.7f;
        private const float _preppedSize = 1.0f;

        private Vector3 _startScale;
        
        // Launching state
        private float launchVelocity = 40f;
        private Vector3 launchDirection;
        
        [SerializeField] private ProjectileDrill projectileDrill;

        private enum DrillState
        {
            Drilling,
            Shot,
            AttachedToTarget,
            Retracting,
            Off
        }
        
        private DrillState drillState = DrillState.Off;

        public bool visible { get; private set; }

        void Awake()
        {
            SetDrillInvisible();
            _startScale = transform.localScale;
            _startPosition = transform.localPosition;
            _startRotation = transform.localRotation;
            _spring.equilibriumPosition = 1.0f;
            
            // gameObject.SetActive(true);
        }
        
        void Update()
        {

            switch (drillState)
            {
                // create basic case for  each state
                case DrillState.Drilling:
                    break;
                case DrillState.Shot:
                    transform.position += launchDirection * (launchVelocity * Time.deltaTime);
                    break;
                case DrillState.AttachedToTarget:
                    break;
                case DrillState.Retracting:
                    break;
                case DrillState.Off:
                    break;
            }
            
            // Update scale based on juicy spring
            transform.localScale = _startScale * _spring.position;
        }

        public void ShootToTarget(Vector3 targetSurfacePosition)
        {
            drillState = DrillState.Shot;
            projectileDrill.ShootDrill(transform.position, targetSurfacePosition);
        }

        public void EndDash()
        {
            projectileDrill.HideDrill();
        }
        
        public void SetDirection(Vector3 direction)
        {
            transform.position = _playerTransform.position + (direction.normalized * _drillDistance);
            transform.rotation = Quaternion.LookRotation(direction);
        }

        public void StartDrillSpin()
        {
            _drillRotator.SetRotating(true);
        }

        public void StopDrillSpin()
        {
            _drillRotator.SetRotating(false);
        }

        public void SetDrillVisible()
        {
            if (visible) return;
            visible = true;
            gameObject.SetActive(true);
            _spring.position = 0.0f;
            _spring.equilibriumPosition = 1.0f;
            transform.localRotation = _startRotation;
            transform.localPosition = _startPosition;
            
            // Flash white when enabling
            
            _trailRenderer.emitting = true;

        }
        
        public void SetDrillInvisible()
        {
            visible = false;
            gameObject.SetActive(false);
            _spring.equilibriumPosition = 0.0f;
            
            // Flash white when disabling
            
            _trailRenderer.emitting = false;
        }

        public void DrillBlip()
        {
            _spring.velocity = 40f;
        }

        public void SetFast()
        {
            _drillRotator.SetFast();
        }

        public void SetSlow()
        {
            _drillRotator.SetSlow();
        }
    }
}
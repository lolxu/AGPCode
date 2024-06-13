using __OasisBlitz.Player.Physics;
using __OasisBlitz.Utility;
using DG.Tweening;
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
        public DrillDirection _drillDirection;
        
        [SerializeField] private Material _drillMaterial;
        [SerializeField] private MeshRenderer[] _drillMeshRenderers;
        [SerializeField] private Color defaultDrillColor;
        [SerializeField] private Color whiteDrillColor;

        private const float _drillDistance = 1.1f;
        [SerializeField] private float drillYOffset = 0.0f;

        private const float _unpreppedSize = 0.7f;
        private const float _preppedSize = 1.0f;

        private Vector3 _startScale;
        
        // Launching state
        private float launchVelocity = 40f;
        private Vector3 launchDirection;

        private Tween drillTween;
        

        public bool visible { get; private set; }

        void Awake()
        {
            SetDrillInvisible();
            _startScale = transform.localScale;
            _startPosition = transform.localPosition;
            _startRotation = transform.localRotation;
            _spring.equilibriumPosition = 1.0f;
            
            // Set the material of each mesh
            foreach (var meshRenderer in _drillMeshRenderers)
            {
                meshRenderer.material = _drillMaterial;
            }
            
        }
        
        void Update()
        {
            
            // Update scale based on juicy spring
            transform.localScale = _startScale * _spring.position;
            
            SetDirection(_drillDirection.GetDrillDirection());
        }
        
        private void SetDirection(Vector3 direction)
        {
            transform.position = _playerTransform.position + new Vector3(0, drillYOffset, 0) + (direction.normalized * _drillDistance);
            transform.rotation = Quaternion.LookRotation(direction);
        }

        public void StartDrill()
        {
            // Check if the drill is already visible, to avoid flashing white when certain animations re-trigger this
            if (!visible)
            {
                _drillMaterial.DOColor(whiteDrillColor, 0.1f).OnComplete(() =>
                {
                    _drillMaterial.DOColor(defaultDrillColor, 0.05f);
                });
            }

            DrillEnable();
            
            // Start a dotween to flash white for 0.05s
        }

        public void StopDrill()
        {
            // Start a dotween to flash white for 0.05s
            _drillMaterial.DOColor(whiteDrillColor, 0.1f).OnComplete(() =>
            {
                DrillDisable();
            });
        }

        private void DrillEnable()
        {
            SetDrillVisible();
            StartDrillSpin();
            
            
        }

        private void DrillDisable()
        {
            SetDrillInvisible();
            StopDrillSpin();
            
        }

        private void StartDrillSpin()
        {
            _drillRotator.SetRotating(true);
        }

        private void StopDrillSpin()
        {
            _drillRotator.SetRotating(false);
        }

        private void SetDrillVisible()
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
        
        private void SetDrillInvisible()
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
    }
}
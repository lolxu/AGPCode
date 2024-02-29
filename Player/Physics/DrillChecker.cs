using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using KinematicCharacterController;
using UnityEngine;

//TODO: this is not in fixed update.  We might want to add fixed update
//TODO: in fixed update: check if we are drilling.  If so, update the check for bounce and penetrable objects
namespace __OasisBlitz.Player.Physics
{
    public class DrillChecker : MonoBehaviour
    {
        private int _layerMaskPenetrable;
        private int _layerMaskBounce;
        private Vector3 _unstuckBounceVelocity;
        private Vector3 _prevPlayerPos;
        private Collider _lastBounceCushionUsed;
        
        // Used for logic that needs to know what terrain you are inside. Namely, the entry and exit velocity tweaks.
        public Collider _lastPenetrableUsed { get; private set; }
        public Rigidbody _CurrPhysicsMoverRigidbody { get; private set; }

        public bool IsInsideLargePenetrable;
        
        public Vector3 PrevPlayerPos
        {
            set { _prevPlayerPos = value; }
        }
        public Vector3 UnstuckBounceVelocity
        {
            get { return _unstuckBounceVelocity; }
        }
        [SerializeField] private float _unstuckBounceMultiplier;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private PlayerStateMachine _playerStateMachine;
        void Awake()
        {
            _layerMaskPenetrable = LayerMask.GetMask("Penetrable", "LargePenetrable");
            _layerMaskBounce = LayerMask.GetMask("Default");
        }
    
        public bool CheckCollidingWithDrillable()
        {
            // Unused linecast based approach
            // See if the line between the last position and the current position intersects penetrable ground
            /*
        bool result = Physics.Linecast(lastPoint, transform.position, _layerMask);

        lastPoint = transform.position;

        Debug.Log("Colliding with penetrable: " + result);
        return result;
        */
            _CurrPhysicsMoverRigidbody = null;
            Vector3 playerPos = transform.position;
            Collider[] colliders = UnityEngine.Physics.OverlapSphere(playerPos, 0.01f, _layerMaskPenetrable);

            bool collidesWithAnything = colliders != null && colliders.Length > 0;
            IsInsideLargePenetrable = false;
            
            // Cache whether the object you collided with is a large penetrable (so track the surface) or a small
            // penetrable (so track the player)
            if (collidesWithAnything)
            {
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.layer == Constants.LargePenetrableLayer)
                    {
                        IsInsideLargePenetrable = true;
                    }
                    
                    _lastPenetrableUsed = collider;
                    Transform parent = collider.transform.parent;
                    if (parent)
                    {
                        //check if the object we are in is a physics mover
                        if (parent.GetComponent<PhysicsMover>())
                        {
                            _CurrPhysicsMoverRigidbody = parent.GetComponent<Rigidbody>();
                        }
                    }
                }
            }
            // Debug.Log("Colliding with penetrable: " + result);
            return collidesWithAnything;
        }

        public Vector3 GetPenetrableNormal()
        {
            Vector3 playerPos = transform.position;

            Vector3 closestPoint = _lastPenetrableUsed.ClosestPointOnBounds(playerPos);
            
            // Debug.Log("Closest point: " + closestPoint + " player pos: " + playerPos);
            //
            // Vector3 normal = (playerPos - closestPoint).normalized;
            //
            // UnityEngine.Physics.SphereCast(Vector3.zero, 1, transform.forward, out RaycastHit hitInfo, 10, ~0,
            //     QueryTriggerInteraction.Collide);
            Vector3 normal = CreateRaycastHitFromCollider(_prevPlayerPos, _lastPenetrableUsed).normal;

            return normal;
        }
    
        //Checks if we are colliding with bounce pad and stores a velocity of -norm * _bounceMultiplier
        public bool CheckCollidingWithBouncePad()
        {
            Collider[] colliders = UnityEngine.Physics.OverlapSphere(_playerTransform.position, 0.8f, _layerMaskBounce, QueryTriggerInteraction.Collide);
            bool collidesWithAnything = colliders != null && colliders.Length > 0;
            // Cache whether the object you collided with is a large penetrable (so track the surface) or a small
            // penetrable (so track the player)
            if (collidesWithAnything)
            {
                bool surroundingsCollider = false;
                foreach (var currcollider in colliders)
                {
                    if (currcollider.CompareTag("BounceObjectSurroundings"))
                    {
                        surroundingsCollider = true;
                        break;
                    }
                }
                //TODO: use surroundings to check if we've hit 
                if (surroundingsCollider)
                {
                    bool result = UnityEngine.Physics.Linecast(_prevPlayerPos, _playerTransform.position, out RaycastHit hitInfo,
                        _layerMaskBounce);
                    if (result)
                    {
                        if (hitInfo.collider.CompareTag("BounceObject"))
                        {
                            Vector3 currToPrev = (_prevPlayerPos - _playerTransform.position).normalized;
                            UnityEngine.Debug.Log("Bounce Pos: " + hitInfo.point + " Prev Pos: " + _prevPlayerPos + " Curr Pos: " + _playerTransform.position);
                            _playerTransform.position = hitInfo.point + currToPrev * 0.4f;
                            //UnityEngine.Debug.Log("Updated Pos: " + _playerTransform.position);
                            //TODO: get normal of intersection
                            RaycastHit closest = CreateRaycastHitFromCollider(_playerTransform.position, hitInfo.collider);
                            Vector3 norm = closest.normal;
                            _unstuckBounceVelocity = norm * _unstuckBounceMultiplier;
                            _prevPlayerPos = _playerTransform.position;
                            return true;
                        }else if (hitInfo.collider.CompareTag("BounceObjectShell"))
                        {
                            //TODO: we hit the outside
                            _playerTransform.position = hitInfo.point;
                            Collider coll = hitInfo.collider;
                            // _playerStateMachine.ImpactWalkOnly(ref coll);
                        }
                    }
                }
            }
            _prevPlayerPos = _playerTransform.position;
            return false;

        }
        public static RaycastHit CreateRaycastHitFromCollider(Vector3 _rayOrigin, Collider _collider)
        {
            var colliderTr = _collider.transform;
            // Returns a point on the given collider that is closest to the specified location.
            // Note that in case the specified location is inside the collider, or exactly on the boundary of it, the input location is returned instead.
            // The collider can only be BoxCollider, SphereCollider, CapsuleCollider or a convex MeshCollider.
            var closestPoint = UnityEngine.Physics.ClosestPoint(_rayOrigin, _collider, colliderTr.position, colliderTr.rotation);
   
            if (_collider is MeshCollider {convex: false} meshCollider)
            {
                Debug.Log("do not use convex mesh-colliders as it does not deal well with physics at all. ");
                // This is not great. If we have complex meshColliders we will encounter issues.
                closestPoint = _collider.ClosestPointOnBounds(_rayOrigin);
            }
         
            var dir = (closestPoint - _rayOrigin).normalized;
            var ray = new Ray(_rayOrigin, dir);
            var hasHit = _collider.Raycast(ray, out var hitInfo, float.MaxValue);
         
            if (hasHit == false)
            {
                Debug.Log($"This case will never happen! IN DRILL Pos: " + _rayOrigin);
                //means we were going too fast
            
            }
 
            return hitInfo;
        }
        
        


    }
}

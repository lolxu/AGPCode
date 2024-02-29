using System.Collections.Generic;
using __OasisBlitz.__Scripts.Collectables;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using Animancer;
using KinematicCharacterController;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace __OasisBlitz.Player
{
    public enum CollisionMode
    {
        Default,
        Drilling,
        Dashing
    }

    public class CharacterController : MonoBehaviour, ICharacterController
    {
        [SerializeField] private KinematicCharacterMotor motor;

        [SerializeField] private TargetSwitcher targetSwitcher;

        [SerializeField] private PlayerPhysics playerPhysics;

        [SerializeField] private PlayerInput playerInput;

        [SerializeField] private PlayerStateMachine playerStateMachine;

        private CollisionMode collisionMode;

        private bool jumpRequested = false;

        //Add tags to this list that we want the player to collide with in drill mode
        [FormerlySerializedAs("TagsToCollideWithList")] [SerializeField] private List<string> TagsToCollideWithListDrilling;
        private HashSet<string> TagsToCollideWithSetDrilling = new HashSet<string>();
        
        [SerializeField] private List<string> TagsToCollideWithListDashing;
        private HashSet<string> TagsToCollideWithSetDashing = new HashSet<string>();

        private void Start()
        {
            motor.CharacterController = this;
            //populate set to allow for constant lookup of tag
            foreach (string tag in TagsToCollideWithListDrilling)
            {
                TagsToCollideWithSetDrilling.Add(tag);
            }
            
            foreach (string tag in TagsToCollideWithListDashing)
            {
                TagsToCollideWithSetDashing.Add(tag);
            }
        }

        public bool IsGrounded
        {
            get { return motor.GroundingStatus.IsStableOnGround && !jumpRequested; }
        }

        [HideInInspector] public bool IsDrillingInsideTerrain = false;

        public CollisionMode CollisionMode
        {
            set { collisionMode = value; }
        }

        public void RequestJump()
        {
            // The actual vertical velocity comes from PlayerPhysics, so this function only handles
            // ungrounding the player.
            motor.ForceUnground();
            jumpRequested = true;
        }

        public void RequestInteract()
        {
            // FruitsManager.Instance.RequestFruit();
            LevelManager.Instance.RequestTeleport();
            CollectableManager.Instance.RequestPickupCollectable();
            CameraStateMachine.Instance.StopCameraCinematics();
            if (SceneManager.GetActiveScene().name.Contains("Burrow"))
            {
                CheckCritterInteract checkCritterInteract = FindObjectOfType<CheckCritterInteract>();
                if (checkCritterInteract != null)
                {
                    checkCritterInteract.InteractWithCritter();
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.name.Contains("Interactable") || other.name.Contains("BackToLevel"))
            {
                HUDManager.Instance.SetCanInteract(true);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.name.Contains("Interactable") || other.name.Contains("BackToLevel"))
            {
                HUDManager.Instance.SetCanInteract(false);
            }
        }
        // *************************** ICharacterController Interface *************************** //
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (playerStateMachine.ToggleWalk)
            {
                playerPhysics.StepPlayerPhysics(playerInput.CurrentInputs.MoveInput, in currentVelocity,
                    deltaTime);
            }
            else
            {
                playerPhysics.StepPlayerPhysics(Vector3.zero, in currentVelocity,
                    deltaTime);
            }

            currentVelocity = playerPhysics.Velocity;
        }

        /// <summary>
        /// This method should be used SPARINGLY, it is necessary for teleporting the player to a new position, but
        /// for the majority of cases the player should be moved by the physics system.
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(Vector3 pos)
        {
            motor.SetPosition(pos);
        }

        /// <summary>
        /// This method can be used to snap a player to a new rotation. (Only used for respawn currently)
        /// </summary>
        /// <param name="quat"> The Quaternion rotation for the character</param>
        public void SetRotation(Quaternion quat)
        {
            motor.SetRotation(quat);
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public void GetRigidOfTerrainInside(out Rigidbody AttachOverride)
        {
            AttachOverride = null;
            if (IsDrillingInsideTerrain)
            {
                AttachOverride = playerStateMachine.DrillChecker._CurrPhysicsMoverRigidbody;
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Allow states to override the motor's grounding
            if (!playerStateMachine.ShouldObeyGroundSnap())
            {
                motor.ForceUnground();
            }
            
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            // Jump requested is only valid for 1 character controller update after sent by the state machine
            jumpRequested = false;
            // DEPRECATED FruitsManager.Instance.FinishRequestFruit();
            
            LevelManager.Instance.FinishRequestTeleport();
            // CameraStateMachine.Instance.CurrentState.RequestCameraSkip = false;
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            //if we are not drilling, collide with everything
            if (collisionMode == CollisionMode.Default)
            {
                return true;
            }

            //Check the flag of the object to determine if we should collide with it
            if (collisionMode == CollisionMode.Drilling && TagsToCollideWithSetDrilling.Contains(coll.tag))
            {
                return true;
            }
            
            if (collisionMode == CollisionMode.Dashing && TagsToCollideWithSetDashing.Contains(coll.tag))
            {
                return true;
            }

            return false; //we should pass through the object
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            playerStateMachine.HandleGroundHit(ref hitCollider, hitNormal, hitPoint);
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            // If we ever need additional detail about the movement hit, we can get it here from the HitStabilityReport
            playerStateMachine.HandleCollision(ref hitCollider, hitNormal, hitPoint);
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }
        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            UnityEngine.Debug.Log("Discrete hit: " + hitCollider.gameObject.name);
        }
    }
}
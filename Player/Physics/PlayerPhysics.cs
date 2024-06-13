using __OasisBlitz.Utility;
using DG.Tweening;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines.Interpolators;

namespace __OasisBlitz.Player.Physics
{
    public class PlayerPhysics : DrillDirection
    {
        public enum GravityMode
        {
            Jump,
            FreeFall,
            DrillAbove,
            DrillBelow,
            Grounded,
            Slide,
            Idle,
            Dash
        }

        public enum DragMode
        {
            Grounded,
            Airborne,
            Slide,
            Blitz,
            Submerged,
            DrillAbove,
            Idle,
            Dash,
            Ball
        }
        
        public enum InputMode
        {
            Grounded,
            Airborne,
            Slide,
            Blitz, // Airborne and going fast
            Idle,
            Dash,
            Submerged,
            Ball
        }
        
        public enum OnSurfaceType
        {
            Slide,
            SlideDanger,
            Penetrable,
            NotPenetrable,
            NotGrounded
        }

        public float terminalYVelocityFromGravity;

        public float maxGameplaySpeed;
        
        [Header("Blitz Speed Values")] 
        public float blitzSpeedThreshold = 40f;

        [Header("Grounded Movement Values")] 
        [SerializeField] private float maxGroundedSpeed = 20f;
        [SerializeField] private float stableMovementSharpness = 10f;
        public float groundedAccelerationSpeed;

        [Header("Slide Movement Values")] [SerializeField]
        private float slideMovementSharpness = 2f;
        public float maxSlideSpeedFromInput;
        public float slideAccelerationSpeed;
        public float maxSlideSpeed;
        
        [Header("Air Movement Values")]
        [SerializeField] private float maxAirSpeed = 20f;
        [SerializeField] private float airAccelerationSpeed = 100f;
        
        [Header("Submerged Movement Values")]
        [SerializeField] private float maxSubmergedSpeed = 20f;
        [SerializeField] private float submergedAccelerationSpeed = 100f;
        
        [Header("Ball Movement Values")]
        [SerializeField] private float maxBallSpeed = 20f;
        [SerializeField] private float ballAccelerationSpeed = 100f;
        
        
        [Header("Gravity Values")]
        [SerializeField] private float jumpGravity = 60f;
        [SerializeField] private float freeFallGravity = 80f;
        [SerializeField] private float drillAboveGravity = 100f;
        [SerializeField] private float drillBelowGravity = -100f;
        [SerializeField] private float groundedGravity = 0f;
        [SerializeField] private float slideGravity = 0f;
        [SerializeField] private float idleGravity = 0f;
        private const float dashGravity = 0f;

        [Header("Drag Values")]
        [SerializeField] private float groundedDrag = 1f;
        [SerializeField] private float airborneDrag = 0.3f;
        [SerializeField] private float slideDrag = 0.3f;
        [SerializeField] private float blitzDrag = 0.3f;
        [SerializeField] private float submergedDrag = 0.3f;
        [SerializeField] private float idleDrag = 0.3f;
        [SerializeField] private float ballDrag = 0.3f;
        [SerializeField] private float drillAboveDrag = 0.3f;
        
        [Header("Ball State Boost")]
        public float velocityBasedBoost = 0f;
        public float inputBasedBoost = 0f;
        public float maxBoostSpeed = 0f;
        
        
        private const float dashDrag = 0f;
        
        private float[] gravityValues;
        private float[] dragValues;

        [SerializeField] private KinematicCharacterMotor Motor;
        public float isStuckTimer { private set; get; } = 0f;
        public bool ApplyGravity { set; get; } = true;
        public Vector3 VelocityBeforeStuck { private set; get; }
        private Vector3 velocityUponUnstuck;

        public Vector3 currentGroundNormal;

        private float currentGroundedDragTime = 0.0f;
        private float maxGroundedDragTime = 3.0f;

        public bool Dashing;
        [SerializeField] private float SlideAccelerationToThreshold = 5.0f;
        
        [Header("Grant Values")]
        public bool PreventSlidingUpSlideSurface = false;
        [SerializeField] private float SlideUpwardFromInputPreventionFactor = 1.0f;
        [Space]
        public bool ReduceUphillDrag = false;
        [Range(0.0f, 1.0f)] [SerializeField] private float UphillDragCut = 0.0f;
        [Range(0.0f, 1.0f)] [SerializeField] private float UphillGravityCut = 0.5f;
        [Space]
        public bool IncreaseDownHillRunningAcceleration = false;
        [Range(0.0f, 1.0f)] [SerializeField] private float minimumDownhillDragFactor = 0.1f;

        void Awake()
        {
            gravityValues = new float[8];
            dragValues = new float[9];
            physicsSolver = new SimplePhysics();
            
            // TODO: This is a bit of a hack. It would be better to have a dictionary of gravity values, and a dictionary of drag values.
            // But they can't be serialized without using something like Odin Inspector, which we don't want to do.
            
            // Set the gravity values
            gravityValues[(int)GravityMode.Jump] = jumpGravity;
            gravityValues[(int)GravityMode.FreeFall] = freeFallGravity;
            gravityValues[(int)GravityMode.DrillAbove] = drillAboveGravity;
            gravityValues[(int)GravityMode.DrillBelow] = drillBelowGravity;
            gravityValues[(int)GravityMode.Grounded] = groundedGravity;
            gravityValues[(int)GravityMode.Slide] = slideGravity;
            gravityValues[(int)GravityMode.Idle] = idleGravity;
            gravityValues[(int)GravityMode.Dash] = dashGravity;

            // Set the drag values
            dragValues[(int)DragMode.Grounded] = groundedDrag;
            dragValues[(int)DragMode.Airborne] = airborneDrag;
            dragValues[(int)DragMode.Slide] = slideDrag;
            dragValues[(int)DragMode.Blitz] = blitzDrag;
            dragValues[(int)DragMode.Submerged] = submergedDrag;
            dragValues[(int)DragMode.DrillAbove] = drillAboveDrag;
            dragValues[(int)DragMode.Idle] = idleDrag;
            dragValues[(int)DragMode.Dash] = dashDrag;
            dragValues[(int)DragMode.Ball] = ballDrag;

            if (SlideUpwardFromInputPreventionFactor < 1.0f)
            {
                SlideUpwardFromInputPreventionFactor = 1.0f;
            }
            
        }

        private SimplePhysics physicsSolver;

        private Vector3 velocity;
        
        
        public Vector3 Velocity
        {
            get
            {
                if (physicsSolver != null)
                {
                    return physicsSolver.AppliedVelocity;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

        public override Vector3 GetDrillDirection()
        {
            return Velocity;
        }

        /// <summary>
        /// The change in velocity over the last frame.
        /// </summary>
        public Vector3 Acceleration { get; private set; }
        
        // These are not publicly gettable, because no outside class should need to observe them.
        // They can be set by the PlayerStateMachine.
        public GravityMode CurrentGravityMode { set; private get; }
        public DragMode CurrentDragMode { set; private get; }
        public InputMode CurrentInputMode { set; private get; }
        
        public OnSurfaceType CurrentOnSurfaceType { set; get; }
        
        public void AddForce(Vector3 force, float MaxResultantSpeed = 1000)
        {
            physicsSolver.AddForce(force, MaxResultantSpeed);
        }

        public void AddAcceleration(Vector3 acceleration, float MaxResultantSpeed = 1000)
        {
            physicsSolver.AddAcceleration(acceleration, MaxResultantSpeed);
        }
        
        public void AddVelocity(Vector3 velocity, float MaxResultantSpeed = 1000)
        {
            physicsSolver.AddVelocity(velocity, MaxResultantSpeed);
        }

        public void AddGravity(Vector3 gravity)
        {
        }
        
        /// <summary>
        /// This should be used sparingly, as it throws out any prior contributions to the velocity.
        /// </summary>
        /// <param name="velocity"></param>
        public void SetVelocity(Vector3 velocity)
        {
            physicsSolver.SetVelocity(velocity);
        }

        /// <summary>
        /// This applies Newton's third law to the player, and should be called whenever the player collides with something.
        /// </summary>
        /// <param name="surfaceNormal"></param>
        public void HandleContact(Vector3 surfaceNormal)
        {
            physicsSolver.HandleContact(surfaceNormal);
        }

        public void ZeroAcceleration()
        {
            physicsSolver.ZeroAcceleration();
        }
        //time to stay stuck, player velocity when timer ends
        public void SetStuck(float isStuckTimer, Vector3 velocityWhenUnstuck)
        {
            VelocityBeforeStuck = Velocity;
            this.isStuckTimer = isStuckTimer;
            ZeroAcceleration();
            velocityUponUnstuck = velocityWhenUnstuck;
            SetVelocity(Vector3.zero);
            physicsSolver.UpdateVelocity();
        }

        public void SetUnStuck()
        {
            this.isStuckTimer = 0.0f;
        }

        public bool CheckBlitzSpeed()
        {
            return Velocity.magnitude >= blitzSpeedThreshold;
        }

        private void HandleDrag(Vector3 moveInputVector, float deltaTime)
        {
            if (CurrentDragMode == DragMode.DrillAbove ||
                CurrentDragMode == DragMode.Submerged ||
                CurrentDragMode == DragMode.Airborne)
            {
                // For these drag modes, apply more vertical drag than horizontal drag
                // float dragHorizontalFactor = 0.125f;
                float dragHorizontalFactor = 1f;
                physicsSolver.ApplyDrag(new Vector3(dragValues[(int)CurrentDragMode] * dragHorizontalFactor,
                    dragValues[(int)CurrentDragMode],
                    dragValues[(int)CurrentDragMode] * dragHorizontalFactor), deltaTime);
            }
            else
            {
                //TODO: Make this idle
                if (CurrentDragMode == DragMode.Idle)
                {
                    //Drag is more intense when you are going SLOWER by a quadratic factor
                    float maxGroundDrag = 100.0f;
                    float currThreshold = 1 - (physicsSolver.CurrentVelocity.magnitude/blitzSpeedThreshold);
                    currThreshold *= currThreshold;
                    currThreshold *= maxGroundDrag;
                    if (currThreshold < 1.0f)
                    {
                        currThreshold = 1.0f;
                    }
                    physicsSolver.ApplyDragNotBasedOnSpeed(currThreshold, deltaTime);
                    
                }else if (CurrentDragMode == DragMode.Slide && ReduceUphillDrag)
                {
                    Vector2 tempCurrVelXZ = new Vector2(physicsSolver.CurrentVelocity.x, physicsSolver.CurrentVelocity.z);
                    Vector2 groundXZ = new Vector2(Motor.GroundingStatus.GroundNormal.x,
                        Motor.GroundingStatus.GroundNormal.z);
                    //We don't want to prevent slide direction on a nearly flat or flat plane
                    if (groundXZ.magnitude > 0.01f && Vector2.Dot(groundXZ,tempCurrVelXZ) < 0)
                    {
                        //if we are pointing up hill, minimize Drag
                        physicsSolver.ApplyDrag(dragValues[(int)CurrentDragMode] * UphillDragCut, deltaTime);
                    }
                    else
                    {
                        //if we are flat or downhill, have normal drag
                        physicsSolver.ApplyDrag(dragValues[(int)CurrentDragMode], deltaTime);
                    }
                }
                else if (Motor.GroundingStatus.IsStableOnGround && CurrentDragMode == DragMode.Grounded && IncreaseDownHillRunningAcceleration)
                {
                    Vector2 tempCurrVelXZ = new Vector2(physicsSolver.CurrentVelocity.x,
                        physicsSolver.CurrentVelocity.z);
                    Vector2 groundXZ = new Vector2(Motor.GroundingStatus.GroundNormal.x,
                        Motor.GroundingStatus.GroundNormal.z);
                    //We want to reduce drag the more a player is running in the downward direction
                    float dot = Vector2.Dot(groundXZ, tempCurrVelXZ);
                    if (groundXZ.magnitude > 0.01f && dot > 0.0f)
                    {
                        physicsSolver.ApplyDrag(Mathf.Clamp(dragValues[(int)CurrentDragMode] * (1 - dot), dragValues[(int)CurrentDragMode] * minimumDownhillDragFactor, dragValues[(int)CurrentDragMode]), deltaTime);
                    }
                    else
                    {
                        physicsSolver.ApplyDrag(dragValues[(int)CurrentDragMode], deltaTime);
                    }
                }else
                {
                    physicsSolver.ApplyDrag(dragValues[(int)CurrentDragMode], deltaTime);
                }
            }
        }
        
        /// <summary>
        /// Take player input and update the physics solver based on current forces
        /// </summary>
        public void StepPlayerPhysics(Vector3 moveInputVector, in Vector3 moveablePlatformExitMomentum, float deltaTime)
        {
            Vector3 previousVelocity = velocity;
            
            currentGroundNormal = Motor.GroundingStatus.GroundNormal;
            
            // Requirements:
            // Handle applied forces and external velocities
            // Handle player input, clamping based on current velocity from applied forces
            // Set velocity for SimplePhysics to new velocity
            if (isStuckTimer <= 0f)
            {
                // Add all current forces to the physics solver
                if (ApplyGravity)
                {
                    Vector3 gravity = gravityValues[(int)CurrentGravityMode] * Vector3.down;
                    // If you're grounded, the ground applies a force to you along its normal
                    if (Motor.GroundingStatus.IsStableOnGround && CurrentDragMode == DragMode.Slide)
                    {
                        if (PreventSlidingUpSlideSurface && CurrentOnSurfaceType != OnSurfaceType.Slide)
                        {
                            Vector2 tempCurrVelXZ = new Vector2(physicsSolver.CurrentVelocity.x,
                                physicsSolver.CurrentVelocity.z);
                            Vector2 groundXZ = new Vector2(Motor.GroundingStatus.GroundNormal.x,
                                Motor.GroundingStatus.GroundNormal.z);
                            //We don't want to prevent slide direction on a nearly flat or flat plane
                            if (groundXZ.magnitude > 0.01f)
                            {
                                if (Vector2.Dot(groundXZ, tempCurrVelXZ) < 0)
                                {
                                    //if we are going uphill and sliding, reduce gravity
                                    gravity *= UphillGravityCut;
                                }
                            }
                        }
                        

                        Vector3 groundNormal = Motor.GroundingStatus.GroundNormal;
                        Vector3 groundForce = physicsSolver.CalculateResultantGravityForceVector(gravity, groundNormal);
                        // Debug.Log("Gravity mode is " + CurrentGravityMode);
                        // Debug.Log("Ground force " + groundForce);
                        // Debug.Log("Ground normal " + groundNormal);
                        physicsSolver.AddAcceleration(groundForce, maxSlideSpeed);
                    }
                    else
                    {
                        physicsSolver.ApplyGravity(gravity, terminalYVelocityFromGravity);
                        // Debug.Log("Applying gravity value " + gravity);
                    }
                    
                }
                
                // Step the physics solver
                physicsSolver.UpdateVelocity();

                Vector3 velocityPlusMovement = HandleInputVelocity(moveInputVector, deltaTime);
                physicsSolver.SetVelocity(velocityPlusMovement);

                // if (Motor.GroundingStatus.)
                // physicsSolver.ApplyDrag(dragValues[(int)CurrentDragMode], deltaTime);
                HandleDrag(moveInputVector, deltaTime);
            }
            else
            {
                isStuckTimer -= Time.deltaTime;
                if (isStuckTimer <= 0f)
                {
                    //set velocity as this is the last frame before being unstuck
                    SetVelocity(velocityUponUnstuck);
                    physicsSolver.UpdateVelocity();
                }
            }
            
            //Add momentum from detaching from a moving platform
            physicsSolver.SetVelocity(physicsSolver.CurrentVelocity + moveablePlatformExitMomentum);

            // Update the acceleration
            Acceleration = (physicsSolver.CurrentVelocity - previousVelocity) / deltaTime;

            // Debug.Log("PLAYER VELOCITY: " + physicsSolver.CurrentVelocity.magnitude);
            // Debug.Log("PLAYER VELOCITY: " + physicsSolver.CurrentVelocity);
            
        }

        private Vector3 HandleInputVelocity(Vector3 moveInputVector, float deltaTime)
        {
            if (CurrentInputMode == InputMode.Airborne)
            {
                return HandleAirMovement(moveInputVector, maxAirSpeed, airAccelerationSpeed, deltaTime);
            }
            if (CurrentInputMode == InputMode.Grounded)
            {
                // return HandleGroundMovement(moveInputVector, deltaTime);
                return HandleGroundMovement(moveInputVector, deltaTime);
            }
            if (CurrentInputMode == InputMode.Slide)
            {
                // return HandleSlideMovement(moveInputVector, deltaTime);
                return HandleSlideMovement(moveInputVector, deltaTime);
            }

            if (CurrentInputMode == InputMode.Submerged)
            {
                return HandleAirMovement(moveInputVector, maxSubmergedSpeed, submergedAccelerationSpeed, deltaTime);
            }

            if (CurrentInputMode == InputMode.Ball)
            {
                return HandleAirMovement(moveInputVector, maxBallSpeed, ballAccelerationSpeed, deltaTime);
            }

            if (CurrentInputMode == InputMode.Idle)
            {
                return physicsSolver.CurrentVelocity;
            }

            if (CurrentInputMode == InputMode.Dash)
            {
                return physicsSolver.CurrentVelocity;
            }

            Debug.Log("This should not be reachable! Invalid input mode in Player Physics");
            
            return Vector3.zero;
        }

        // The foundation for this code comes from the example character controller provided by the KinematicCharacterController asset
        private Vector3 HandleAirMovement(Vector3 moveInputVector, float maxSpeed, float accelerationSpeed, float deltaTime)
        {
            Vector3 currentVelocity = physicsSolver.CurrentVelocity;
            if (moveInputVector.sqrMagnitude > 0f)
            {
                Vector3 addedVelocity = moveInputVector * accelerationSpeed * deltaTime;

                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                // Limit air velocity from inputs
                if (currentVelocityOnInputsPlane.magnitude < maxSpeed)
                {
                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity,
                        maxSpeed);
                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                }
                else
                {
                    // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                    {
                        addedVelocity =
                            Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                    }
                }

                // Prevent air-climbing sloped walls
                if (Motor.GroundingStatus.FoundAnyGround)
                {
                    if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                    {
                        Vector3 perpenticularObstructionNormal = Vector3
                            .Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal),
                                Motor.CharacterUp).normalized;
                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                    }
                }

                // Apply added velocity
                currentVelocity += addedVelocity;
            }

            return currentVelocity;
        }

        // The foundation for this code comes from the example character controller provided by the KinematicCharacterController asset
        // private Vector3 HandleGroundMovement(Vector3 moveInputVector, float deltaTime)
        // {
        //     Vector3 currentVelocity = physicsSolver.CurrentVelocity;
        //
        //     float currentVelocityMagnitude = physicsSolver.CurrentVelocity.magnitude;
        //
        //     Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
        //
        //     // Reorient velocity on slope
        //     currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) *
        //                       currentVelocityMagnitude;
        //
        //     // Calculate target velocity
        //     Vector3 inputRight = Vector3.Cross(moveInputVector, Motor.CharacterUp);
        //     Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized *
        //                               moveInputVector.magnitude;
        //     Vector3 targetMovementVelocity = reorientedInput * maxGroundedSpeed;
        //
        //     // Smooth movement Velocity
        //     currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity,
        //         1f - Mathf.Exp(-stableMovementSharpness * deltaTime));
        //
        //     return currentVelocity;
        //
        //
        // }
        public Vector3 HandleGroundMovement(Vector3 moveInputVector, float deltaTime)
        {
            Vector3 currentVelocity = physicsSolver.CurrentVelocity;
            if (moveInputVector.sqrMagnitude > 0f)
            {
                Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
                Vector3 inputRight = Vector3.Cross(moveInputVector, Motor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized *
                                          moveInputVector.magnitude;
                
                Vector3 addedVelocity = reorientedInput * groundedAccelerationSpeed * deltaTime;

                // Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);
                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.GroundingStatus.GroundNormal);

                // Limit air velocity from inputs
                if (currentVelocityOnInputsPlane.magnitude < maxGroundedSpeed)
                {
                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity,
                        maxGroundedSpeed);
                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                }
                else
                {
                    // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                    {
                        addedVelocity =
                            Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                    }
                }
                
                // Apply added velocity
                currentVelocity += addedVelocity;
            }

            return currentVelocity;
        }

        private Vector3 HandleSlideMovement(Vector3 moveInputVector, float deltaTime)
        {
            Vector3 currentVelocity = physicsSolver.CurrentVelocity;
            if (moveInputVector.sqrMagnitude > 0f)
            {
                // Calculate target velocity
                Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
                Vector3 inputRight = Vector3.Cross(moveInputVector, Motor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized *
                                         moveInputVector.magnitude;
                  // Vector3 reorientedInput = Vector3.ProjectOnPlane(moveInputVector, effectiveGroundNormal);
                
                
                Vector3 addedVelocity = reorientedInput * slideAccelerationSpeed * deltaTime;

                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.GroundingStatus.GroundNormal);

                // Limit air velocity from inputs
                if (currentVelocityOnInputsPlane.magnitude < maxSlideSpeedFromInput)
                {
                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity,
                        maxSlideSpeedFromInput);
                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                }
                else
                {
                    // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                    {
                        addedVelocity =
                            Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                    }
                }
                
                if (CurrentOnSurfaceType == OnSurfaceType.Slide && PreventSlidingUpSlideSurface)
                {
                    Vector2 tempCurrVelXZ = new Vector2(currentVelocity.x, currentVelocity.z);
                    tempCurrVelXZ += new Vector2(addedVelocity.x, addedVelocity.z);
                    Vector2 groundXZ = new Vector2(Motor.GroundingStatus.GroundNormal.x,
                        Motor.GroundingStatus.GroundNormal.z);
                    //We don't want to prevent slide direction on a nearly flat or flat plane
                    if (groundXZ.magnitude > 0.01f)
                    {
                        if (Vector2.Dot(groundXZ,tempCurrVelXZ) < 0)
                        {
                            //if we are pointing up hill, move toward down hill
                            currentVelocity += (new Vector3(groundXZ.x, 0.0f, groundXZ.y) * SlideUpwardFromInputPreventionFactor);
                            if (currentVelocity.y > 0.0f)
                            {
                                currentVelocity.y = 0.0f;
                            }
                            if (currentVelocity.magnitude < blitzSpeedThreshold)
                            {
                                float speedLeft = blitzSpeedThreshold - currentVelocity.magnitude;
                                float toAdd = speedLeft * Mathf.Min(1f,SlideAccelerationToThreshold / speedLeft * Time.deltaTime);
                                currentVelocity += toAdd * currentVelocity.normalized;
                            }
                            return currentVelocity;
                        }
                    }
                }
                // Apply added velocity
                currentVelocity += addedVelocity;
            }
            else
            {
                if (CurrentOnSurfaceType == OnSurfaceType.Slide && PreventSlidingUpSlideSurface)
                {
                    Vector2 tempCurrVelXZ = new Vector2(currentVelocity.x, currentVelocity.z);
                    Vector2 groundXZ = new Vector2(Motor.GroundingStatus.GroundNormal.x,
                        Motor.GroundingStatus.GroundNormal.z);
                    //We don't want to prevent slide direction on a nearly flat or flat plane
                    if (groundXZ.magnitude > 0.01f)
                    {
                        if (Vector2.Dot(groundXZ,tempCurrVelXZ) < 0)
                        {
                            //if we are pointing up hill, move toward down hill
                            currentVelocity += (new Vector3(groundXZ.x, 0.0f, groundXZ.y) * SlideUpwardFromInputPreventionFactor);
                            if (currentVelocity.y > 0.0f)
                            {
                                currentVelocity.y = 0.0f;
                            }
                            if (currentVelocity.magnitude < blitzSpeedThreshold)
                            {
                                float speedLeft = blitzSpeedThreshold - currentVelocity.magnitude;
                                float toAdd = speedLeft * Mathf.Min(1f,SlideAccelerationToThreshold / speedLeft * Time.deltaTime);
                                currentVelocity += toAdd * currentVelocity.normalized;
                            }
                            return currentVelocity;
                        }
                    }
                }
            }
            
            if (currentVelocity.magnitude < blitzSpeedThreshold)
            {
                float speedLeft = blitzSpeedThreshold - currentVelocity.magnitude;
                float toAdd = speedLeft * Mathf.Min(1f,SlideAccelerationToThreshold / speedLeft * Time.deltaTime);
                currentVelocity += toAdd * currentVelocity.normalized;
            }
            return currentVelocity;
            
        }
        public bool WalkSpeedFromGravityOnly()
        {
            Debug.Log(Velocity);
            if (Velocity.magnitude - groundedGravity * Time.deltaTime < 0.05f)
            {
                return true;
            }
            return false;
        }
        // Old, ground based slide movement
        // private Vector3 HandleSlideMovement(Vector3 moveInputVector, float deltaTime)
        // {
        //     Vector3 currentVelocity = physicsSolver.CurrentVelocity;
        //
        //     float currentVelocityMagnitude = physicsSolver.CurrentVelocity.magnitude;
        //     
        //     Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
        //     
        //     // Reorient velocity on slope
        //     currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) *
        //                       currentVelocityMagnitude;
        //     
        //     // Calculate target velocity
        //     Vector3 inputRight = Vector3.Cross(moveInputVector, Motor.CharacterUp);
        //     Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized *
        //                               moveInputVector.magnitude;
        //     
        //     // Old slide code
        //     // Default is to slide forward
        //     if (reorientedInput.magnitude < 0.1f)
        //     {
        //         reorientedInput = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp).normalized;
        //     }
        //     
        //     Vector3 targetMovementVelocity = reorientedInput.normalized * currentVelocityMagnitude;
        //     
        //     // Smooth movement Velocity
        //     currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity,
        //         1f - Mathf.Exp(-slideMovementSharpness * deltaTime));
        //     
        //     return currentVelocity;
        // }

        /// <summary>
        /// Applies the reflected velocity given a hit normal
        /// </summary>
        /// <param name="hitNormal">normal vector for the collision</param>
        /// <param name="reflectionPoint"></param>
        /// <param name="magnitude">defaults to 0.0f which uses original velocity magnitude</param>
        public void ReflectVelocity(Vector3 hitNormal, Vector3 reflectionPoint, float magnitude = 0.0f)
        {
            // Calculate reflection direction
            if (magnitude == 0.0f)
            {
                magnitude = Velocity.magnitude;
            }
            // Debug.Log(reflectionPoint);
            
            Vector3 curDirection = Velocity.normalized;
            Vector3 reflectDirection = curDirection - 2 * Vector3.Dot(curDirection, hitNormal) * hitNormal;

            // Debug.Log(reflectDirection);
            // Debug.DrawLine(transform.position, transform.position + curDirection.normalized * 50.0f, Color.blue, 30.0f);
            // Debug.DrawLine(reflectionPoint, reflectionPoint + reflectDirection.normalized * 50.0f, Color.red, 30.0f);
            
            // Removes all velocity for now
            ZeroAcceleration();
            SetVelocity(Vector3.zero);
            
            // Debug.Log(reflectDirection.normalized * magnitude);

            AddVelocity(reflectDirection.normalized * magnitude);
        }
    }
}
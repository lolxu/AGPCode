using UnityEngine;

namespace __OasisBlitz.Utility
{
    /// <summary>
    /// A simple physics solver built around the velocity verlet integration method.
    /// Lightweight, and only used on the player character. Readability is prioritized over performance.
    /// </summary>
    public class SimplePhysics
    {
        private Vector3 currentVelocity;
        private Vector3 appliedVelocity;
        // private Vector3 currentAcceleration;
        private float mass = 5;

        /// <summary>
        /// The velocity value that should be used in the upcoming frame to move the position
        /// of the player.
        /// </summary>
        public Vector3 AppliedVelocity
        {
            get => appliedVelocity;
        }
        
        /// <summary>
        /// The true velocity of the player (though notably, not the value that should be used
        /// to move the position).
        /// </summary>
        public Vector3 CurrentVelocity
        {
            get => currentVelocity;
        }
        
        public void AddForce(Vector3 force, float MaxResultantSpeed = 1000)
        {
            // F = ma
            Vector3 resultantAcceleration = force / mass;   
            AddAcceleration(resultantAcceleration, MaxResultantSpeed);
            
        }

        public void AddAcceleration(Vector3 acceleration, float MaxResultantSpeed = 1000)
        {
            // currentAcceleration += acceleration;
            AddVelocity(acceleration * Time.deltaTime, MaxResultantSpeed);
        }
        
        public void AddVelocity(Vector3 velocity, float MaxResultantSpeed = 1000)
        {
            float speedBeforeAdd = currentVelocity.magnitude;
            
            currentVelocity += velocity;
            float speedAfterAdd = currentVelocity.magnitude;
            
            // If this addition put you over the maximum speed that source is allowed to give, we will have 
            // to clamp it
            if (speedAfterAdd > MaxResultantSpeed)
            {
                // If the speed you already had was greater than the maximum speed, we set you back to the 
                // speed you were at
                if (speedBeforeAdd > MaxResultantSpeed)
                {
                    currentVelocity = currentVelocity.normalized * speedBeforeAdd;
                }
                // Otherwise, we set you to the maximum speed from this source
                else
                {
                    currentVelocity = currentVelocity.normalized * MaxResultantSpeed;
                }
            }
        }

        public void ApplyGravity(Vector3 gravity, float maxResultantYSpeed = 1000)
        {
            
            float ySpeedBeforeAdd = currentVelocity.y;
            
            currentVelocity += gravity * Time.deltaTime;
            
            float ySpeedAfterAdd = currentVelocity.y;
            
            // If this addition put you over the maximum speed that source is allowed to give, we will have 
            // to clamp it
            if (ySpeedAfterAdd > maxResultantYSpeed)
            {
                // If the speed you already had was greater than the maximum speed, we set you back to the 
                // speed you were at
                if (ySpeedBeforeAdd > maxResultantYSpeed)
                {
                    currentVelocity.y = ySpeedBeforeAdd;
                }
                // Otherwise, we set you to the maximum speed from this source
                else
                {
                    currentVelocity.y = maxResultantYSpeed;
                }
            }
            else if (ySpeedAfterAdd < maxResultantYSpeed * -1)
            {
                if (ySpeedBeforeAdd < maxResultantYSpeed * -1)
                {
                    currentVelocity.y = ySpeedBeforeAdd;
                }
                // Otherwise, we set you to the maximum speed from this source
                else
                {
                    currentVelocity.y = maxResultantYSpeed * -1;
                }
                
            }
            
        }

        public void SetVelocity(Vector3 velocity)
        {
            currentVelocity = velocity;
        }

        public void ZeroAcceleration()
        {
            // currentAcceleration = Vector3.zero;
        }
        
        public Vector3 CalculateResultantGravityForceVector(Vector3 gravity, Vector3 surfaceNormal)
        {
            Vector3 projection = Vector3.Project(gravity, surfaceNormal);
            
            return gravity - projection;
        }

        public void HandleContact(Vector3 surfaceNormal)
        {
            Vector3 contactForce = -(surfaceNormal * Vector3.Dot(currentVelocity, surfaceNormal));
            AddVelocity(contactForce);
        }
        
        public void UpdateVelocity()
        {
            // // In Verlet integration, we calculate the true velocity, and then average it with the previous velocity.
            // Vector3 previousVelocity = currentVelocity;
            // currentVelocity += currentAcceleration * Time.deltaTime;
            // // appliedVelocity = (previousVelocity + currentVelocity) * .5f;
            // appliedVelocity = currentVelocity;
            //
            // // The acceleration is reset every frame, because it is calculated from scratch every frame.
            // // currentAcceleration = Vector3.zero;
            appliedVelocity = currentVelocity;
        }

        public void ApplyDrag(float drag, float deltaTime)
        {
            // Account for the square of speed
            float speedSquared = currentVelocity.sqrMagnitude;

            float attenuator = 0.003f;
            
            float dragMultiplier = 1f / (1f + (speedSquared * attenuator * drag * deltaTime));
            currentVelocity *= dragMultiplier;
        }

        public void ApplyDragNotBasedOnSpeed(float drag, float deltaTime)
        {
            float dragMultiplier = 1f / (1f + (drag * deltaTime));
            currentVelocity *= dragMultiplier;
        }
        
        public void ApplyDragNotBasedOnSpeed(Vector3 drag, float deltaTime)
        {
            float dragMultiplierX = 1f / (1f + (drag.x * deltaTime));
            float dragMultiplierY = 1f / (1f + (drag.y * deltaTime));
            float dragMultiplierZ = 1f / (1f + (drag.z * deltaTime));
            
            currentVelocity.x *= dragMultiplierX;
            currentVelocity.y *= dragMultiplierY;
            currentVelocity.z *= dragMultiplierZ;
        }

        public void ApplyDrag(Vector3 drag, float deltaTime)
        {
            float speedSquaredX = currentVelocity.x * currentVelocity.x;
            float speedSquaredY = currentVelocity.y * currentVelocity.y;
            float speedSquaredZ = currentVelocity.z * currentVelocity.z;

            float attenuator = 0.003f;
            
            float dragMultiplierX = 1f / (1f + (speedSquaredX * attenuator * drag.x * deltaTime));
            float dragMultiplierY = 1f / (1f + (speedSquaredY * attenuator * drag.y * deltaTime));
            float dragMultiplierZ = 1f / (1f + (speedSquaredZ * attenuator * drag.z * deltaTime));
            
            currentVelocity.x *= dragMultiplierX;
            currentVelocity.y *= dragMultiplierY;
            currentVelocity.z *= dragMultiplierZ;
            
        }
        
        
    }
}
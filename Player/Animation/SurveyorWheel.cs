using __OasisBlitz.Player.Physics;
using UnityEngine;

namespace __OasisBlitz.Player.Animation
{
    public class SurveyorWheel : MonoBehaviour
    {
        [SerializeField] private float wheelRadius = 0.5f;
        [SerializeField] private float stepsPerRotation = 4f;
        [SerializeField] private PlayerPhysics playerPhysics;
        
        
        private Vector3 lastPosition;
        private float angle = 0f;

        private float radiansPerStep;
        private float wheelCircumference;
        private float fullCycleInRadians;
        private float radianCounter;

        private const float radiansToDegrees = 180 / Mathf.PI;

        // Gizmo variables
        // The center of the wheel
        public Vector3 wheelCenter = Vector3.zero;

        // The rotation of the wheel
        public Quaternion wheelRotation = Quaternion.identity;
        
        /// <summary>
        /// A float between 0 and 2 representing how far in the walk cycle the wheel is
        /// (0, 1 and 2 are points at which the foot touches the ground)
        /// </summary>
        public float RunStage
        {
            get => radianCounter / radiansPerStep;
        }

        void Start()
        {
            lastPosition = transform.position;

        }

        public void SetWheelRadius(float radius)
        {
            wheelRadius = radius;
            
            wheelCircumference = 2 * wheelRadius * Mathf.PI;
            radiansPerStep = 2 * Mathf.PI / stepsPerRotation;
            
            // A full cycle consists of 2 footfalls
            fullCycleInRadians = 2 * radiansPerStep;
            
        }
        
        public void UpdateWheel(Vector3 velocity, float deltaTime)
        {
            float dist = playerPhysics.Velocity.magnitude * deltaTime; 
             
            // float dist = Vector3.Distance (lastPosition, currPosition);
            
            //turnAngleRadians determines speed of wheel and animation
            float turnAngleRadians = (dist / wheelRadius);

            // Angle is for displaying the gizmo
            angle += turnAngleRadians * radiansToDegrees;
            if (angle > 360)
            {
                angle -= 360;
            }
            
            // radianCounter is for updating procedural animations
            radianCounter += turnAngleRadians;

            if (radianCounter > fullCycleInRadians)
                radianCounter -= fullCycleInRadians;
            
        }

        /// <summary>
        /// Draw a wheel in the Scene view
        /// </summary>
        void OnDrawGizmos()
        {
            wheelRotation = transform.rotation;

            Quaternion faceForwards = Quaternion.Euler(0, 90, 0);
            wheelRotation *= faceForwards;
            // Quaternion spin = Quaternion.Euler(0, 0, angle);
            // wheelRotation *= spin;
            // wheelRotation = wheelRotation * Quaternion.Euler(0, angle, 0);
            
            // The bottom of the wheel should touch the ground
            wheelCenter = new Vector3(0, wheelRadius, 0);
            
            // Apply the wheel's rotation
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, wheelRotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            // Draw the wheel's spokes
            for (int i = 0; i < stepsPerRotation ; i++)
            {
                // Calculate the angle for this spoke
                float spokeAngle = (i * 360f / stepsPerRotation) + angle;

                // Convert the angle to radians
                float radians = spokeAngle * Mathf.Deg2Rad;

                // Calculate the position of the end of the spoke
                Vector3 spokeEndPosition = wheelCenter + new Vector3(wheelRadius * Mathf.Cos(radians),
                    wheelRadius * Mathf.Sin(radians), 0f);

                // Draw the spoke
                Gizmos.DrawLine(wheelCenter, spokeEndPosition);
            }

            // Reset Gizmos matrix to avoid affecting other Gizmos
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
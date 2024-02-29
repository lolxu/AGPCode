using UnityEngine;

namespace __OasisBlitz.Player
{
    public class DrillRotator : MonoBehaviour
    {
        public float fastRotationSpeed = 6000f;
        public float slowRotationSpeed = 3000f;
    
        private float angle = 0.0f;

        private Quaternion _startRotation;

        public float RotationSpeed;

        void Awake()
        {
            _startRotation = transform.localRotation;    
            SetRotating(false);
        }
    
        // Update is called once per frame
        void Update()
        {
            angle += RotationSpeed * Time.deltaTime;
            if (angle > 360.0f)
            {
                angle -= 360.0f;
            }

            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle) * _startRotation;
        }

        public void SetRotating(bool isRotating)
        {
            if (isRotating)
            {
                RotationSpeed = slowRotationSpeed;
            }
            else
            {
                RotationSpeed = 0;
            }
        }

        public void SetFast()
        {
            RotationSpeed = fastRotationSpeed;
        }

        public void SetSlow()
        {
            RotationSpeed = slowRotationSpeed;
        }
    
    }
}

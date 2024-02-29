using UnityEngine;

namespace __OasisBlitz.Utility
{
    public class Spring1D : MonoBehaviour
    {
        public float position = 0;
        public float velocity = 0;
        public float equilibriumPosition = 0;

        public float angularFrequency;
        public float dampingRatio;

        void Update()
        {
            SpringMotion.CalcDampedSimpleHarmonicMotion(ref position, ref velocity, equilibriumPosition, Time.deltaTime, angularFrequency, dampingRatio);
        }

    }
}

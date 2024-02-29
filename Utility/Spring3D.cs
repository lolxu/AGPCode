using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace __OasisBlitz.Utility
{
    public class Spring3D : MonoBehaviour
    {
        public Vector3 position = Vector3.zero;
        public Vector3 velocity = Vector3.zero;
        public Vector3 equilibriumPosition = Vector3.zero;

        public float angularFrequency;
        public float dampingRatio;

        void Update()
        {
            SpringMotion.CalcDampedSimpleHarmonicMotion(ref position, ref velocity, equilibriumPosition, Time.deltaTime,
                angularFrequency, dampingRatio);
        }
    }
}
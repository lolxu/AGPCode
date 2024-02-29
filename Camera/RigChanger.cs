using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Rendering;
using __OasisBlitz.Utility;
using Unity.Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace __OasisBlitz.Camera
{
    public enum RigValue
    {
        topHeight = 0,
        topRadius = 1,
        middleHeight = 2,
        middleRadius = 3,
        bottomHeight = 4,
        bottomRadius = 5
    }
    
    [System.Serializable]
    public struct CameraRigValues
    {
        public float topHeight;
        public float topRadius;
        public float middleHeight;
        public float middleRadius;
        public float bottomHeight;
        public float bottomRadius;

        public float[] values;
        
        public void InitArray()
        {
            values = new float[6]
            {
                topHeight,
                topRadius,
                middleHeight,
                middleRadius,
                bottomHeight,
                bottomRadius
            };
        }

        public float GetValue(RigValue valueName)
        {
            return values[(int)valueName];
        }
        
    }
    
    public class RigChanger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float diveDistanceThreshold = 20.0f;
        [SerializeField] private float epsilon = 0.5f;
        
        [Header("References")]
        [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
        [SerializeField] private CinemachineDeoccluder deoccluder;
        
        private Tween[] rigTweens = new Tween[6];

        private bool cameraTweening;

        private float[] currentRigValues = new float[6];
        
        private void Update()
        {
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            SetCurrentRigValues();               
        }

        private void SetCurrentRigValues()
        {
            orbitalFollow.Orbits.Top.Height = currentRigValues[(int)RigValue.topHeight];
            orbitalFollow.Orbits.Top.Radius = currentRigValues[(int)RigValue.topRadius];
            orbitalFollow.Orbits.Center.Height = currentRigValues[(int)RigValue.middleHeight];;
            orbitalFollow.Orbits.Center.Radius = currentRigValues[(int)RigValue.middleRadius];;
            orbitalFollow.Orbits.Bottom.Height = currentRigValues[(int)RigValue.bottomHeight];;
            orbitalFollow.Orbits.Bottom.Radius = currentRigValues[(int)RigValue.bottomRadius];;
        }

        public void SetOccluderMask(LayerMask mask)
        {
            deoccluder.CollideAgainst = mask;
        }

        public void TweenToRig(CameraRigValues rig, float duration)
        {
            for (int i = 0; i < 6; i++)
            {
                if (rigTweens[i] != null)
                {
                    rigTweens[i].Kill();
                }

                int iCapture = i;

                float value = rig.values[i];
                rigTweens[i] = DOTween.To(() => currentRigValues[iCapture], x => currentRigValues[iCapture] = x, rig.GetValue((RigValue)i), duration);
                // Stop the tween
            
            }
        
        }
        
    }
}

using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using DG.Tweening;

namespace __OasisBlitz.Camera
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CustomCameraController : MonoBehaviour
    {
        [System.Serializable]
        public struct FreeLookRig
        {
            public float topRigHeight;
            public float middleRigHeight;
            public float bottomRigHeight;
        }

        [SerializeField] private CinemachineFreeLook freelookCamera;
        [SerializeField] private UnityEngine.Camera physicalCamera;
        
        private CinemachineCollider collider;

        // You can assign initial values in the inspector, or pass them in at runtime
        public FreeLookRig targetRig;

        private void Start()
        {
            if (freelookCamera == null)
            {
                freelookCamera = GetComponent<CinemachineFreeLook>();
            }
            
            collider = freelookCamera.GetComponent<CinemachineCollider>();
        }

        public void ApplyFreeLookRig(FreeLookRig rig, float duration = 0.1f)
        {
            // Interpolate Rig Heights
            DOTween.To(() => freelookCamera.m_Orbits[0].m_Height, x => freelookCamera.m_Orbits[0].m_Height = x,
                rig.topRigHeight, duration);
            DOTween.To(() => freelookCamera.m_Orbits[1].m_Height, x => freelookCamera.m_Orbits[1].m_Height = x,
                rig.middleRigHeight, duration);
            DOTween.To(() => freelookCamera.m_Orbits[2].m_Height, x => freelookCamera.m_Orbits[2].m_Height = x,
                rig.bottomRigHeight, duration);
        }

        public float GetDistanceToLookTarget()
        {
            // TODO: This could be improved, and at the very least the physical camera needs to be replaced with something
            // else, because sometimes the physical camera is dive cam and sometimes it's surface cam
            return Vector3.Distance(physicalCamera.transform.position, freelookCamera.LookAt.position);
        }

        public void SetColliderEnabled(bool enabled)
        {
            collider.enabled = enabled;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Collectables
{
    public class CritterBillboard : MonoBehaviour
    {
        [SerializeField] private bool useStaticBillboard;
        private GameObject mainCam;
        private Coroutine facingRoutine = null;

        void Start()
        {
            mainCam = GameObject.FindGameObjectWithTag("PlayerCamera");
        }

        public void EnableBillboard()
        {
            facingRoutine = StartCoroutine(updateFacingAngle());
        }
        
        public void DisableBillboard()
        {
            StopCoroutine(facingRoutine);
        }

        private IEnumerator updateFacingAngle()
        {
            while(true)
            {
                if (!useStaticBillboard)
                {
                    transform.LookAt(mainCam.transform);
                }
                else
                {
                    transform.rotation = mainCam.transform.rotation;
                }
                transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 180.0f, 0.0f);
                yield return null;
            }
        }
    }
}

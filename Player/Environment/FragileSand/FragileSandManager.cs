using System.Collections;
using System.Collections.Generic;
using System.Data;
using __OasisBlitz.Utility;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace __OasisBlitz.__Scripts.Player.Environment.FragileSand
{
    public class FragileSandManager : MonoBehaviour
    {
        public static FragileSandManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        // For player body collision with fragile sand
        public void FragileSandStartShrinkBodyCollide(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint)
        {
            if (coll != null)
            {
                // Debug.Log("Player collides with fragile sand");
                GameObject targetFragileSand = coll.gameObject;
                FragileSand targetScript = targetFragileSand.GetComponent<FragileSand>();
                targetFragileSand.tag = "FragileSandShrinkState";
                targetScript.StartCoroutine(targetScript.WaitToShrink());
            }
        }
        
        // For player drill collision with fragile sand
        public void FragileSandStartShrinkDrillCollide(Collider coll)
        {
            if (coll != null)
            {
                Debug.Log("Drill collides with fragile sand");
                GameObject targetFragileSand = coll.gameObject;
                FragileSand targetScript = targetFragileSand.GetComponent<FragileSand>();
                targetFragileSand.tag = "FragileSandShrinkState";
                targetScript.StartCoroutine(targetScript.WaitToShrink());
            }
        }

        
    };
}


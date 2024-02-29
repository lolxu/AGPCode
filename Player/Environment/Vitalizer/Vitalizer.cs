using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment;
using __OasisBlitz.Player.Environment;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Player.Environment.Vitalizer
{
    public class Vitalizer : MonoBehaviour, IPooledObject
    {
        [SerializeField] private float spinTime = 1.0f;
        [SerializeField] private float bobTime = 1.0f;

        private GameObject _myPlayer;

        public void OnObjectAllocate()
        {
            _myPlayer = GameObject.FindGameObjectWithTag("Player");
            
            // Pick a random start rotation between 0 and 360 degrees
            transform.localRotation = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f);
            
            transform.DOLocalRotate(new Vector3(0.0f, 360.0f, 0.0f), spinTime, RotateMode.FastBeyond360).SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            // Make a tween that bobs the vitalizer up and down on the y axis using a sin wave
            // Also randomize where in the sin wave we start
            /*transform.DOMoveY(0.5f, bobTime).SetRelative(true).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)
                .SetDelay(UnityEngine.Random.Range(0.0f, 1.0f));*/

        }

        public void OnObjectDeallocate()
        {
            // Do something if needed 
            // Currently nothing...
        }

        private void Update()
        {

            if (Vector3.Distance(gameObject.transform.position, _myPlayer.transform.position) <= 2.5f)
            {
                if (GetComponentInChildren<Collider>().isTrigger)
                {
                    VitalizerManager.Instance.NearVitalizerCollision(gameObject, false);
                }
                else
                {
                    VitalizerManager.Instance.NearVitalizerCollision(gameObject, true);
                }
                
            }
        }
    }
}

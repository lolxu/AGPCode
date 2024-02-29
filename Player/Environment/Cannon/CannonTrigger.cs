using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Serialization;

namespace __OasisBlitz.__Scripts.Player.Environment.Cannon
{
    public class CannonTrigger : MonoBehaviour
    {
        [SerializeField] private Cannon cannon;
        private PlayerStateMachine ctx;
        private Bounds _bounds;

        public bool isUnlocked = true;
        public bool canEnterCannon = false;
        public bool isBurrowCannon = false;
        public LevelCannonObjects CannonObj;

        public bool IsUnlocked
        {
            get { return isUnlocked;}
            set
            {
                isUnlocked = value;
                if (cannon != null)
                {
                    cannon.SetEnabledMaterial(value);
                }
            }
        }

        private void Awake()
        {
            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            _bounds = sphereCollider.bounds;
            sphereCollider.enabled = false;
            cannon.SetEnabledMaterial(isUnlocked);
        }

        private void Update()
        {
            // if (ctx && ctx.Drilling &&
            if ((ctx && 
                _bounds.Contains(ctx.transform.position) && canEnterCannon) || (!isBurrowCannon && _bounds.Contains(ctx.transform.position) && ctx))
            {
                cannon.BeginLaunchRoutine();
            }
        }
    }
}

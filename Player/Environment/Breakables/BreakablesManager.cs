using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using __OasisBlitz.Player.Physics;
using UnityEngine;
using UnityEngine.Serialization;

namespace __OasisBlitz.__Scripts.Player.Environment.Chest
{
    public class BreakablesManager : MonoBehaviour
    {
        public static BreakablesManager Instance;
        
        [SerializeField] private GameObject breakParticles;
        [SerializeField] private PlayerAudio audio;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void BreakableDrillCollide(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint, ref PlayerPhysics physics)
        {
            if (coll != null)
            {
                SpawnParticles(hitPoint);
                // We can add code to spawn loot here
                Breakables myBreakables = coll.gameObject.GetComponent<Breakables>();
                StartCoroutine(myBreakables.SpawnLoot(hitPoint));
                audio.PlayChestBreak();

                if (myBreakables.bCanBounce)
                {
                    Bounce.Instance.WeakBounce(ref coll, hitNormal, ref physics);
                }

                Destroy(coll.gameObject);
            }
        }

        private void SpawnParticles(Vector3 spawnPos)
        {
            GameObject particle = Instantiate(breakParticles, spawnPos, Quaternion.identity);
            particle.GetComponent<ParticleSystem>().Play();
        }

    }
}


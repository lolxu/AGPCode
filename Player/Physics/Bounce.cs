using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using Unity.Cinemachine;
using UnityEngine;

namespace __OasisBlitz.Player.Physics
{
    public class Bounce : MonoBehaviour
    {
        [SerializeField] private PlayerAudio audio;

        [SerializeField] private PlayerStateMachine playerStateMachine;
        //how big should the force in the direction of the normal be
        public enum BounceTypeNormal
        {
            None,
            Tiny,
            Small,
            Medium,
            Large
        }
        //how big should the force of velocity reflected off the normal
        public enum BounceTypeReflective
        {
            None,
            Tiny,
            Small,
            Medium,
            Large
        }

        private float NoBounceForce = 0.0f;
        [SerializeField] private float TinyBounceForce;
        [SerializeField] private float SmallBounceForce;
        [SerializeField] private float MediumBounceForce;
        [SerializeField] private float LargeBounceForce;
        [SerializeField] private float MetalBounceForce;
        private List<float> bounceForces = new List<float>();


        public static Bounce Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                bounceForces.Add(NoBounceForce);
                bounceForces.Add(TinyBounceForce);
                bounceForces.Add(SmallBounceForce);
                bounceForces.Add(MediumBounceForce);
                bounceForces.Add(LargeBounceForce);
            }
        }
    
        public void StickThenBounce(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint, ref PlayerPhysics physics, BounceTypeNormal bounceNormal)
        {
            if (physics.isStuckTimer <= 0f)
            {
                //calculate Bounce in direction of normal
                //bounce should not happen if the player velocity is in the same direction of the bounce pad normal
                if (Vector3.Dot(hitNormal, physics.Velocity.normalized) > 0.0f)
                {
                    return;
                }
                //cancel out acceleration and prev velocity
                physics.ZeroAcceleration();
                physics.SetVelocity(Vector3.zero);
                hitNormal *= bounceForces[(int)bounceNormal] * 2;//multiply by 2 to account for lack of BounceTypeReflective
                audio.PlayStickInBounce();
                physics.SetStuck(1.0f, hitNormal);
                StartCoroutine(playBounceSound());
            }
        }

        private IEnumerator playBounceSound()
        {
            yield return new WaitForSeconds(1.0f);
            audio.PlayBounce();
            playerStateMachine.CharacterController.RequestJump();
        }
        public void BounceCollider(ref Collider coll, Vector3 hitNormal, Vector3 hitPoint, ref PlayerPhysics physics, BounceTypeNormal bounceNormal, BounceTypeReflective bounceReflective)
        {
            //TODO: get normalized position to impact
            Vector3 currVelocity = physics.Velocity.normalized * bounceForces[(int)bounceReflective];
            //bounce should not happen if the player velocity is in the same direction of the bounce pad normal
            if (Vector3.Dot(hitNormal, physics.Velocity.normalized) > 0.0f)
            {
                return;
            }
            //add projection and position to impact
            Vector3 proj = (Vector3.Dot(currVelocity, hitNormal) / Vector3.Magnitude(hitNormal)) * hitNormal;
            //subtract projection to get parallel distance
            Vector3 projToNorm = currVelocity - proj;
            Vector3 bounceReflection = -proj + projToNorm;
            //if velocity is zero, we are stuck on the pad, so we must make the velocity the opposite of the normal to bounce the character
            //make bounce reflection cancel out our current velocity then bounce
            audio.PlayBounce();
            if (bounceReflection.AlmostZero())
            {
#if DEBUG
                if (Constants.DebugBounce)
                {
                    Debug.Log("ZERO PREVENTED");
                }
#endif
                bounceReflection = hitNormal * bounceForces[(int)bounceNormal];
                //cancel out acceleration and prev velocity
                physics.ZeroAcceleration();
                physics.SetVelocity(Vector3.zero);
                //apply bounce
                physics.AddVelocity(bounceReflection);
#if DEBUG
                if (Constants.DebugBounce)
                {
                    Debug.Log("Bouncing: " + physics.gameObject.name + "Old: " + physics.Velocity + " Reflection: " +
                              bounceReflection);
                }
#endif
            }
            else // if (Vector3.Dot(norm, currVelocity.normalized) <= 0.0f)
            {
                bounceReflection += hitNormal * bounceForces[(int)bounceNormal];
                //cancel out acceleration
                physics.ZeroAcceleration();
                physics.SetVelocity(Vector3.zero);
                //apply bounce
                physics.AddVelocity(bounceReflection);
                
#if DEBUG
                if (Constants.DebugBounce)
                {
                    Debug.Log("Bouncing: " + physics.gameObject.name + "Old: " + physics.Velocity + " Reflection: " +
                              bounceReflection);
                }
#endif
            }
            playerStateMachine.CharacterController.RequestJump();
            //if velocity is in the same direction as the normal, ignore it
        }

        public void BouncePlayerInWorldDirection(BounceTypeNormal bounceForce)
        {
            //TODO: get normalized position to impact
            Vector3 bounceVel = Vector3.up * bounceForces[(int)bounceForce];
            //cancel out acceleration and prev velocity
            playerStateMachine.PlayerPhysics.ZeroAcceleration();
            //apply bounce
            playerStateMachine.PlayerPhysics.AddVelocity(bounceVel);
            playerStateMachine.CharacterController.RequestJump();
            //if velocity is in the same direction as the normal, ignore it
        }

        // This is a modified approach to bouncing, specifically for when the player hits metal. The old implementation felt
        // slightly too aggressive, as it would zero player velocity. Now, we calculate the correct value to add instead.
        // The desired behavior here is that all velocity in the direction of the normal is cancelled out and set to a 
        // constant value. The remaining velocity is left alone.
        public void WeakBounce(ref Collider coll, Vector3 hitNormal, ref PlayerPhysics physics)
        {
            // Cancel out the part of the player velocity that is in the direction of the hitNormalal.
            Vector3 currentVelocity = physics.Velocity;

            // Theoretically the hitNormalal is already a unit vector, but this isn't specified in the documentation so I'm
            // hitNormalalizing it just in case. 
            // TODO: Investigate if hitNormal is always a unit vector.
            float projectionScalar = Vector3.Dot(currentVelocity, hitNormal) / hitNormal.magnitude;

            if (projectionScalar < 0)
            {
                Vector3 movementAgainstNormal = hitNormal * -projectionScalar;
                
                // This cancels out the velocity going against the hitNormal
                physics.AddVelocity(movementAgainstNormal);

                Vector3 defaultBounceVelocity = hitNormal * MetalBounceForce;
                physics.AddVelocity(defaultBounceVelocity);
            }

            // Ctx.PlayerPhysics.AddVelocity(blastVelocity);
            
            
        }

        public void BounceRigid(ref Rigidbody body, Vector3 hitNormal, BounceTypeNormal bounceNormal, BounceTypeReflective bounceReflective)
        {
            //TODO: get normalized position to impact
            Vector3 currVelocity = body.velocity.normalized * bounceForces[(int)bounceReflective];
            // //bounce should not happen if the player velocity is in the same direction of the bounce pad normal
            // if (Vector3.Dot(hitNormal, body.velocity.normalized) > 0.0f)
            // {
            //     return;
            // }
            //add projection and position to impact
            Vector3 proj = (Vector3.Dot(currVelocity, hitNormal) / Vector3.Magnitude(hitNormal)) * hitNormal;
            //subtract projection to get parallel distance
            Vector3 projToNorm = currVelocity - proj;
            Vector3 bounceReflection = -proj + projToNorm;
            //if velocity is zero, we are stuck on the pad, so we must make the velocity the opposite of the normal to bounce the character
            //make bounce reflection cancel out our current velocity then bounce
            if (bounceReflection.AlmostZero())
            {
                bounceReflection = hitNormal * bounceForces[(int)bounceNormal];
                //cancel out acceleration and prev velocity
                //physics.ZeroAcceleration();
                //physics.SetVelocity(Vector3.zero);
                //apply bounce
                //physics.AddVelocity(bounceReflection);
                body.velocity = bounceReflection;
            }
            else // if (Vector3.Dot(norm, currVelocity.normalized) <= 0.0f)
            {
                bounceReflection += hitNormal * bounceForces[(int)bounceNormal];
                //cancel out acceleration
                //physics.ZeroAcceleration();
                //physics.SetVelocity(Vector3.zero);
                //apply bounce
                //physics.AddVelocity(bounceReflection);
                body.velocity = bounceReflection;
            }
            audio.PlayBounce();
        }
    };
}
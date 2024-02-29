using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.Player
{
    public class BlastAbility : MonoBehaviour
    {
        [SerializeField] private float blastForce;
        [SerializeField] private float blastRadius;
        [SerializeField] private float blastCooldown;
        [SerializeField] private GameObject blastEffect;
        [SerializeField] private FMODUnity.EventReference blastEvent;
    
        private const int MaxBlastSurfaces = 20;
        // If we collide with more than 20 surfaces we 
        private Collider[] blastSurfaces = new Collider[MaxBlastSurfaces];
    
        // TODO: Make a layermask for things you can blast off of.
    
        private bool canBlast = true;
    

        public bool CanBlast
        {
            get { return canBlast; }
        }

        /// <summary>
        /// Returns the force that should be applied to the player from the blast.
        /// </summary>
        /// <returns></returns>
        public Vector3 DoBlast()
        {
            // First, set canBlast to false and start a tween to set it back to true after a delay.
            canBlast = false;
            DOVirtual.DelayedCall(blastCooldown, () => { canBlast = true; }, false);
        
            // Overlap Sphere to find all surfaces within range of the player.
            int numSurfacesHit = UnityEngine.Physics.OverlapSphereNonAlloc(transform.position, blastRadius, blastSurfaces);
        
            if (numSurfacesHit > MaxBlastSurfaces)
            {
                Debug.LogError("Too many surfaces hit by blast! We didn't expect this would happen -- increase MaxBlastSurfaces. or decrease the number of surfaces in this scene");
                return Vector3.zero;
            }
        
            Vector3 resultantForce = Vector3.zero;
        
            // Loop through blastsurfaces and find the closest point to the player for each one, then calculate the force from that surface.
            for (int i = 0; i < numSurfacesHit; i++)
            {
                var position = transform.position;
            
                Vector3 closestPoint = blastSurfaces[i].ClosestPoint(position);
            
                Vector3 direction = (position - closestPoint).normalized;
            
                float distance = Vector3.Distance(position, closestPoint);
            
                // Use inverse square falloff for the force, and clamp it to be no lower than 1
                // distance = Mathf.Max(distance * distance, 1);
            
                resultantForce += direction;
            }
        
            // Create the visual effect
            GameObject effect = Instantiate(blastEffect, transform.position, Quaternion.identity);
            effect.transform.localScale = blastRadius * 2 * Vector3.one;
        
            // Play the sound effect
            FMODUnity.RuntimeManager.PlayOneShot(blastEvent, transform.position);

            return resultantForce.normalized * blastForce;
        }
    }
}

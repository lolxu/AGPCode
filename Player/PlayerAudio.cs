using __OasisBlitz.Player.Physics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = FMOD.Debug;


namespace __OasisBlitz.Player
{
    public class PlayerAudio : MonoBehaviour
    {
        // Fmod event for drilling
        //[SerializeField] private FMODUnity.EventReference drillEvent;
        FMOD.Studio.EventInstance drillEventInstance;
        private FMOD.Studio.PARAMETER_ID drillEndID;
        
        //[SerializeField] private FMODUnity.EventReference walkEvent;
        FMOD.Studio.EventInstance walkEventInstance;
        FMOD.Studio.EventInstance slideEventInstance;
        
        [SerializeField] private FMODUnity.EventReference sandImpactEvent;
        
        [SerializeField] private FMODUnity.EventReference boostEvent;
        FMOD.Studio.EventInstance boostEventInstance;
        
        [SerializeField] private FMODUnity.EventReference walkOnlyImpactEvent;

        [SerializeField] private FMODUnity.EventReference playerDeathEvent;

        [SerializeField] private FMODUnity.EventReference blastReadyEvent;
        
        [Header("Environmental SFX")]
        [SerializeField] private FMODUnity.EventReference vitalizerCollectEvent;
        [SerializeField] private FMODUnity.EventReference chestBreakEvent;
        [SerializeField] private FMODUnity.EventReference bounceEvent;
        [SerializeField] private FMODUnity.EventReference stickInBounceEvent;

        public bool paused { get; private set; }
        
        public bool sliding { get; private set; }
        public bool walking { get; private set; }   // Private get for UIManager

        private const float walkSpeedHighMin = 14f;
        
        private const float walkSpeedLowMin = 1f;
        
        public bool bDrillSoundDisabled = false;
        
        public void StartWalk()
        {
            walkEventInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.walk, this.transform);
            walkEventInstance.start();
            walking = true;
        }
        public void StopWalk()
        {
            walkEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            walking = false;
        }

        public void StartWalkFromPause()
        {
            if (walking)
            {
                walkEventInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.walk, this.transform);
                walkEventInstance.start();
            }
            paused = false;
        }
        
        public void StopWalkFromPause()
        {
            walkEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            paused = true;
        }
        
        public void StartSlide()
        {
            slideEventInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.slide, this.transform);
            slideEventInstance.start();
            sliding = true;
        }
        public void StopSlide()
        {
            slideEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            sliding = false;
        }

        public void StartSlideFromPause()
        {
            if (sliding)
            {
                slideEventInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.slide, this.transform);
                slideEventInstance.start();
            }
            paused = false;
        }
        
        public void StopSlideFromPause()
        {
            slideEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            paused = true;
        }

        public void PlayFootstep(PlayerPhysics.OnSurfaceType surfaceType)
        {
            switch (surfaceType)
            {
                case PlayerPhysics.OnSurfaceType.Penetrable:
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.footstepSand, transform.position);
                    break;
                case PlayerPhysics.OnSurfaceType.NotPenetrable:
                    // TODO: You're on rock
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.footstepSand, transform.position);
                    break;
                case PlayerPhysics.OnSurfaceType.Slide:
                    // This case can be ignored for footsteps, this is a surface that forces you to slide
                    // So you should never make footsteps
                    break;
                case PlayerPhysics.OnSurfaceType.NotGrounded:
                    // This case can be ignored for footsteps
                    break;
                default:
                    break;
                
            }
        }

        public void StopWalkImmediate()
        {
            walkEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            walking = false;
        }
        public void UpdateWalkAudio(float speed)
        {
            if (!walking || paused) return;
            
            if (speed > walkSpeedHighMin)
                walkEventInstance.setParameterByName("WalkSpeed", 1.0f);
            else if (speed > walkSpeedLowMin)
            {
                walkEventInstance.setParameterByName("WalkSpeed", 0.3f);
            }
            else
            {
                walkEventInstance.setParameterByName("WalkSpeed", 0f);
            }
        }
        
        public void SetWalkSpeed(float speed)
        {
            walkEventInstance.setParameterByName("WalkSpeed", speed);
        }

        public void PlayFormDrill()
        {
            if (bDrillSoundDisabled)
            {
                return;
            }
            AudioManager.instance.PlayOneShot(FMODEvents.instance.drillForm, transform.position);    
        }

        public void PlayUnformDrill()
        {
            if (bDrillSoundDisabled)
            {
                return;
            }
            AudioManager.instance.PlayOneShot(FMODEvents.instance.drillUnForm, transform.position);    
        }
        
        public void StartDrill()
        {
            if (bDrillSoundDisabled)
            {
                return;
            }
            
            drillEventInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.drill, this.transform);
            
            FMOD.Studio.EventDescription drillEventDescription;
            drillEventInstance.getDescription(out drillEventDescription);

            FMOD.Studio.PARAMETER_DESCRIPTION endDescription;
            drillEventDescription.getParameterDescriptionByName("EndDrill", out endDescription);

            drillEndID = endDescription.id;

            // Start
            drillEventInstance.start();
        }

        public void StopDrill()
        {
            drillEventInstance.setParameterByID(drillEndID, 1.0f);
        }

        public void SetSubmerged(bool submerged)
        {
            if (submerged)
            {
                drillEventInstance.setParameterByName("Submerged", 1.0f);
            }
            else
            {
                drillEventInstance.setParameterByName("Submerged", 0f);
            }
        }

        public void PlayDash()
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.dash, transform.position);
        }
        
        public void PlaySandImpact()
        {
            AudioManager.instance.PlayOneShot(sandImpactEvent, transform.position);
        }

        public void PlayVitalizerCollect()
        {
            AudioManager.instance.PlayOneShot(vitalizerCollectEvent, transform.position);
        }
        
        public void PlayChestBreak()
        {
            AudioManager.instance.PlayOneShot(chestBreakEvent, transform.position);
        }

        public void PlayBounce()
        {
            AudioManager.instance.PlayOneShot(bounceEvent, transform.position);
        }
        
        public void PlayStickInBounce()
        {
            AudioManager.instance.PlayOneShot(stickInBounceEvent, transform.position);
        }

        public void SetDrillFast()
        {
            drillEventInstance.setParameterByName("DrillSpeed", 1.0f);
        }

        public void SetDrillSlow()
        {
            drillEventInstance.setParameterByName("DrillSpeed", 0.0f);
        }

        public void PlayBlastReady()
        {
            AudioManager.instance.PlayOneShot(blastReadyEvent, transform.position);
        }
        public void WalkOnlyImpact()
        {
            AudioManager.instance.PlayOneShot(walkOnlyImpactEvent, transform.position);
        }
        
        public void PlayDeathSound()
        {
            AudioManager.instance.PlayOneShot(playerDeathEvent, transform.position);
        }
        /*
        public void StartBoost()
        {
            boostEventInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.drillBoost, this.transform);
            boostEventInstance.start();
        }

        public void StopBoost()
        {
            boostEventInstance.setParameterByName("EndBoost", 1.0f);
        }
        

        private void OnDestroy()
        {
            // Stop all events
            if (drillEventInstance.isValid())
            {
                drillEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            if (walkEventInstance.isValid())
            {
                walkEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
        */
    }
}
using System;
using System.Collections;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = FMOD.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;


namespace __OasisBlitz.Player
{
    public class PlayerAudio : MonoBehaviour
    {
        // Fmod event for drilling
        //[SerializeField] private FMODUnity.EventReference drillEvent;
        FMOD.Studio.EventInstance drillEventInstance;
        private FMOD.Studio.PARAMETER_ID drillEndID;
        
        //[SerializeField] private FMODUnity.EventReference walkEvent;
        private FMOD.Studio.PARAMETER_ID globalSurfaceParameterId;
        private FMOD.Studio.EventInstance slideEventInstance;
        private Coroutine slideRoutine;
        [SerializeField] private PlayerStateMachine Ctx;
        
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
        
        public bool bDrillSoundDisabled = false;

        private void OnEnable()
        {
            slideEventInstance =  RuntimeManager.CreateInstance(FMODEvents.instance.slide);
            FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("FootStepMat", out PARAMETER_DESCRIPTION walkParameterDescription);
            globalSurfaceParameterId = walkParameterDescription.id;
        }

        private void OnDisable()
        {
            slideEventInstance.stop(STOP_MODE.IMMEDIATE);
            slideEventInstance.release();
        }

        public void StartSlide()
        {
            if (slideRoutine != null)
            {
                StopCoroutine(slideRoutine);
            }
            slideRoutine = StartCoroutine(updateSlide());
            slideEventInstance.start();
            sliding = true;
        }
        public void StopSlide()
        {
            if (slideRoutine != null)
            {
                StopCoroutine(slideRoutine);
                slideRoutine = null;
            }
            slideEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            sliding = false;
        }

        public void StartSlideFromPause()
        {
            if (sliding)
            {
                slideEventInstance.start();
            }
            paused = false;
        }
        
        public void StopSlideFromPause()
        {
            slideEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            paused = true;
        }

        private IEnumerator updateSlide()
        {
            while (true)
            {
                if (!paused)
                {
                    if (Ctx.InWaterTrigger)
                    {
                        RuntimeManager.StudioSystem.setParameterByID(globalSurfaceParameterId, 2f);
                    }
                    else
                    {
                        switch (Ctx.PlayerPhysics.CurrentOnSurfaceType)
                        {
                            case PlayerPhysics.OnSurfaceType.Penetrable:
                                RuntimeManager.StudioSystem.setParameterByID(globalSurfaceParameterId, 0f);
                                break;
                            case PlayerPhysics.OnSurfaceType.NotPenetrable:
                                // TODO: You're on rock
                                RuntimeManager.StudioSystem.setParameterByID(globalSurfaceParameterId, 1f);
                                break;
                            default:
                                break;
                        }
                    }
                    RuntimeManager.AttachInstanceToGameObject(slideEventInstance, transform);
                }
                yield return null;
            }
        }

        public void PlayFootstep()
        {
            if (Ctx.InWaterTrigger)
            {
                RuntimeManager.StudioSystem.setParameterByID(globalSurfaceParameterId, 2f);
                RuntimeManager.PlayOneShot(FMODEvents.instance.footstep, transform.position);
                return;
            }
            switch (Ctx.PlayerPhysics.CurrentOnSurfaceType)
            {
                case PlayerPhysics.OnSurfaceType.Penetrable:
                    RuntimeManager.StudioSystem.setParameterByID(globalSurfaceParameterId, 0f);
                    RuntimeManager.PlayOneShot(FMODEvents.instance.footstep, transform.position);
                    break;
                case PlayerPhysics.OnSurfaceType.NotPenetrable:
                    // TODO: You're on rock
                    RuntimeManager.StudioSystem.setParameterByID(globalSurfaceParameterId, 1f);
                    RuntimeManager.PlayOneShot(FMODEvents.instance.footstep, transform.position);
                    break;
                case PlayerPhysics.OnSurfaceType.NotGrounded:
                    // This case can be ignored for footsteps
                    break;
                default:
                    break;
            }
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
            // drillEventInstance.setParameterByID(drillEndID, 1.0f);
            drillEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

        public void PlayJump()
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.jump, transform.position);
        }

        public void PlayLand()
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.land, transform.position);
        }

        public void PlaySplashSound()
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.splash, transform.position);
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
using System;
using __OasisBlitz.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using TMPro;

namespace __OasisBlitz.UI
{
    //  Drillixir indicator/bar behavior
    //  https://www.youtube.com/watch?v=BLfNP4Sc_iA
    //  Gradient
    //  https://forum.unity.com/threads/progress-bar-slider-gradient-fill.706544/

    public class DrillixirIndicator : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] private Image _barColor;
        [SerializeField] private Gradient _gradient;
        [SerializeField] private Transform _cameraLocation;
        [SerializeField] private PlayerStateMachine psm;        // Drilling
        // Drillixir Bar Fading
        [Header("Drillixir Bar Fade")]
        [SerializeField] private CanvasGroup DrillixirHUD;      // Bar + blast indicator
        [SerializeField] private float duration;
        private Tween fade;
        private bool barActive = false;

        [Header("Bar Indicators")]
        [SerializeField] private CanvasGroup lowIndicator;
        [SerializeField] private GameObject exclamation;
        [SerializeField] private CanvasGroup consumedIndicator;
        [SerializeField] private float lowThreshold;        // % to start the blinking red indicator
        private Tween lowTween, consumedTween;
        [SerializeField] private float loopDuration;
        public bool recovering;       // Time based refills in DrillixirManager
        [SerializeField] private GameObject blastIndicator, lowBlastIndicator, recoveringBlastIndicator;

        // If drill button is pressed down
        private bool drillPressed = false;

        [Header("Blast Ready Particles")] 
        [SerializeField] private ParticleSystem leftSparks;
        [SerializeField] private ParticleSystem rightSparks;

        [Header("Player Input for moving")] 
        [SerializeField] private PlayerInput pInput;
        private bool moving = false;
        
        [Header("Player Audio")] 
        [SerializeField] private PlayerAudio pAudio;

        private Coroutine releaseTimer;
        private bool isRunning;
        void Start()
        {
            _fill.fillAmount = 1.0f;
            var leftEmission = leftSparks.emission;
            var rightEmission = rightSparks.emission;
            leftEmission.rateOverTime = 15;
            rightEmission.rateOverTime = 15;
            // StartCoroutine(TestFade());
            if (BounceAbility.Instance.BounceEnabled)
            {
                bool touchGroundToBlast = BounceAbility.Instance.TouchGroundToBlast;
                blastIndicator.SetActive(touchGroundToBlast);
                lowBlastIndicator.SetActive(touchGroundToBlast);
                recoveringBlastIndicator.SetActive(touchGroundToBlast);
            }
            else
            {
                blastIndicator.SetActive(false);
                lowBlastIndicator.SetActive(false);
                recoveringBlastIndicator.SetActive(false);
            }
        }
        private void LateUpdate()
        {
            // Make the bar constantly face the camera
            transform.LookAt(transform.position + _cameraLocation.forward);
            // Debug.Log(psm.CurrentState.StateName());
        }
        public void SetPercentFull(float percent)
        {
            _fill.fillAmount = percent;
            _barColor.color = _gradient.Evaluate(_fill.fillAmount);

            if (percent >= 0.99f)       // Natually charged up and no DrillPressed call
            {
                recovering = false;
                // StopDrillixirRecovering();
                // Handles drillixir bar and blast indicator
                //Invoke("PlayDrillixirBarFadeOut", duration);
            }

        }

        public void DrillPressed(bool status)
        {
            drillPressed = status;

            if(drillPressed)
            {
                PlayDrillixirBarFadeIn();
                if (recovering)
                {
                    recovering = false;
                    // StopDrillixirRecovering();
                }
            }
            else
            {
                StartReleaseTimer();
            }
        }

        public void StartReleaseTimer()
        {
            if (gameObject.activeInHierarchy)
            {
                if (isRunning)
                {
                    StopCoroutine(releaseTimer);
                } // Reset

                releaseTimer = StartCoroutine(ReleaseTimer(duration));

            }

        }
        private IEnumerator ReleaseTimer(float howLong)
        {
            isRunning = true;
            yield return new WaitForSeconds(howLong);
            PlayDrillixirBarFadeOut();
            isRunning = false;
        }
        public void PlayDrillixirLowLimited()
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.blastReady, this.transform.position);     // Just use blast ready sound for now because it's an unused sound that's already in
            if (lowTween != null) { lowTween.Kill(false); }        // Don't re-call this if it's already in progress
            InstantShowDrillixirHUD();
            StartReleaseTimer();
            lowIndicator.alpha = 1.0f;
            lowTween = exclamation.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), loopDuration / 5)
                .SetLoops(3, LoopType.Yoyo)
                .OnComplete(ResetLowAlpha);
        }
        private void ResetLowAlpha()
        {
            lowIndicator.alpha = 0.0f;
            exclamation.transform.localScale = Vector3.one;

        }
        public void PlayDrillixirLow()
        {
            if(lowTween != null) { lowTween.Kill(false); }        // Don't re-call this if it's already in progress
            lowTween = lowIndicator.DOFade(1, loopDuration).SetLoops(-1, LoopType.Yoyo);
        }
        public void StopDrillixirLow()
        {
            if(lowTween != null) { 
                lowTween.Kill(false);
                lowTween = null;
            }
            lowIndicator.alpha = 0.0f;
        }

        public void PlayDrillixirConsumed()
        {
            // No more audio needed?
            if(consumedTween != null) { consumedTween.Kill(false); }
            consumedIndicator.alpha = 0.0f;
            consumedTween = consumedIndicator.DOFade(1, loopDuration / 5)
                .SetLoops(3, LoopType.Yoyo)
                .OnComplete(() => consumedIndicator.alpha = 0.0f);
        }
       /* public void PlayDrillixirRecovering()
        {
            if (recoveringTween != null) { recoveringTween.Kill(false); }        // Don't re-call this if it's already in progress
            recoveringTween = recoveringIndicator.DOFade(1, loopDuration).SetLoops(-1, LoopType.Yoyo);
        }

        public void StopDrillixirRecovering()
        {
            if(recoveringTween != null) {
                recoveringTween.Kill(false);
                recoveringTween = null;
            }
            recoveringIndicator.alpha = 0.0f;
        }*/

        public void PlayBlastReady()
        {
            pAudio.PlayBlastReady();
        }
        public void PlayDrillixirBarFadeIn()
        {
            if (BounceAbility.Instance &&
                !BounceAbility.Instance.BounceEnabled)
            {
                return;
            }
            
            if (fade != null) { fade.Kill(false); }  // If a fade is already in process, kill it
            barActive = true;

            // Reset fade tween
            fade = DrillixirHUD.DOFade(1, duration);
        }
        public void PlayDrillixirBarFadeOut()
        {
            // if(drillPressed || _fill.fillAmount <= 0.99f) { return; }       // If pressed when this is called, do not fade out
            if (fade != null) { fade.Kill(false); }  // If already in process, kill it

            // Reset fade tween
            fade = DrillixirHUD.DOFade(0, duration);
            barActive = false;
        }
        
        public void InstantShowDrillixirHUD()
        {
            if (fade != null) { fade.Kill(false); }
            DrillixirHUD.alpha = 1;
            //blastIndicator.alpha = 1;
            barActive = true;
        }

        public void SetBlastReadyParticles()
        {
            var leftEmission = leftSparks.emission;
            var rightEmission = rightSparks.emission;

            if (moving)
            {
                leftEmission.rateOverTime = 100;
                rightEmission.rateOverTime = 100;
            }
            else
            {
                leftEmission.rateOverTime = 15;
                rightEmission.rateOverTime = 15;
            }
        }

        private void Update()
        {
            if (BounceAbility.Instance &&
                !BounceAbility.Instance.BounceEnabled)
            {
                DrillixirHUD.alpha = 0;
                return;
            }
/*            // Low Indicator
            if (_fill.fillAmount <= lowThreshold && lowTween == null && !recovering)
            {
                PlayDrillixirLow();
            }
            else if (_fill.fillAmount > lowThreshold)
            {
                StopDrillixirLow();
            }*/

/*            // Recovering Indicator
            if (recovering && recoveringTween == null && !drillPressed)
            {
                StopDrillixirLow();
                PlayDrillixirRecovering();
            }
            else if (drillPressed)
            {
                recovering = false;
                StopDrillixirRecovering();
            }*/


            // Blast Particles
            if (pInput.currentMovementInputRaw != Vector2.zero)
            {
                if (!moving)
                {
                    moving = true;
                    SetBlastReadyParticles();
                    // Debug.Log("NOW MOVING"); 
                }
            }
            else
            {
                if (moving)
                {
                    moving = false; 
                    SetBlastReadyParticles();
                    // Debug.Log("NOW NOT MOVING");  
                }
            }
        }
    }
}
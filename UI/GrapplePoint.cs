using System;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace __OasisBlitz.__Scripts.UI
{
    public class GrapplePoint : MonoBehaviour
    {
        [Header("Core Settings")]
        [SerializeField] private GameObject outerRing;
        [SerializeField] private Image m_innerImage;
        [SerializeField] private Image m_outerImage;
        [SerializeField] private Sprite enemyCrosshairInner;
        [SerializeField] private Sprite enemyCrosshairOuter;
        [SerializeField] private Sprite regularCrosshairInner;
        [SerializeField] private Sprite regularCrosshairOuter;
        
        [Header("Animation Related Settings")]
        [Tooltip("Fade in animation time and curve")]
        [SerializeField] private float fadeInTime = 0.35f;
        [SerializeField] private Ease fadeInEase;

        [SerializeField] private float endAlpha = 1.0f;
        [SerializeField] private float startAlpha = 0.0f;
        
        [Tooltip("Fade out animation time")]
        [SerializeField] private float fadeOutTime = 0.25f;
        [SerializeField] private Ease fadeOutEase;

        [Tooltip("Enable this to have a simple scaling/breathing animation for outer ring")]
        [SerializeField] private bool outerRingAnimates = true;
        [SerializeField] private float outerRingAnimateTime = 0.5f;
        [SerializeField] private float outerRingAnimateScale = 1.15f;
        
        [Tooltip("Secret setting to induce motion sickness")]
        [SerializeField] private bool outerRingCanRotate = false;
        
        [Tooltip("Time to rotate outer ring for 1 cycle")]
        [SerializeField] private float outerRingRotateTime = 1.0f;
        
        [Tooltip("Scale to fade out to when grapple point is no longer active")]
        [SerializeField] private Vector3 objectUnfocusScale;
        
        [Tooltip("Scale to fade in to for outer ring")]
        [SerializeField] private Vector3 outerRingFocusScale;
        
        [Tooltip("Scale to fade out to for outer ring")]
        [SerializeField] private Vector3 outerRIngUnfocusScale;
        
        [Header("Distance Related Settings")]
        [Tooltip("The furthest range where the grapple point is activated")]
        [SerializeField] private float potentialTargetRadius;
        [Tooltip("The step that the scale for grapple point changes as you move closer or further away")]
        [SerializeField] private float scaleIncrementStep = 3.0f;
        [SerializeField] private float minScale = 1.0f;
        [SerializeField] private float maxScale = 5.0f;

        private RectTransform m_rectTransform;
        private Sequence s_startDashable;
        // private Vector3 m_orgScale;
        private GameObject m_dashTarget;
        private bool grapplePointDoneAnimating = false;
        private GameObject m_playerRef;
        private bool m_show = true;

        private void Awake()
        {
            s_startDashable = DOTween.Sequence();
            // m_orgScale = new Vector3(1.35f, 1.35f, 1.35f);
            m_rectTransform = GetComponent<RectTransform>();
            m_playerRef = GameObject.FindGameObjectWithTag("Player");
        }

        private void LateUpdate()
        {
            // Update on screen location
            if (m_dashTarget != null)
            {
                var dashPos = m_dashTarget.transform.position;
                Vector3 viewPos = CameraStateMachine.Instance.CameraSurface.GetComponent<UnityEngine.Camera>()
                    .WorldToScreenPoint(dashPos);
                m_rectTransform.anchoredPosition3D = viewPos;
                m_rectTransform.localScale = CalculateGrapplePointSize(m_dashTarget.gameObject);
            }

            if (!m_show && gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
            else if (m_show && !gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
        }

        public void ForceHideGrapplePoint()
        {
            m_show = false;
        }

        public void ForceShowGrapplePoint()
        {
            m_show = true;
        }

        public void SetNewDashTarget(GameObject newDashTarget)
        {
            m_dashTarget = newDashTarget;
            //Debug.Log(m_dashTarget.tag);
            if (m_dashTarget.transform.parent.CompareTag("Enemy"))
            {
                m_innerImage.sprite = enemyCrosshairInner;
                m_outerImage.sprite = enemyCrosshairOuter;
            }
            else
            {
                m_innerImage.sprite = regularCrosshairInner;
                m_outerImage.sprite = regularCrosshairOuter;
            }
        }

        public void UnsetNewDashTarget()
        {
            m_dashTarget = null;
        }

        public void DoGrapplePointFadeIn()
        {
            // Debug.Log("Fade In");
            grapplePointDoneAnimating = false;
            Sequence fadeIn = DOTween.Sequence();
            fadeIn.Join(m_rectTransform.DOScale(CalculateGrapplePointSize(m_dashTarget), fadeInTime)
                .SetEase(fadeInEase));
            fadeIn.Join(GetComponent<Image>().DOFade(endAlpha, fadeInTime).SetEase(fadeInEase));
            fadeIn.OnComplete(() =>
            {
                grapplePointDoneAnimating = true;
            });
            
            FadeInDashable();
        }

        public void DoGrapplePointFadeOut()
        {
            // Debug.Log("Fade Out");
            grapplePointDoneAnimating = false;
            FadeOutDashable();
            Sequence fadeOut = DOTween.Sequence();
            fadeOut.Join(m_rectTransform.DOScale(objectUnfocusScale, fadeOutTime).SetEase(fadeOutEase));
            fadeOut.Join(m_innerImage.DOFade(startAlpha, fadeOutTime).SetEase(fadeOutEase));
            fadeOut.OnComplete(() =>
            {
                grapplePointDoneAnimating = true;
                gameObject.SetActive(false);
            });
        }

        private void FadeInDashable()
        {
            s_startDashable = DOTween.Sequence();
            s_startDashable.Join(outerRing.GetComponent<RectTransform>().DOScale(outerRingFocusScale, fadeInTime)
                .SetEase(fadeInEase));
            s_startDashable.Join(outerRing.GetComponent<Image>().DOFade(endAlpha, fadeInTime).SetEase(fadeInEase));

            if (outerRingCanRotate)
            {
                var startRot = outerRing.GetComponent<RectTransform>().localEulerAngles;
                s_startDashable.Join(outerRing.GetComponent<RectTransform>()
                    .DOLocalRotate(startRot + new Vector3(0.0f, 0.0f, 360.0f), outerRingRotateTime,
                        RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear));
            }

            if (outerRingAnimates)
            {
                s_startDashable.Append(outerRing.GetComponent<RectTransform>().DOScale(outerRingFocusScale * outerRingAnimateScale, outerRingAnimateTime)
                    .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine));
            }
        }

        private void FadeOutDashable()
        {
            s_startDashable.Kill();
            outerRing.GetComponent<Image>().DOFade(startAlpha, fadeOutTime).SetEase(fadeOutEase);
            outerRing.GetComponent<RectTransform>().DOScale(outerRIngUnfocusScale, fadeOutTime).SetEase(fadeOutEase);
        }

        public void StartSpinning()
        {
            var startRot = outerRing.GetComponent<RectTransform>().localEulerAngles;
            outerRing.GetComponent<RectTransform>()
                .DOLocalRotate(startRot + new Vector3(0.0f, 0.0f, 360.0f), 0.75f,
                    RotateMode.FastBeyond360).SetEase(Ease.OutExpo);
        }
        
        private Vector3 CalculateGrapplePointSize(GameObject newDashTarget)
        {
            var dashPos = newDashTarget.transform.position;
            // This script hates me
            if (!m_playerRef)
            {
                m_playerRef = GameObject.FindGameObjectWithTag("Player");
            }
            var playerPos = m_playerRef.transform.position;
            // Update size according to distance (max: 3.0f, min: 0.5f)
            float distance = Vector3.Distance(dashPos, playerPos);
            float size = Mathf.Clamp(scaleIncrementStep * (1.0f - distance / potentialTargetRadius), minScale, maxScale);
            // Debug.Log(distance);
            return new Vector3(size, size, size);
        }
    }
}
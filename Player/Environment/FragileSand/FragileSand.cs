using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Player.Environment.FragileSand
{
    public class FragileSand : MonoBehaviour
    {
        [Header("Fragile Sand Attributes")] 
        [SerializeField] private bool canComeBack = true;
        [SerializeField] private float shrinkTime = 1.5f;
        [SerializeField] private float comeBackTime = 1.0f;
        [SerializeField] private float waitShrinkTime = 1.0f;
        
        private Vector3 orgScale;
        
        private void Start()
        {
            orgScale = transform.localScale;
        }

        private void StartExpanding()
        {
            gameObject.tag = "FragileSand";
            gameObject.transform.DOScale(orgScale, shrinkTime)
                .SetEase(Ease.InOutSine);
        }

        public void RestartFragileSand()
        {
            if (canComeBack)
            {
                StartCoroutine(WaitToComeBack());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        private void StartShrinking(Vector3 scaleTo)
        {
            transform.DOScale(scaleTo, shrinkTime)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => RestartFragileSand());
        }

        IEnumerator WaitToComeBack()
        {
            yield return new WaitForSeconds(comeBackTime);
            StartExpanding();
        }

        public IEnumerator WaitToShrink()
        {
            ParticleSystem sandParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
            var main = sandParticles.main;
            main.duration = (shrinkTime + waitShrinkTime) * 0.95f;
            if (!sandParticles.isPlaying)
            {
                sandParticles.Play();
            }
            yield return new WaitForSeconds(waitShrinkTime);
            StartShrinking(Vector3.zero);
        }
    }
}
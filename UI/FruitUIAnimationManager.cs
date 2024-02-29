using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using Animancer;
using Animancer.Units;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace __OasisBlitz.Player.Animation
{
    public class FruitUIAnimationManager : MonoBehaviour
    {
        [SerializeField] private AnimancerComponent _Animancer;
        
        [SerializeField] private ClipTransition SpinAnimation;
        [SerializeField] private TextMeshProUGUI QuantityText;
        [SerializeField] private GameObject hotKeyImage;
        [Range(0.5f, 3.0f)][SerializeField] private float Speed;

        //[Range(0.5f, 6.0f)][SerializeField] private float range;
        
        private void Awake()
        {
            SpinAnimation.Events.OnEnd = OnSpinEnd;

            //// Start paused at the beginning of the animation.
            _Animancer.Play(SpinAnimation);
            _Animancer.Playable.PauseGraph();
            //// Normally Unity would evaluate the Playable Graph every frame and apply its output to the model,
            //// but that won't happen since it is paused so we manually call Evaluate to make it apply the first frame.
            _Animancer.Evaluate();
            _Animancer.Stop();
            //StartCoroutine(test());
        }

        public void PlaySpinAnimation()
        {
            if (_Animancer.States[SpinAnimation].IsPlaying)
            {
                if (SpinAnimation.Speed > 0)
                {
                    SpinAnimation.Speed = -Speed;
                }
                else
                {
                    SpinAnimation.Speed = Speed;
                }
                _Animancer.Play(SpinAnimation);
            }
            else
            {
                AnimancerState state = _Animancer.Play(SpinAnimation);
                state.NormalizedTime = 0;
                state.Speed = Speed;
                QuantityText.enabled = false;
                hotKeyImage.SetActive(false);
            }
            _Animancer.Playable.UnpauseGraph();
        }
        
        private void OnSpinEnd()
        {
            _Animancer.Playable.PauseGraph();
            SpinAnimation.State.IsPlaying = false;
            QuantityText.enabled = true;
            hotKeyImage.SetActive(true);
        }

        //private IEnumerator test()
        //{
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(range);
        //        PlaySpinAnimation();
        //        Debug.Log("Animate!!!!");
        //    }
        //}

    }
}

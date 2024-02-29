using System.Collections;
using TMPro;
using UnityEngine;

namespace __OasisBlitz.UI
{
    public class JumpIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _jumpIndicator;
        //Stuck variables
        public const float ForceToGetUnstuck = 10.0f;
        private float _unStickForce = 0.0f;
        bool _justStop = false;
        public float UnStickForce
        {
            get { return _unStickForce; }
            set { _unStickForce = value; }
        }
        public void EnableAlter()
        {
            StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            while (!_justStop)
            {
                if (_jumpIndicator.enabled)
                {
                    _jumpIndicator.enabled = false;
                }
                else
                {
                    _jumpIndicator.enabled = true;
                }
                yield return new WaitForSeconds(0.5f);
            }
            Disable();
            _justStop = false;
        }

        public void Enable()
        {
            if (!_jumpIndicator.enabled)
            {
                _jumpIndicator.enabled = true;
            }
        }

        public void StopFlash()
        {
            _justStop = true;
        }
        public void Disable()
        {
            if (_jumpIndicator.enabled)
            {
                _jumpIndicator.enabled = false;
            }
        }
    }
}

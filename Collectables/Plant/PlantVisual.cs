using System;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Collectables
{
    public class PlantVisual : MonoBehaviour
    {
        private Sequence plantAnim;
        private Sequence spinAnim;
        
        private Vector3 orgScale = Vector3.zero;
        private ModelRotator playerModel;
        
        private void Awake()
        {
            plantAnim = DOTween.Sequence();
            spinAnim = DOTween.Sequence();
            orgScale = transform.localScale;
            transform.localScale = Vector3.zero;
            playerModel = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>().ModelRotator;
            transform.position = playerModel.transform.position + new Vector3(0.0f, 1.25f, 0.0f);
            Vector3 currentAngle = transform.localEulerAngles;
            spinAnim.Append(transform.DORotate(currentAngle + new Vector3(0.0f, 360.0f, 0.0f), 0.75f,
                RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear));
            
            plantAnim.Append(transform.DOScale(orgScale, 0.35f));
            plantAnim.AppendInterval(0.75f);
            plantAnim.Append(transform.DOMoveY(transform.position.y - 2.0f, 1.75f));
            plantAnim.Join(transform.DOScale(Vector3.zero, 1.75f).OnComplete(() =>
            {
                Destroy(gameObject);
            }));
            
            
        }

        private void OnDestroy()
        {
            plantAnim.Kill();
            spinAnim.Kill();
        }
    }
}
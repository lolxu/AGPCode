using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Pulsing : MonoBehaviour
{
    
    private GameObject obj;
    [SerializeField] private float to;
    private float tweenDuration = 0.4f;
    private Sequence pulse;
    private float originalScale;

    private void OnEnable()
    {
        // StartPulse();
        obj = this.gameObject;
    }
    private void OnDisable()
    {
        StopPulse();
    }
    public void StartPulse(float interval = 0.1f)
    {
        originalScale = obj.transform.localScale.x;
        if(pulse != null) { pulse.Kill(false); }

        pulse = DOTween.Sequence();
        pulse.Append(obj.transform.DOScale(to, tweenDuration))
            .SetDelay(interval)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }
    public void StopPulse()
    {
        pulse?.Kill(false);
        if (Mathf.Abs(originalScale) <= .01)
        {
            originalScale = 1;
        }
        obj.transform.localScale = new Vector3(originalScale, originalScale, originalScale);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

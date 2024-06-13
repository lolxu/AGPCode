using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class LightFlicker : MonoBehaviour
{
    private Light light;
    private float delay;
    [SerializeField] private float startIntensity;
    [SerializeField] private float endIntensity;
    [SerializeField] private float tweenTime;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    private void Start()
    {
        if (light != null)
        {
            light.intensity = (endIntensity + startIntensity)/2.0f;
            StartCoroutine(offsetPulses());
        }
        else
        {
            Debug.LogWarning("Light not attached to LightFlicker Script");
        }
    }

    private IEnumerator offsetPulses()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, tweenTime * 1.9f));
        Pulse();
    }
    private void Pulse()
    {
        light.enabled = true;
        light.DOIntensity(endIntensity, tweenTime).OnComplete(PulseBack).SetEase(Ease.InQuad);
    }

    private void PulseBack()
    {
        light.DOIntensity(startIntensity, tweenTime).OnComplete(Pulse).SetEase(Ease.OutQuad);
    }
}

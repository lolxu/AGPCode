using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentWall : MonoBehaviour
{
    private Coroutine FadeRoutine;
    private Coroutine ResetFadeRoutine;
    private float ResetAlpha = 1.0f;
    private float FadeAlpha = 0.0f;
    private float FadeOutSpeedMultiplier = 8.0f;
    private float FadeInSpeedMultiplier = 3.0f;
    private float currAlpha;
    private int alphaPropertyID;
    [SerializeField] private MeshRenderer renderer;
    public bool IsFading { get; private set; }

    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Material opaqueMaterial;

    private void Awake()
    {
        alphaPropertyID = Shader.PropertyToID("_GrantVal");
        renderer = GetComponent<MeshRenderer>();
        renderer.material = opaqueMaterial;
        // renderer.material = transparentMaterial;
        currAlpha = ResetAlpha;
        // renderer.material.SetFloat(alphaPropertyID, currAlpha);
        SetAlpha(currAlpha);
    }
    
    public void Fade()
    {
        IsFading = true;
        if (ResetFadeRoutine != null)
        {
            StopCoroutine(ResetFadeRoutine);
            ResetFadeRoutine = null;
        }
        FadeRoutine = StartCoroutine(FadeWall());
    }

    public void ResetFade()
    {
        IsFading = false;
        if (FadeRoutine != null)
        {
            StopCoroutine(FadeRoutine);
            FadeRoutine = null;
        }
        ResetFadeRoutine = StartCoroutine(ResetWallFade());
    }

    private void SetAlpha(float alpha)
    {
        Color currentColor = renderer.material.color;
        currentColor.a = alpha;
        renderer.material.color = currentColor;
    }

    private IEnumerator FadeWall()
    {
        // TODO: Uncomment this to apply fog 
        renderer.material = transparentMaterial;
        
        while (currAlpha > FadeAlpha)
        {
            currAlpha -= FadeOutSpeedMultiplier * Time.deltaTime;
            
            SetAlpha(currAlpha);
            // renderer.material.SetFloat(alphaPropertyID, currAlpha);
            yield return null;
        }
        currAlpha = FadeAlpha;
        // renderer.material.SetFloat(alphaPropertyID, currAlpha);
        SetAlpha(currAlpha);
        
        FadeRoutine = null;
    }
    
    private IEnumerator ResetWallFade()
    {
        while (currAlpha < ResetAlpha)
        {
            currAlpha += FadeInSpeedMultiplier * Time.deltaTime;
            // renderer.material.SetFloat(alphaPropertyID, currAlpha);
            SetAlpha(currAlpha);
            yield return null;
        }
        currAlpha = ResetAlpha;
        // renderer.material.SetFloat(alphaPropertyID, currAlpha);
        SetAlpha(currAlpha);
        ResetFadeRoutine = null;

        // TODO: Uncomment this to apply fog 
        renderer.material = opaqueMaterial;
    }
}

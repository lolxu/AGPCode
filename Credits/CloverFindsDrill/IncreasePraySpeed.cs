using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class IncreasePraySpeed : MonoBehaviour
{
    [SerializeField] private Animator clover;
    [SerializeField] private Transform cameraTransform;
    public void IncreaseSpeed(float newSpeed)
    {
        clover.SetFloat("praySpeed", newSpeed);
    }
    
    public void PlayYeahAudio()
    {
        RuntimeManager.PlayOneShotAttached(CreditsAudio.instance.cloverYeah, cameraTransform.gameObject);
    }
    
}

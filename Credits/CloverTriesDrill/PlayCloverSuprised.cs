using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class PlayCloverSuprised : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    public void PlaySuprised()
    {
        RuntimeManager.PlayOneShotAttached(CreditsAudio.instance.cloverHey, cameraTransform.gameObject);
    }
}

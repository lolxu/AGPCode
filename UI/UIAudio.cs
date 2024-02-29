using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance;

    [SerializeField] private FMODUnity.EventReference pause;
    [Header("Button Sounds")]
    [SerializeField] private FMODUnity.EventReference buttonHover;
    [SerializeField] private FMODUnity.EventReference buttonSelect;
    

    public void PlayPause()
    {
        FMODUnity.RuntimeManager.PlayOneShot(pause);
    }
    public void PlayButtonHover()
    {
        FMODUnity.RuntimeManager.PlayOneShot(buttonHover);
    }
    public void PlayButtonSelect()
    {
        FMODUnity.RuntimeManager.PlayOneShot(buttonSelect);
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

}

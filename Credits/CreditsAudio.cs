using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class CreditsAudio : MonoBehaviour
{
    //Global
    [field: SerializeField] public EventReference punch { get; private set; } //static instance
    [field: SerializeField] public EventReference cloverHey { get; private set; } //static instance
    [field: SerializeField] public EventReference cloverYeah { get; private set; } //static instance
    
    [field: SerializeField] public EventReference cloverAhh { get; private set; } //static instance
    public static CreditsAudio instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            // Debug.LogError("Found more than one FMOD Events instance in the scene.");
            Destroy(this);
        }
        instance = this;
    }
}

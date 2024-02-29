using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // AudioManager.instance.InitializeMusic(FMODEvents.instance.musicBurrowTheme);
    }

    private void OnDestroy()
    {
        // AudioManager.instance.StopMusic();
    }
}

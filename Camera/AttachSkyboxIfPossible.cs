using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using DistantLands.Cozy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttachSkyboxIfPossible : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        RespawnManager.OnReset += AttachToSkybox;
    }

    void OnDisable()
    {
        // SceneManager.sceneLoaded -= (a, b) => AttachToSkybox();
        RespawnManager.OnReset -= AttachToSkybox;
    }

    private void AttachToSkybox()
    {
        Debug.Log("Attaching to skybox");
        
        CozyWeather weather = FindObjectOfType<CozyWeather>();
        
        if (weather == null)
        {
            return;
        }

        weather.lockToCamera = CozyWeather.LockToCameraStyle.useCustomCamera;
        weather.cozyCamera = GetComponent<Camera>();
    }
}

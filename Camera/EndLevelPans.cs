using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Camera.StateMachine.Subroutines;
using Unity.Cinemachine;
using UnityEngine;

public class EndLevelPans : MonoBehaviour
{
    private CinemachineSequencerCamera _sequencerCamera;
    //private CinemachineBrain[] brains;


    private void Awake()
    {
        _sequencerCamera = GetComponent<CinemachineSequencerCamera>();
    }

    private void OnEnable()
    {
        CinematicsCameraSubroutine.DrillDownOver += StartPanning;
    }

    private void OnDisable()
    {
        CinematicsCameraSubroutine.DrillDownOver -= StartPanning;
    }

    // private IEnumerator Start()
    // {
    //     yield return null;
    //     brains = FindObjectsByType<CinemachineBrain>(FindObjectsSortMode.None);
    // }

    private void StartPanning()
    {
        // for (int i = 0; i < brains.Length; i++)
        // {
        //     brains[i].IgnoreTimeScale = true;
        // }
        _sequencerCamera.enabled = true;
        //Time.timeScale = 1.0f;
        //StartCoroutine(timeScale());
    }

    // private IEnumerator timeScale()
    // {
    //     while (true)
    //     {
    //         if (Time.timeScale != 1.0f)
    //         {
    //             Time.timeScale = 1.0f;
    //             Debug.Log("Timescale set to oneeeeeeeeeeeeeeeeeeeeeeeeeee");
    //         }
    //
    //         yield return null;
    //     }
    // }
}

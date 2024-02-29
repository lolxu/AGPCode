using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerToggle : MonoBehaviour
{
    public bool isStart = true;
    public bool isEnd = false;

    private bool running = false;
    private bool stopped = false;

    private void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.CompareTag("Player"))
        // {
        //     if (isStart)
        //         // TimerStart();
        //     if (isEnd)
        //         // TimerStop();
        // }
    }

    // private void TimerStart()
    // {
    //     if (running == false)
    //     {
    //         UIManager.Instance.StartStopTime();
    //         running = true;
    //     }
    //     else
    //     {
    //         UIManager.Instance.RestartTime();
    //         UIManager.Instance.StartStopTime();
    //     }
    // }

    // private void TimerStop()
    // {
    //     if (stopped == false)
    //     {
    //         UIManager.Instance.StartStopTime();
    //         stopped = true;
    //     }
    // }
}

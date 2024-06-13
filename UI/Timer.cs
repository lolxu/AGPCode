using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public bool running { get; private set; }

    public float personalBest;      // Public to access when saving or setting data
    public float runTime;
    
    public Action OnStartTimer;
    public Action OnPauseTimer;
    public Action OnUnpauseTimer;
    
    public void StartTime()
    {
        Debug.Log("Start time");
        if (OnStartTimer != null)
        {
            OnStartTimer();
        }
        running = true;
    }

    public void PauseTime()
    {
        if (OnPauseTimer != null)
        {
            OnPauseTimer();
        }
        running = false;
    }

    public void UnpauseTime()
    {
        if (OnUnpauseTimer != null)
        {
            OnUnpauseTimer();
        }
        running = true;
    }
    public void StopTime()
    {
        running = false;
    }
    
    public static string TimeToString(float seconds)
    {
        int min = (int)seconds / 60;
        int sec = (int)seconds - 60 * min;
        int ms = (int)(1000 * (seconds - min * 60 - sec));
        return string.Format("{0:00}:{1:00}:{2:000}", min, sec, ms);
    }

    public void RestartTime()
    {


        runTime = 0.0f;
    }
    public void LoadTime(float loadData)      // To call only on loading save data
    {
        personalBest = loadData;
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            runTime += Time.deltaTime;
        }
    }
}

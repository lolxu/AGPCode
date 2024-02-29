using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public bool running { get; private set; }

    public float personalBest;      // Public to access when saving or setting data
    public float runTime;
    public void StartTime()
    {
        Debug.Log("Start time");
        running = true;
    }
    public void StopTime()
    {
        running = false;
    }

    /*
     *  Format:  https://answers.unity.com/questions/1476208/string-format-to-show-float-as-time.html
     */
    public string GetTime()
    {
        int min = (int)runTime / 60;
        int sec = (int)runTime - 60 * min;
        int ms = (int)(1000 * (runTime - min * 60 - sec));
        return string.Format("{0:00}:{1:00}:{2:000}", min, sec, ms);
    }
    public string GetPB()
    {
        int min = (int)personalBest / 60;
        int sec = (int)personalBest - 60 * min;
        int ms = (int)(1000 * (personalBest - min * 60 - sec));
        return string.Format("{0:00}:{1:00}:{2:000}", min, sec, ms);
    }
    public void RestartTime()
    {
        // if (runTime >= personalBest)
        // {
        //     personalBest = runTime;
        // }
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

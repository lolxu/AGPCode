using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class SlowDownOverTime : MonoBehaviour
{
    //endingTimeScale must be < time scale
    [SerializeField] private float timeBeforeReducingTimeScale;
    [SerializeField] private float endingTimeScale;
    [SerializeField] private float timeToReachEndingTimeScale;
    [SerializeField] private bool returnBack = false;
    [SerializeField] private float timeToWaitBeforeTurningBack;
    [SerializeField] private float timeToReachTurnTimeScaleOne;

    [SerializeField] private CinemachineCamera cam;


    private void Start()
    {
        StartCoroutine(changeTime());
        
    }

    private IEnumerator changeTime()
    {
        while (!cam.IsLive)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(timeBeforeReducingTimeScale);
        float changePerSec = Time.timeScale - endingTimeScale;
        changePerSec /= timeToReachEndingTimeScale;
        while (Time.timeScale > endingTimeScale)
        {
            Time.timeScale -= changePerSec * Time.deltaTime;
            yield return null;
        }
        Time.timeScale = endingTimeScale;
        yield return new WaitForSecondsRealtime(timeToWaitBeforeTurningBack);
        if (returnBack)
        {
            changePerSec = 1.0f - endingTimeScale;
            changePerSec /= timeToReachTurnTimeScaleOne;
            while (Time.timeScale < 1f)
            {
                Time.timeScale += changePerSec * Time.unscaledDeltaTime;
                yield return null;
            }
        }
        Time.timeScale = 1.0f;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class StartDollyOnActive : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CinemachineSplineDolly dolly;
    private bool startsNegative = false;
    [SerializeField] private float Speed;
    [SerializeField] private float decreaseRate = 0.0f;
    private float minSpeed = 0.01f;
    [SerializeField] private float blendTime;
    [SerializeField] private bool waitTillFullyTransitioned = false;
    private float StartDist;
    
    private void Start()
    {
        if (Speed < 0.0f)
        {
            startsNegative = true;
        }

        StartDist = dolly.CameraPosition;
        StartCoroutine(startDolly());
    }

    private IEnumerator startDolly()
    {
        dolly.CameraPosition = StartDist;
        while (!cam.IsLive)
        {
            yield return null;
        }
        if (waitTillFullyTransitioned)
        {
            yield return new WaitForSecondsRealtime(blendTime);
        }
        float speed = Mathf.Abs(Speed);
        while (cam.IsLive)
        {
            speed = Mathf.Max(minSpeed, speed);
            if (startsNegative)
            {
                dolly.CameraPosition -= speed * Time.deltaTime;
            }
            else
            {
                dolly.CameraPosition += speed * Time.deltaTime;
            }

            speed -= Time.unscaledDeltaTime * decreaseRate;
            yield return null;
        }

        StartCoroutine(startDolly());
    }
}

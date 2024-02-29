using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RuntimeEnvironmentLighting : MonoBehaviour
{
    public static RuntimeEnvironmentLighting Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    private float defaultFogValue = 300.0f;
    private float timeToReturn = 2.0f;

    public void ResetFogValue()
    {
        StartCoroutine(ResetFog());
    }
    
    private IEnumerator ResetFog()
    {
        float MaxFogDist = RenderSettings.fogEndDistance;
        MaxFogDist -= defaultFogValue;
        if (MaxFogDist < 0)
        {
            MaxFogDist = 0;
            Debug.LogError("GRANT SAYS FOG VALUE IS SET BELOW 300");
        }
        float currentTime = timeToReturn;
        while (currentTime >= 0)
        {
            currentTime -= Time.deltaTime;
            RenderSettings.fogEndDistance = defaultFogValue + MaxFogDist * (currentTime / timeToReturn);
            yield return null;
        }
        RenderSettings.fogEndDistance = defaultFogValue;
    }
}

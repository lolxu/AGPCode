using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIAnimation : MonoBehaviour
{
    // UIAnimation controller - Singleton for access to several DOTween functions that will be used throughout the project's UI
    // TODO: Scale, Rotate, Transform, ...

    public static UIAnimation Instance;

    /// <summary>
    /// Animation that plays when you acquire a fruit
    /// </summary>
    /// <param name="_gameObject"> The fruit frame game object should be passed in here </param>
    public void PlayFruitGetAnimation(GameObject _gameObject)
    {
        
    }
    
    /// <summary>
    /// Animation that plays when you consume a fruit
    /// </summary>
    /// <param name="_gameObject"> The fruit frame game object should be passed in here </param>
    public void PlayFruitUseAnimation(GameObject _gameObject)
    {
        
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}

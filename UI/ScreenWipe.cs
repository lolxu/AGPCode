using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class ScreenWipe : MonoBehaviour
{
    [SerializeField] private Image wipe;
    [SerializeField] private float TweenDuration;
   
    private Tween wipeTween;


    public void WipeLeft(Action callback = null)
    {
        Debug.Log("WIPING LEFT");
        wipe.transform.localScale = new Vector3(-1, 1, 1);
        wipe.fillAmount = 0;
        wipe.gameObject.SetActive(true);

        if(wipeTween != null) { wipeTween.Kill(false); }
        wipeTween = wipe.DOFillAmount(1, TweenDuration)
            .OnComplete(() =>
            {
                callback();
            })
            .SetUpdate(true);
    }
    public void WipeRight(Action callback = null)
    {
        //Debug.Log("WIPING RIGHT");
        wipe.transform.localScale = new Vector3(1, 1, 1);
        wipe.fillAmount = 0;
        wipe.gameObject.SetActive(true);

        if (wipeTween != null) { wipeTween.Kill(false); }
        wipeTween = wipe.DOFillAmount(1, TweenDuration)
            .OnComplete(() =>
            {
                if (callback != null)
                {
                    callback();
                }
            })
            .SetUpdate(true);
    }


    public void ClearLeft(Action callback = null)
    {
        wipe.transform.localScale = new Vector3(1, 1, 1);
        wipe.fillAmount = 1;
        wipe.gameObject.SetActive(true);

        if (wipeTween != null) { wipeTween.Kill(false); }
        wipeTween = wipe.DOFillAmount(0, TweenDuration)
            .OnComplete(() =>
            {
                callback();
            })
            .SetUpdate(true);
        Debug.Log("CLEARED LEFT");
    }
    public void ClearRight(Action callback = null)
    {
        wipe.transform.localScale = new Vector3(-1, 1, 1);
        wipe.fillAmount = 1;
        wipe.gameObject.SetActive(true);

        if (wipeTween != null) { wipeTween.Kill(false); }
        wipeTween = wipe.DOFillAmount(0, TweenDuration)
            .OnComplete(() =>
            {
                if (callback != null)
                {
                    callback();
                }
            })
            .SetUpdate(true);
        //Debug.Log("CLEARED RIGHT");
    }
}

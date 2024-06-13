using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTweens : MonoBehaviour
{
    [Header("Tween Settings")]
    [SerializeField] private float tweenDuration;
    // Tweens
    public Tween settingsTween { get; private set; }
    private Sequence settingsSeq;

    public Tween controlsTween { get; private set; }
    public Tween displayTween { get; private set; }
    public Tween soundTween { get; private set; }

    [SerializeField] private float settingsDestination, controlsDestination, displayDestination, soundDestination;
    private float settingsDistanceToMove = 1500;   // Main settings interface -- same as main Pause interface
    private float distanceToMove = 1500;

    [Header("Settings Interfaces")]
    [SerializeField] private GameObject SettingsInterface;
    [SerializeField] private GameObject ControlsInterface;
    [SerializeField] private GameObject DisplayInterface;
    [SerializeField] private GameObject SoundInterface;

    [Header("Main Settings Buttons")]
    [SerializeField] private Button ControlsButton;
    [SerializeField] private Button DisplayButton;
    [SerializeField] private Button SoundButton;

    [Header("First Buttons")]
    // [SerializeField] private Button FirstSettingsButton;
    [SerializeField] private Button FirstControlsButton;
    [SerializeField] private Button FirstDisplayButton;
    [SerializeField] private Button FirstSoundButton;

    [Header("Highlights")]
    [SerializeField] private GameObject ControlsButtonHighlight;
    [SerializeField] private GameObject DisplayButtonHighlight;
    [SerializeField] private GameObject SoundButtonHighlight;

    public void SlideSettingsIn(Action DoAfter = null)
    {
        SettingsInterface.transform.localPosition = new Vector2(settingsDestination-settingsDistanceToMove, SettingsInterface.transform.localPosition.y);
        SettingsInterface.SetActive(true);
        ControlsInterface.SetActive(true);
        ControlsButtonHighlight.SetActive(true);
        // main settings interface & controls (bc controls is first)?
        settingsSeq = DOTween.Sequence();

        settingsSeq.Append(SettingsInterface.transform.DOLocalMoveX(settingsDestination, tweenDuration))
            .Join(ControlsInterface.transform.DOLocalMoveX(controlsDestination, tweenDuration))
            .SetUpdate(true)
            .OnComplete(() =>
            {
                if (DoAfter != null)
                {
                    DoAfter();
                }
                // FirstControlsButton first to set the navigation for the main settings buttons
                FirstControlsButton.Select();
                ControlsButton.Select();
            });
            
    }
    public void SlideSettingsOut(Action DoAfter = null)
    {
        SettingsInterface.transform.localPosition = new Vector2(settingsDestination, SettingsInterface.transform.localPosition.y);
        SettingsInterface.SetActive(true);

        /*        if (settingsTween != null) { settingsTween.Kill(false); }
                settingsTween = SettingsInterface.transform.DOLocalMoveX(settingsDestination-settingsDistanceToMove, tweenDuration)
                    .OnComplete(() =>
                    {
                        if(DoAfter != null)
                        {
                            DoAfter();
                        }
                    })
                    .SetUpdate(true);*/

        // Slide out settings & the active interface
        settingsSeq = DOTween.Sequence();
        if(ControlsInterface.activeInHierarchy)
        {
            settingsSeq.Append(ControlsInterface.transform.DOLocalMoveX(controlsDestination + distanceToMove, tweenDuration))
                .Join(SettingsInterface.transform.DOLocalMoveX(settingsDestination - settingsDistanceToMove, tweenDuration))
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (DoAfter != null)
                    {
                        DoAfter();
                        ControlsInterface.SetActive(false);
                        ControlsButtonHighlight.SetActive(false);
                    }
                });
        }
        else if(DisplayInterface.activeInHierarchy)
        {
            settingsSeq.Append(DisplayInterface.transform.DOLocalMoveX(displayDestination + distanceToMove, tweenDuration))
                .Join(SettingsInterface.transform.DOLocalMoveX(settingsDestination - settingsDistanceToMove, tweenDuration))
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (DoAfter != null)
                    {
                        DoAfter();
                        DisplayInterface.SetActive(false);
                        DisplayButtonHighlight.SetActive(false);
                    }
                });
        }
        else if (SoundInterface.activeInHierarchy)
        {
            settingsSeq.Append(SoundInterface.transform.DOLocalMoveX(soundDestination + distanceToMove, tweenDuration))
                .Join(SettingsInterface.transform.DOLocalMoveX(settingsDestination - settingsDistanceToMove, tweenDuration))
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (DoAfter != null)
                    {
                        DoAfter();
                        SoundInterface.SetActive(false);
                        SoundButtonHighlight.SetActive(false);
                    }
                });
        }
    }
    public void SlideControlsIn()
    {
        ControlsInterface.SetActive(true);
        if(controlsTween != null) { controlsTween.Kill(false); }
        controlsTween = ControlsInterface.transform.DOLocalMoveX(controlsDestination, tweenDuration)
            .OnComplete(() =>
            {
                FirstControlsButton.Select();
            })
            .SetUpdate(true);
    }
    public void SlideControlsOut(Action callback)
    {
        if (controlsTween != null) { controlsTween.Kill(false); }
        controlsTween = ControlsInterface.transform.DOLocalMoveX(controlsDestination+distanceToMove, tweenDuration)
            .OnComplete(() =>
            {
                callback();
                //if(displayTween == null && soundTween == null && SettingsUI.Instance.settingsActive) { ControlsButton.Select(); }     // No other interface coming out
                
            })
            .SetUpdate(true);
        
    }
    public void SlideDisplayIn()
    {
        DisplayInterface.SetActive(true);
        if (displayTween != null) { displayTween.Kill(false); }
        displayTween = DisplayInterface.transform.DOLocalMoveX(displayDestination, tweenDuration)
            .OnComplete(() =>
            {
                FirstDisplayButton.Select();
            })
            .SetUpdate(true);
    }
    public void SlideDisplayOut(Action callback)
    {
        if (displayTween != null) { displayTween.Kill(false); }
        displayTween = DisplayInterface.transform.DOLocalMoveX(displayDestination+distanceToMove, tweenDuration)
            .OnComplete(() =>
            {
                callback();
                //if (controlsTween == null && soundTween == null && SettingsUI.Instance.settingsActive) { DisplayButton.Select(); }     // No other interface coming out
            })
            .SetUpdate(true);

    }
    public void SlideSoundIn()
    {
        SoundInterface.SetActive(true);
        if (soundTween != null) { soundTween.Kill(false); }
        soundTween = SoundInterface.transform.DOLocalMoveX(soundDestination, tweenDuration)
            .OnComplete(() =>
            {
                FirstSoundButton.Select();
            })
            .SetUpdate(true);
    }
    public void SlideSoundOut(Action callback)
    {
        if (soundTween != null) { soundTween.Kill(false); }
        soundTween = SoundInterface.transform.DOLocalMoveX(soundDestination+distanceToMove, tweenDuration)
            .OnComplete(() =>
            {
                callback();
                //if (displayTween == null && controlsTween == null && SettingsUI.Instance.settingsActive) { SoundButton.Select(); }     // No other interface coming out
            })
            .SetUpdate(true);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

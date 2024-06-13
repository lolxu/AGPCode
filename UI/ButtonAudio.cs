using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{

    public void PlayButtonHover()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.buttonHover);
    }
    public void PlayButtonSelect()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.buttonPress);
    }

    public void PlayCheckBox()
    {
        AudioManager.instance.PlayCheckBox();
    }

    public void PlayClickEmpty()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.clickEmpty);
    }

    public void PlayPageForward()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pageForward);
    }

    public void PlayPageBack()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.pageBack);
    }

    public void PlaySliderSet()
    {
        AudioManager.instance.PlaySlider();
    }

    public void PlayStartGame()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.startGame);
    }


}

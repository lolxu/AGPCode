using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public void PlayButtonHover()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.buttonHover, this.transform.position);
    }
    public void PlayButtonSelect()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.buttonPress, this.transform.position);
    }
}

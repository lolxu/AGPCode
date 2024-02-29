using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SlideShowImageSkipper : MonoBehaviour, IPointerClickHandler
{
    public Slideshow _Slideshow;
    public void OnPointerClick(PointerEventData eventData)
    {
        _Slideshow.GoToNextImage();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;  

public class SkipText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color MouseOver = Color.white;
    public Color MouseNotOver = Color.gray;
    public TMP_Text _text;
    
    private bool _mouseOver = false;

    private void Awake()
    {
        _text.color = MouseNotOver;
    }

    public void SetActiveTextColor(bool isActive)
    {
        _text.color = isActive? MouseOver : MouseNotOver;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GlobalSettings.Instance.displayedController == "KEYBOARD")
        {
            _text.color = MouseOver;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GlobalSettings.Instance.displayedController == "KEYBOARD")
        {
            _text.color = MouseNotOver;
        }
    }
}

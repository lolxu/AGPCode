using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillOutAfterPeriod : MonoBehaviour
{
    [SerializeField] private GameObject keyboardPrompt;
    [SerializeField] private GameObject controllerPrompt;

    public CanvasGroup _canvasGroup;
    public bool bController = true;
    private string hotkeyType;
    

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (hotkeyType != GlobalSettings.Instance.displayedController)
        {
            hotkeyType = GlobalSettings.Instance.displayedController;
            switch (hotkeyType)
            {
                case "KEYBOARD":
                    if (bController)
                    {
                        controllerPrompt.SetActive(false);
                        keyboardPrompt.SetActive(true);
                        bController = false;
                    }

                    break;
                case "XBOX":
                case "PLAYSTATION":
                case "OTHER":
                    if (!bController)
                    {
                        controllerPrompt.SetActive(true);
                        keyboardPrompt.SetActive(false);
                        bController = true;
                    }

                    break;
            }
        }
    }
}

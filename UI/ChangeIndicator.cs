using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeIndicator : MonoBehaviour
{
    // Some game objects use sprite renderer, some use image
    [SerializeField] private SpriteRenderer PromptSR;
    [SerializeField] private Image Prompt;

    [SerializeField] private Sprite KeyboardSprite, ControllerSprite;

    public void ChangeIndicatorSprite()
    {
        // Debug.Log("Changing Indicator for " + this.name);
        if(GlobalSettings.Instance)
        {
            switch(GlobalSettings.Instance.displayedController)
            {
                case "KEYBOARD":
                    if(PromptSR)
                    {
                        PromptSR.sprite = KeyboardSprite;
                    }
                    else
                    {
                        Prompt.overrideSprite = KeyboardSprite;
                    }
                    break;
                case "XBOX":
                case "PLAYSTATION":
                case "OTHER":
                    // Controller
                    if (PromptSR)
                    {
                        PromptSR.sprite = ControllerSprite;
                    }
                    else
                    {
                        Prompt.overrideSprite = ControllerSprite;
                    }
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GlobalSettings.ControlsChangedEvent += ChangeIndicatorSprite;
        ChangeIndicatorSprite();        // Initialize indicator to the right sprite
    }
    private void OnDestroy()
    {
        GlobalSettings.ControlsChangedEvent -= ChangeIndicatorSprite;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

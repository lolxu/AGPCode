using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using __OasisBlitz.Player;
using PlayerInput = __OasisBlitz.Player.PlayerInput;

public class SkipCredits : MonoBehaviour
{
    public CreditInputs creditInputs;
    [SerializeField] private LevelNames names;
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float delayBeforeFadeIn = 5f;
    [SerializeField] private RectTransform tweenedUI;
    [SerializeField] private float endScale = 2.65f;
    [SerializeField] private float skipTime = 1.5f;
    private PlayerInput input;

    private void Awake()
    {
        creditInputs = new CreditInputs();

        creditInputs.Credits.SkipCredits.started += StartSkipInput;
        creditInputs.Credits.SkipCredits.canceled += StopSkippingInput;
    }

    private IEnumerator Start()
    {
        yield return null;
        creditInputs.Credits.Enable();
        input = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
        if (input != null)
        {
            input.SwitchCurrentInputState(PlayerInput.PlayerInputState.Nothing);
        }
    }

    private void OnEnable()
    {
        // PlayerInput.StartHoldingSkip += StartSkip;
        // PlayerInput.StopHoldingSkip += StopSkipping;
    }

    private void OnDisable()
    {
        // StopSkipping();
        // PlayerInput.StartHoldingSkip -= StartSkip;
        // PlayerInput.StopHoldingSkip -= StopSkipping;
        creditInputs.Credits.Disable();
        if (input != null)
        {
            input.SwitchCurrentInputState(PlayerInput.PlayerInputState.UI);
        }
    }
    
    private void Skip()
    {
        SceneManager.LoadScene(names.MainMenuSceneName);
    }
    
    // private IEnumerator Start()
    // {
    //     yield return new WaitForSeconds(delayBeforeFadeIn);
    //     skipText.DOColor(Color.white, fadeInTime);
    // }

    private void StartSkipInput(InputAction.CallbackContext context)
    {
        StartSkip();
    }
    
    private void StartSkip()
    {
        if (skipText.color != Color.white)
        {
            //skipText.DOKill();
            skipText.color = Color.white;
        }
        tweenedUI.DOScaleX(endScale, skipTime)
                .OnComplete(Skip).SetUpdate(true);
    }
    
    private void StopSkippingInput(InputAction.CallbackContext context)
    {
        StopSkipping();
    }
    
    private void StopSkipping()
    {
        tweenedUI.DOKill();
        skipText.color = new Color(1f, 1f, 1f, 0f);
        tweenedUI.localScale = new Vector3(0f, tweenedUI.localScale.y, 1f);
    }
}

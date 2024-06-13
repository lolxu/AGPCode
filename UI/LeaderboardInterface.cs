using __OasisBlitz.Utility;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using __OasisBlitz.Player;
using TMPro;

public class LeaderboardInterface : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerAudio playerAudio;

    [Header("Interface Display")]
    [SerializeField] private GameObject MainDisplay;
    [SerializeField] private float hideLocation;
    [SerializeField] private float showLocation;
    [SerializeField] public TextMeshProUGUI TimeText;
    [SerializeField] private GameObject TimeDisplay;
    [SerializeField] private GameObject Top10Display;
    [SerializeField] private GameObject PBDisplay;
    [SerializeField] private GameObject PBText;
    private Timer timer;

    [Header("Buttons")]
    [SerializeField] private Button RetryButton;
    [SerializeField] private Button BurrowButton;

    [Header("Tween")]
    private Sequence DisplaySequence;
    private Sequence PBSequence;
    [SerializeField] private float tweenDuration;


    private void ShowDisplay(Action DoAfter = null)
    {
        DisplaySequence = DOTween.Sequence();

        DisplaySequence.Append(MainDisplay.transform.DOLocalMoveX(showLocation, tweenDuration).SetEase(Ease.OutBounce))     // Main display with buttons slide right
            .Append(TimeDisplay.transform.DOScale(Vector3.one, tweenDuration).SetEase(Ease.InElastic))       // Time text display scale up
            .OnComplete(() =>
            {
                RetryButton.Select();

                ShowPersonalBest(HUDManager.Instance.pb);

                if(DoAfter != null)
                {
                    DoAfter();
                }
            })
            .SetUpdate(true);
    }
    private void HideDisplay(Action DoAfter)
    {
        // Removed for consistency with leaderboard
        /*        DisplaySequence = DOTween.Sequence();

                // Slide all interface back up
                DisplaySequence.Append(TimeDisplay.transform.DOLocalMoveX(hideLocation, tweenDuration))
                    .OnComplete(() => {
                    // DoAfter depends on how this is called -- from Retry or Burrow button
                    if(DoAfter != null)
                        {
                            DoAfter();
                        }
                    })
                    .SetUpdate(true);*/

        DoAfter();
    }

    public void Retry()
    {
        HideDisplay(() =>
        {
            Time.timeScale = 1.0f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            UIManager.Instance.canPauseGame = true;
            playerInput.EnableCharacterControls();
            LevelManager.Instance.LoadAnySceneAsync(SceneManager.GetActiveScene().name, false);
        });
    }
    public void ToBurrow()
    {
        HideDisplay(() =>
        {
            Time.timeScale = 1.0f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            UIManager.Instance.canPauseGame = true;
            playerInput.EnableCharacterControls();
            LevelManager.Instance.LoadAnySceneAsync(Levels.Instance.GetLevels()[0], false);
        });
    }

    public void ShowTop10(bool status, int ranking, GameObject entry)
    {
        Top10Display.SetActive(status);
        Pulsing t10pulse = Top10Display.GetComponentInChildren<Pulsing>();
        t10pulse.StartPulse(ranking / 50);      // 1 = slowest, speed up closer you get to 1st place
        if(status)
        {
            Pulsing epulse = entry.GetComponentInChildren<Pulsing>();
            epulse.StartPulse(ranking / 50);
            Pulsing tpulse = TimeText.GetComponent<Pulsing>();
            tpulse.StartPulse(ranking / 50);
        }

    }

    public void ShowPersonalBest(bool status)
    {
        PBDisplay.transform.localScale = Vector3.zero;
        PBDisplay.SetActive(status);
        if (status)
        {
            PBSequence = DOTween.Sequence();
            PBSequence.Append(PBDisplay.transform.DOScale(Vector3.one, tweenDuration).SetEase(Ease.InElastic))
                .SetUpdate(true);
        }
        StartPBPulsing();
    }
    public void StartPBPulsing()
    {
        Pulsing pbPulse = PBText.GetComponent<Pulsing>();
        pbPulse.StartPulse();
    }
    public void StopPBPulsing()
    {
        Pulsing pbPulse = PBText.GetComponent<Pulsing>();
        pbPulse.StopPulse();
    }

    private void OnEnable()
    {
        if(!playerInput) { playerInput = GameObject.Find("PlayerBase").GetComponent<PlayerInput>(); }
        if(!playerAudio) { playerAudio = GameObject.Find("PlayerBase").GetComponent<PlayerAudio>(); }
        playerAudio.StopDrill();
        HapticsManager.Instance.StopDrillHaptic();
        //Time.timeScale = 0.0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if(MainDisplay.transform.localPosition.x != hideLocation)
        {
            MainDisplay.transform.localPosition = new Vector3(hideLocation, MainDisplay.transform.localPosition.y, MainDisplay.transform.localPosition.z);
        }

        if (UIManager.Instance)
        {
            timer = UIManager.Instance.timer;
            if (timer)
            {
                TimeText.text = Timer.TimeToString(timer.runTime);
            }
        }

        // Start off false, true if needed in ShowDisplay
        PBDisplay.SetActive(false);
        TimeDisplay.transform.localScale = Vector3.zero;

        playerInput.EnableUIControls();
        ShowDisplay();
    }


    private void OnSceneLoaded(Scene name, LoadSceneMode mode)
    {
        this.gameObject.SetActive(false);
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

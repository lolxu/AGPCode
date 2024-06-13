using System;
using __OasisBlitz.__Scripts.Player.Environment.Fruits;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.UI;
using __OasisBlitz.Camera.StateMachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [SerializeField] private Image fruitSelected;       // Display the fruit that is equipped
    [SerializeField] private TextMeshProUGUI fruitQuantity;
    //[SerializeField] private Image pauseHotkey;
    //[SerializeField] private Sprite pauseKeyboardP, pauseKeyboardEsc, pauseGeneric, pauseXbox;
    //[SerializeField] private GameObject blastAvailable, blastUnavailable;
    [SerializeField] private AdaptiveButtonsHUD adaptiveButtons;
    [SerializeField] private GameObject grapplePoint;

    // Scene transition fade settings
    [Header("Scene Transition Fade Settings")] 
    [SerializeField] private Image transitionFade;
    
    // Timer result panel
    [Header("Timer Panel Settings")] 
    [SerializeField] private LeaderboardInterface timerPanel;
    [SerializeField] private LeaderboardInterface timerPanelNoLeaderboard;
    private Timer myTimer;
    public bool pb { get; private set; }

    // Time should be managed in a separate class...
    public Action OnPersonalBestAchieved;

    private Color empty = new Color(1, 1, 1, 0.1019608f), drillixirFruit = new Color(0.3529411f, 0.925838f, 1, 1), highJumpFruit = new Color(1, 0.3537736f, 0.6438807f, 1);

    public bool canInteract { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        fruitSelected.color = empty;
        fruitQuantity.text = " ";
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public GrapplePoint GetGrapplePoint()
    {
        return grapplePoint != null ? grapplePoint.GetComponent<GrapplePoint>() : null;
    }

    public void SetCanInteract(bool status)
    {
        canInteract = status;
        adaptiveButtons.DisplayInteract(canInteract);
    }
    public void SetDisplayDashPrompt(bool status)
    {
        adaptiveButtons.DisplayDashPrompt(status);
    }
    public void SetBlastNotAvailable()
    {
        // blastAvailable.SetActive(false);
        // blastUnavailable.SetActive(true);
        adaptiveButtons.SetBlastReady(false);
    }
    
    public void SetBlastAvailable()
    {
        // blastAvailable.SetActive(true);
        // blastUnavailable.SetActive(false);
        adaptiveButtons.SetBlastReady(true);
    }

    public void ToggleAdaptiveHud(bool status)
    {
        // Override -- don't show at all in burrow or slideshow
        if (SceneManager.GetActiveScene().name.ToLower().Contains("burrow") || SceneManager.GetActiveScene().name.ToLower().Contains("slideshow"))
        {
            adaptiveButtons.AdaptiveButtons.SetActive(false);
            return;
        }
        adaptiveButtons.AdaptiveButtons.SetActive(status);
    }
    public void ToggleTimerDisplay(bool status)
    {
        // Override -- don't show at all in burrow or slideshow
        if (SceneManager.GetActiveScene().name.ToLower().Contains("burrow") || SceneManager.GetActiveScene().name.ToLower().Contains("slideshow"))
        {
            UIManager.Instance.TimerText().SetActive(false);
            return;
        }
        UIManager.Instance.TimerText().SetActive(status);
    }

    private LeaderboardInterface GetCurrentLeaderboardInterface()
    {
#if LEADERBOARD
        return timerPanel;
#else
        return timerPanelNoLeaderboard;
#endif
    }

    public void ShowHideTimerPanel()
    {
        UIManager.Instance.canPauseGame = false;
        float currentTime = -1.0f;
        myTimer = UIManager.Instance.gameObject.GetComponent<Timer>();
        // UIManager.Instance.StopTime();
        LeaderboardInterface currLeaderboardInterface = GetCurrentLeaderboardInterface();
        if (currLeaderboardInterface.gameObject.activeInHierarchy)
        {
            currLeaderboardInterface.gameObject.SetActive(false);
        }

        else
        {
            CameraStateMachine.Instance.isLoadRestart = false;
            if (myTimer)
            {
                GetCurrentLeaderboardInterface().TimeText.text = Timer.TimeToString(myTimer.runTime);
                currentTime = myTimer.runTime;
            }

            XMLFileManager.Instance.Load();
            XMLFileManager.Instance.LoadTimeData();

            // Saving Time status if is best time
            if (!SceneManager.GetActiveScene().name.Contains("Burrow") && !SceneManager.GetActiveScene().name.Contains("Onboard"))
            {
                bool isPB = myTimer.personalBest > currentTime || myTimer.personalBest <= 0.0f;
                pb = isPB;
                if (isPB)
                {
                    if (OnPersonalBestAchieved != null)
                    {
                        OnPersonalBestAchieved();
                    }
                    //personalBest.SetActive(true);
                    XMLFileManager.Instance.SaveLevelStatus(SceneManager.GetActiveScene().name, myTimer.runTime);
                }
/*                else
                {
                    personalBest.SetActive(false);
                }*/
                GetCurrentLeaderboardInterface().gameObject.SetActive(true);
                UIManager.Instance.RestartTime();
                UIManager.Instance.HideTimer();
                //StartCoroutine(WaitToLoadBurrow());
            }
            else
            {
                transitionFade.DOFade(1.0f, LevelManager.Instance.m_transitionDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(
                        () =>
                        {
                            LoadNextScene();
                        });
            }
        }
    }

    public Image GetSceneTransitionImage()
    {
        return transitionFade;
    }
    
    private IEnumerator WaitToLoadBurrow()
    {
        yield return new WaitForSeconds(2.0f);
        transitionFade.DOFade(1.0f, LevelManager.Instance.m_transitionDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(
                () =>
                {
                    ShowHideTimerPanel();
                    LoadNextScene();
                });
    }

    private void LoadNextScene()
    {
        LevelSettingsManager perLevelSettings = FindObjectOfType<LevelSettingsManager>();
        if (perLevelSettings == null
            || perLevelSettings.settings == null
            || perLevelSettings.settings.NextLevelOnCollectFlower.Length == 0)
        {
            LevelManager.Instance.LoadBurrowAsync();
        }
        else
        {
            // load custom level:
            LevelManager.Instance.LoadAnySceneAsync(perLevelSettings.settings.NextLevelOnCollectFlower, false);
        }
    }
    private void OnSceneLoaded(Scene name, LoadSceneMode mode)
    {
        ToggleAdaptiveHud(GlobalSettings.Instance.controlsHUD);
    }
}

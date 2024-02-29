using __OasisBlitz.__Scripts.Player.Environment.Fruits;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Camera.StateMachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using __OasisBlitz.Player;
using __OasisBlitz.Utility;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [SerializeField] private Image fruitSelected;       // Display the fruit that is equipped
    [SerializeField] private TextMeshProUGUI fruitQuantity;
    [SerializeField] private Image pauseHotkey;
    [SerializeField] private Sprite pauseKeyboardP, pauseKeyboardEsc, pauseGeneric, pauseXbox;
    [SerializeField] private GameObject blastAvailable, blastUnavailable;
    [SerializeField] private AdaptiveButtonsHUD adaptiveButtons;

    // Scene transition fade settings
    [Header("Scene Transition Fade Settings")] 
    [SerializeField] private Image transitionFade;
    
    // Timer result panel
    [Header("Timer Panel Settings")] 
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private TextMeshProUGUI timerResult;
    [SerializeField] private GameObject personalBest;
    private Timer myTimer;

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
    }

    private void Update()
    {
        /*
        if(FruitsManager.Instance.CurrentFruit != null)
        {
            switch (FruitsManager.Instance.CurrentFruit.GetFruitName())
            {
                case "ElixirReplenish":
                    fruitSelected.color = drillixirFruit;
                    fruitQuantity.text = FruitsManager.Instance.GetFruitUseAmount().ToString();

                    break;
                case "HighJump":
                    fruitSelected.color = highJumpFruit;
                    fruitQuantity.text = " ";

                    break;

                /*  ================    Add More Fruits As We Go~   =================== 
                default:
                    fruitSelected.color = empty;
                    fruitQuantity.text = " ";
                    break;
            }
        }
        else
        {
            fruitSelected.color = empty;
            fruitQuantity.text = " ";
        }
        */
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
        blastAvailable.SetActive(false);
        blastUnavailable.SetActive(true);
        adaptiveButtons.SetBlastReady(false);
    }
    
    public void SetBlastAvailable()
    {
        blastAvailable.SetActive(true);
        blastUnavailable.SetActive(false);
        adaptiveButtons.SetBlastReady(true);
    }

    public void ShowHideTimerPanel()
    {
        UIManager.Instance.canPauseGame = false;
        float currentTime = -1.0f;
        myTimer = UIManager.Instance.gameObject.GetComponent<Timer>();
        UIManager.Instance.StopTime();
        
        if (timerPanel.activeInHierarchy)
        {
            timerPanel.SetActive(false);
        }
        else
        {
            CameraStateMachine.Instance.isLoadRestart = false;
            if (myTimer)
            {
                timerResult.text = myTimer.GetTime();
                currentTime = myTimer.runTime;
            }

            XMLFileManager.Instance.Load();

            // Saving Time status if is best time
            if (!SceneManager.GetActiveScene().name.Contains("Burrow") && !SceneManager.GetActiveScene().name.Contains("Onboard"))
            {
                bool isPB = myTimer.personalBest > currentTime || myTimer.personalBest <= 0.0f;
                if (isPB)
                {
                    personalBest.SetActive(true);
                    XMLFileManager.Instance.SaveLevelStatus(SceneManager.GetActiveScene().name, myTimer.runTime);
                }
                else
                {
                    personalBest.SetActive(false);
                }

                timerPanel.SetActive(true);
                UIManager.Instance.RestartTime();
                UIManager.Instance.HideTimer();
                StartCoroutine(WaitToLoadBurrow());
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
            LevelManager.Instance.LoadAnySceneAsync(perLevelSettings.settings.NextLevelOnCollectFlower);
        }
    }
}

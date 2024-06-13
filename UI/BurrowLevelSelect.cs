using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Cannon;
using __OasisBlitz.__Scripts.Collectables;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class BurrowLevelSelect : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject Interface;
    [SerializeField] private Canvas InterfaceCanvas;
    [SerializeField] private Button LeftArrow;
    [SerializeField] private Button RightArrow;
    [SerializeField] private TextMeshProUGUI LevelName;
    [SerializeField] private TextMeshProUGUI LevelPR;
    [SerializeField] private GameObject LevelsObject;
    [SerializeField] private Button[] LevelsButtons;
    [SerializeField] private Button CloseButton;
    [SerializeField] private Image SelectedLevelGlow;
    [SerializeField] private float scaleInterfaceTo;
    private PlayerInput pInput;
    [Header("World Elements")]
    [SerializeField] private CannonLevelTransit Cannon;
    [SerializeField] private LevelCannonObjects LevelCannonObject;
    [SerializeField] private GameObject LevelSelectCam;
    [SerializeField] private GameObject BurrowCam;

    // Tweens
    private Sequence InterfaceSequence, OpenSequence, CloseSequence;
    private Tween GlowTween, InterfaceTween;
    private float tweenDuration = 0.2f;

    
    private float[] levelsPositions = { 0.0f, -1475.0f, -2900.0f };      // [0] Level 1, [1] Level 2, [2] Level 3 -- tween Levels to here
    // private float[] levelsScale = { 0.5f, .75f };   // [0] unselected, [1] selected
    private Color[] ArrowColors = { new Color(0f, 0f, 0f, 1), new Color(1.0f, 1.0f, 1.0f, 1) };   // [0] when can't move, [1] when can move

    private int selectedLevelIndex;

    public void SetCannonLevelByIndex(int index)
    {
        switch(index)
        {
            // case 0 -- burrow, don't need here
            case 1:
                SetCannonLevel(Levels.Instance.GetLevels()[1]);
                break;
            case 2:
                SetCannonLevel(Levels.Instance.GetLevels()[2]);
                break;
            case 3:
                SetCannonLevel(Levels.Instance.GetLevels()[3]);
                break;
        }
    }

    public void SetCannonLevel(string level)
    {
        if (level != SceneManager.GetActiveScene().name)
        {
            // CameraStateMachine.Instance.isLoadRestart = false;
        }
        // LoadScreenCanvas.gameObject.SetActive(true);
        // loadScreen.LoadScene(level);
        LevelCannonObject.loadSceneName = level;

        UIManager.Instance.UnpauseGame();
        // pInput.EnableCharacterControls();

        CloseLevelSelectInterface();

        Cannon.SceneSelected();
    }

    private void SetLevelName(int index)
    {
        switch(index)
        {
            case 0:
                LevelName.text = "Pillars";
                break;
            case 1:
                LevelName.text = "Serpent";
                break;
/*            case 2:
                LevelName.text = "Temple";
                break;*/
        }
    }
    private void SetPBText(string sceneName)
    {
        float PB = XMLFileManager.Instance.LookupPBTime(sceneName);
        if (PB == -1)
        {
            LevelPR.gameObject.SetActive(false);
        }
        else
        {
            LevelPR.gameObject.SetActive(true);
            int min = (int)PB / 60;
            int sec = (int)PB - 60 * min;
            int ms = (int)(1000 * (PB - min * 60 - sec));
            LevelPR.text = string.Format("{0:00}:{1:00}:{2:000}", min, sec, ms);
            //LevelPR.text = PB.ToString();
        }
    }
    //// Start is called before the first frame update
    //void Start()
    //{
    //    SetPBText(this.gameObject.name);
    //}
    public void Left() // Left Arrow   
    {
        if (selectedLevelIndex >= 1) { SelectLevel(selectedLevelIndex - 1); }
    }
    public void Right() // Right Arrow   
    {
        if (selectedLevelIndex <= 1) { SelectLevel(selectedLevelIndex + 1); }
    }
    public void SelectLevel(int index)
    {

        // Hovering over same level (from Close or Left/Right button maybe)
        if(index == selectedLevelIndex)
        {
            // Switch to handle first opening to initialize game objects and texts
            switch(index)
            {
                case 0:
                    LevelsButtons[0].Select();
                    EnableArrow(LeftArrow, false);
                    EnableArrow(RightArrow, CollectableManager.Instance.LookupPlantPlacement(1));
                    SetLevelName(0);
                    SetPBText(Levels.Instance.GetLevels()[1]);
                    break;
                case 1:
                    LevelsButtons[1].Select();
                    EnableArrow(LeftArrow, true);
                    EnableArrow(RightArrow, false);
                    SetLevelName(1);
                    SetPBText(Levels.Instance.GetLevels()[2]);
                    break;
            }
            EnableGlow(true);
            return;
        }


        selectedLevelIndex = index;
        switch (selectedLevelIndex)
        {
            case 0:
                LevelsButtons[0].Select();
                EnableArrow(LeftArrow, false);
                EnableArrow(RightArrow, CollectableManager.Instance.LookupPlantPlacement(1));
                EnableGlow(false, () => TweenToLevel());
                break;
            case 1:
                LevelsButtons[1].Select();
                EnableArrow(LeftArrow, true);
                EnableArrow(RightArrow, false);
                EnableGlow(false, () => TweenToLevel());
                break;
/*            case 2:
                LevelsButtons[2].Select();
                EnableArrow(LeftArrow, true);
                EnableArrow(RightArrow, false);
                EnableGlow(false, () => TweenToLevel());
                break;*/
            default:
                //EnableArrow(LeftArrow, false);
                //EnableArrow(RightArrow, false);
                EnableGlow(false, () => TweenToLevel());
                break;
        }
    }

    // For buttons
    public void EnableGlowButtons(bool status)
    {
        /*        if(GlowTween != null) { GlowTween.Kill(false); }

                GlowTween = SelectedLevelGlow.DOFade(status ? 1.0f : 0.0f, tweenDuration/10)
                    .OnComplete(() => {
                        if (DoAfter != null) { 
                            DoAfter();
                        }
                    });*/
        SelectedLevelGlow.color = new Color(SelectedLevelGlow.color.r, SelectedLevelGlow.color.g, SelectedLevelGlow.color.b, status ? 1.0f : 0.0f);
    }
    private void EnableGlow(bool status, Action DoAfter = null)
    {
/*        if(GlowTween != null) { GlowTween.Kill(false); }

        GlowTween = SelectedLevelGlow.DOFade(status ? 1.0f : 0.0f, tweenDuration/10)
            .OnComplete(() => {
                if (DoAfter != null) { 
                    DoAfter();
                }
            });*/
        SelectedLevelGlow.color = new Color(SelectedLevelGlow.color.r, SelectedLevelGlow.color.g, SelectedLevelGlow.color.b, status ? 1.0f : 0.0f);
        if(DoAfter != null)
        {
            DoAfter();
        }
    }
    private void EnableArrow(Button arrow, bool status)
    {
        arrow.gameObject.SetActive(status);
        //arrow.image.color = status ? ArrowColors[1] : ArrowColors[0];
        //arrow.interactable = status;
    }
    private void TweenToLevel()
    {
        InterfaceSequence = DOTween.Sequence();
        if(InterfaceTween != null) { InterfaceTween.Kill(false); }

        switch (selectedLevelIndex)
        {
            case 0:
                SetLevelName(0);
                SetPBText(Levels.Instance.GetLevels()[1]);
                InterfaceTween = LevelsObject.transform.DOLocalMoveX(levelsPositions[0], tweenDuration)
                    .OnComplete(() =>
                    {
                        EnableGlow(true);
                    });
                break;
            case 1:
                SetLevelName(1);
                SetPBText(Levels.Instance.GetLevels()[2]);
                InterfaceTween = LevelsObject.transform.DOLocalMoveX(levelsPositions[1], tweenDuration)
                    .OnComplete(() =>
                    {
                        EnableGlow(true);
                    });
                break;
/*            case 2:
                SetLevelName(2);
                SetPBText(Levels.Instance.GetLevels()[3]);
                InterfaceTween = LevelsObject.transform.DOLocalMoveX(levelsPositions[0], tweenDuration)
                    .OnComplete(() =>
                    {
                        EnableGlow(true);
                    });
                break;*/
        }
    }

    public void OpenLevelSelectInterface()
    {
        UIManager.Instance.canPauseGame = false;
        pInput.EnableUIControls();
        StartCoroutine(FocusBurrowLevelInterface());
    }

    public void CancelLevelSelect()
    {
        CloseLevelSelectInterface();
        Cannon.KickPlayerOutOfCannon();
        UIManager.Instance.canPauseGame = true;
        pInput.EnableCharacterControls();
    }
    public void CloseLevelSelectInterface()
    {
        CloseInterfaceTween();
        //levelSelectInterface.SetActive(false);
        UnFocusBurrowLevelInterface();
        UIManager.Instance.canPauseGame = true;
        pInput.EnableCharacterControls();
    }
    private void OpenInterfaceTween()
    {
        EnableGlow(false);      // Hide glow first, show after level selected
        OpenSequence = DOTween.Sequence();
        OpenSequence.Append(Interface.transform.DOScale(new Vector3(scaleInterfaceTo, scaleInterfaceTo, scaleInterfaceTo), tweenDuration))      // Open interface
            .OnComplete(() =>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            });
        // LevelsButtons[0].Select();
        //if (LevelsButtons[2].gameObject.activeInHierarchy)
        //{
        //    SelectLevel(2);
        //}
        if (LevelsButtons[1].gameObject.activeInHierarchy)
        {
            SelectLevel(1);
        }
        else if (LevelsButtons[0].gameObject.activeInHierarchy)
        {
            SelectLevel(0);
        }
    }
    private void CloseInterfaceTween()
    {
        //if (InterfaceTween != null) { InterfaceTween.Kill(false); }
        CloseSequence = DOTween.Sequence();
        CloseSequence.Append(Interface.transform.DOScale(Vector3.zero, tweenDuration).SetUpdate(true))
            .OnComplete(() =>
            {
                // Reset values
                selectedLevelIndex = 0;
                LevelsObject.transform.position = new Vector3(levelsPositions[0], LevelsObject.transform.position.y, LevelsObject.transform.position.z);

                InterfaceCanvas.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            });
    }

    private IEnumerator FocusBurrowLevelInterface()
    {
        // CameraStateMachine.Instance.freeLookCam.gameObject.SetActive(false);
        LevelSelectCam.SetActive(true);
        //
        // yield return null;
        // yield return null;
        //
        // while (CameraStateMachine.Instance.CameraSurface.GetComponent<CinemachineBrain>().IsBlending)
        // {
        //     yield return null;
        // }
        //
        // LevelSelectCam.SetActive(false);
        
        yield return null;
        // yield return null;
        //
        // while (CameraStateMachine.Instance.CameraSurface.GetComponent<CinemachineBrain>().IsBlending)
        // {
        //     yield return null;
        // }

        if (!InterfaceCanvas.worldCamera)
        {
            InterfaceCanvas.worldCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        }
        InterfaceCanvas.gameObject.SetActive(true);
        SetActiveButtons();
        OpenInterfaceTween();
    }
    public void UnFocusBurrowLevelInterface()
    {
        pInput.EnableCharacterControls();
        LevelSelectCam.SetActive(false);
        
    }

    private void SetActiveButtons()
    {
        // Just have Level 1 always active -- interface only accessed when Level 1 should be available
        bool level2Active = CollectableManager.Instance.LookupPlantPlacement(1);
        // bool level3Active = CollectableManager.Instance.LookupPlantPlacement(2);

        LevelsButtons[0].gameObject.SetActive(true);
        LevelsButtons[1].gameObject.SetActive(level2Active);
        // LevelsButtons[2].gameObject.SetActive(level3Active);
    }

    private void Awake()
    {
        pInput = GameObject.Find("PlayerBase").GetComponent<PlayerInput>();

    }
}

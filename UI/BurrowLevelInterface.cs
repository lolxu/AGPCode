using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Cannon;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class BurrowLevelInterface : MonoBehaviour
{
    private LoadingScreen loadScreen;
    private PlayerInput pInput;
    [SerializeField] private Canvas levelSelectCanvas;
    [SerializeField] private GameObject levelSelectInterface;
    [SerializeField] private Canvas LoadScreenCanvas;
    [SerializeField] private LevelCannonObjects levelSelectCannon;
    [SerializeField] private GameObject levelSelectCamera;
    [SerializeField] private GameObject burrowCamera;

    [SerializeField] private float tweenDuration;
    private Tween levelSelectTween;

    // For tracking
    private Transform orgTrackingTransform;
    private Transform orgLookAtTransform;
    public CannonLevelTransit levelCannon;

    public void SetCannonLevel(string level)
    {
        if (level != SceneManager.GetActiveScene().name)
        {
            // CameraStateMachine.Instance.isLoadRestart = false;
        }
        // LoadScreenCanvas.gameObject.SetActive(true);
        // loadScreen.LoadScene(level);
        levelSelectCannon.loadSceneName = level;
        
        UIManager.Instance.UnpauseGame();
        // pInput.EnableCharacterControls();
        
        CloseLevelSelectInterface();
        
        levelCannon.SceneSelected();
    }
    public void OpenLevelSelectInterface()
    {
        if(!levelSelectCanvas.worldCamera)
        {
            levelSelectCanvas.worldCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        }
        UIManager.Instance.canPauseGame = false;
        pInput.DisableCharacterControls();
        StartCoroutine(FocusBurrowLevelInterface());
    }

    public void CancelLevelSelect()
    {
        CloseLevelSelectInterface();
        levelCannon.KickPlayerOutOfCannon();
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
        if (levelSelectTween != null) { levelSelectTween.Kill(false); }

        levelSelectTween = levelSelectInterface.transform.DOScale(new Vector3(67.0f, 67.0f, 67.0f), tweenDuration)
            .OnComplete(() =>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            });
    }
    private void CloseInterfaceTween()
    {
        if (levelSelectTween != null) { levelSelectTween.Kill(false); }

        levelSelectTween = levelSelectInterface.transform.DOScale(Vector3.zero, tweenDuration)
            .OnComplete(() => {
                levelSelectCanvas.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            });
    }
    private IEnumerator FocusBurrowLevelInterface()
    {
        // pInput.DisableCharacterControls();
        if (burrowCamera)
        {
            burrowCamera.SetActive(false);
        }
        CameraStateMachine.Instance.freeLookCam.gameObject.SetActive(false);

        levelSelectCamera.SetActive(true);

        yield return null;
        yield return null;
        
        while (CameraStateMachine.Instance.CameraSurface.GetComponent<CinemachineBrain>().IsBlending)
        {
            yield return null;
        }

        levelSelectCanvas.gameObject.SetActive(true);
        OpenInterfaceTween();
    }
    public void UnFocusBurrowLevelInterface()
    {
        pInput.EnableCharacterControls();
        levelSelectCamera.SetActive(false);
        if (burrowCamera)
        {
            burrowCamera.SetActive(true);
        }
        else
        {
            CameraStateMachine.Instance.freeLookCam.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!pInput) { pInput = GameObject.Find("PlayerBase").GetComponent<PlayerInput>(); }
        // if (!loadScreen) { loadScreen = LoadScreenCanvas.GetComponent<LoadingScreen>(); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

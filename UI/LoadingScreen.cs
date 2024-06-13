using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    [SerializeField] private Canvas LoadScreenCanvas;

    [SerializeField] private GameObject LoadScreen;
    [SerializeField] private ScreenWipe wipe;

    [SerializeField] private TextMeshProUGUI[] loadingText;
    [SerializeField] private Slider progressBarFill;

    // Loading Scene no longer used
    public void LoadScene(string sceneName)
    {
        wipe.WipeRight(() => {
            //LoadScreen.SetActive(true);
            if (!(SceneManager.GetActiveScene().name == "MainMenu")
               && !(SceneManager.GetActiveScene().name == "Slideshow")
               && !(SceneManager.GetActiveScene().name == "EndSlideshow")
               && !(SceneManager.GetActiveScene().name == "MainMenuToSteal"))
            {
                PlayerStateMachine ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
                if (ctx)
                {
                    ctx.PlayerPhysics.SetVelocity(Vector3.zero);
                }
            }

            StartCoroutine(LoadSceneAsync(sceneName));
        });

    }

    public void LoadCurrentScene()
    {
        // LoadScene(SceneManager.GetActiveScene().name);
        LevelManager.Instance.LoadAnySceneAsync(SceneManager.GetActiveScene().name, false);
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        GameMetadataTracker.Instance.ResetAllCheckpointForLevel(SceneManager.GetActiveScene().name);
        
        // Stop Timer
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RestartTime();
            UIManager.Instance.HideTimer();
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!op.isDone)
        {
            //float progress = Mathf.Clamp01(op.progress / 0.9f);
            //progressBarFill.value = progress;

            yield return null;
        }
        // LoadScreen.SetActive(false);
        //Debug.Log("LoadingScreen --> ClearRight");
        wipe.ClearRight();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name.Contains("Slideshow"))
        {
            LoadScreenCanvas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }
        else
        {
            LoadScreenCanvas.worldCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        }
    }
    private void Awake()
    {
        // if(GameObject.Find("LoadScreenCanvas") != null && GameObject.Find("LoadScreenCanvas") != this) { Destroy(this.gameObject); }

        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

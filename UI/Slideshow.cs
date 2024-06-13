using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using __OasisBlitz.Utility;
using DG.Tweening;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

[Serializable]
public class Slide
{
    public Sprite slideImage;
    public float slideDuration = 1.0f;
    public bool hasFade = false;
}

public class Slideshow : MonoBehaviour
{
    public Slide[] slides;
    public float imageDuration;
    public float fadeDuration;
    public float darknessDuration;
    public string nextSceneName;
    public Image image;
    public Image fadeOutImage;
    public Image finalFadeOutImage;

    public float lastSlideFadeDuration;
    public float musicEndingFadeTime;

    public int currSlide;

    private bool hasCancelRequest = false;
    
    public Action OnSlideshowEnd;
    public Action OnSlideshowMusicEnd;
    public Action OnSlideshowSkip;

    //Slideshow Music
    private bool canSkip = true;
    private bool isEndCutScene = false;
    
    // josh stuff
    [SerializeField] private LoadingScreen loadScreen;
    [SerializeField] private Canvas LoadScreenCanvas;
    
    public GameObject AButtonImage;
    private string hotkeyType;

    public void GoToNextScene() {
        InLevelMetrics.Instance.LogEvent(MetricAction.CutsceneSkip);
        //loadScreen.LoadScene(nextSceneName);
        LoadingScreen.Instance.LoadScene(nextSceneName);

        // LoadScreen has animated wipe now
        /*
        finalFadeOutImage.DOColor(Color.black, fadeDuration / 2.0f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            //LoadScreenCanvas.gameObject.SetActive(true);
            // XMLFileManager.Instance.Load();
            loadScreen.LoadScene(nextSceneName);
        });
        */
        // StartCoroutine(FadeOutNextScene());
    }
    
    public void GoToNextImage()
    {
        hasCancelRequest = true;
    }

    private void Awake()
    {
        Debug.Log("Awake, about to look for canvas");
        loadScreen = GameObject.FindObjectOfType(typeof(LoadingScreen)) as LoadingScreen;
        Debug.Log("Found canvas");
    }

    // Start is called before the first frame update
    public IEnumerator StartSlideshow(bool endCutScene = false)
    {
        Debug.Log("STARTING SLIDESHOW");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (endCutScene)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.musicEndCutScene);
            isEndCutScene = true;
        }
        else
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.musicCutScene);
        }

        hotkeyType = GlobalSettings.Instance.displayedController;

        Time.timeScale = 1f;
        // Debug.Log("start!");
        currSlide = 0;
        while (currSlide < slides.Length)
        {
            yield return StartCoroutine(ShowSlide(slides[currSlide]));
            currSlide++;
        }
        
        EndSlideshow();

        yield return new WaitForSeconds(musicEndingFadeTime);
        
        EndSlideShowMusic();
    }

    public void EndSlideshow()
    {
        canSkip = false;
        StopSkipping();
        GetComponent<Canvas>().enabled = false;
        OnSlideshowEnd?.Invoke();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UIManager.Instance.canPauseGame = true;
    }

    public void EndSlideShowMusic()
    {
        OnSlideshowMusicEnd?.Invoke();
        gameObject.SetActive(false);
    }

    private bool lastSlideFaded = true;

    public IEnumerator ShowSlide(Slide slide)
    {
        image.sprite = slide.slideImage;
        
        // fade back in if we left off on a fade
        if (lastSlideFaded)
        {
            fadeOutImage.color = Color.black;
            // leave it black for this long
            yield return new WaitForSeconds(darknessDuration / 2);
        
            // fade out, then wait till next image 
            fadeOutImage.DOColor(Color.clear, fadeDuration).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(fadeDuration);
        }
        
        // if not fading, ignore fade code
        if (!slide.hasFade)
        {
            lastSlideFaded = false;
            // allow you to cancel here!
            float timer = slide.slideDuration;
            hasCancelRequest = false;
            while (timer >= 0
                   && !hasCancelRequest)
            {
                yield return null;
                timer -= Time.deltaTime;
            }
        }
        else
        {
            lastSlideFaded = true;
            // fade to black
            fadeOutImage.DOColor(Color.black, fadeDuration).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(fadeDuration);
            // wait for half a darkness duration
            yield return new WaitForSeconds(darknessDuration / 2);
        }
    }

    [SerializeField] private Transform tweenedUI;
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private float delayBeforeFadeIn = 5f;
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float endScale = 2.65f;
    [SerializeField] private float skipTime = 1.5f;
    private void OnEnable()
    {
        PlayerInput.StartHoldingSkip += StartSkip;
        PlayerInput.StopHoldingSkip += StopSkipping;
    }

    private void OnDisable()
    {
        StopSkipping();
        PlayerInput.StartHoldingSkip -= StartSkip;
        PlayerInput.StopHoldingSkip -= StopSkipping;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(delayBeforeFadeIn);
        skipText.DOColor(Color.white, fadeInTime);
    }

    private void StartSkip()
    {
        if (skipText.color != Color.white)
        {
            skipText.DOKill();
            skipText.color = Color.white;
        }
        if (canSkip)
        {
            tweenedUI.DOScale(new Vector3(endScale, tweenedUI.localScale.y, 1f), skipTime)
                .OnComplete(SkipSlideshow);
        }
    }

    private void SkipSlideshow()
    {
        if (isEndCutScene)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.musicEndCutSceneEnd);    
        }
        else
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.musicCutSceneEnd);
        }

        OnSlideshowSkip?.Invoke();
        gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Can't seem to pause after skipping
        UIManager.Instance.canPauseGame = true;
    }
    private void StopSkipping()
    {
        tweenedUI.DOKill();
        tweenedUI.localScale = new Vector3(0f, tweenedUI.localScale.y, 1f);
    }
}

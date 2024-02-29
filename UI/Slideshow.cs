using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public int currSlide;

    private bool hasCancelRequest = false;
    
    // josh stuff
    [SerializeField] private LoadingScreen loadScreen;
    [SerializeField] private Canvas LoadScreenCanvas;

    public SkipText _SkipText;
    public GameObject AButtonImage;
    private string hotkeyType;

    public void GoToNextScene()
    {
        loadScreen.LoadScene(nextSceneName);

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
        loadScreen = GameObject.Find("LoadScreenCanvas").GetComponent<LoadingScreen>();
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        hotkeyType = GlobalSettings.Instance.displayedController;
        if (GlobalSettings.Instance.displayedController != "KEYBOARD")
        {
            _SkipText.SetActiveTextColor(true);
            AButtonImage.gameObject.SetActive(true);
        }
        else
        {
            _SkipText.SetActiveTextColor(false);
            AButtonImage.gameObject.SetActive(false);
        }
        
        Time.timeScale = 1f;
        // Debug.Log("start!");
        currSlide = 0;
        while (currSlide < slides.Length)
        {
            yield return StartCoroutine(ShowSlide(slides[currSlide]));
            currSlide++;
        }
        GoToNextScene();
    }

    private void Update()
    {
        if (hotkeyType != GlobalSettings.Instance.displayedController)
        {
            hotkeyType = GlobalSettings.Instance.displayedController;
            switch (hotkeyType)
            {
                case "KEYBOARD":
                    _SkipText.SetActiveTextColor(false);
                    AButtonImage.gameObject.SetActive(false);
                    break;
                case "XBOX":
                case "PLAYSTATION":
                case "OTHER":
                    _SkipText.SetActiveTextColor(true);
                    AButtonImage.gameObject.SetActive(true);
                    break;
            }
        }
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
}

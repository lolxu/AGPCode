using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

/*
 * John note: selfishly, in the interest of time, I have coupled the idea of deciding which scene to start in with the
 *              UI code for this first scene. But since I only need this scene for expo... LOL
 */
public class FirstLevelGateway : MonoBehaviour
{
    [SerializeField] private LevelNames levelNames;
    // private PlayerInput pInput;

    public static FirstLevelGateway Instance;

    public GameObject UsernameCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameObject sceneEssentials = GameObject.FindGameObjectWithTag("Essentials");
        if (sceneEssentials)
        {
            Destroy(sceneEssentials);
        }
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        bool bShouldLoadOnboarding = XMLFileManager.Instance.GetNumPlantsCollected() == 0;
        if (!bShouldLoadOnboarding)
        {
            GoToMainMenuBurrow();
        }
        else
        {
            UsernameCanvas.SetActive(true);
                UsernameEntrySequence usernameEntrySequence = UsernameCanvas.GetComponent<UsernameEntrySequence>();
                usernameEntrySequence.FadeImage.color = Color.black;
            #if !LEADERBOARD
                LoadOnboarding();
            #else
                usernameEntrySequence.FadeImage.DOFade(0.0f, 3f);
            #endif
        }
    }

    public void GoToMainMenuBurrow()
    {
        if (XMLFileManager.Instance.ShouldPlayCutscene())
        {
            XMLFileManager.Instance.SaveCutsceneViewed();
        }
        if (XMLFileManager.Instance.ShouldPlayEndingCutscene())
        {
            XMLFileManager.Instance.SaveEndingCutsceneViewed();
        }
        
        SceneManager.LoadSceneAsync(levelNames.BurrowSceneName);
    }

    public void LoadOnboarding()
    {
        UsernameEntrySequence entrySequence = FindObjectOfType<UsernameEntrySequence>();
        entrySequence.FadeOutAndLoadScene(levelNames.OnboardingSceneName);
        Instance = null;
        Destroy(gameObject);
    }
}

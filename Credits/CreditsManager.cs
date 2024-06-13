using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class CreditItem
{
    public GameObject item;
    public bool timeBased = false;
    public float timeToAdvance;
    public bool freezeTime = false;
    public bool preload = false;
}
public class CreditsManager : MonoBehaviour
{
    public static CreditsManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            // Debug.LogError("Found more than one Audio Manager in the scene");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    [SerializeField] private LevelNames names;
    [SerializeField] private List<CreditItem> creditItems;
    private int currIndex = 1;
    private bool advance = false;

    private IEnumerator Start()
    {
        yield return null;
        for (;currIndex < creditItems.Count; currIndex++)
        {
            creditItems[currIndex - 1].item.SetActive(false);
            creditItems[currIndex].item.SetActive(true);
            //Set timescale
            if (creditItems[currIndex].freezeTime)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
            //check for preload
            if (creditItems[currIndex].preload)
            {
                if (currIndex + 1 < creditItems.Count)
                {
                    creditItems[currIndex + 1].item.SetActive(true);
                }
            }
            //check if time based
            if (creditItems[currIndex].timeBased)
            {
                //advance after time
                yield return new WaitForSecondsRealtime(creditItems[currIndex].timeToAdvance);
                advance = false;
            }
            else
            {
                while (!advance)
                {
                    yield return null;
                }
                advance = false;
            }
        }
        SceneManager.LoadScene(names.MainMenuSceneName);
    }

    public void AdvanceCredits()
    {
        advance = true;
    }
}

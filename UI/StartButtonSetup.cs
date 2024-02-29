using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButtonSetup : MonoBehaviour
{
    [Header("Overrides")]
    [Tooltip("Override the auto loading into burrow with the level specified in overrideName")]
    [SerializeField] private bool _override;
    [SerializeField] private string overrideName;

    [Header("Level Names To Auto Load")]
    [SerializeField] private string burrowName;
    [SerializeField] private string onboardName;
    [SerializeField] private string slideshowName;

    [Header("Necessary Objects")]
    [SerializeField] private Button StartButton;
    [SerializeField] private MainMenu mainMenu;

    private void SetupButton()
    {
        if(_override)
        {
            StartButton.onClick.AddListener(() => mainMenu.StartGame(overrideName));
        }
        else
        {
            XMLFileManager.Instance.Load();
            bool onboardComplete = XMLFileManager.Instance.IsLevelAvailable(onboardName);

            if (onboardComplete)
            {
                StartButton.onClick.AddListener(() => mainMenu.StartGame(burrowName));
            }
            else
            {
                //StartButton.onClick.AddListener(() => mainMenu.StartGame(onboardName));
                StartButton.onClick.AddListener(() => mainMenu.StartGame(slideshowName));
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupButton();
    }
    private void OnDestroy()
    {
        StartButton.onClick.RemoveAllListeners();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

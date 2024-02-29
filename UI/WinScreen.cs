using System;
using __OasisBlitz.Player;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using __OasisBlitz.Utility;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private Button RestartButton;
    [SerializeField] private PlayerInput pInput;
    
    [SerializeField] private Canvas LoadCanvas;
    [SerializeField] private LoadingScreen loadScreen;
    
    public void LoadCurrentLevel()
    {
        GameMetadataTracker.Instance.ResetAllCheckpointForLevel(SceneManager.GetActiveScene().name);
        // GameMetadataTracker.Instance.ResetActivatedFruit();

        //LoadCanvas.gameObject.SetActive(true);
        //loadScreen.LoadScene(SceneManager.GetActiveScene().name);
        LevelManager.Instance.LoadAnySceneAsync(SceneManager.GetActiveScene().name);
        pInput.EnableCharacterControls();
        this.gameObject.SetActive(false);
        
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadLevel(string level)
    {
        // Temporary
        GameMetadataTracker.Instance.ResetAllCheckpointForLevel(SceneManager.GetActiveScene().name);
        // GameMetadataTracker.Instance.ResetActivatedFruit();
        if (level == "MainMenu")
        {
            Destroy(GameObject.FindGameObjectWithTag("Essentials"));
            //LoadCanvas.gameObject.SetActive(true);
            LevelManager.Instance.LoadAnySceneAsync("MainMenu");
        }
        else
        {
            pInput.EnableCharacterControls();
            this.gameObject.SetActive(false);
            //LoadCanvas.gameObject.SetActive(true);
            LevelManager.Instance.LoadAnySceneAsync(level);
        }

        // SceneManager.LoadScene(level);
    }
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();     // Eventually some save funcitonality
#endif
    }

    private void OnEnable()
    {
        RestartButton.Select();
    }

    // Update is called once per frame
    void Update()
    {
        // CLICKING removes the selected button -- need to always have one otherwise can't interact using controller
        if (UIManager.Instance.eventSystem.currentSelectedGameObject == null)
        {
            RestartButton.Select();
        }
    }
}

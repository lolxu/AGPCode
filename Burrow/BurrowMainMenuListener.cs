using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using DG.Tweening;
using UnityEngine;

public class BurrowMainMenuListener : MonoBehaviour
{
    private MainMenu mainMenu;
    private void Awake()
    {
        mainMenu = FindObjectOfType<MainMenu>();

        if (mainMenu == null)
        {
            Destroy(gameObject);
        }
        else
        {
            // start burrow music:
            
            mainMenu.OnStartGamePressed += LoadBanditIntoBurrow;
        }
    }

    private void OnDisable()
    {
        if (mainMenu)
        {
            mainMenu.OnStartGamePressed -= LoadBanditIntoBurrow;
        }
    }

    public void LoadBanditIntoBurrow()
    {
        // fade out main menu
        mainMenu.FadeOutAndDestroyMainMenu(1.5f);
        // lock cursor to screen, we are gamers now
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // enable character controls:
        PlayerInput playerInput = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
        if (playerInput)
        {
            playerInput.EnableCharacterControls();
        }
        
        UIManager.Instance.canPauseGame = true;
    }
}

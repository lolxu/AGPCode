using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetSettingsNavigation : MonoBehaviour
{
    MainMenuSettings mms;
    public void SetSettingsNav()
    {
        if(SettingsUI.Instance) { SettingsUI.Instance.SetSettingsNavigations(this.GetComponent<Button>()); }
        //if(SceneManager.GetActiveScene().name == "MainMenu") { mms.SetSettingsNavigations(this.GetComponent<Button>()); }
        //else if(PauseManager.Instance) { PauseManager.Instance.SetSettingsNavigations(this.GetComponent<Button>()); }
    }

    void Awake()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            mms = GameObject.Find("MainMenuCanvas").GetComponent<MainMenuSettings>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

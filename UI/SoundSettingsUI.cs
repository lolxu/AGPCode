using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    [SerializeField] private Button masterMinus, masterPlus;
    [SerializeField] private Button musicMinus, musicPlus;
    [SerializeField] private Button playerMinus, playerPlus;
    [SerializeField] private Button enemyMinus, enemyPlus;
    [SerializeField] private Button environmentMinus, environmentPlus;
    [SerializeField] private Button uiMinus, uiPlus;

    [SerializeField] private Slider masterS, musicS, playerS, enemyS, environmentS, uiS;    // Sliders for the fill
    // Volume            0      0.1    0.2     0.3     0.4   0.5      0.6     0.7     0.8      0.9     1.0

    public void ChangeMasterVolume(string sign)
    {
        SoundSettings.Instance.ChangeMasterVolume(sign);
    }
    public void ChangeMusicVolume(string sign)
    {
        SoundSettings.Instance.ChangeMusicVolume(sign);
    }
    public void ChangePlayerVolume(string sign)
    {
        SoundSettings.Instance.ChangePlayerVolume(sign);
    }
    public void ChangeEnemyVolume(string sign)
    {
        SoundSettings.Instance.ChangeEnemyVolume(sign);
    }
    public void ChangeEnvironmentVolume(string sign)
    {
        SoundSettings.Instance.ChangeEnvironmentVolume(sign);
    }
    public void ChangeUIVolume(string sign)
    {
        SoundSettings.Instance.ChangeUIVolume(sign);
    }

    // Referenced in slider
    public void OnMasterSliderChanged()
    {
        SoundSettings.Instance.SetMasterVolume((int)masterS.value);
    }
    public void IncMasterVolume()
    {
        if((int)masterS.value > 9) { return; }
        masterS.value += 1;
    }
    public void DecMasterVolume()
    {
        if ((int)masterS.value < 1) { return; }
        masterS.value -= 1;
    }

    // Referenced in slider
    public void OnMusicSliderChanged()
    {
        SoundSettings.Instance.SetMusicVolume((int)musicS.value);
    }
    public void IncMusicVolume()
    {
        if ((int)musicS.value > 9) { return; }
        musicS.value += 1;
    }
    public void DecMusicVolume()
    {
        if ((int)musicS.value < 1) { return; }
        musicS.value -= 1;
    }

    // Referenced in slider
    public void OnPlayerSliderChanged()
    {
        SoundSettings.Instance.SetPlayerVolume((int)playerS.value);
    }
    public void IncPlayerVolume()
    {
        if ((int)playerS.value > 9) { return; }
        playerS.value += 1;
    }
    public void DecPlayerVolume()
    {
        if ((int)playerS.value < 1) { return; }
        playerS.value -= 1;
    }

    // Referenced in slider
    public void OnEnemySliderChanged()
    {
        SoundSettings.Instance.SetEnemyVolume((int)enemyS.value);
    }
    public void IncEnemyVolume()
    {
        if ((int)enemyS.value > 9) { return; }
        enemyS.value += 1;
    }
    public void DecEnemyVolume()
    {
        if ((int)enemyS.value < 1) { return; }
        enemyS.value -= 1;
    }

    // Referenced in slider
    public void OnEnvironmentSliderChanged()
    {
        SoundSettings.Instance.SetEnvironmentVolume((int)environmentS.value);
    }
    public void IncEnvironmentVolume()
    {
        if ((int)environmentS.value > 9) { return; }
        environmentS.value += 1;
    }
    public void DecEnvironmentVolume()
    {
        if ((int)environmentS.value < 1) { return; }
        environmentS.value -= 1;
    }

    // Referenced in slider
    public void OnUISliderChanged()
    {
        SoundSettings.Instance.SetUIVolume((int)uiS.value);
    }
    public void IncUIVolume()
    {
        if ((int)uiS.value > 9) { return; }
        uiS.value += 1;
    }
    public void DecUIVolume()
    {
        if ((int)uiS.value < 1) { return; }
        uiS.value -= 1;
    }


    private void InitSliderValues()
    {
        masterS.value = System.Array.IndexOf(SoundSettings.Instance.GetVolumeValues(), SoundSettings.Instance.masterVolume);
        musicS.value = System.Array.IndexOf(SoundSettings.Instance.GetVolumeValues(), SoundSettings.Instance.musicVolume);
        playerS.value = System.Array.IndexOf(SoundSettings.Instance.GetVolumeValues(), SoundSettings.Instance.playerVolume);
        enemyS.value = System.Array.IndexOf(SoundSettings.Instance.GetVolumeValues(), SoundSettings.Instance.enemyVolume);
        environmentS.value = System.Array.IndexOf(SoundSettings.Instance.GetVolumeValues(), SoundSettings.Instance.environmentVolume);
        uiS.value = System.Array.IndexOf(SoundSettings.Instance.GetVolumeValues(), SoundSettings.Instance.uiVolume);
    }
    private void OnEnable()
    {
        InitSliderValues();
    }
    private void OnDisable()
    {
        GlobalSettings.Instance.SavePlayerPrefs();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && GlobalSettings.Instance.controlScheme != "KEYBOARD")
        {
            masterMinus.Select();
        }
    }
}

using Animancer.Units;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    // PLAYER PREFS:
    // Master Volume
    // Player Volume
    // Enemy Volume
    // Environment Volume
    // UI Volume

    public static SoundSettings Instance;

    public float masterVolume { get; private set; }
    public float musicVolume { get; private set; }
    public float playerVolume { get; private set; }
    public float enemyVolume { get; private set; }
    public float environmentVolume { get; private set; }
    public float uiVolume { get; private set; }

    private int masterI, musicI, playerI, enemyI, environmentI, uiI;
    private float[] values = { 0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f };
    private SoundSettingsUI soundUI;

    private Bus masterBus;
    private Bus musicBus;
    private Bus playerBus;
    private Bus enemyBus;
    private Bus environmentBus;
    private Bus uiBus;

    // Other ones I found

    public float[] GetVolumeValues()
    {
        return values;
    }
    public void SetMasterVolume(int val)
    {
        masterI = val;
        masterVolume = values[masterI];
        masterBus.setVolume(masterVolume);
        PlayerPrefs.SetFloat("Master Volume", masterVolume);
    }
    public void ChangeMasterVolume(string sign)      // Should be increments of +0.1f or -0.1f
    {
        if (soundUI == null) { soundUI = GameObject.Find("SoundSettings").GetComponent<SoundSettingsUI>(); }
        if (sign == "+")
        {
            if (masterI <= 9)
            {
                masterI += 1;
                masterVolume = values[masterI];
                masterBus.setVolume(masterVolume);
                PlayerPrefs.SetFloat("Master Volume", masterVolume);
                // Debug.Log($"Master volume: {masterVolume}");
            }
            return;
        }
        if (sign == "-")
        {
            if (masterI >= 1)
            {
                masterI -= 1;
                masterVolume = values[masterI];
                masterBus.setVolume(masterVolume);
                PlayerPrefs.SetFloat("Master Volume", masterVolume);
                // Debug.Log($"Master volume: {masterVolume}");
            }
            return;
        }
        // Debug.Log("ChangeMasterVolume called but no sign given.");
    }

    public void SetMusicVolume(int val)
    {
        musicI = val;
        musicVolume = values[musicI];
        //musicBus.setVolume(musicVolume);
        AudioManager.instance.SetVolume("Music", musicVolume);
        PlayerPrefs.SetFloat("Music Volume", musicVolume);
    }
    public void ChangeMusicVolume(string sign)      // Should be increments of +0.1f or -0.1f
    {
        if (soundUI == null) { soundUI = GameObject.Find("SoundSettings").GetComponent<SoundSettingsUI>(); }
        if (sign == "+")
        {
            if (musicI <= 9)
            {
                musicI += 1;
                musicVolume = values[musicI];
                //musicBus.setVolume(musicVolume);
                AudioManager.instance.SetVolume("Music", musicVolume);
                PlayerPrefs.SetFloat("Music Volume", musicVolume);
            }
            return;
        }
        if (sign == "-")
        {
            if (musicI >= 1)
            {
                musicI -= 1;
                musicVolume = values[musicI];
                //musicBus.setVolume(musicVolume);
                AudioManager.instance.SetVolume("Music", musicVolume);
                PlayerPrefs.SetFloat("Music Volume", musicVolume);
            }
            return;
        }
    }

    public void SetPlayerVolume(int val)
    {
        playerI = val;
        playerVolume = values[playerI];
        //playerBus.setVolume(playerVolume);
        AudioManager.instance.SetVolume("SFX", playerVolume);
        PlayerPrefs.SetFloat("Player Volume", playerVolume);
    }
    public void ChangePlayerVolume(string sign)      // Should be increments of +0.1f or -0.1f
    {
        if (soundUI == null) { soundUI = GameObject.Find("SoundSettings").GetComponent<SoundSettingsUI>(); }
        if (sign == "+")
        {
            if (playerI <= 9)
            {
                playerI += 1;
                playerVolume = values[playerI];
                //playerBus.setVolume(playerVolume);
                AudioManager.instance.SetVolume("SFX", playerVolume);
                PlayerPrefs.SetFloat("Player Volume", playerVolume);
                // Debug.Log($"Player volume: {playerVolume}");
            }
            return;
        }
        if (sign == "-")
        {
            if (playerI >= 1)
            {
                playerI -= 1;
                playerVolume = values[playerI];
                //playerBus.setVolume(playerVolume);
                AudioManager.instance.SetVolume("SFX", playerVolume);
                PlayerPrefs.SetFloat("Player Volume", playerVolume);
                // Debug.Log($"Player volume: {playerVolume}");
            }
            return;
        }
        // Debug.Log("ChangePlayerVolume called but no sign given.");
    }

    public void SetEnemyVolume(int val)
    {
        enemyI = val;
        enemyVolume = values[enemyI];
        //enemyBus.setVolume(enemyVolume);
        PlayerPrefs.SetFloat("Enemy Volume", enemyVolume);
    }
    public void ChangeEnemyVolume(string sign)      // Should be increments of +0.1f or -0.1f
    {
        if (soundUI == null) { soundUI = GameObject.Find("SoundSettings").GetComponent<SoundSettingsUI>(); }
        if (sign == "+")
        {
            if (enemyI <= 9)
            {
                enemyI += 1;
                enemyVolume = values[enemyI];
                //enemyBus.setVolume(enemyVolume);
                PlayerPrefs.SetFloat("Enemy Volume", enemyVolume);
                // Debug.Log($"Enemy volume: {enemyVolume}");
            }
            return;
        }
        if (sign == "-")
        {
            if (enemyI >= 1)
            {
                enemyI -= 1;
                enemyVolume = values[enemyI];
                //enemyBus.setVolume(enemyVolume);
                PlayerPrefs.SetFloat("Enemy Volume", enemyVolume);
                // Debug.Log($"Enemy volume: {enemyVolume}");
            }
            return;
        }
        // Debug.Log("ChangeEnemyVolume called but no sign given.");
    }

    public void SetEnvironmentVolume(int val)
    {
        environmentI = val;
        environmentVolume = values[environmentI];
        //environmentBus.setVolume(environmentVolume);
        AudioManager.instance.SetVolume("Ambience", environmentVolume);
        PlayerPrefs.SetFloat("Environment Volume", environmentVolume);
    }
    public void ChangeEnvironmentVolume(string sign)      // Should be increments of +0.1f or -0.1f
    {
        if (soundUI == null) { soundUI = GameObject.Find("SoundSettings").GetComponent<SoundSettingsUI>(); }
        if (sign == "+")
        {
            if (environmentI <= 9)
            {
                environmentI += 1;
                environmentVolume = values[environmentI];
                //environmentBus.setVolume(environmentVolume);
                AudioManager.instance.SetVolume("Ambience", environmentVolume);
                PlayerPrefs.SetFloat("Environment Volume", environmentVolume);
                // Debug.Log($"Environment volume: {environmentVolume}");
            }
            return;
        }
        if (sign == "-")
        {
            if (environmentI >= 1)
            {
                environmentI -= 1;
                environmentVolume = values[environmentI];
                //environmentBus.setVolume(environmentVolume);
                AudioManager.instance.SetVolume("Ambience", environmentVolume);
                PlayerPrefs.SetFloat("Environment Volume", environmentVolume);
                // Debug.Log($"Environment volume: {environmentVolume}");
            }
            return;
        }
        // Debug.Log("ChangeEnvironmentVolume called but no sign given.");
    }

    public void SetUIVolume(int val)
    {
        uiI = val;
        uiVolume = values[uiI];
        //uiBus.setVolume(uiVolume);
        AudioManager.instance.SetVolume("UI", uiVolume);
        PlayerPrefs.SetFloat("UI Volume", uiVolume);
    }
    public void ChangeUIVolume(string sign)      // Should be increments of +0.1f or -0.1f
    {
        if (soundUI == null) { soundUI = GameObject.Find("SoundSettings").GetComponent<SoundSettingsUI>(); }
        if (sign == "+")
        {
            if (uiI <= 9)
            {
                uiI += 1;
                uiVolume = values[uiI];
                //uiBus.setVolume(uiVolume);
                AudioManager.instance.SetVolume("UI", uiVolume);
                PlayerPrefs.SetFloat("UI Volume", uiVolume);
                // Debug.Log($"UI volume: {uiVolume}");
            }
            return;
        }
        if (sign == "-")
        {
            if (uiI >= 1)
            {
                uiI -= 1;
                uiVolume = values[uiI];
                //uiBus.setVolume(uiVolume);
                AudioManager.instance.SetVolume("UI", uiVolume);
                PlayerPrefs.SetFloat("UI Volume", uiVolume);
                // Debug.Log($"UI volume: {uiVolume}");
            }
            return;
        }
        // Debug.Log("ChangeUIVolume called but no sign given.");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("Master Volume"))
        {
            masterVolume = 0.5f;
            PlayerPrefs.SetFloat("Master Volume", masterVolume);
        }
        else
        {
            masterVolume = PlayerPrefs.GetFloat("Master Volume");
        }
        if (!PlayerPrefs.HasKey("Music Volume"))
        {
            musicVolume = 0.5f;
            PlayerPrefs.SetFloat("Music Volume", musicVolume);
        }
        else
        {
            musicVolume = PlayerPrefs.GetFloat("Music Volume");
        }
        if (!PlayerPrefs.HasKey("Player Volume"))
        {
            playerVolume = 0.5f;
            PlayerPrefs.SetFloat("Player Volume", playerVolume);
        }
        else
        {
            playerVolume = PlayerPrefs.GetFloat("Player Volume");
        }
        if (!PlayerPrefs.HasKey("Enemy Volume"))
        {
            enemyVolume = 0.5f;
            PlayerPrefs.SetFloat("Enemy Volume", enemyVolume);
        }
        else
        {
            enemyVolume = PlayerPrefs.GetFloat("Enemy Volume");
        }
        if (!PlayerPrefs.HasKey("Environment Volume"))
        {
            environmentVolume = 0.5f;
            PlayerPrefs.SetFloat("Environment Volume", environmentVolume);
        }
        else
        {
            environmentVolume = PlayerPrefs.GetFloat("Environment Volume");
        }
        if (!PlayerPrefs.HasKey("UI Volume"))
        {
            uiVolume = 0.5f;
            PlayerPrefs.SetFloat("UI Volume", uiVolume);
        }
        else
        {
            uiVolume = PlayerPrefs.GetFloat("UI Volume");
        }
        // soundUI = GameObject.Find("SoundSettings").GetComponent<SoundSettingsUI>();

/*        Debug.Log(masterVolume);
        Debug.Log(playerVolume);
        Debug.Log(enemyVolume);
        Debug.Log(environmentVolume);
        Debug.Log(uiVolume);*/

        masterBus.setVolume(masterVolume);
        //musicBus.setVolume(musicVolume);
        AudioManager.instance.SetVolume("Music", musicVolume);
        //playerBus.setVolume(playerVolume);
        AudioManager.instance.SetVolume("SFX", playerVolume);
        //enemyBus.setVolume(enemyVolume);
        //environmentBus.setVolume(environmentVolume);
        AudioManager.instance.SetVolume("Ambience", environmentVolume);
        //uiBus.setVolume(uiVolume);
        AudioManager.instance.SetVolume("UI", uiVolume);

        masterI = (int)(10 * masterVolume);
        musicI = (int)(10 * musicVolume);
        playerI = (int)(10 * playerVolume);
        enemyI = (int)(10 * enemyVolume);
        environmentI = (int)(10 * environmentVolume);
        uiI = (int)(10 * uiVolume);
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        // The names of the buses changed so I pulled this hacky code from the internet to get the names of the buses.
        // For now the volume settings will be broken.
        // Bus[] busList = new Bus[20];
        //  FMODUnity.RuntimeManager.StudioSystem.getBankList(out FMOD.Studio.Bank[] loadedBanks);
        //         foreach (FMOD.Studio.Bank bank in loadedBanks)
        //         {
        //             bank.getPath(out string path);
        //             bank.getBusList(out busList);
        //
        //             int busCount;
        //             bank.getBusCount(out busCount);
        //             if (busCount > 0)
        //             {
        //                 foreach (var bus in busList)
        //                 {
        //                     string busName;
        //                     bus.getPath(out busName);
        //                     Debug.Log(busName);
        //                 }
        //             }
        //         }
        
        // TODO: Replace these once the bus names are sorted out
        masterBus = RuntimeManager.GetBus("bus:/");

        // Not needed anymore if using AudioManager to adjust VCA volume instead of bus volume, keep in case we need to do this instead?
        //musicBus = RuntimeManager.GetBus("bus:/MIX/NoRvb/Music");
        //playerBus = RuntimeManager.GetBus("bus:/MIX/SendToRvb/Player");
        //enemyBus = RuntimeManager.GetBus("bus:/MIX/SendToRvb/Enemy");
        //environmentBus = RuntimeManager.GetBus("bus:/MIX/SendToRvb/Environment");
        //uiBus = RuntimeManager.GetBus("bus:/MIX/NoRvb/UI");

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using __OasisBlitz.__Scripts.Collectables;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XMLFileManager : MonoBehaviour
{
    public static XMLFileManager Instance;
    public static string SavePath;

    [SerializeField] private string saveFileName = "BlitzSecretSave";
    
    public class SaveData
    {
        
        /// <summary>
        /// Deprecated
        /// </summary>
        public class BurrowFruits
        {
            public class Fruit
            {
                public string m_type;
                public bool m_collected;
            }

            public List<Fruit> m_fruits;
        }
        public BurrowFruits m_burrowFruits;

        /// <summary>
        /// Deprecated
        /// </summary>
        public class Vitalizers
        {
            public int m_count;
        }
        public Vitalizers m_vitalizers;
        
        //Support for level unlocks
        public class LevelUnlocks
        {
            public List<string> unlockedScenes;
            public List<float> levelPBTime;
        }
        public LevelUnlocks m_levelUnlocks = null;

        public class PlantUnlocks
        {
            public List<int> plantIndex;
            public List<string> plantNames;
            public List<bool> isPlantPlaced;
        }
        public PlantUnlocks m_plantUnlocks = null;

        public class DecorUnlocks
        {
            public List<int> decorIndex;
            public List<string> decorNames;
            public List<bool> isDecorPlaced;
        }
        public DecorUnlocks m_decorUnlocks = null;

        public bool m_blastUnlock = false;

        public bool m_playCutscene = true;

        public bool m_playEndingCutscene = false;

        public string username;
    }

    private SaveData mySaveData;
    private const string defaultUsername = "bandit";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            mySaveData = new SaveData();
            Load();
            SavePath = GetSavePath();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        mySaveData = new SaveData();
        Save();

        // Also clears collectable status
        if (CollectableManager.Instance != null)
        {
            CollectableManager.Instance.ClearAllCollectableStatus();
        }
    }

    /// <summary>
    /// Tries to save all the data for this current scene
    /// </summary>
    /// <param name="sceneName"> The current scene name </param>
    /// <param name="collectIndex"> Default is -1, doesn't save </param>
    /// <param name="time"> Default is -1.0f, doesn't save </param>
    public void SaveLevelStatus(string sceneName, float time = -1.0f)
    {
        // Debug.LogError("Here");
        mySaveData.m_levelUnlocks ??= new SaveData.LevelUnlocks
        {
            unlockedScenes = new List<string>(),
            levelPBTime = new List<float>()
        };

        bool isOldLevel = mySaveData.m_levelUnlocks.unlockedScenes.Contains(sceneName);

        if (!isOldLevel)
        {
            mySaveData.m_levelUnlocks.unlockedScenes.Add(sceneName);
            mySaveData.m_levelUnlocks.levelPBTime.Add(time);
        }
        else
        {
            // Debug.LogError("Not new level");
            var ind = mySaveData.m_levelUnlocks.unlockedScenes.IndexOf(sceneName);
            if (time > 0.0f)
            {
                // Debug.LogError("Here");
                mySaveData.m_levelUnlocks.levelPBTime[ind] = time;
            }
        }
        
        Save();
    }
    public bool IsLevelAvailable(string levelName)
    {
        if (mySaveData == null)
        {
            return false;
        }

        if (mySaveData.m_levelUnlocks == null)
        {
            return false;
        }
        return mySaveData.m_levelUnlocks.unlockedScenes.Contains(levelName);
    }

    public void SavePlantStatus(int index, string plantName, bool plantIsPlaced)
    {
        mySaveData.m_plantUnlocks ??= new SaveData.PlantUnlocks
        {
            plantIndex = new List<int>(),
            plantNames = new List<string>(),
            isPlantPlaced = new List<bool>()
        };
        
        bool isOldPlant = mySaveData.m_plantUnlocks.plantNames.Contains(plantName);

        if (!isOldPlant)
        {
            mySaveData.m_plantUnlocks.plantIndex.Add(index);
            mySaveData.m_plantUnlocks.plantNames.Add(plantName);
            mySaveData.m_plantUnlocks.isPlantPlaced.Add(plantIsPlaced);
        }
        else
        {
            var ind = mySaveData.m_plantUnlocks.plantIndex.IndexOf(index);
            if (ind != -1)
            {
                // Dealing plant placement
                bool isCurrentPlaced = mySaveData.m_plantUnlocks.isPlantPlaced[ind];
                if (!isCurrentPlaced && plantIsPlaced)
                {
                    mySaveData.m_plantUnlocks.isPlantPlaced[ind] = true;
                }
            }
            else
            {
                Debug.LogError("This Plant Doesn't exist in save...");
            }
        }
        Save();
    }

    public void SaveCutsceneViewed()
    {
        mySaveData.m_playCutscene = false;
        Save();
    }
    
    public bool ShouldPlayCutscene()
    {
        return false; //mySaveData.m_playCutscene;
    }
    
    public void SaveEndingCutsceneViewed()
    {
        mySaveData.m_playEndingCutscene = false;
        Save();
    }
    
    public void SaveEndingCutsceneNeedToView()
    {
        mySaveData.m_playEndingCutscene = true;
        Save();
    }
    public bool ShouldPlayEndingCutscene()
    {
        return false;//(mySaveData.m_playEndingCutscene);
    }

    public void SaveDecorStatus(int index, string decorName, bool decorIsPlaced)
    {
        mySaveData.m_decorUnlocks ??= new SaveData.DecorUnlocks()
        {
            decorIndex = new List<int>(),
            decorNames = new List<string>(),
            isDecorPlaced = new List<bool>()
        };
        
        bool isOldDecor = mySaveData.m_decorUnlocks.decorNames.Contains(decorName);

        if (!isOldDecor)
        {
            mySaveData.m_decorUnlocks.decorIndex.Add(index);
            mySaveData.m_decorUnlocks.decorNames.Add(decorName);
            mySaveData.m_decorUnlocks.isDecorPlaced.Add(decorIsPlaced);
        }
        else
        {
            var ind = mySaveData.m_decorUnlocks.decorIndex.IndexOf(index);
            if (ind != -1)
            {
                // Dealing plant placement
                bool isCurrentPlaced = mySaveData.m_decorUnlocks.isDecorPlaced[ind];
                if (!isCurrentPlaced && decorIsPlaced)
                {
                    mySaveData.m_decorUnlocks.isDecorPlaced[ind] = true;
                }
            }
            else
            {
                Debug.LogError("This Decor Doesn't exist in save...");
            }
        }
        Save();
    }

    public string GetUsername()
    {
        string username = mySaveData.username;

        try
        {
            if (username.Length == 0)
            {
                username = defaultUsername;
            }
        }
        catch (Exception e)
        {
            username = defaultUsername;
        }

        return username;
    }

    public void SetUsername(string username)
    {
        if (username.Length == 0)
        {
            username = defaultUsername;
        }

        mySaveData.username = username;
        Save();
    }

    public bool LookupPlantPlacement(int index)
    {
        if (mySaveData != null)
        {
            var ind = mySaveData.m_plantUnlocks.plantIndex.IndexOf(index);
            if (ind != -1)
            {
                return mySaveData.m_plantUnlocks.isPlantPlaced[ind];
            }
            return false;
        }
        return false;
    }

    public int GetNumPlantsCollected()
    {
        if (mySaveData != null)
        {
            if (mySaveData.m_plantUnlocks != null)
            {
                return mySaveData.m_plantUnlocks.plantNames.Count;
            }
        }

        return 0;
    }

    public int GetNumPlantsPlaced()
    {
        int count = 0;
        if (mySaveData != null)
        {
            if (mySaveData.m_plantUnlocks != null)
            {
                foreach (bool plantPlaced in mySaveData.m_plantUnlocks.isPlantPlaced)
                {
                    count += plantPlaced ? 1 : 0;
                }
            }
        }

        return count;
    }
    public float LookupPBTime(string sceneName)
    {
        if (mySaveData != null)
        {
            int ind = mySaveData.m_levelUnlocks.unlockedScenes.IndexOf(sceneName);
            if (ind != -1)
            {
                return mySaveData.m_levelUnlocks.levelPBTime[ind];
            }
            return -1.0f;
        }
        return -1.0f;
    }

    private void Save()
    {
        // Supported for level unlocks
        StringWriter myStringWriter = new StringWriter();
        StreamWriter myXMLStream = new StreamWriter(GetSavePath());
        XmlSerializer myXMLSerializer = new XmlSerializer(typeof(SaveData));
        
        myXMLSerializer.Serialize(myStringWriter, mySaveData);
        myXMLStream.Write(myStringWriter.ToString());
        myStringWriter.Close();
        myXMLStream.Close();
    }

    /// <summary>
    /// Use this function for a load from save file when game starts
    /// </summary>
    public void Load()
    {
        // Debug.LogError("Here");
        if (File.Exists(GetSavePath()))
        {
            StreamReader myStreamReader = new StreamReader(GetSavePath());
            string myFile = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            
            StringReader myStringReader = new StringReader(myFile);
            XmlSerializer myXMLSerializer = new XmlSerializer(typeof(SaveData));
            SaveData mySave = myXMLSerializer.Deserialize(myStringReader) as SaveData;

            // Load collectables
            if (mySave != null)
            {
                mySaveData = mySave;
            }
        }
        else
        {
            Debug.LogError("No Save File Found");
        }
    }

    /// <summary>
    /// Use this function for loading anything burrow related
    /// </summary>
    public void LoadAllData()
    {
        if (mySaveData.m_levelUnlocks != null)
        {
            if (mySaveData.m_plantUnlocks != null)
            {
                // Loading plant
                for (int i = 0; i < mySaveData.m_plantUnlocks.plantIndex.Count; i++)
                {
                    var index = mySaveData.m_plantUnlocks.plantIndex[i];
                    var isPlaced = mySaveData.m_plantUnlocks.isPlantPlaced[i];
                    CollectableManager.Instance.LoadPlantCollectableStatus(index, true, isPlaced);
                }
            }

            if (mySaveData.m_decorUnlocks != null)
            {
                // Loading decors
                for (int i = 0; i < mySaveData.m_decorUnlocks.decorIndex.Count; i++)
                {
                    var index = mySaveData.m_decorUnlocks.decorIndex[i];
                    var isPlaced = mySaveData.m_decorUnlocks.isDecorPlaced[i];
                    CollectableManager.Instance.LoadDecorCollectableStatus(index, true, isPlaced);
                }
            }

            LoadTimeData();
        }
    }

    public void LoadTimeData()
    {
        // Getting level best time
        int ind = mySaveData.m_levelUnlocks.unlockedScenes.IndexOf(SceneManager.GetActiveScene().name);
        // Debug.Log(ind);
        if (ind != -1)
        {
            UIManager.Instance.gameObject.GetComponent<Timer>().personalBest =
                mySaveData.m_levelUnlocks.levelPBTime[ind];
        }
    }

    public string GetSavePath()
    {
        string path = Application.persistentDataPath + "/" + saveFileName + ".xml";
        return path;
    }

    public bool SaveExists()
    {
        return File.Exists(GetSavePath());
    }
    public void DeleteSaves()
    {
        File.Delete(GetSavePath());
    }
}

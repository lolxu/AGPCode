using System;
using System.Collections.Generic;
using System.IO;
using __OasisBlitz.Player;
using __OasisBlitz.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace __OasisBlitz.LevelLoading
{
    public class LevelLoaderUIManager : MonoBehaviour
    {
        [SerializeField] private Canvas LevelLoaderHUD;
        public static event Action CreateEmptyAction;
        public static event Action<string> LoadAction;
        public static event Action<string> SaveAction;

        private PlayerInput input;
        public enum MainButtons
        {
            Resume,
            CreateEmpty,
            Load,
            Save
        }
        private MainButtons _currMainButtonSelected;
        //Important - this list MUST match the indicies in the MainButtons Enum
        [SerializeField] private List<Button> MainButtonList;
    
        private void Start()
        {
            input.Pause.Invoke();
        }

        private void OnEnable()
        {
            input.Resume += DisableLevelLoaderHUD;
            input.Pause += EnableLevelLoaderHUD;
            input.Resume += ResumeTime;
            input.Pause += StopTime;
            input.Resume += RemoveMouse;
            input.Pause += DisplayMouse;
            input.Pause += InitialHighlight;
        }
        private void OnDisable()
        {
            input.Resume -= DisableLevelLoaderHUD;
            input.Pause -= EnableLevelLoaderHUD;
            input.Resume -= ResumeTime;
            input.Pause -= StopTime;
            input.Resume -= RemoveMouse;
            input.Pause -= DisplayMouse;
            input.Pause -= InitialHighlight;
        }
        private void EnableLevelLoaderHUD()
        {
            LevelLoaderHUD.enabled = true;
        }
        private void DisableLevelLoaderHUD()
        {
            LevelLoaderHUD.enabled = false;
        }

        private void StopTime()
        {
            //set timescale to 0
            Time.timeScale = 0.0f;
        }

        private void ResumeTime()
        {
            //set timescale to 1
            Time.timeScale = 1.0f;
        }

        public void DisplayMouse()
        {
            //check we are on PC
            //enable mouse
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void RemoveMouse()
        {
            //check we are on PC
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        public void InitialHighlight()//(MainButtons buttonToHighlight)
        {
            //hover over resume
            MainButtonList[(int)MainButtons.Resume].Select();
            _currMainButtonSelected = MainButtons.Resume;
        }
        public void Highlight(int buttonToHighlight)
        {
            //change the highlighted item in the menu
            if (buttonToHighlight != (int)_currMainButtonSelected)
            {
                MainButtonList[(int)buttonToHighlight].Select();
                _currMainButtonSelected = (MainButtons)buttonToHighlight;
            }
        }

        public void Resume()
        {
            input.Resume.Invoke();
        }
        public void CreateEmpty()
        {
            CreateEmptyToMainMenu();
            if (CreateEmptyAction != null)
            {
                CreateEmptyAction.Invoke();
            }
        }
        public void Load()
        {
            ConfirmLoadToLoadLevel();
            LoadLevelToMainMenu();
            if (LoadAction != null)
            {
                LoadAction.Invoke(_currFileSelected);
            }
            if (input.Resume != null)
            {
                input.Resume.Invoke();
            }
        }

        public void Save()
        {
            ConfirmSaveToSaveLevel();
            SaveLevelToMainMenu();
            if (SaveAction != null)
            {
                SaveAction.Invoke(_currFileSelected);
            }
        }

        [SerializeField] private TMP_Dropdown levelSelectDropdown;
        [SerializeField] private TMP_Text LoadButtonText;
        private string _currFileSelected;

        public string CurrFileSelected
        {
            get { return _currFileSelected;  }
#if UNITY_EDITOR
            set { _currFileSelected = value;  }
#endif
        }
        public void ListLevels()
        {
            //clear file selected
            _currFileSelected = "";
            //clear dropdown menu
            levelSelectDropdown.options.Clear();
            //display the list of all .xml files in the directory
            DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] info = dir.GetFiles("*.xml");
            foreach (FileInfo f in info)
            {
#if DEBUG
                if (Constants.DebugLevelLoader)
                {
                    Debug.Log(f.Name);
                }
#endif
                string file = f.Name.Replace(".xml","");
                file = file.Replace("Level", "");
                //add name to dropdown
                levelSelectDropdown.options.Add(new TMP_Dropdown.OptionData(file));
            }
            //Auto select the first item
            LevelDropdownItemSelected();
            //Set Listener for value change
            levelSelectDropdown.onValueChanged.AddListener(delegate{ LevelDropdownItemSelected(); });
        }
    
        private void DeleteFile(string fileName)
        {
            //TODO:delete level
        }
#if UNITY_EDITOR
        private bool firstCall = true;
#endif
        private void LevelDropdownItemSelected()
        {
            int index = levelSelectDropdown.value;
#if UNITY_EDITOR
            LevelLoaderForScene temp = GameObject.FindGameObjectWithTag("LevelLoaderScene")
                .GetComponent<LevelLoaderForScene>();
            if (firstCall)
            {
                if (temp._DropDownIndex < levelSelectDropdown.options.Count)
                {
                    if (levelSelectDropdown.options[temp._DropDownIndex].text == temp._levelToLoadInEditor)
                    {
                        index = temp._DropDownIndex;
                        levelSelectDropdown.value = index;
                    }
                }
                firstCall = false;
            }

            temp._DropDownIndex = index;
            temp._levelToLoadInEditor = levelSelectDropdown.options[index].text;
            _currFileSelected = levelSelectDropdown.options[index].text;
            UpdateLoadOutOfPlayModeText();
#endif
        
            //set current file selected
            _currFileSelected = levelSelectDropdown.options[index].text;
            //set current selected dropdown item text
            levelSelectDropdown.captionText.SetText(_currFileSelected);
            LoadButtonText.SetText("Load "+ _currFileSelected);
        }
#if UNITY_EDITOR
        [SerializeField] private TMP_Text LoadOutOfPlayMode;
        public void UpdateLoadOutOfPlayModeText()
        {
            LoadOutOfPlayMode.SetText("Load " + _currFileSelected + " out of Play Mode");
        }
#endif
        //Save level
        [SerializeField] private TMP_InputField saveInputFileName;

        private void InitializeSaveInputField()
        {
            saveInputFileName.characterLimit = 200;
            saveInputFileName.text = _currFileSelected;
            saveInputFileName.onValueChanged.AddListener(delegate { SaveFileNameValueChanged(); });
        }

        private void SaveFileNameValueChanged()
        {
            _currFileSelected = saveInputFileName.text;
        }
        public static bool FileExists(string fileName)
        {
            //Checks if file name exists
            DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] info = dir.GetFiles("*.xml*");
            foreach (FileInfo f in info)
            {
#if DEBUG
                if (Constants.DebugLevelLoader)
                {
                    Debug.Log(f.Name);
                }
#endif
                string file = f.Name.Replace(".xml","");
                file = file.Replace("Level", "");
                if (file.Length == fileName.Length)
                {
                    if (file == fileName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    
    
        //Menu Navigation
        [SerializeField] private GameObject MainMenu;
        [SerializeField] private GameObject SelectToLoad;
        [SerializeField] private GameObject ConfirmLoad;
        [SerializeField] private TMP_Text ConfirmLoadTextPrompt;
        [SerializeField] private GameObject SelectSaveTitle;
        [SerializeField] private GameObject ConfirmSave;
        [SerializeField] private TMP_Text ConfirmSaveTextPrompt;
        [SerializeField] private GameObject ConfirmCreateEmpty;
        public void LoadLevelToConfirm()
        {
            SelectToLoad.SetActive(false);
            ConfirmLoad.SetActive(true);
            ConfirmLoadTextPrompt.SetText("Are you sure you want to load " + _currFileSelected + " (any unsaved progress will be lost)?");
        }

        public void ConfirmLoadToLoadLevel()
        {
            ConfirmLoad.SetActive(false);
            SelectToLoad.SetActive(true);
            ListLevels();
        }

        public void LoadLevelToMainMenu()
        {
            SelectToLoad.SetActive(false);
            MainMenu.SetActive(true);
        }
    
        public void MainToLoadLevel()
        {
            MainMenu.SetActive(false);
            SelectToLoad.SetActive(true);
            ListLevels();
        }
    
        public void SaveLevelToConfirm()
        {
            //check if there is a level file with this name
            if (FileExists(_currFileSelected))
            {
                //prompt a confirmation
                SelectSaveTitle.SetActive(false);
                ConfirmSave.SetActive(true);
                ConfirmSaveTextPrompt.SetText("A level already exists with the name " + _currFileSelected + ".  Are you sure you want to override it? (Note: this can't be undone)");
            }
            else
            {
                Save();
            }
        }
    
        public void ConfirmSaveToSaveLevel()
        {
            ConfirmSave.SetActive(false);
            SelectSaveTitle.SetActive(true);
            InitializeSaveInputField();
        }
    
        public void SaveLevelToMainMenu()
        {
            SelectSaveTitle.SetActive(false);
            MainMenu.SetActive(true);
        }
    
        public void MainToSaveLevel()
        {
            MainMenu.SetActive(false);
            SelectSaveTitle.SetActive(true);
            InitializeSaveInputField();
        }
        public void CreateEmptyToMainMenu()
        {
            ConfirmCreateEmpty.SetActive(false);
            MainMenu.SetActive(true);
        }
    
        public void MainToCreateEmpty()
        {
            MainMenu.SetActive(false);
            ConfirmCreateEmpty.SetActive(true);
        }
    }
}

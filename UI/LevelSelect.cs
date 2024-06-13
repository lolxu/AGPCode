using __OasisBlitz.Player;      // PlayerInput
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __OasisBlitz.__Scripts.Collectables;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    private string[] levels;

    [SerializeField] private Button buttonPrefab;

    [SerializeField] private GameObject InterfaceNotAvailable;  // When using Level Select from pause in the burrow

    [SerializeField] private Button closeButton;
    
    private PauseManager pauseManager;

    [SerializeField] private TextMeshProUGUI NoLevelText;

    private bool AlreadyAddedButtons = false;

    [SerializeField] private PlayerInput pInput;
    private BurrowLevelSelect burrowLevelSelect;
    [SerializeField] bool isBurrowInterface;

    [field: SerializeField]
    public List<Button> buttons { get; private set; } = new List<Button>();
    private List<Button> ActiveButtons = new List<Button>();

    private void InstantiateButtons()       // INCLUDING the navigation for the Close button
    {
        // Load first to get the Best Time for each level
        XMLFileManager.Instance.Load();
        if (SceneManager.GetActiveScene().name != "Level 0 - Onboard" && !AlreadyAddedButtons) // temporary
        {
            levels = Levels.Instance.GetLevels();
            foreach (string level in levels)
            {
                //if(level.Contains("Burrow") && SceneManager.GetActiveScene().name.Contains("Burrow")) { continue; }     // No need to have burrow option in the selection screen in the burrow itself
                Button btn = Instantiate(buttonPrefab, this.transform); // As child of object with vertical layout group
                btn.name = level;
                btn.onClick.AddListener(() => { 
                    if(SceneManager.GetActiveScene().name.Contains("Burrow")) {
                        burrowLevelSelect.SetCannonLevel(level);
                    }
                    else {
                        pauseManager.LoadLevel(level);
                    }
                    
                });

                btn.GetComponentInChildren<TextMeshProUGUI>().text =
                    level.Contains("Burrow") ? "Burrow" : "Level " + (buttons.Count);
                buttons.Add(btn);
            }
            AlreadyAddedButtons = true;
        }
        else if(!AlreadyAddedButtons)
        {
            NoLevelText.enabled = true;
            closeButton.Select();
        }

        int index = 0;
        if(isBurrowInterface)
        {
            // Update all button LEFT and RIGHT navigation
            foreach (Button btn in buttons)
            {
                Navigation nav = new Navigation();
                nav.mode = Navigation.Mode.Explicit;

                if (index == 0)
                {
                    nav.selectOnLeft = null;
                    nav.selectOnRight = buttons.Count == 1 ? null : buttons[1];       // If only one level in list

                }
                else if (index == buttons.Count - 1)
                {
                    nav.selectOnLeft = buttons[index - 1];
                    nav.selectOnRight = null;
                    nav.selectOnDown = closeButton;

                }
                else
                {
                    nav.selectOnUp = buttons[index - 1];
                    nav.selectOnDown = buttons[index + 1];

                    nav.selectOnDown = closeButton;

                }

                btn.navigation = nav;
                index++;
            }
        }
        else
        {
            // Update all button UP and DOWN navigation
            foreach (Button btn in buttons)
            {
                Navigation nav = new Navigation();
                nav.mode = Navigation.Mode.Explicit;

                if (index == 0) // First button -- UP is the Close button
                {
                    nav.selectOnUp = closeButton;

                    nav.selectOnDown = buttons.Count == 1 ? closeButton : buttons[1];       // If only one level in list then go to close
                }
                else if (index == buttons.Count - 1) // Last button -- DOWN is the Close button
                {
                    nav.selectOnUp = buttons[index - 1];
                    nav.selectOnDown = closeButton;
                }
                else
                {
                    nav.selectOnUp = buttons[index - 1];
                    nav.selectOnDown = buttons[index + 1];
                }

                btn.navigation = nav;
                index++;
            }
        }


        // Close button navigation
        Navigation cNav = new Navigation();
        cNav.mode = Navigation.Mode.Explicit;
        cNav.selectOnUp = buttons[buttons.Count - 1];
        cNav.selectOnDown = buttons[0];
        closeButton.navigation = cNav;
    }

    private void SetActiveLevels()
    {
        bool haveBurrowButton = false;
        // Disable every button instantiated
        foreach (Button btn in buttons)
        {
            if (btn.name == "Burrow")
            {
                haveBurrowButton = true;
                continue;
            }
            btn.gameObject.SetActive(false);
        }

        // If pausing in the Burrow, show no levels
        if (SceneManager.GetActiveScene().name.Contains("Burrow") && !isBurrowInterface)        // Levels = pause menu, BurrowLevels = burrow level interface
        {
            InterfaceNotAvailable.SetActive(true);
            closeButton.Select();
            return;
        }
        else
        {
            InterfaceNotAvailable.SetActive(false);
        }

        ActiveButtons.Clear();

        // bool level1Active = CollectableManager.Instance.LookupPlantPlacement(0);
        bool level1Active = CollectableManager.Instance.LookupPlantPlacement(0);
        bool level2Active = CollectableManager.Instance.LookupPlantPlacement(1);
        bool level3Active = CollectableManager.Instance.LookupPlantPlacement(2);
        // Debug.Log($"Level 2: {level2Active}\tLevel 3: {level3Active}");

        if (isBurrowInterface)
        {
            Debug.Log("============================In burrow");
            buttons[0].gameObject.SetActive(false);
        }
        else
        {
            buttons[0].gameObject.SetActive(true);
            ActiveButtons.Add(buttons[0]);
        }
        if (level1Active)
        {
            buttons[1].gameObject.SetActive(true);
            ActiveButtons.Add(buttons[1]);
        }
        if (level2Active)
        {
            buttons[2].gameObject.SetActive(true);
            ActiveButtons.Add(buttons[2]);
        }
        if (level3Active)
        {
            buttons[3].gameObject.SetActive(true);
            ActiveButtons.Add(buttons[3]);
        }
        int index = 0;
        foreach (Button btn in ActiveButtons)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;

            if (index == 0) // First button -- UP is the Close button
            {
                nav.selectOnUp = closeButton;

                nav.selectOnDown = ActiveButtons.Count == 1 ? closeButton : ActiveButtons[1];       // If only one level in list then go to close
            }
            else if (index == ActiveButtons.Count - 1) // Last button -- DOWN is the Close button
            {
                nav.selectOnUp = ActiveButtons[index - 1];
                nav.selectOnDown = closeButton;
            }
            else
            {
                nav.selectOnUp = ActiveButtons[index - 1];
                nav.selectOnDown = ActiveButtons[index + 1];
            }

            btn.navigation = nav;
            index++;
        }
        // Close button navigation
        Navigation cNav = new Navigation();
        cNav.mode = Navigation.Mode.Explicit;
        cNav.selectOnUp = ActiveButtons[ActiveButtons.Count - 1];
        cNav.selectOnDown = ActiveButtons[0];
        closeButton.navigation = cNav;

        ActiveButtons[0].Select();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(!(this.gameObject.name == "BurrowLevels")) { pauseManager = GameObject.Find("PauseCanvas").GetComponent<PauseManager>(); }
        // closeButton = GameObject.Find("CloseLevels").GetComponent<Button>();

        // OnEnable got this covered
/*        if (buttons.Count == 0) { InstantiateButtons(); }

        SetActiveLevels();
        if (ActiveButtons.Count > 0)
        {
            ActiveButtons[0].Select();
        }
        else
        {
            closeButton.Select();
        }*/
    }

    private void OnEnable()
    {
        if(SceneManager.GetActiveScene().name.Contains("Burrow") && !pInput) { pInput = GameObject.Find("PlayerBase").GetComponent<PlayerInput>(); }
        if(SceneManager.GetActiveScene().name.Contains("Burrow") && !burrowLevelSelect) { burrowLevelSelect = GameObject.Find("LevelSelectBurrow").GetComponent<BurrowLevelSelect>(); }

        if (SceneManager.GetActiveScene().name == "Level 0 - Onboard") // temporary
        {
            NoLevelText.enabled = true;
            closeButton.Select();
        }
        if (!AlreadyAddedButtons) { InstantiateButtons(); }
        SetActiveLevels();
    }

    private void OnDisable()
    {
        // NoLevelText.enabled = false;
    }
}

using __OasisBlitz.__Scripts.Collectables;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class DebugOpenMenu : MonoBehaviour
{
    public __OasisBlitz.Player.PlayerInput playerInput;
    public GameObject menuPanel; // Assign this in the inspector with the GameObject you want to enable as the menu
    [FormerlySerializedAs("keyBind1")] public KeyCode keyBindFirstKey = KeyCode.LeftShift;
    [FormerlySerializedAs("keyBind2")] public KeyCode keyBindSecondKey = KeyCode.G;
    public DebugCheckpointList debugCheckpointList;

    public Toggle BlastToggle;
    public Toggle DashToggle;
    public Toggle DrillToggle;
    
    private bool isMenuActive = false; // To keep track of the menu's active state
    private PlayerStateMachine ctx;
    private void Awake()
    {
        menuPanel.SetActive(false);
        ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
    }
    
    void Update()
    {
        // Check if the specific combination of keys (Ctrl + M) is pressed
        if (Input.GetKey(keyBindFirstKey) && Input.GetKeyDown(keyBindSecondKey))
        {
            ToggleMenu(!menuPanel.activeSelf);
        }
        if (Input.GetKey(KeyCode.Escape))
            ToggleMenu(false);
    }

    void ToggleMenu(bool active)
    {
        if (isMenuActive == active) return;
        // Toggle the active state of the menu
        isMenuActive = active;
        menuPanel.SetActive(isMenuActive);

        // Show or hide the mouse cursor based on the menu's active state
        Cursor.visible = isMenuActive;

        // Lock or unlock the cursor based on the menu's active state
        Cursor.lockState = isMenuActive ? CursorLockMode.None : CursorLockMode.Locked;

        if (isMenuActive)
        {
            debugCheckpointList.OnOpen();
            InitializeAbilityToggles();
            // playerInput.DisableCharacterControls();
        }
        else
        {
            // playerInput.EnableCharacterControls();
        }
    }
    
    void InitializeAbilityToggles() {
        if (BlastToggle != null)
            BlastToggle.SetIsOnWithoutNotify(BounceAbility.Instance.BounceEnabled);
        if (DashToggle != null)
            DashToggle.SetIsOnWithoutNotify(ctx.TargetedDash.DashEnabled);
        if (DrillToggle != null)
            DrillToggle.SetIsOnWithoutNotify(ctx.ToggleDrill);
    }
    
    public void DeleteSaveAndReload()
    {
        XMLFileManager.Instance.DeleteSaves();
        // PlayerPrefs.DeleteAll();
        // DOTween.KillAll();
        // SceneManager.LoadScene($"MainMenu");
        // Destroy(GameObject.Find($"SceneEssentials"));
        QuitGame();
    }

    private void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OpenSaveFolder()
    {
        GUIUtility.systemCopyBuffer = Application.persistentDataPath;
        System.Diagnostics.Process.Start("explorer.exe", "/select,"+ Application.persistentDataPath);
    }

    public void GetPlant()
    {
        Debug.Log("DebugOpenMenu: You get a plant. Congrats!");
        Plant plant = FindObjectOfType<Plant>();
        ToggleMenu(false);
        if (plant == null)
        {
            Debug.LogWarning("DebugOpenMenu: No Plant found in this scene!");
            return;
        }
        ctx.CharacterController.SetPosition(plant.gameObject.transform.position);
    }

    public void KillAndRespawn()
    {
        Debug.Log("DebugOpenMenu: Kill and Respawn.");
        FindObjectOfType<PlayerStateMachine>().InstantKill();
    }

    public void UnlockBlast() {
        if (XMLFileManager.Instance.SaveExists() == false)
            XMLFileManager.Instance.NewGame();
        // XMLFileManager.Instance.SaveBlastStatus(true);
    }
    
    public void ToggleBlast(bool toggle) {
        BounceAbility.Instance.BounceEnabled = toggle;
    }

    public void ToggleDash(bool toggle) {
        ctx.TargetedDash.DashEnabled = toggle;
    }

    public void ToggleDrill(bool toggle) {
        ctx.ToggleDrill = toggle;
    }
}


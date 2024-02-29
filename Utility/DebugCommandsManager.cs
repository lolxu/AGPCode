using __OasisBlitz.__Scripts.Player.Environment.Fruits;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using UnityEngine;
using UnityEngine.UI;

public class DebugCommandsManager : MonoBehaviour
{
    public static DebugCommandsManager Instance { get; private set; }

    [SerializeField] private GameObject _debugWindow;       // Just need to set active or inactive
    [SerializeField] private __OasisBlitz.Player.CharacterController _character;
    [SerializeField] private Image _godModeStatus;
    [SerializeField] private FruitsElixirReplenish fer;
    [SerializeField] private FruitsHighJump fhj;
    FruitsElixirReplenish fruitDrillixir;
    FruitsHighJump fruitHighJump;


    private bool _debugMode;
    private bool _godMode;

    public DebugCommandsManager()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        fruitDrillixir = Instantiate(fer, new Vector3(-99999, -99999, -99999), Quaternion.identity);
        fruitHighJump = Instantiate(fhj, new Vector3(-99999, -99999, -99999), Quaternion.identity);
        _debugMode = false;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
    }

    public bool godModeStatus()
    {
        return _godMode;
    }

    // Update is called once per frame
    void Update()
    {
        // TOGGLE DEBUG MODE
        // "CTRL" + "="
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                if (_debugMode)
                { // Debug Mode is on --> turn it off
                  // Also disable God Mode if enabled
                    _godMode = false;
                    _godModeStatus.color = Color.red;
                    _debugWindow.SetActive(false);
                    _debugMode = false;
                }
                else
                {
                    _debugWindow.SetActive(true);
                    _debugMode = true;
                }

                Debug.Log("Debug Mode is now " + _debugMode.ToString().ToUpper());
            }
        }

        // Commands only active in Debug Mode
        if (_debugMode)
        {
            /*
            // EQUIP DRILIXIR REPLENISH FRUIT
            // "F1"
            if(Input.GetKeyDown(KeyCode.F1))
            {
                if(FruitsManager.Instance.CurrentFruit != null) { FruitsManager.Instance.UnequipFruit(); }
                FruitsManager.Instance.EquipFruit(fruitDrillixir);
            }
            // EQUIP HIGH JUMP FRUIT
            // "F2"
            if (Input.GetKeyDown(KeyCode.F2))
            {
                if (FruitsManager.Instance.CurrentFruit != null) { FruitsManager.Instance.UnequipFruit(); }
                FruitsManager.Instance.EquipFruit(fruitHighJump);
            }
            */
            //  TO PREVIOUS SPAWN POINT
            //  "SHIFT" + "R"
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    if (RespawnManager.Instance.GetSpawnPoint() != Vector3.zero) { _character.SetPosition(RespawnManager.Instance.GetSpawnPoint()); }
                }
            }

            //  TOGGLE GOD MODE
            //  "SHIFT" + "G"
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    if (_godMode)
                    {
                        _godMode = false;
                        _godModeStatus.color = Color.red;
                    }
                    else
                    {
                        _godMode = true;
                        _godModeStatus.color = Color.green;
                    }
                }
            }
        }
    }

    public bool GetDebugMode()
    {
        return _debugMode;
    }
}

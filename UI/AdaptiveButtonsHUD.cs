using __OasisBlitz.__Scripts.Player.Environment.Fruits;
using __OasisBlitz.Player.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdaptiveButtonsHUD : MonoBehaviour
{
    private enum AdaptiveStates
    {
        GroundNoDrill,
        GroundDrill,
        AirNoDrill,
        AirDrill,
        Underground
    }

    public GameObject AdaptiveButtons;

    [SerializeField] private PlayerStateMachine pStateMachine;
    private string currentPlayerState;
    [SerializeField] private BounceAbility bounce;

    [Header("Keyboard Images")]
    [SerializeField] private Image LMB;
    [SerializeField] private Image RMB;
    [SerializeField] private Image Spacebar;
    [SerializeField] private Image E;


    [Header("Controller Images")]
    [SerializeField] private Image RB;
    [SerializeField] private Image RT;
    [SerializeField] private Image LB;
    [SerializeField] private Image LT;
    [SerializeField] private Image A;
    [SerializeField] private Image B;
    [SerializeField] private Image X;
    [SerializeField] private Image Y;

    [Header("Control Schemes")]
    [SerializeField] private GameObject Keyboard;
    [SerializeField] private GameObject Controller;

    [Header("Prompts")]
    [SerializeField] private GameObject _GroundNoDrill;
    [SerializeField] private GameObject _GroundDrill;
    [SerializeField] private GameObject _AirNoDrill;
    [SerializeField] private GameObject _AirDrill;
    [SerializeField] private GameObject _Underground;
    [SerializeField] private GameObject BlastReadyAirDrill;
    [SerializeField] private GameObject BlastReadyAirNoDrill;
    [SerializeField] private GameObject Interact;
    [SerializeField] private GameObject Dash;

    private AdaptiveStates displayState;

    private bool blastReady = true;
    private bool drillOut = false;

    public void SetBlastReady(bool status)
    {
        blastReady = status;
    }
    public void SetDrillOut(bool status)
    {
        drillOut = true;
        print("Drill set to: " + drillOut);
        ChangeAdaptiveDisplay();
    }
    public void DisplayDashPrompt(bool status)
    {
        Dash.SetActive(status);
        if(status)
        {
            Active(RMB);
            Active(LT);
        }
        else
        {
            Inactive(RMB);
            Inactive(LT);
        }
    }

    private void Inactive(Image img)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0.25f);
    }
    private void Active(Image img)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1.0f);
    }

    private void DisableAllPrompts()
    {
        _GroundNoDrill.gameObject.SetActive(false);
        _GroundDrill.gameObject.SetActive(false);
        _AirNoDrill.gameObject.SetActive(false);
        _AirDrill.gameObject.SetActive(false);
        _Underground.gameObject.SetActive(false);
    }
    private void DisableActivePrompt()
    {
        switch(displayState)
        {
            case (AdaptiveStates.GroundNoDrill):
                _GroundNoDrill.gameObject.SetActive(false);
                break;
            case (AdaptiveStates.GroundDrill):
                _GroundDrill.gameObject.SetActive(false);
                break;
            case (AdaptiveStates.AirNoDrill):
                _AirNoDrill.gameObject.SetActive(false);
                break;
            case (AdaptiveStates.AirDrill):
                _AirDrill.gameObject.SetActive(false);
                break;
            case (AdaptiveStates.Underground):
                _Underground.gameObject.SetActive(false);
                break;
        }
    }
    private void GroundNoDrill()
    {
        DisableActivePrompt();
        displayState = AdaptiveStates.GroundNoDrill;

        _GroundNoDrill.gameObject.SetActive(true);

        if(GlobalSettings.Instance.displayedController == "KEYBOARD")
        {
            Active(Spacebar);
            Active(LMB);


/*            if(FruitsManager.Instance.CurrentFruit != null)
            {
                if (FruitsManager.Instance.CurrentFruit.GetFruitName() == "ElixirReplenish") { Active(E); }
            }
            else { Inactive(E); }*/

            // Inactive(RMB);
        }
        else
        {
            Active(A);
            Active(B);

            Active(RT);
            Active(LB);
            Active(RB);
/*            if (FruitsManager.Instance.CurrentFruit != null)
            {
                if (FruitsManager.Instance.CurrentFruit.GetFruitName() == "ElixirReplenish")
                {
                    Active(X);
                    Active(Y);
                }
            }
            else
            {
                Inactive(X);
                Inactive(Y);
            }*/
            //Inactive(LT);
        }
    }

    private void GroundDrill()
    {
        DisableActivePrompt();
        displayState = AdaptiveStates.GroundDrill;

        _GroundDrill.gameObject.SetActive(true);

        if (GlobalSettings.Instance.displayedController == "KEYBOARD")
        {
            Active(Spacebar);
            Active(LMB);

            Inactive(E);
            // Inactive(RMB);
        }
        else
        {
            Active(A);
            Active(B);
            Active(RT);
            Active(LB);
            Active(RB);

            Inactive(X);
            Inactive(Y);
            //Inactive(LT);
        }
    }

    private void AirNoDrill()
    {
        DisableActivePrompt();
        displayState = AdaptiveStates.AirNoDrill;

        _AirNoDrill.gameObject.SetActive(true);

        if (GlobalSettings.Instance.displayedController == "KEYBOARD")
        {
            Active(LMB);

            if (blastReady && BounceAbility.Instance.BounceEnabled) {
                Active(Spacebar);
                BlastReadyAirNoDrill.SetActive(true);
            }
            else {
                Inactive(Spacebar);
                BlastReadyAirNoDrill.SetActive(false);
            }

/*            if (FruitsManager.Instance.CurrentFruit != null)
            {
                if (FruitsManager.Instance.CurrentFruit.GetFruitName() == "ElixirReplenish") { Active(E); }
            }
            else { Inactive(E); }*/

        }
        else
        {

            Active(RT);
            Active(LB);
            Active(RB);
            if(blastReady && BounceAbility.Instance.BounceEnabled)
            {
                Active(A);
                Active(B);
                BlastReadyAirNoDrill.SetActive(true);
            }
            else
            {
                Inactive(A);
                Inactive(B);
                BlastReadyAirNoDrill.SetActive(false);
            }
/*            if (FruitsManager.Instance.CurrentFruit != null)
            {
                if (FruitsManager.Instance.CurrentFruit.GetFruitName() == "ElixirReplenish") { 
                    Active(X);
                    Active(Y);
                }
            }
            else
            {
                Inactive(X);
                Inactive(Y);
            }*/
        }
    }

    private void AirDrill()
    {
        DisableActivePrompt();
        displayState = AdaptiveStates.AirDrill;

        _AirDrill.gameObject.SetActive(true);

        if (GlobalSettings.Instance.displayedController == "KEYBOARD")
        {
            Active(LMB);
            if (blastReady && BounceAbility.Instance.BounceEnabled) { 
                Active(Spacebar);
                BlastReadyAirDrill.SetActive(true);
            }
            else { 
                Inactive(Spacebar);
                BlastReadyAirDrill.SetActive(false);
            }
/*            if(FruitsManager.Instance.CurrentFruit != null)
            {
                if (FruitsManager.Instance.CurrentFruit.GetFruitName() == "ElixirReplenish") { Active(E); }
            }
            else { Inactive(E); }*/
        }
        else
        {
            Active(RT);
            Active(LB);
            Active(RB);
            if (blastReady && BounceAbility.Instance.BounceEnabled)
            {
                Active(A);
                Active(B);
                BlastReadyAirDrill.SetActive(true);
            }
            else
            {
                Inactive(A);
                Inactive(B);
                BlastReadyAirDrill.SetActive(false);
            }
/*            if(FruitsManager.Instance.CurrentFruit != null)
            {
                if (FruitsManager.Instance.CurrentFruit.GetFruitName() == "ElixirReplenish")
                {
                    Active(X);
                    Active(Y);
                }
            }
            else
            {
                Inactive(X);
                Inactive(Y);
            }*/
        }
    }

    private void Underground()
    {
        DisableActivePrompt();
        displayState = AdaptiveStates.Underground;

        _Underground.gameObject.SetActive(true);

        if (GlobalSettings.Instance.displayedController == "KEYBOARD")
        {
            Inactive(LMB);
            Inactive(Spacebar);
/*            if(FruitsManager.Instance.CurrentFruit != null)
                                    {
                                        if (FruitsManager.Instance.CurrentFruit.GetFruitName() == "ElixirReplenish") { Active(E); }
                                    }
                                    else { Inactive(E); }*/
        }
        else
        {
            Inactive(RT);
            Inactive(A);
            Inactive(B);
            /*            if(FruitsManager.Instance.CurrentFruit != null)
                        {
                            if (FruitsManager.Instance.CurrentFruit.GetFruitName() == "ElixirReplenish")
                            {
                                Active(X);
                                Active(Y);
                            }
                        }
                        else
                        {
                            Inactive(X);
                            Inactive(Y);
                        }*/

        }
    }

    public void DisplayInteract(bool status)
    {
        if(status)
        {
            Interact.gameObject.SetActive(true);
            Active(E);
            Active(X);
            Active(Y);
        }
        else
        {
            Interact.gameObject.SetActive(false);
            Inactive(E);
            Inactive(X);
            Inactive(Y);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        displayState = AdaptiveStates.GroundNoDrill;
        currentPlayerState = pStateMachine.CurrentState.StateName();

        DisableAllPrompts();

        AdaptiveButtons.SetActive(GlobalSettings.Instance.controlsHUD);

        Inactive(E);

        Inactive(X);
        Inactive(Y);

        OverrideAdaptiveHUDSetting();   // First load of the UI will not use callback, do this instead
        SceneManager.sceneLoaded += OverrideAdaptiveHUDSetting;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OverrideAdaptiveHUDSetting;
    }
    void OverrideAdaptiveHUDSetting(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name.Contains("Burrow"))
        {
            AdaptiveButtons.SetActive(false);
            UnityEngine.Debug.Log("Force hide adaptive buttons");
        }
        else if (scene.name.Contains("Onboard"))
        {
            AdaptiveButtons.SetActive(true);
            UnityEngine.Debug.Log("Force show adaptive buttons");
        }
        else
        {
            AdaptiveButtons.SetActive(GlobalSettings.Instance.controlsHUD);
        }
    }
    // Override used for first call
    void OverrideAdaptiveHUDSetting()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name.Contains("Burrow"))
        {
            AdaptiveButtons.SetActive(false);
            UnityEngine.Debug.Log("Force hide adaptive buttons");
        }
        else if (SceneManager.GetActiveScene().name.Contains("Onboard"))
        {
            AdaptiveButtons.SetActive(true);
            UnityEngine.Debug.Log("Force show adaptive buttons");
        }
        else
        {
            AdaptiveButtons.SetActive(GlobalSettings.Instance.controlsHUD);
        }
    }
    /* 
    Idle,
    Walk,
    RestrictedHorizontalMovement,
    Drill,
    DrillAbove,
    DrillBelow,
    FreeFall,
    Grounded,
    Dead


     */

    public void ChangeAdaptiveDisplay()
    {
        if (GlobalSettings.Instance.displayedController == "KEYBOARD")
        {
            Keyboard.SetActive(true);
            Controller.SetActive(false);
        }
        else
        {
            Keyboard.SetActive(false);
            Controller.SetActive(true);
        }

        switch (currentPlayerState)
        {
            case "Grounded":
                if (pStateMachine.GauntletManager.extended) { GroundDrill(); }
                else { GroundNoDrill(); }
                break;
            case "Drill":
                if (pStateMachine.IsSubmerged) { Underground(); }
                else { AirDrill(); }
                break;
            case "FreeFall":
                AirNoDrill();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // UnityEngine.Debug.Log(blastReady);
        if(AdaptiveButtons.activeInHierarchy)
        {
            if (currentPlayerState != pStateMachine.CurrentState.StateName()) { currentPlayerState = pStateMachine.CurrentState.StateName(); }
            ChangeAdaptiveDisplay();
        }
    }
}

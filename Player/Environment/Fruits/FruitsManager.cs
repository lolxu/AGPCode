using System;
using System.Collections;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Player.Animation;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.__Scripts.Player.Environment.Fruits
{
    /// <summary>
    /// This is Deprecated
    /// </summary>
    public class FruitsManager : MonoBehaviour
    {
        public static FruitsManager Instance;
        public Fruits CurrentFruit { get; private set; }= null;

        [Header("Fruit General Settings")]
        [SerializeField] private GameObject elixirFruit;
        [SerializeField] private GameObject highJumpFruit;
        [SerializeField] private FruitUIAnimationManager animationController;
        

        [Header("Elixir Fruit Settings")] 
        public int elixirUseAmount = 5;
        public float elixirReplenishAmount = 0.0f;
        
        [Header("High Jump Fruit Settings")]
        public float highJumpVelocity = 80f;
        private float orgJumpVelocity;

        private FruitsElixirReplenish elixirFruitScript;
        private FruitsHighJump highJumpFruitScript;

        public bool isFruitRequested { get; private set; } = false;
        public bool canEquipFruit { get; private set; } = false;

        private PlayerStateMachine ctx;
        
        // Other Private fruit variables here
        private int fruitUseTimes;
        
        // private void Awake()
        // {
        //     if (Instance == null)
        //     {
        //         Instance = this;
        //     }
        //
        //     elixirFruitScript = elixirFruit.GetComponent<FruitsElixirReplenish>();
        //     highJumpFruitScript = highJumpFruit.GetComponent<FruitsHighJump>();
        //     
        //     // Register Event to SceneManager
        //     SceneManager.sceneLoaded += OnSceneLoaded;
        // }

        // private void OnSceneLoaded(Scene myScene, LoadSceneMode myMode)
        // {
        //     if (gameObject)
        //     {
        //         StartCoroutine(WaitForSceneLoad());
        //     }
        // }
        //
        // IEnumerator WaitForSceneLoad()
        // {
        //     yield return new WaitForSeconds(0.01f);
        //
        //     ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        //     
        //     // Initialize other variables here
        //     orgJumpVelocity = ctx.OrgInitialJumpVelocity;
        //     
        //     switch (GameMetadataTracker.Instance.GetActivatedFruit())
        //     {
        //         case "HighJump":
        //             // Debug.Log("High jump fruit is in pouch");
        //             CurrentFruit = highJumpFruitScript;
        //             EquipFruit(CurrentFruit);
        //             break;
        //         case "ElixirReplenish":
        //             // Debug.Log("Elixir Replenish fruit in pouch");
        //             CurrentFruit = elixirFruitScript;
        //             fruitUseTimes = elixirUseAmount;
        //             EquipFruit(CurrentFruit);
        //             break;
        //         default:
        //             // Debug.Log("No fruits");
        //             break;
        //     }
        // }

        public void RequestFruit()
        {
            isFruitRequested = true;
        }

        public void FinishRequestFruit()
        {
            isFruitRequested = false;
        }

        public int GetFruitUseAmount()
        {
            return fruitUseTimes;
        }

        // public void EquipFruit(Fruits thisFruit)
        // {
        //     Debug.Log("Equipped fruit: " + thisFruit);
        //     CurrentFruit = thisFruit;
        //     CurrentFruit.FruitAbilityActivate();
        //     canEquipFruit = false;
        //     GameMetadataTracker.Instance.StoreActivatedFruit(thisFruit.GetFruitName());
        //     animationController.PlaySpinAnimation();
        // }
        //
        // public void UnequipFruit()
        // {
        //     CurrentFruit.FruitAbilityDeactivate();
        //     CurrentFruit = null;
        //     GameMetadataTracker.Instance.StoreActivatedFruit("nothing");
        // }

        /*
         *  FRUIT ABILITIES
         */

        /// <summary>
        /// Elixir Fruit
        /// </summary>
        public void InitializeElixirFruit()
        {
            fruitUseTimes = elixirUseAmount;
        }

        public void RemoveElixirFruit()
        {
            fruitUseTimes = 0;
        }
        // public void ReplenishElixir()
        // {
        //     if (fruitUseTimes > 0)
        //     {
        //         Debug.Log("Replenished");
        //         fruitUseTimes--;
        //         ctx.DrillixirManager.RefillDrillixir(elixirReplenishAmount);
        //     }
        //
        //     // Unequip fruit
        //     if (fruitUseTimes == 0)
        //     {
        //         UnequipFruit();
        //     }
        // }

        /// <summary>
        /// High Jump Fruit
        /// </summary>
        public void InitializeHighJumpFruit()
        {
            ctx.InitialJumpVelocity = highJumpVelocity;
        }

        public void RemoveHighJumpFruit()
        {
            ctx.InitialJumpVelocity = orgJumpVelocity;
        }
    }
}
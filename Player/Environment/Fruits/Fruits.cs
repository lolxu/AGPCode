using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.__Scripts.Player.Environment.Vitalizer;
using __OasisBlitz.Player.StateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace __OasisBlitz.__Scripts.Player.Environment.Fruits
{
    public class Fruits : MonoBehaviour
    {
        // [Header("General fruit settings")]
        // [SerializeField] protected float fruitRadius = 10.0f;
        // [SerializeField] protected GameObject canvas, prompt;
        // [SerializeField] protected TextMeshProUGUI promptText;
        // [SerializeField] private Image hotkey;
        // [SerializeField] private Sprite keyboardHotkey, genericHotkey, xboxHotkey;
        // [SerializeField] private Material collectedMaterial;
        // [SerializeField] private int fruitGrowCost = 5;
        // private string hotkeyType;
        //
        // protected bool hasBeenCollected = false;
        // protected bool hasActivated = false;
        // protected PlayerStateMachine ctx;
        // protected string fruitName = "";
        // protected bool fruitInRange = false;
        // protected Transform myCameraTransform;
        //
        //
        // // Inheritable code for different fruit abilities
        // public virtual void FruitAbilityActivate(){}
        // public virtual void FruitAbilityDeactivate(){}
        //
        // private void Start()
        // {
        //     promptText.text = "Use " + fruitGrowCost + " Vitalizers to unlock";
        //     hasBeenCollected = GameMetadataTracker.Instance.LookupFruitCollectStatus(fruitName);
        //     if (hasBeenCollected)
        //     {
        //         SetCollectStatus(true);
        //     }
        // }
        //
        // public string GetFruitName()
        // {
        //     return fruitName;
        // }
        //
        // public void SetCollectStatus(bool status)
        // {
        //     promptText.text = "Get " + fruitName;
        //     StartCoroutine(CollectTimeout(status));
        // }
        //
        // IEnumerator CollectTimeout(bool status)
        // {
        //     yield return null;
        //     hasBeenCollected = status;
        //     if (hasBeenCollected)
        //     {
        //         GetComponent<Renderer>().material = collectedMaterial;
        //         GameMetadataTracker.Instance.SetFruitCollectStatus(fruitName, hasBeenCollected);
        //     }
        // }
        //
        // public bool GetCollectStatus()
        // {
        //     return hasBeenCollected;
        // }
        //
        // private void Update()
        // {
        //     // Make the text face towards player. Not Camera at the moment
        //     canvas.transform.LookAt(2 * gameObject.transform.position - myCameraTransform.position);
        //     
        //     // If the fruit is close enough to the player
        //     if (Vector3.Distance(gameObject.transform.position, ctx.gameObject.transform.position) < fruitRadius)
        //     {
        //         prompt.SetActive(true);
        //         if(hotkeyType != GlobalSettings.Instance.displayedController)
        //         {
        //             hotkeyType = GlobalSettings.Instance.displayedController;
        //             switch(hotkeyType)
        //             {
        //                 case "KEYBOARD":
        //                     hotkey.sprite = keyboardHotkey;
        //                     break;
        //                 case "XBOX":
        //                     hotkey.sprite = xboxHotkey;
        //                     break;
        //                 case "PLAYSTATION":
        //                 case "OTHER":
        //                     hotkey.sprite = genericHotkey;
        //                     break;
        //
        //             }
        //         }
        //         fruitInRange = true;
        //     }
        //     else
        //     {
        //         prompt.SetActive(false);
        //         fruitInRange = false;
        //     }
        //
        //     if (fruitInRange)
        //     {
        //         Debug.Log("Fruit In Range");
        //     }
        //
        //     // Debug.Log(ctx.ConsumeFruitRequested);
        //     if (fruitInRange)
        //     {
        //         if (FruitsManager.Instance.isFruitRequested)
        //         {
        //             FruitsManager.Instance.FinishRequestFruit();
        //             if (!hasBeenCollected)
        //             {
        //                 if (GameMetadataTracker.Instance.GetVitalizerCount() >= fruitGrowCost)
        //                 {
        //                     SetCollectStatus(true);
        //                     VitalizerManager.Instance.ChangeVitalizerCountBy(-fruitGrowCost);
        //                     GameMetadataTracker.Instance.StoreVitalizerCount(VitalizerManager.Instance
        //                         .VitalizerPieceCount);
        //                 }
        //             }
        //             else
        //             {
        //                 if (!hasActivated)
        //                 {
        //                     if (FruitsManager.Instance.CurrentFruit != null)
        //                     {
        //                         FruitsManager.Instance.UnequipFruit();
        //                     }
        //
        //                     FruitsManager.Instance.EquipFruit(this);
        //                 }
        //             }
        //
        //         }
        //     }
        //     
        // }
        
    }
}



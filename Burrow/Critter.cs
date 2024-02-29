using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Collectables;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity.Example;

public class Critter : MonoBehaviour
{
    public enum CritterName
    {
        Clover,
        Juno,
        Miles,
        NullCritter
    }

    public enum CritterState
    {
        Objective,
        Random
    }
    
    public CritterName critterName;
    public CritterState critterState;
    
    //Interaction indicator
    [SerializeField] private CritterBillboard billboard;
    private string hotkeyType;
    [SerializeField] private Sprite m_keyboardInteractPrompt;
    [SerializeField] private Sprite m_controllerInteractPrompt;
    [SerializeField] private SpriteRenderer m_currentActivePrompt;
    private bool IndicatorActive = false;
    //set this on initialize in CritterManager
    public CinemachineCamera thisCritterCamera;
    public Vector3 banditPosDuringInteraction;

    [SerializeField] 
    void Awake()
    {
        GetComponent<YarnCharacter>().characterName = Name();
        m_currentActivePrompt.sprite = m_controllerInteractPrompt;
    }

    public string Name()
    {
        return critterName.ToString();
    }

    public string GetDesiredDialogueNode()
    {
        string dialogueName = Name() + critterState.ToString();

        return dialogueName;

        // TODO: Add a random number to the end if this state has multiple options (need to know how many random options,
        // probably shouldn't have to be the same for each critter)
    }
    public void EnableInteractIndicator()
    {
        if(!IndicatorActive)
        {
            m_currentActivePrompt.transform.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.InOutBounce);
            billboard.EnableBillboard();
            IndicatorActive = true;
        }
    }

    public void DisableInteractIndicator()
    {
        if (IndicatorActive)
        {
            m_currentActivePrompt.transform.DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InOutBounce);
            billboard.DisableBillboard();
            IndicatorActive = false;
        }
    }
}

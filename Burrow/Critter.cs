using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Collectables;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;
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
        Ordered
    }

    private int numPlantsCollected = -1;
    //number of ordered lines
    private int orderedLines = 4;

    private int currOrderedLine = 0;
    //Number of random lines
    private int randomLines = 3;

    private int SkipLinesUntilIndex = 0;
    
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
    
    //Set critter animations
    public CritterAnimations critterAnimations;

    [SerializeField] 
    void Awake()
    {
        GetComponent<YarnCharacter>().characterName = Name();
        m_currentActivePrompt.sprite = m_controllerInteractPrompt;
    }

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        numPlantsCollected = Mathf.Clamp(XMLFileManager.Instance.GetNumPlantsCollected(), 1, 3);
    }

    public string Name()
    {
        return critterName.ToString();
    }

    public string GetDesiredDialogueNode()
    {
        string dialogueName = Name() + critterState.ToString() + numPlantsCollected + "-";
        while (currOrderedLine < SkipLinesUntilIndex)
        {
            currOrderedLine++;
        }
        if (critterState == CritterState.Ordered)
        {
            dialogueName += currOrderedLine.ToString();
            currOrderedLine++;
            currOrderedLine %= orderedLines;
        }
        return dialogueName;

        // Add a random number to the end if this state has multiple options (need to know how many random options,
        // probably shouldn't have to be the same for each critter)
    }

    [YarnCommand("Intro")]
    public void SetIntroduced()
    {
        //prevents critter from introducing themselves more than once
        SkipLinesUntilIndex = 1;
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

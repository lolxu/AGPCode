using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference desertAmbience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference musicMainTheme { get; private set; }
    [field: SerializeField] public EventReference musicBurrowTheme { get; private set; }
    [field: SerializeField] public EventReference musicLevel1 { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public EventReference buttonHover { get; private set; }
    [field: SerializeField] public EventReference buttonPress { get; private set; }
    [field: SerializeField] public EventReference pause { get; private set; }

    [field: Header("Player")] 
    private HashSet<EventReference> playerSounds = new HashSet<EventReference>();

    public bool isPlayerSound(EventReference soundReference)
    {
        return playerSounds.Contains(soundReference);
    }
    [field: SerializeField] public EventReference blast { get; private set; }
    [field: SerializeField] public EventReference blastReady { get; private set; }
    [field: SerializeField] public EventReference bounce { get; private set; }
    [field: SerializeField] public EventReference coinCollection { get; private set; }
    [field: SerializeField] public EventReference death { get; private set; }
    [field: SerializeField] public EventReference drill { get; private set; }
    [field: SerializeField] public EventReference drillForm { get; private set; }
    [field: SerializeField] public EventReference drillUnForm { get; private set; }
    [field: SerializeField] public EventReference drillBoost { get; private set; }
    [field: SerializeField] public EventReference drillSandImpact { get; private set; }
    [field: SerializeField] public EventReference jump { get; private set; }
    [field: SerializeField] public EventReference dash { get; private set; }
    [field: SerializeField] public EventReference pickUpVitalizer { get; private set; }
    [field: SerializeField] public EventReference stuckInBouncePad { get; private set; }
    [field: SerializeField] public EventReference walk { get; private set; }
    [field: SerializeField] public EventReference footstepSand { get; private set; }
    [field: SerializeField] public EventReference walkOnlyDrillImpact { get; private set; }
    [field: SerializeField] public EventReference windUp { get; private set; }
    [field: SerializeField] public EventReference slide { get; private set; }

    [field: Header("Enemy")]
    [field: SerializeField] public EventReference bounceKnightSpikeShield { get; private set; }
    [field: SerializeField] public EventReference enemyHurt { get; private set; }
    [field: SerializeField] public EventReference enemyShoot { get; private set; }

    [field: Header("Interactable")]
    [field: SerializeField] public EventReference chestBreak { get; private set; }
    [field: SerializeField] public EventReference cannonBoom { get; private set; }
    [field: SerializeField] public EventReference cannonEnter { get; private set; }

    [field: Header("GameState")]
    [field: SerializeField] public EventReference checkPointFlag { get; private set; }
    [field: SerializeField] public EventReference flowerCollected { get; private set; }

    [field: Header("NPC")]
    [field: SerializeField] public EventReference dialogueStart { get; private set; }
    [field: SerializeField] public EventReference dialogueNext { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
            Destroy(this);
        }
        instance = this;
        
        //Add player sounds to set
        playerSounds.Add(blast);
        playerSounds.Add(blastReady);
        playerSounds.Add(bounce);
        playerSounds.Add(coinCollection);
        playerSounds.Add(death);
        playerSounds.Add(drill);
        playerSounds.Add(drillForm);
        playerSounds.Add(drillUnForm);
        playerSounds.Add(drillBoost);
        playerSounds.Add(drillSandImpact);
        playerSounds.Add(jump);
        playerSounds.Add(dash);
        playerSounds.Add(pickUpVitalizer);
        playerSounds.Add(stuckInBouncePad);
        playerSounds.Add(walk);
        playerSounds.Add(footstepSand);
        playerSounds.Add(walkOnlyDrillImpact);
        playerSounds.Add(windUp);
        playerSounds.Add(slide);
    }
}

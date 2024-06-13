using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    //Global
    //Set parameter "ReverbSpace" at scene start. Level, Burrow 

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference desertAmbience { get; private set; } //static instance
    [field: SerializeField] public EventReference cutsceneAmbience { get; private set; }//instance
    [field: SerializeField] public EventReference burrowAmbience { get; private set; } //static instance
    //Emitters are placed inside 3D space
    [field: SerializeField] public EventReference burrowWater { get; private set; } //emitter instance
    [field: SerializeField] public EventReference burrowFire { get; private set; } //emitter instance
    [field: SerializeField] public EventReference burrowGrass { get; private set; } //emitter instance

    [field: Header("Music")]
    //All music are instances
    [field: SerializeField] public EventReference musicMainTheme { get; private set; }
    [field: SerializeField] public EventReference musicBurrowTheme { get; private set; }
    [field: SerializeField] public EventReference musicLevel1 { get; private set; }
    [field: SerializeField] public EventReference musicLevel2 { get; private set; }
    [field: SerializeField] public EventReference musicCutScene { get; private set; }
    [field: SerializeField] public EventReference musicCutSceneEnd { get; private set; }

    [field: SerializeField] public EventReference musicEndCutScene { get; private set; }
    [field: SerializeField] public EventReference musicEndCutSceneEnd { get; private set; }

    [field: Header("UI")]
    //one shots
    [field: SerializeField] public EventReference buttonHover { get; private set; }
    [field: SerializeField] public EventReference buttonPress { get; private set; }
    [field: SerializeField] public EventReference buttonPressInvalid { get; private set; }
    [field: SerializeField] public EventReference clickEmpty { get; private set; }
    [field: SerializeField] public EventReference checkBoxOn { get; private set; }
    [field: SerializeField] public EventReference checkBoxOff { get; private set; }
    [field: SerializeField] public EventReference pageForward { get; private set; }
    [field: SerializeField] public EventReference pageBack { get; private set; }
    [field: SerializeField] public EventReference startGame { get; private set; }
    [field: SerializeField] public EventReference sliderSet { get; private set; }
    [field: SerializeField] public EventReference sliderMax { get; private set; }
    [field: SerializeField] public EventReference sliderMin { get; private set; }
    [field: SerializeField] public EventReference pause { get; private set; }
    [field: SerializeField] public EventReference unPause { get; private set; }
    //not implemented:
    [field: SerializeField] public EventReference burrowLevelMenu { get; private set; } //menu showing up
    [field: SerializeField] public EventReference burrowLevelSelect { get; private set; } // level selected

    [field: Header("Player")] 
    
    [field: SerializeField] public EventReference blast { get; private set; } //oneshot
    [field: SerializeField] public EventReference blastReady { get; private set; } //oneshot
    [field: SerializeField] public EventReference bounce { get; private set; } //oneshot
    [field: SerializeField] public EventReference coinCollection { get; private set; } //instance ignore
    [field: SerializeField] public EventReference death { get; private set; } //oneshot    missing
    [field: SerializeField] public EventReference drill { get; private set; } //instance parameter: Submerged, EndDrill
    [field: SerializeField] public EventReference drillForm { get; private set; } //oneshot
    [field: SerializeField] public EventReference drillUnForm { get; private set; } //oneshot
    [field: SerializeField] public EventReference drillBoost { get; private set; } //ignore
    [field: SerializeField] public EventReference drillSandEnter{ get; private set; } //oneshot
    [field: SerializeField] public EventReference drillSandExit { get; private set; } //oneshot
    [field: SerializeField] public EventReference jump { get; private set; } //oneshot
    [field: SerializeField] public EventReference splash { get; private set; } //oneshot
    [field: SerializeField] public EventReference land { get; private set; } //oneshot
    [field: SerializeField] public EventReference dash { get; private set; } //oneshot
    [field: SerializeField] public EventReference pickUpVitalizer { get; private set; } //ignore
    [field: SerializeField] public EventReference stuckInBouncePad { get; private set; } //ignore
    [field: SerializeField] public EventReference walk { get; private set; } //ignore
    [field: SerializeField] public EventReference footstep { get; private set; } //oneshot parameter: FootStepMat: sand, stone
    [field: SerializeField] public EventReference walkOnlyDrillImpact { get; private set; } //drillClank
    [field: SerializeField] public EventReference windUp { get; private set; } //ingore
    [field: SerializeField] public EventReference slide { get; private set; } //instance parameter: EndSile, SlideSpeed, SlideMat: sand, stone

    [field: Header("Enemy")]
    [field: SerializeField] public EventReference bounceKnightSpikeShield { get; private set; } //ignore
    [field: SerializeField] public EventReference enemyHurt { get; private set; } //oneshot When enemy die
    [field: SerializeField] public EventReference enemyShoot { get; private set; } //ignore
    [field: SerializeField] public EventReference enemySkillPrepare { get; private set; } //instance. Loop, call when prefab is instantiated
    [field: SerializeField] public EventReference enemySkillActiveHorizontal { get; private set; } //oneshot Oneshot, when they actually fire, stop and release the referenced enemySkillPrepare when calling
    [field: SerializeField] public EventReference enemySkillActiveVertical { get; private set; } //same above

    [field: Header("Interactable")]
    [field: SerializeField] public EventReference chestBreak { get; private set; } //ignore
    [field: SerializeField] public EventReference cannonBoom { get; private set; } //oneshot
    [field: SerializeField] public EventReference cannonEnter { get; private set; } // oneshot

    [field: Header("GameState")]
    [field: SerializeField] public EventReference levelStartDrill { get; private set; } //instance parameter: StartSceneDrillProgress
    [field: SerializeField] public EventReference levelStartDrillEnd { get; private set; } //make no sound , this calls drillSandExit
    [field: SerializeField] public EventReference checkPointFlag { get; private set; } //oneshot
    [field: SerializeField] public EventReference flowerCollected { get; private set; } //oneshot This event is delayed, need fix
    [field: SerializeField] public EventReference levelEndDrillDown { get; private set; } //oneshot for level ends
    [field: SerializeField] public EventReference resetDrill { get; private set; }

    [field: Header("NPC")]
    [field: SerializeField] public EventReference dialogueStart { get; private set; } //Call this when any dialogue Starts, make no sound
    [field: SerializeField] public EventReference dialogueNext { get; private set; } //ignore for bubble sfx
    [field: SerializeField] public EventReference dialogueEnd { get; private set; } //call this when any dialogue has finished, make no sound
    [field: SerializeField] public EventReference milesDialogue { get; private set; } //oneshot
    [field: SerializeField] public EventReference cloverDialogue { get; private set; } //oneshot
    [field: SerializeField] public EventReference junoDialogue { get; private set; } //oneshot

    [field: Header("Cutscenes")]
    [field: SerializeField] public EventReference openingCutscene { get; private set; } //ignore
    [field: SerializeField] public EventReference cutsceneNextLine { get; private set; } //ignore

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            // Debug.LogError("Found more than one FMOD Events instance in the scene.");
            Destroy(this);
        }
        instance = this;
    }
}

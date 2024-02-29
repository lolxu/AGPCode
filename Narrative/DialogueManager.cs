using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using UnityEditor.Rendering;
using __OasisBlitz.__Scripts.Collectables;
using Obi;
using DG.Tweening;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Yarn")]
    [SerializeField] private DialogueRunner runner;
    private string currentDialogue;
    public bool dialogueOpen { get; private set; } = false;
    [Tooltip("Number of random dialogue options each critter has")]
    [SerializeField] private int numDialogues;

    [Header("Canvas Setup")]
    [SerializeField] private Canvas DialogueCanvas;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject Player;
    [Tooltip("How much offset from the critter position to place the dialogue box (vertically)")]
    [SerializeField] private float offset;

    [Header("Continue Button")]
    [SerializeField] private Image ContinueButton;
    [SerializeField] private Sprite ContinueKeyboard;
    [SerializeField] private Sprite ContinueController;
    private string hotkeyType = "KEYBOARD";
    public Critter ActiveCritter { get; private set; }
    private GameObject currentCritterInteractPrompt;

    [Header("Dialogue Tweening")]
    [Tooltip("How long the pop in/out animation should be for the dialogue boxes")]
    [SerializeField] float tweenDuration;
    private Tween DialogueTween;
    private Vector3 TweenToScale = new Vector3(0.0045f, 0.0045f, 0.0045f);

    // Bools for each critter to check if objective is completed
    private InMemoryVariableStorage YarnVariables;
    private bool JackrabbitObjectiveCompleted, LizardObjectiveCompleted, BatObjectiveCompleted, TurtleObjectiveCompleted, OwlObjectiveCompleted;
    
    //Critter Enums
    public enum CritterName
    {
        None,
        Clover,
        Jette,
        Juno,
        Miles,
        Noodle
    }

    // Set when Bandit starts talking to a critter
    public void InteractWithCritter(Critter critter, GameObject interactPrompt, CritterName critterName)
    {
        // If starting to talk to critter
        if(ActiveCritter != critter)
        {
            currentCritterInteractPrompt = interactPrompt;
            TweenOutInteractPrompt();
            // If previously talking to another critter
            if (ActiveCritter != null)
            {
                // Tween out the canvas from previous critter
                TweenDialogueOut(() =>
                {
                    // Then set the new active critter
                    ActiveCritter = critter;
                    Vector3 critterDialoguePosition = new Vector3(critter.transform.position.x, critter.transform.position.y + offset, critter.transform.position.z);
                    DialogueCanvas.transform.position = critterDialoguePosition;
                    TweenDialogueIn(critterName);
                });
            }
            else // Just set the new active critter if previously null
            {
                ActiveCritter = critter;
                Vector3 critterDialoguePosition = new Vector3(critter.transform.position.x, critter.transform.position.y + offset, critter.transform.position.z);
                DialogueCanvas.transform.position = critterDialoguePosition;
                TweenDialogueIn(critterName);
            }
            AudioManager.instance.DialogueAudio(false);
        }
        else
        {
            //Continue the dialogue
            AudioManager.instance.DialogueAudio(true);
            // If dialogue was completed already
            if(!runner.IsDialogueRunning && currentDialogue != null)
            {
                runner.StartDialogue(currentDialogue);
            }
        }
    }

    //Stop ActiveCritter From Talking
    public void RemoveActiveCritter()
    {
        if (ActiveCritter != null)
        {
            ActiveCritter = null;
            TweenDialogueOut();
        }
    }
    // Overload for SceneManager
    public void RemoveActiveCritter(Scene scene)
    {
        if(ActiveCritter != null)
        {
            ActiveCritter = null;
            TweenDialogueOut();
        }
    }

    // Callbacks for events in Dialogue System
    public void TweenDialogueIn(CritterName critterName)
    {
        if (DialogueTween != null) { DialogueTween.Kill(false); }
        dialogueOpen = true;
        DialogueTween = DialogueCanvas.transform.DOScale(TweenToScale, tweenDuration)
            .OnComplete(() =>
            {
                switch(critterName)
                {
                    case CritterName.Clover:
                        if(JackrabbitObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues);
                            currentDialogue = "CloverRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "CloverObjectiveDialogue";
                        }
                        break;
                    case CritterName.Jette:
                        if (LizardObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues + 1);
                            currentDialogue = "JetteRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "JetteObjectiveDialogue";
                        }
                        break;
                    case CritterName.Juno:
                        if (BatObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues + 1);
                            currentDialogue = "JunoRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "JunoObjectiveDialogue";
                        }
                        break;
                    case CritterName.Miles:
                        if (TurtleObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues + 1);
                            currentDialogue = "MilesRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "MilesObjectiveDialogue";
                        }
                        break;
                    case CritterName.Noodle:
                        if (OwlObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues + 1);
                            currentDialogue = "NoodleRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "NoodleObjectiveDialogue";
                        }
                        break;
                }
                StartDialogue(currentDialogue);
            });
    }
    public void TweenDialogueOut()
    {
        if (DialogueTween != null) { DialogueTween.Kill(false); }
        DialogueTween = DialogueCanvas.transform.DOScale(Vector3.zero, tweenDuration)
            .OnComplete(() =>
            {
                dialogueOpen = false;
                StopActiveDialogue();
                ActiveCritter = null;
            });
    }

    // Overload for use in this script
    private void TweenDialogueIn(CritterName critterName, Action after)
    {
        if (DialogueTween != null) { DialogueTween.Kill(false); }
        dialogueOpen = true;
        DialogueTween = DialogueCanvas.transform.DOScale(TweenToScale, tweenDuration)
            .OnComplete(() =>
            {
                switch (critterName)
                {
                    case CritterName.Clover:
                        if (JackrabbitObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues);
                            currentDialogue = "CloverRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "CloverObjectiveDialogue";
                        }
                        break;
                    case CritterName.Jette:
                        if (LizardObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues + 1);
                            currentDialogue = "JetteRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "JetteObjectiveDialogue";
                        }
                        break;
                    case CritterName.Juno:
                        if (BatObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues + 1);
                            currentDialogue = "JunoRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "JunoObjectiveDialogue";
                        }
                        break;
                    case CritterName.Miles:
                        if (TurtleObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues + 1);
                            currentDialogue = "MilesRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "MilesObjectiveDialogue";
                        }
                        break;
                    case CritterName.Noodle:
                        if (OwlObjectiveCompleted)
                        {
                            int roll = UnityEngine.Random.Range(1, numDialogues + 1);
                            currentDialogue = "NoodleRandomDialogue" + roll.ToString();
                        }
                        else
                        {
                            currentDialogue = "NoodleObjectiveDialogue";
                        }
                        break;
                }
                StartDialogue(currentDialogue);
            });
    }
    private void TweenDialogueOut(Action after)
    {
        if (DialogueTween != null) { DialogueTween.Kill(false); }
        DialogueTween = DialogueCanvas.transform.DOScale(Vector3.zero, tweenDuration)
            .OnComplete(() =>
            {
                StopActiveDialogue();
                dialogueOpen = false;
                after();
                // TweenInInteractPrompt();     // Not here --> This function used when talking to another critter
            });
    }

    private void TweenOutInteractPrompt()
    {
        currentCritterInteractPrompt.transform.DOScale(Vector3.zero, tweenDuration);
    }
    private void TweenInInteractPrompt()
    {
        currentCritterInteractPrompt.transform.DOScale(Vector3.one, tweenDuration);
    }

    // Turn canvas to always face camera when a critter is active
    private void LateUpdate()
    {
        if(ActiveCritter != null)
        {
            // DialogueCanvas.transform.LookAt(camera.transform);
            
            // Try to rotate only X and Z, do not look up/down
            DialogueCanvas.transform.LookAt(new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z));

        }
    }

    public void StartDialogue(string dialogueNode)
    {
        runner.StartDialogue(dialogueNode);
    }

    // Call when exiting critter trigger
    public void ClearCurrentDialogue()
    {
        currentDialogue = null;
    }
    private void StopActiveDialogue()
    {
        runner.Stop();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += RemoveActiveCritter;
        YarnVariables = GameObject.FindObjectOfType<InMemoryVariableStorage>();
        
        if (camera == null)
        {
            camera = GameObject.Find("UICamera").GetComponent<Camera>();
        }
        DialogueCanvas.transform.localScale = Vector3.zero;     // Keep the size tween in/out?
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= RemoveActiveCritter;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Burrow"))
        {
            JackrabbitObjectiveCompleted = false;
            BatObjectiveCompleted = false;
            TurtleObjectiveCompleted = false;

            XMLFileManager.Instance.Load();
            // Critter conditions for completing their objective
            // Clover -- 1 plant collected (not including onboard) -- 2 plants total
            if (XMLFileManager.Instance.GetNumPlantsCollected() >= 2)
            {
                JackrabbitObjectiveCompleted = true;
            }
            // Juno -- 2 plants collected -- 3 plants total
            if (XMLFileManager.Instance.GetNumPlantsCollected() >= 3)
            {
                BatObjectiveCompleted = true;
            }            
            // Miles -- 3 plants collected -- 4 plants total
            if (XMLFileManager.Instance.GetNumPlantsCollected() >= 4)
            {
                TurtleObjectiveCompleted = true;
            }            
            // Extra critters
            LizardObjectiveCompleted = false;
            OwlObjectiveCompleted = false;

        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalSettings.Instance)
        {
            if (hotkeyType != GlobalSettings.Instance.displayedController)
            {
                hotkeyType = GlobalSettings.Instance.displayedController;
                switch (hotkeyType)
                {
                    case "KEYBOARD":
                        ContinueButton.sprite = ContinueKeyboard;
                        break;
                    case "XBOX":
                    case "PLAYSTATION":
                    case "OTHER":
                        ContinueButton.sprite = ContinueController;
                        break;
                }
            }
        }
    }



}

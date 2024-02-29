// using System;
// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using UnityEngine;
// using UnityEngine.AI;
// using UnityEngine.SceneManagement;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
//
// namespace __OasisBlitz.__Scripts.Collectables
// {
//     public class Critter : CollectableObject
//     {
//         private Animator m_anim;
//         [FormerlySerializedAs("m_renderer")]
//         [Header("Critter Settings")]
//         [SerializeField] private GameObject m_dialogueBox;
//         [SerializeField] private Canvas m_dialogueCanvas;
//         [SerializeField] private Image m_dialogueImage;
//         [SerializeField] private int associatedIndex = 0;
//         [SerializeField] private List<Sprite> m_critterDialogues;
//         [SerializeField] private GameObject m_keyboardInteractPrompt;
//         [SerializeField] private GameObject m_controllerInteractPrompt;
//         [SerializeField] private Collider m_blockingBox;
//         [SerializeField] private GameObject model;
//         [SerializeField] private bool repeatOneThingForever;
//         private bool talking = false;
//
//         private GameObject m_currentActivePrompt;
//         private SpriteRenderer m_dialogue;
//         private int dialogueIndex = 0;
//         private bool isInteracted = false;
//         private bool isHappy = false;
//         private bool hasFinishedSpeaking = false;
//         private string hotkeyType;
//         
//         // Agent stuff
//         private NavMeshAgent m_agent;
//         private bool canMove = true;
//
//         [SerializeField] private DialogueManager.CritterName critterName; 
//
//         private void Awake()
//         {
//             m_agent = GetComponent<NavMeshAgent>();
//             m_dialogue = m_dialogueBox.GetComponent<SpriteRenderer>();
//             m_dialogueCanvas.worldCamera = GameObject.Find("UICamera").GetComponent<UnityEngine.Camera>();
//             m_currentActivePrompt = m_controllerInteractPrompt;       // Just choose a default
//         }
//
//         protected override void OnStart()
//         {
//             if (CollectableManager.Instance.CheckIsSaved(associatedIndex))
//             {
//                 isHappy = true;
//             }
//             else
//             {
//                 isHappy = false;
//             }
//
//             if (isHappy)
//             {
//                 model.transform.DOScaleY(1.15f, 0.25f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
//             }
//         }
//
//         private void OnTriggerEnter(Collider other)
//         {
//             if (other.CompareTag("Player"))
//             {
//                 if (!DialogueManager.Instance.dialogueOpen)
//                 {
//                     //m_currentActivePrompt.SetActive(false);
//                     m_currentActivePrompt.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f)
//                         .SetEase(Ease.InOutBounce);
//                 }
//             }
//         }
//         private void OnTriggerExit(Collider other)
//         {
//             if (other.CompareTag("Player"))
//             {
//                 if (!isInteracted)
//                 {
//                     //m_currentActivePrompt.SetActive(false);
//                 }
//                 if(DialogueManager.Instance.dialogueOpen)
//                 {
//                     DialogueManager.Instance.TweenDialogueOut();
//                 }
//                 m_currentActivePrompt.transform.DOScale(Vector3.zero, 0.2f)
//                     .SetEase(Ease.InOutBounce);
//             }
//         }
//
//         protected override void OnInteract()
//         {
//             // DialogueManager.Instance.InteractWithCritter(this, m_currentActivePrompt, critterName);
//         }
//         private void TweenInPrompt()
//         {
//             m_currentActivePrompt.transform.DOScale(Vector3.one, 0.2f);
//         }
//         private void TweenOutPrompt()
//         {
//             m_currentActivePrompt.transform.DOScale(Vector3.zero, 0.2f);
//         }
//         // Old interact implementation here in case something goes wrong
//         private void OldInteract()
//         {
//             //m_dialogueImage.gameObject.SetActive(true);
//             m_currentActivePrompt.SetActive(false);
//             // TODO: This is a hack for the under construction guy for state of the game
//             if (repeatOneThingForever)
//             {
//                 //m_dialogue.sprite = m_critterDialogues[0];
//                 //m_dialogueBox.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f).SetEase(Ease.InOutBounce);
//                 m_dialogueImage.sprite = m_critterDialogues[0];
//                 m_dialogueImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f).SetEase(Ease.InOutBounce);
//                 return;
//
//             }
//
//             isInteracted = true;
//             // m_currentActivePrompt.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InOutBounce).OnComplete(()=>
//             // {
//             Debug.Log($"Dialogue Index {dialogueIndex}\tDialogue Count: {m_critterDialogues.Count}\tHappy? {isHappy}");
//             // Check if the last image displayed was the final one
//             if (dialogueIndex == m_critterDialogues.Count)
//             {
//                 // Now, hide the image and move the critter
//                 dialogueIndex = 0;
//                 /*                    m_dialogueBox.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InOutBounce).OnComplete(() =>
//                                     {
//                                         // isInteracted = false;
//                                         if (isHappy && canMove)
//                                         {
//                                             m_blockingBox.enabled = false;
//                                             Vector3 goalDestination = transform.position - new Vector3(15.0f, 0.0f, 15.0f);
//                                             m_agent.SetDestination(goalDestination);
//                                             canMove = false;
//                                             hasFinishedSpeaking = true;
//                                         }
//                                     });*/
//                 m_dialogueImage.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InOutBounce).OnComplete(() =>
//                 {
//                     // isInteracted = false;
//
//                     //if (isHappy && canMove)
//                     if (canMove)
//                     {
//                         Debug.Log("MOVING");
//                         m_blockingBox.enabled = false;
//                         Vector3 goalDestination = transform.position - new Vector3(15.0f, 0.0f, 15.0f);
//                         m_agent.SetDestination(goalDestination);
//                         canMove = false;
//                         hasFinishedSpeaking = true;
//                         //m_dialogueImage.gameObject.SetActive(false);
//                         m_currentActivePrompt.SetActive(true);
//                     }
//                 });
//                 return;
//             }
//
//             // Play the next image
//             if (m_dialogue)
//             {
//                 //m_dialogue.sprite = m_critterDialogues[dialogueIndex];
//                 m_dialogueImage.sprite = m_critterDialogues[dialogueIndex];
//                 dialogueIndex++;
//             }
//
//             if (dialogueIndex <= m_critterDialogues.Count)
//             {
//                 //m_dialogueBox.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f).SetEase(Ease.InOutBounce);
//                 m_dialogueImage.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f).SetEase(Ease.InOutBounce);
//             }
//
//             // });
//         }
//         private void Update()
//         {
//             // Show interact prompt
//             if (GlobalSettings.Instance)
//             {
//                 if (hotkeyType != GlobalSettings.Instance.displayedController)
//                 {
//                     hotkeyType = GlobalSettings.Instance.displayedController;
//                     switch (hotkeyType)
//                     {
//                         case "KEYBOARD":
//                             m_keyboardInteractPrompt.SetActive(true);
//                             m_controllerInteractPrompt.SetActive(false);
//                             m_currentActivePrompt = m_keyboardInteractPrompt;
//                             break;
//                         case "XBOX":
//                         case "PLAYSTATION":
//                         case "OTHER":
//                             m_keyboardInteractPrompt.SetActive(false);
//                             m_controllerInteractPrompt.SetActive(true);
//                             m_currentActivePrompt = m_controllerInteractPrompt;
//                             break;
//                     }
//                 }
//             }
//         }
//     }
// }
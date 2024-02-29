using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;
using Yarn.Unity.Example;
using CharacterController = __OasisBlitz.Player.CharacterController;

public class CheckCritterInteract : MonoBehaviour
{
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private LayerMask targetPointMask;
    private Collider[] queryResult;
    public float potentialTargetRadius;
    public float critterTargetAngle;
    private Transform playerTransform;
    private Critter closestCritter = null;
    private bool ForcedInteractionInPlace = false;
    private PlayerInput playerInput;
    private PlayerStateMachine Ctx;
    private Coroutine faceCritter = null;

    private void OnEnable()
    {
        PlayerInput.CancelCritterInteraction += StopCritterInteraction;
    }

    private void OnDisable()
    {
        PlayerInput.CancelCritterInteraction -= StopCritterInteraction;
    }

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        queryResult = new Collider[10];
        playerInput = playerTransform.GetComponent<PlayerInput>();
        Ctx = playerTransform.GetComponent<PlayerStateMachine>();
    }

    private void Start()
    {
        StartCoroutine(CheckForClosestCritter());
    }

    private IEnumerator CheckForClosestCritter()
    {
        while (true)
        {
            GetClosestCritter();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void GetClosestCritter()
    {
        if (ForcedInteractionInPlace)
        {
            Debug.Log("Forced interact");
            return;
        }
        // Target enemies within FOV
        
        // Overlap sphere non alloc to find critters
        int numCrittersInRange = Physics.OverlapSphereNonAlloc(playerTransform.position, potentialTargetRadius, queryResult, targetPointMask);

        // If no critters in range, return null
        if (numCrittersInRange == 0)
        {
            if (closestCritter != null)
            {
                if (runner.IsDialogueRunning)
                {
                    runner.Stop();
                }
                closestCritter.DisableInteractIndicator();
                closestCritter = null;
            }
            return;
        }

        
        // Otherwise, return closest critter
        float closestDistance = float.MaxValue;
        Critter newClosestCritter = null;
        for (int i = 0; i < numCrittersInRange; i++)
        {
            //Check if critter object is valid
            Critter critter = queryResult[i].GetComponent<Critter>();

            if (critter == null)
            {
                continue;
            }

            Vector3 critterPos = queryResult[i].transform.position;

            float distance = Vector3.Distance(playerTransform.position, critterPos);

            // Only look for targets within the acceptable angle
            var camPos = CameraStateMachine.Instance.CameraSurface.transform.position;
            Vector3 toTarget = critterPos - camPos;
            Vector3 camForward = CameraStateMachine.Instance.CameraSurface.transform.forward;
            float angle =
                Mathf.Acos(Vector3.Dot(toTarget, camForward) / (toTarget.magnitude * camForward.magnitude)) *
                Mathf.Rad2Deg;
            if (angle < critterTargetAngle)
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    newClosestCritter = critter;
                }
            }
        }

        if (newClosestCritter != closestCritter)
        {
            if (closestCritter != null)
            {
                closestCritter.DisableInteractIndicator();
                if (runner.IsDialogueRunning)
                {
                    runner.Stop();
                }
            }
            if (newClosestCritter != null)
            {
                newClosestCritter.EnableInteractIndicator();
            }
            closestCritter = newClosestCritter;
        }else if (closestCritter != null && !runner.IsDialogueRunning)
        {
            closestCritter.EnableInteractIndicator();
        }
    }

    public void InteractWithCritter()
    {
        if (closestCritter != null && !runner.IsDialogueRunning)
        {
            StartCritterInteract(closestCritter);
        }
    }
    
    public void MandatoryInteractWithCritter(Critter critter)
    {
        if (critter == null)
        {
            return;
        }
        if (runner.IsDialogueRunning)
        {
            runner.Stop();
        }
        ForcedInteractionInPlace = true;
        closestCritter = critter;
        StartCoroutine(startMandatory(critter));
    }
    
    private IEnumerator startMandatory(Critter critter)
    {
        //call when camera fly in to level finishes
        while (Ctx.cameraStateMachine.CurrentState.StateName() != "SurfaceDefaultState")
        {
            yield return null;
        }
        InteractWithCritter(critter);
    }
    public void InteractWithCritter(Critter critter)
    {
        if (critter == null)
        {
            return;
        }
        if (runner.IsDialogueRunning)
        {
            runner.Stop();
        }
        closestCritter = critter;
        StartCritterInteract(critter);
    }

    private void StopCritterInteraction()
    {
        if (runner.IsDialogueRunning)
        {
            runner.Stop();
        }
    }

    private void StartCritterInteract(Critter critter)
    {
        //stop searching for nearby critters to interact with
        ForcedInteractionInPlace = true;
        closestCritter = critter;
        //Start dialogue
        runner.StartDialogue(critter.GetDesiredDialogueNode());
        //Make critter cam the active one
        critter.thisCritterCamera.Priority.Value = 1000;
        //Disable Player Input Mapping
        playerInput.EnterCritterInteractState();
        //Fade to black
        HUDManager.Instance.GetSceneTransitionImage().DOFade(1.0f, 0.25f)
            .SetEase(Ease.OutExpo).OnComplete(MoveBanditForInteract);
    }

    private void MoveBanditForInteract()
    {
        HUDManager.Instance.GetSceneTransitionImage().DOFade(0.0f, 0.25f)
            .SetEase(Ease.InExpo);
        //Move bandit to location
        Ctx.CharacterController.SetPosition(closestCritter.banditPosDuringInteraction);
        //rotate towards critter (set vel towards critter but lock pos)
        Ctx.ModelRotator.SetAndLockYaw(closestCritter.transform.position - playerTransform.position);
        Ctx.PlayerPhysics.SetVelocity(Vector3.zero);
        //Disable Interact Indicator
        closestCritter.DisableInteractIndicator();
    }

    public void EndCritterInteract(Critter critter)
    {
        //Kill fade to black if still running
        HUDManager.Instance.GetSceneTransitionImage().DOKill();
        HUDManager.Instance.GetSceneTransitionImage().color = Color.clear;
        //continue searching for nearby critters to interact with
        ForcedInteractionInPlace = false;
        //reduce priority of critter cam
        critter.thisCritterCamera.Priority.Value = -1;
        //Enable Player Input Mapping
        playerInput.EnableCharacterControls();
        //release rotation lock
        Ctx.ModelRotator.ReleaseLockedRotation();
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawWireSphere(playerTransform.position, potentialTargetRadius);
    // }
}

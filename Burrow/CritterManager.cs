using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using __OasisBlitz.Camera.StateMachine;
using Unity.Cinemachine;
using UnityEngine;
using Yarn.Unity;
using Random = UnityEngine.Random;

public class CritterManager : MonoBehaviour
{
    public CritterLocation mandatoryLocation;
    public GameObject idleLocationsParent;
    
    [Description("The indices of this array must match up with the order of the critter names enum")]
    public GameObject[] critterPrefabs;
    
    public CheckCritterInteract critterInteract;
    public CinemachineCamera dialogueCamera;

    private Critter mandatoryCritterInstance = null;
    
    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        SpawnCorrectCritters();
        //Trigger mandatory dialogue
        if (mandatoryCritterInstance != null)
        {
            // TODO: After start of scene works, force mandatory critter dialogue at the correct time
            // StartMandatoryDialogue();
        }
    }

    public void SpawnCorrectCritters()
    {
        // First, determine if a mandatory critter should be spawned, and which one
        bool spawnMandatoryCritter = true;
        Critter.CritterName mandatoryCritter = Critter.CritterName.Clover;
        
        List<Critter.CritterName> otherCritters = new List<Critter.CritterName>();

        if (XMLFileManager.Instance.GetNumPlantsCollected() == 1)
        {
            mandatoryCritter = Critter.CritterName.Clover;
        }
        else if (XMLFileManager.Instance.GetNumPlantsCollected() == 2)
        {
            mandatoryCritter = Critter.CritterName.Juno;
            
            otherCritters.Add(Critter.CritterName.Clover);
        }
        // else if (XMLFileManager.Instance.GetNumPlantsCollected() == 3)
        // {
        //     mandatoryCritter = Critter.CritterName.Miles;
        //     
        //     otherCritters.Add(Critter.CritterName.Clover);
        //     otherCritters.Add(Critter.CritterName.Juno);
        // }
        else
        {
            spawnMandatoryCritter = false;
            
            otherCritters.Add(Critter.CritterName.Clover);
            otherCritters.Add(Critter.CritterName.Juno);
            ///otherCritters.Add(Critter.CritterName.Miles);
        }

        // Create and populate set of idle locations
        RandomList<CritterLocation> idleLocationsList = new RandomList<CritterLocation>();
        
        CritterLocation [] idleLocations = idleLocationsParent.GetComponentsInChildren<CritterLocation>();
        foreach (var location in idleLocations)
        {
            idleLocationsList.Add(location);
        }
        
        // Spawn the mandatory critter

        if (spawnMandatoryCritter)
        {
            mandatoryCritterInstance = SpawnCritter(mandatoryCritter, Critter.CritterState.Ordered, mandatoryLocation).
                GetComponent<Critter>();
        }
        
        // Spawn the idle critters
        foreach (var critter in otherCritters)
        {
            SpawnCritter(critter, Critter.CritterState.Ordered, idleLocationsList.RemoveRandom());
        }
        
    }
    
    private Critter SpawnCritter(Critter.CritterName critterName, Critter.CritterState critterState, CritterLocation location)
    {
        // Get prefab from array based on critter name
        GameObject critterPrefab = critterPrefabs[(int)critterName];
        
        // Instantiate it
        GameObject critter = Instantiate(critterPrefab, location.transform.position, location.transform.localRotation);

        Critter newCritterInstance = critter.GetComponent<Critter>();
        
        // Set its state
        newCritterInstance.critterState = critterState;
        //Set cam and bandit pos
        newCritterInstance.thisCritterCamera = location.thisLocationCamera;
        newCritterInstance.banditPosDuringInteraction = location.banditPosDuringInteraction.position;

        return newCritterInstance;
    }

    private void StartMandatoryDialogue()
    {
        critterInteract.MandatoryInteractWithCritter(mandatoryCritterInstance);
    }
}

public class RandomList<T>
{
    private List<T> items = new List<T>();

    // Add an item to the list
    public void Add(T item)
    {
        items.Add(item);
    }

    // Remove a random item from the list and return its value
    public T RemoveRandom()
    {
        if (items.Count == 0)
        {
            throw new InvalidOperationException("Cannot remove from an empty list.");
        }

        int index = Random.Range(0, items.Count); // Get a random index
        T item = items[index]; // Retrieve the item
        items.RemoveAt(index); // Remove the item from the list
        return item; // Return the removed item
    }
}
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Collectables;
using Unity.Cinemachine;
using UnityEngine;

public class BurrowPlantManager : MonoBehaviour
{
    public Plant unplacedPlant { get; private set; }
    
    // The ID for each extra plants batch is the ID for the collectable plant that triggers them
    public ExtraPlantsBatch[] extraPlantsBatches;
    
    public void SetUnplacedPlant(Plant plant)
    {
        unplacedPlant = plant;
    }
    
    public void SpawnExtraPlantsForPlant(int plantID, bool animated)
    {
        if (animated)
        {
            extraPlantsBatches[plantID]?.PlaceAllPlantsAnimated();
        }
        else
        {
            extraPlantsBatches[plantID]?.PlaceAllPlants();
        }
    }
    

}

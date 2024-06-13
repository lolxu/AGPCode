using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraPlantsBatch : MonoBehaviour
{
    private List<ExtraPlant> extraPlants = new List<ExtraPlant>();
    
    void Awake()
    {
        // Get all children with an ExtraPlant script
        foreach (Transform child in transform)
        {
            ExtraPlant extraPlant = child.GetComponent<ExtraPlant>();
            if (extraPlant != null)
            {
                extraPlants.Add(extraPlant);
            }
        }
    }
    
    public void PlaceAllPlants()
    {
        foreach (ExtraPlant extraPlant in extraPlants)
        {
            extraPlant.PlaceInstant();
        }
    }
    
    public void PlaceAllPlantsAnimated()
    {
        foreach (ExtraPlant extraPlant in extraPlants)
        {
            extraPlant.PlaceAnimated();
        }
    }
}

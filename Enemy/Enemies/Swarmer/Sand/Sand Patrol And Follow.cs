using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandPatrolAndFollow : SwarmerPatrolAndFollow
{
    [SerializeField] private GameObject tempSandPrefab;
    [SerializeField] private Transform tempSandSpawnLocation;
    private GameObject currSand;
    public override void HandleEnterDeath()
    {
        currSand = Instantiate(tempSandPrefab, null);
        currSand.transform.position = tempSandSpawnLocation.position;
        base.HandleEnterDeath();
        //tempSand.transform.SetParent(null);
    }
    
    private void OnEnable()
    {
        KillSand();
    }

    private void KillSand()
    {
        if (currSand != null)
        {
            Destroy(currSand);
        }
    }
}

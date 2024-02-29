using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class RopeManager : MonoBehaviour
{
    public static RopeManager Instance;
    private ObiFixedUpdater updater;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        updater = GetComponent<ObiFixedUpdater>();
    }

    public void AddSolver(ObiSolver solver)
    {
        updater.solvers.Add(solver);
    }
}

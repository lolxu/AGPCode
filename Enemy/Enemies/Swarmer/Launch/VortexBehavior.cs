using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class VortexBehavior : MonoBehaviour
{
    [SerializeField] private GameObject vortexPrefab;
    private GameObject currVortex;
    [SerializeField] private Transform spawnFrom;

    public void StartVortex(float vortexLifespan)
    {
        //gameObject.SetActive(true);
        //transform.SetParent(null);
        StopVortex();
        currVortex = Instantiate(vortexPrefab, null);
        currVortex.transform.position = spawnFrom.position;
        currVortex.GetComponent<VortexCheckForPlayer>().StartVortex(vortexLifespan);
    }

    private void OnEnable()
    {
        StopVortex();
    }

    public void StopVortex()
    {
        if (currVortex != null)
        {
            Destroy(currVortex);
        }
    }
}

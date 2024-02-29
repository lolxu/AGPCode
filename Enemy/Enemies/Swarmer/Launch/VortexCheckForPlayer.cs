using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.Physics;
using UnityEngine;

public class VortexCheckForPlayer : MonoBehaviour
{
    [SerializeField] private Transform triggerBoxBoundsParent;
    [SerializeField] private List<Bounds> vortexBounds;
    [SerializeField] private Transform playerTransform;
    
    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        foreach(Transform child in triggerBoxBoundsParent)
        {
            vortexBounds.Add(child.GetComponent<BoxCollider>().bounds);
        }
        
    }

    public void StartVortex(float vortexLifespan)
    {
        StartCoroutine(CheckIfPlayerInVortex(vortexLifespan));
    }
    
    private IEnumerator CheckIfPlayerInVortex(float vortexLifespan)
    {
        yield return new WaitForSeconds(1.0f);
        while (vortexLifespan > 0.0f)
        {
            foreach (Bounds bounds in vortexBounds)
            {
                if (bounds.Contains(playerTransform.position))
                {
                    Bounce.Instance.BouncePlayerInWorldDirection(Bounce.BounceTypeNormal.Small);
                    break;
                }
            }
            yield return null;
            vortexLifespan -= Time.deltaTime;
        }
        Destroy(gameObject);
    }
}

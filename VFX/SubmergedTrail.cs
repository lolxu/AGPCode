using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmergedTrail : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        trailRenderer.emitting = false;
    }

    public void OnSubmerge()
    {
        trailRenderer.Clear();
        trailRenderer.emitting = true;
    }

    public void OnSurface()
    {
        trailRenderer.emitting = false;
    }
}

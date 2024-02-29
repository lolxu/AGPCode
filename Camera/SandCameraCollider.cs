using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandCameraCollider : MonoBehaviour
{
    
    public bool IsInsideLargePenetrable { get; private set; }
    
    public float cameraHitboxRadius = 0.1f;
    private LayerMask largePenetrableLayerMask;
    
    private Collider[] results = new Collider[10];

    void Awake()
    {
        largePenetrableLayerMask = LayerMask.GetMask("LargePenetrable");
    }

    // Update is called once per frame
    void Update()
    {
        int numOverlaps = Physics.OverlapSphereNonAlloc(transform.position, cameraHitboxRadius, results, largePenetrableLayerMask);
        IsInsideLargePenetrable = numOverlaps > 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for enabling and disabling the dive camera
/// based on the camera collider
/// </summary>
public class DiveCamera : MonoBehaviour
{
    [SerializeField] private SandCameraCollider sandCameraCollider;
    [SerializeField] private Camera diveCamera;
    
    public bool diveCameraActive = false;

    void Start()
    {
        diveCameraActive = false;
        diveCamera.enabled = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!diveCameraActive && sandCameraCollider.IsInsideLargePenetrable)
        {
            diveCameraActive = true;
            diveCamera.enabled = true;
        }
        else if (diveCameraActive && !sandCameraCollider.IsInsideLargePenetrable)
        {
            diveCameraActive = false;
            diveCamera.enabled = false;
        }
    }
}

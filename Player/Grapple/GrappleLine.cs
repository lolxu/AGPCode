using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using __OasisBlitz.Utility;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrappleLine : MonoBehaviour {
    private LineRenderer lr;
    public int quality;
    public float startVelocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve affectCurve;

    public ProjectileDrill drill;
    public Transform gauntletAttachPoint;
    public Spring1D spring;

    private bool grappling;
    
    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            DrawRope();
        }
        else
        {
            Reset();
        }
    }

    public void DrawRope() {
        //If not grappling, don't draw rope

        if (lr.positionCount == 0) {
            lr.positionCount = quality + 1;
        }
        
        var grapplePoint = drill.GetGrapplePoint();
        var attachPoint = gauntletAttachPoint.position;
        var up = Quaternion.LookRotation((grapplePoint - attachPoint).normalized) * Vector3.up;

        for (var i = 0; i < quality + 1; i++) {
            var delta = i / (float) quality;
            var offset = up * (waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.position * affectCurve.Evaluate(delta));
            
            lr.SetPosition(i, Vector3.Lerp(attachPoint, grapplePoint, delta) + offset);
        }
    }

    void Reset()
    {
        lr.positionCount = 0;
    }

    public void StartGrapple()
    {
        grappling = true;
        
        // Twang the spring
        spring.velocity = startVelocity;
        spring.position = 0;
    }

    public void EndGrapple()
    {
        Reset();
        grappling = false;
    }
}

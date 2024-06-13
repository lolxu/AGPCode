using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using __OasisBlitz.Utility;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(LineRenderer))]
public class GrappleLine : MonoBehaviour {
    public LineRenderer lineRenderer;
    public int quality;
    public float startVelocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve affectCurve;

    public Transform targetAttachPoint;
    public Transform banditAttachPoint;
    public Spring1D spring;

    private bool grappling;
    
    void Start() {
        Reset();
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

        if (lineRenderer.positionCount == 0) {
            lineRenderer.positionCount = quality + 1;
        }

        try
        {
            var targetPoint = targetAttachPoint.position;
            var gauntletPoint = banditAttachPoint.position;


            var up = Quaternion.LookRotation((targetPoint - gauntletPoint).normalized) * Vector3.up;

            for (var i = 0; i < quality + 1; i++)
            {
                var delta = i / (float)quality;
                var offset = up * (waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.position *
                                   affectCurve.Evaluate(delta));

                lineRenderer.SetPosition(i, Vector3.Lerp(gauntletPoint, targetPoint, delta) + offset);
            }
        }
        catch (Exception e)
        {
            return;
        }
    }

    void Reset()
    {
        lineRenderer.positionCount = 0;
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

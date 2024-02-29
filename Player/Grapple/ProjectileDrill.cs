using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class ProjectileDrill : MonoBehaviour
{
    public Transform endPoint;

    public float launchDuration;

    private TweenerCore<Vector3, Vector3, VectorOptions> shotTween;
    
    [SerializeField] private GrappleLine leftGrappleLine;
    [SerializeField] private GrappleLine rightGrappleLine;

    private bool active = false;
    
    public void ShootDrill(Vector3 startPosition, Vector3 targetPoint)
    {
        active = true;
        gameObject.SetActive(true);
        
        transform.position = startPosition;
        
        Vector3 upDirection = (targetPoint - transform.position).normalized;
        transform.up = upDirection;

        var tween = transform.DOMove(targetPoint, launchDuration);
        
        leftGrappleLine.StartGrapple();
        rightGrappleLine.StartGrapple();
    }

    public void HideDrill()
    {
        active = false;
        leftGrappleLine.EndGrapple();
        rightGrappleLine.EndGrapple();
        
        gameObject.SetActive(false);
    }
    
    public Vector3 GetGrapplePoint()
    {
        return endPoint.position;
    }
    
    
    
}


using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleAttachMover : MonoBehaviour
{
    public float launchDuration;
    public GrappleLine grappleLine;
    
    
    public void ShootToAttachPoint(Transform gauntletPoint, Vector3 destination)
    {
        grappleLine.banditAttachPoint = gauntletPoint;

        float tweenValue = 0;

        DOTween.To(() => tweenValue, x => tweenValue = x, 1, launchDuration)
            .OnUpdate(() =>
        {
            transform.position = Vector3.Lerp(gauntletPoint.position, destination, tweenValue);
        });
        
        grappleLine.StartGrapple();
    }
    
    public void EndGrapple()
    {
        DOVirtual.DelayedCall(0.2f, CleanUp);
    }

    private void CleanUp()
    {
        grappleLine.EndGrapple();
        Destroy(gameObject);
    }
    
    
    
}

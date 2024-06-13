using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;

public class GrappleAnimation : MonoBehaviour
{
    [SerializeField] private string LeftWristBoneName = "left_palm_ik_handle";
    [SerializeField] private string RightWristBoneName = "right_palm_ik_handle";
        
    [SerializeField] private GameObject grappleAttachMoverPrefab;
    private GrappleAttachMover leftGrappleAttachMover;
    private GrappleAttachMover rightGrappleAttachMover;
    
    private Transform leftGauntletAttachPoint;
    private Transform rightGauntletAttachPoint;

    private Tween currentDestroyTween;
    
    // Start is called before the first frame update
    void Start()
    {
        // Create the two grapple points for the player
        leftGauntletAttachPoint = new GameObject("Left Gauntlet Attach Point").transform;
        rightGauntletAttachPoint = new GameObject("Right Gauntlet Attach Point").transform;

        Transform leftGauntletBone = transform.MMFindDeepChildDepthFirst(LeftWristBoneName);
        Transform rightGauntletBone = transform.MMFindDeepChildDepthFirst(RightWristBoneName);
        leftGauntletAttachPoint.transform.SetParent(leftGauntletBone.transform, false);
        rightGauntletAttachPoint.transform.SetParent(rightGauntletBone.transform, false);
        
        leftGauntletAttachPoint.localPosition = Vector3.zero;
        rightGauntletAttachPoint.localPosition = Vector3.zero;
        
    }
    
    public void PlayGrappleAnimation(Vector3 targetCenter, float width)
    {
        Vector3 targetToPlayer = transform.position - targetCenter;
        targetToPlayer.y = 0;
        targetToPlayer.Normalize();
        
        Vector3 rightOfTarget = Vector3.Cross(targetToPlayer, Vector3.up);
        
        Vector3 leftGrappleDestination = targetCenter - rightOfTarget * (width / 2);
        Vector3 rightGrappleDestination = targetCenter + rightOfTarget * (width / 2);
        
        // // Play dash visuals
        leftGrappleAttachMover = Instantiate(grappleAttachMoverPrefab, transform.position, Quaternion.identity).GetComponent<GrappleAttachMover>();
        rightGrappleAttachMover = Instantiate(grappleAttachMoverPrefab, transform.position, Quaternion.identity).GetComponent<GrappleAttachMover>();
        
        leftGrappleAttachMover.ShootToAttachPoint(leftGauntletAttachPoint, leftGrappleDestination);
        rightGrappleAttachMover.ShootToAttachPoint(rightGauntletAttachPoint, rightGrappleDestination);
    }

    public void End()
    {
        leftGrappleAttachMover.EndGrapple();
        rightGrappleAttachMover.EndGrapple();
    }
    
}

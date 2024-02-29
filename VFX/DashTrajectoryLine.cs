using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DashTrajectoryLine : MonoBehaviour
{
    private Vector3 playerPos;
    private Vector3 targetPos;

    private Vector3[] positions = new Vector3[2];

    [SerializeField] private LineRenderer lineRenderer;

    void Update()
    {
        UpdateLinePosition();
    }
    
    private void UpdateLinePosition()
    {
        positions[0] = playerPos;
        positions[1] = targetPos;
        lineRenderer.SetPositions(positions);
    }
    
    public void SetPositions(Vector3 playerPos, Vector3 targetPos)
    {
        this.playerPos = playerPos;
        this.targetPos = targetPos;
    }

    public void OnDashTargetChanged()
    {
        // Create a DOTween tween that lerps the playerPos to the targetPos and then destroys the gameobject
        // Create a tween for moving playerPos to targetPos
        float duration = 0.2f; // Duration of the lerp, adjust as needed
        DOTween.To(() => playerPos, x => playerPos = x, targetPos, duration)
            .OnUpdate(() => UpdateLinePosition()) // Update line position during tween
            .OnComplete(() => Destroy(gameObject)); // Destroy the game object on completion
    }

    public void OnDash()
    {
        // Create a DOTween tween that lerps the playerPos to the targetPos and then destroys the gameobject
        float duration = 0.2f; // Duration of the lerp, adjust as needed
        DOTween.To(() => playerPos, x => playerPos = x, targetPos, duration)
            .OnUpdate(() => UpdateLinePosition()) // Update line position during tween
            .OnComplete(() => Destroy(gameObject)); // Destroy the game object on completion
    }
}

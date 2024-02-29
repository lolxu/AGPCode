using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Camera.StateMachine;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class FreelookAdjuster : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    private Vector3 currentLookPoint;
    [FormerlySerializedAs("lookPointLerpSpeed")] public float keyPointLerpSpeed = 0.1f;
    public float velocityLerpSpeed = 1.0f;
    public float minVelocityToLookAtTarget = 0.1f;

    public void UpdateLookdirection()
    {
        LerpTowardsTargetRotation();
    }

    private void LerpTowardsTargetRotation()
    {
        float lerpSpeed;

        if (CameraStateMachine.Instance.assistMode == CameraStateMachine.AssistMode.KeyPoint)
        {
            lerpSpeed = keyPointLerpSpeed;
        }
        else
        {
            lerpSpeed = velocityLerpSpeed;
        }
        
        Vector2 targetOrientation = GetCameraTargetOrientation(currentLookPoint);
        orbitalFollow.HorizontalAxis.Value = Mathf.LerpAngle(orbitalFollow.HorizontalAxis.Value, targetOrientation.x, lerpSpeed * Time.deltaTime);
        orbitalFollow.VerticalAxis.Value = Mathf.Lerp(orbitalFollow.VerticalAxis.Value, targetOrientation.y, lerpSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Set the point in world space that the camera should automatically look towards
    /// </summary>
    /// <param name="worldPoint"></param>
    public void SetCurrentLookPoint(Vector3 worldPoint)
    {
        currentLookPoint = worldPoint;
    }

    /// <summary>
    /// Calculate the target orientation of the camera based on the world point to look at
    /// </summary>
    /// <param name="worldPoint">The point in world space to look towards (while still focused on the player)</param>
    /// <returns> A Vector2 where X is the horizontal axis of the camera and Y is the vertical axis of the camera </returns>
    private Vector2 GetCameraTargetOrientation(Vector3 worldPoint)
    {
        Vector3 relativeDirection = worldPoint - orbitalFollow.FollowTargetPosition;
        relativeDirection.Normalize();
        
        // Convert to azimuth angle
        float azimuth = Mathf.Atan2(relativeDirection.x, relativeDirection.z) * Mathf.Rad2Deg;

        // Determine Y axis value (blend between rings)
        float elevation = Mathf.Asin(relativeDirection.y) * Mathf.Rad2Deg;
        float yAxisValue = CalculateYAxisValue(elevation);
        
        // Debug.Log("Elevation angle: " + elevation);
        // Debug.Log("Y axis value: " + yAxisValue);

        return new Vector2(azimuth, yAxisValue);


    }
    
    /// <summary>
    /// Instantly set the camera to look towards the given world point
    /// </summary>
    /// <param name="worldPoint"></param>
    public void SetCameraDirectionInstant(Vector3 worldPoint)
    {
        Vector2 cameraParams = GetCameraTargetOrientation(worldPoint);

        // Apply the calculated values
        orbitalFollow.HorizontalAxis.Value = cameraParams.x;
        orbitalFollow.VerticalAxis.Value = cameraParams.y;
    }
    
    float CalculateYAxisValue(float elevationAngle)
    {
        // Implement your logic here to map the elevation angle to the Y axis value
        // This is an example, you might need to adjust it based on your camera setup
        float normalizedElevation = (elevationAngle + 90f) / 180f;
        
        // Normalized elevation is currently inverted from what we want, so we subtract it from 1
        return 1 - Mathf.Clamp01(normalizedElevation);
    }
    
}

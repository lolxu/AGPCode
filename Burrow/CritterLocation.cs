using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CritterLocation : MonoBehaviour
{
    public CinemachineCamera thisLocationCamera;
    public Transform banditPosDuringInteraction;
    void OnDrawGizmos()
    {
        // Draw a gizmo sphere
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}

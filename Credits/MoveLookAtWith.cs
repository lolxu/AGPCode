using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class MoveLookAtWith : MonoBehaviour
{
    [SerializeField] private Transform lookAtPoint;
    [SerializeField] private Vector3 offset;
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private CinemachineSplineDolly dolly;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private bool waitTillFullyTransitioned = false;

    private void Start()
    {
        StartCoroutine(startMoveLookAt());
    }

    private IEnumerator startMoveLookAt()
    {
        while (!cam.IsLive)
        {
            yield return null;
        }
        while (waitTillFullyTransitioned && brain.IsBlending)
        {
            yield return null;
        }
        while (cam.IsLive)
        {
            lookAtPoint.position = transform.position + offset;
        }
    }
}

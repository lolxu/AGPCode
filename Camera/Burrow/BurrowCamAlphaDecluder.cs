using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class BurrowCamAlphaDecluder : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float WallRaycastMultiplier;
    [SerializeField] private float BackWallRaycastMultiplier;
    private HashSet<TransparentWall> activeTransparentWalls = new HashSet<TransparentWall>();
    private Transform camTransform;
    

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        GetComponent<CinemachineCamera>().Target.TrackingTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        camTransform = GameObject.FindWithTag("PlayerCamera").transform;
    }

    private void Update()
    {
        HashSet<TransparentWall> NewTransparentWalls = new HashSet<TransparentWall>();
        for (int j = -1; j <= 1; j++)
        {
            Vector3 from = (camTransform.position + camTransform.right * j * WallRaycastMultiplier -
                            camTransform.forward * BackWallRaycastMultiplier);
            Vector3 dir = Player.position - from;
            RaycastHit[] result = Physics.RaycastAll(from, dir, dir.magnitude, mask);
            for (int i = 0; i < result.Length; i++)
            {
                TransparentWall wall = result[i].transform.GetComponent<TransparentWall>();
                if (wall)
                {
                    if (!NewTransparentWalls.Contains(wall))
                    {
                        if (!wall.IsFading)
                        {
                            wall.Fade();
                        }
                        NewTransparentWalls.Add(wall);
                        if (activeTransparentWalls.Contains(wall))
                        {
                            activeTransparentWalls.Remove(wall);
                        }
                    }
                }
            }
        }
        //reset alpha on walls that are no longer blocking LOS on player
        foreach (var wall in activeTransparentWalls)
        {
            wall.ResetFade();
        }
        activeTransparentWalls.Clear();
        foreach (var wall in NewTransparentWalls)
        {
            activeTransparentWalls.Add(wall);
        }
    }

/*    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Player.position, transform.position - transform.forward * BackWallRaycastMultiplier);
    }*/
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class chomp : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float waitTillChomp;
    [SerializeField] private CinemachineCamera cam;
    private void Start()
    {
        StartCoroutine(changeTime());
        
    }

    private IEnumerator changeTime()
    {
        while (!cam.IsLive)
        {
            yield return null;
        }
        yield return new WaitForSecondsRealtime(waitTillChomp);
        anim.SetBool("aggro", true);
    }
}

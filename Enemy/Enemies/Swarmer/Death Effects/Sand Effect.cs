using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandEffect : MonoBehaviour
{
    //Spawn in a temp sand at death location
    [SerializeField] private GameObject tempSand;
    private void OnDestroy()
    {
        tempSand.SetActive(true);
        tempSand.transform.SetParent(null);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedGame : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
        }
    }
}

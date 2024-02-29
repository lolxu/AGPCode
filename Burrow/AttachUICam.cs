using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachUICam : MonoBehaviour
{
    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        Debug.Log("World Cam is: " + canvas.worldCamera.name);
    }
}

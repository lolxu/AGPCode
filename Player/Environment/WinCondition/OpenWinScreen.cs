using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWinScreen : MonoBehaviour
{
    [SerializeField] private string objectiveText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OpenWinScreen: Collide with this trigger");
        if (other.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.Win(objectiveText);
        }
    }
}

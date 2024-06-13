using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using Febucci.UI.Core;
using UnityEngine;

public class DisplayTextWhenNear : MonoBehaviour
{
    public Transform player;

    public float triggerRadius;
    public float hideRadius;
    
    public string textToDisplay;
    
    public TextAnimator_TMP textAnimator;
    
    private bool isDisplaying = false;

    public TypewriterByCharacter typeWriter;
    
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }    
        
        typeWriter.ShowText("");
    }

    // Update is called once per frame
    void Update()
    {
        // if (isDisplaying && Vector3.Distance(player.position, transform.position) > hideRadius)
        // {
        //     isDisplaying = false;
        //     Hide();
        // }
        if (!isDisplaying && Vector3.Distance(player.position, transform.position) < triggerRadius)
        {
            isDisplaying = true;
            Show();
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            ClearText();
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            Show();
        }
    }

    private void Show()
    {
        typeWriter.ShowText(textToDisplay);
    }

    private void Hide()
    {
        
    }
    
    // DEBUG TOOLS FOR TRAILER FOOTAGE
    // TODO: AIDAN REMOVE THIS
    private void ClearText()
    {
        typeWriter.ShowText("");
    }
    
    
    
}

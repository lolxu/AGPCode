using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnHover : MonoBehaviour
{
    // Left and Right buttons in Level Select in Burrow
    [SerializeField] private Button CloseButton, LeftButton, RightButton;
    public void OnHover(BaseEventData eventData)
    {
        if (this.GetComponent<Button>()) { this.GetComponent<Button>().Select(); }
        if (this.GetComponent<Scrollbar>()) { this.GetComponent<Scrollbar>().Select(); }
        if (this.GetComponent<TMP_InputField>()) { this.GetComponent<TMP_InputField>().Select(); }
    }

    public void SetSelectedLevel()      // Only in burrow level button prefab for cannon aiming purposes
    {
        //Debug.Log("Selected Level: " + this.gameObject.name);
        Levels.Instance.selectedLevel = this.gameObject.name;

        // Close Button navigation
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = this.GetComponent<Button>();

        CloseButton.navigation = nav;

        if(LeftButton != null)
        {
            Navigation lNav = new Navigation();
            lNav.mode = Navigation.Mode.Explicit;
            lNav.selectOnDown = CloseButton;
            lNav.selectOnRight = this.GetComponent<Button>();

            LeftButton.navigation = lNav;
        }
        if (RightButton != null)
        {
            Navigation rNav = new Navigation();
            rNav.mode = Navigation.Mode.Explicit;
            rNav.selectOnDown = CloseButton;
            rNav.selectOnLeft = this.GetComponent<Button>();

            RightButton.navigation = rNav;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

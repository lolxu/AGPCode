using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnHover : MonoBehaviour
{

    public void OnHover(BaseEventData eventData)
    {
        if (this.GetComponent<Button>()) { this.GetComponent<Button>().Select(); }
        if (this.GetComponent<Scrollbar>()) { this.GetComponent<Scrollbar>().Select(); }
    }
    public void SetSelectedLevel()      // Only in burrow level button prefab for cannon aiming purposes
    {
        Debug.Log("Selected Level: " + this.gameObject.name);
        Levels.Instance.selectedLevel = this.gameObject.name;
        
        // TODO Rotate the cannon here
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

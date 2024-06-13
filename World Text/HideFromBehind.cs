using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideFromBehind : MonoBehaviour
{
    [SerializeField] private GameObject [] worldSpaceObjects;
    [SerializeField] private GameObject compareObject;
    private Camera cam;
    public bool showStat = false;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Camera Surface").GetComponent<Camera>();
    }

    private void Show(bool status)
    {
        foreach (GameObject obj in worldSpaceObjects)
        {
            obj.SetActive(status);
        }
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 camDir = cam.transform.position - compareObject.transform.position;
        float camDot = -Vector3.Dot(Vector3.Normalize(camDir), compareObject.transform.forward);
        //if(showStat) { Debug.Log($"DOT: {camDot}"); }
        if(camDot <= 0)    // Camera directly to side or behind       -- happened to be 17?
        {
            Show(false);
        }
        else
        {
            Show(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePosition : MonoBehaviour
{
    [SerializeField] private float bottomYPos;
    [SerializeField] private float topYPos;
    public enum Position
    {
        Sand,
        PipeSerp
    }

    public enum Background
    {
        Serp,
        Pill,
        None
    }
    
    [SerializeField] private Position currPos = Position.Sand;
    [SerializeField] private Background currBack = Background.None;
    [SerializeField] private List<Transform> backgrounds;

    public void SwitchBackground(Background ground)
    {
        if (currBack == ground)
        {
            return;
        }
        if (currBack != Background.None)
        {
            backgrounds[(int)currBack].position = new Vector3(backgrounds[(int)currBack].position.x, bottomYPos,
                backgrounds[(int)currBack].position.z);
        }
        if (ground != Background.None)
        {
            backgrounds[(int)currBack].position = new Vector3(backgrounds[(int)currBack].position.x, topYPos,
                backgrounds[(int)currBack].position.z);
        }
    }


}

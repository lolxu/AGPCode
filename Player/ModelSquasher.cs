using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Utility;
using UnityEngine;

public class ModelSquasher : MonoBehaviour
{
    [SerializeField] private Spring1D spring;

    // Update is called once per frame
    void Update()
    {
        float stretchVertical = spring.position;
        float squashHorizontal = 1f / stretchVertical;
        
        transform.localScale = new Vector3(squashHorizontal, stretchVertical, squashHorizontal);
    }

    public void BumpStretch()
    {
        spring.velocity = 10f;
    }
    
    
}

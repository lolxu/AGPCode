using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Utility;
using UnityEngine;

public class InvertThisMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshFilter>().mesh = MeshInverter.InvertMesh(GetComponent<MeshFilter>().mesh);
    }

}

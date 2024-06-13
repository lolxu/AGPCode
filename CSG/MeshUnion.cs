using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __OasisBlitz.Utility;
using UnityEngine;
using Parabox.CSG;

public class MeshUnion : MonoBehaviour
{
    public GameObject left;
    public GameObject right;
    public GameObject parent;
    
    // Start is called before the first frame update
    void Start()
    {
        // TestUnion();
    }

    public void TestUnion()
    {
        GameObject[] unionObjects = new GameObject[2];
        unionObjects[0] = left;
        unionObjects[1] = right;
        ConstructUnion(unionObjects, parent.transform);
    }

    public void FindAndConstructAllMeshUnions()
    {
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        
        GameObject[] meshUnions = allGameObjects.Where(
            go => go.CompareTag("MeshUnion")).ToArray();
        
        foreach (GameObject meshUnion in meshUnions)
        {
            // Get children of this object
            MeshRenderer[] children = meshUnion.GetComponentsInChildren<MeshRenderer>();
            
            // Get the gameobjects from the children
            GameObject[] childGameObjects = children.Select(child => child.gameObject).ToArray();
            
            // Construct the union
            ConstructUnion(childGameObjects, meshUnion.transform);
        }
        
    }

    public void ConstructUnion(GameObject[] toUnion, Transform parentTransform)
    {
        Model result;

        if (toUnion.Length < 2)
        {
            Debug.Log("Mesh union with only 1 object is invalid; returning");
            return;
        }
        
        GameObject left = toUnion[0];
        GameObject right = toUnion[1];

        result = CSG.Union(left, right);

        // Disable the rendering for the original meshes, only the newly constructed mesh should be visible
        left.GetComponent<MeshRenderer>().enabled = false;
        right.GetComponent<MeshRenderer>().enabled = false;

        GameObject composite = new GameObject();
        composite.transform.SetParent(parentTransform);
        
        composite.AddComponent<MeshFilter>().sharedMesh = result.mesh;
        composite.AddComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
        composite.layer = Constants.LargePenetrableLayer;

    }
}

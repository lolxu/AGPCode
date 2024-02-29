using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DebugCheckpointList : MonoBehaviour
{
    public List<CheckPoint> checkpointList;
    public GameObject teleportButtonPrefab;
    public GameObject checkpointListGameObject;

    // Start is called before the first frame update
    void Start()
    {
        // if (checkpointList.Count > 0) return; // use defined list, not searching
        // checkpointList = FindObjectsOfType<PermanentCheckpoint>().ToList();
        // checkpointList.Sort((a, b) 
        //     => String.Compare(a.name, b.name, StringComparison.Ordinal));
    }
    

    // Update is called once per frame
    public void OnOpen()
    {
        checkpointList = FindObjectsOfType<CheckPoint>().ToList();
        checkpointList.Sort((a, b) 
            => CompareHierarchy(a.gameObject, b.gameObject));
        Debug.Log($"DebugCheckpointList: {checkpointList.Count} checkpoints found.");
        
        // Destroy all buttons from last scene
        foreach (Transform child in checkpointListGameObject.transform)
        {
            Destroy(child.gameObject);
        }

        // Add button for checkpoints in new scene
        foreach (var checkpoint in checkpointList)
        {
            GameObject teleportButton = Instantiate(teleportButtonPrefab, checkpointListGameObject.transform);
            TMP_Text buttonName = teleportButton.GetComponentInChildren<TMP_Text>();
            buttonName.text = checkpoint.gameObject.name;
            teleportButton.GetComponent<DebugTeleportButton>().checkpoint = checkpoint;
        }
    }
    
    // Function to get the hierarchy path as a list of integers for sorting
    private List<int> GetHierarchyPath(GameObject go)
    {
        List<int> path = new List<int>();
        Transform current = go.transform;
        
        // Traverse up the hierarchy and record each sibling index
        while (current != null)
        {
            path.Add(current.GetSiblingIndex());
            current = current.parent;
        }

        // Reverse the list to have the root at the beginning
        path.Reverse();
        return path;
    }

    // Comparison function for sorting based on hierarchy path
    private int CompareHierarchy(GameObject x, GameObject y)
    {
        var xPath = GetHierarchyPath(x);
        var yPath = GetHierarchyPath(y);

        // Compare each element in the hierarchy path lists
        for (int i = 0; i < Mathf.Min(xPath.Count, yPath.Count); i++)
        {
            int comparison = xPath[i].CompareTo(yPath[i]);
            if (comparison != 0)
            {
                return comparison;
            }
        }

        // If one path is a subset of the other, the shorter one comes first
        return xPath.Count.CompareTo(yPath.Count);
    }
}

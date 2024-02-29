using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Levels : MonoBehaviour
{
    public static Levels Instance;
    [SerializeField] private string[] levels;
    public string selectedLevel;

    public string[]  GetLevels()
    {
        return levels;
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

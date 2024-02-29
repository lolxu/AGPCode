using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CreditsMainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI entryPrefab;

    [SerializeField] private List<string> entriesString = new List<string>();

    private List<TextMeshProUGUI> entries = new List<TextMeshProUGUI>();

    private void InitializeEntries()
    {
        foreach (string _entry in entriesString)
        {
            TextMeshProUGUI entry = Instantiate(entryPrefab, this.transform);
            entry.text = _entry;
            
            entries.Add(entry);
        }
    }


    // // Start is called before the first frame update
    // void Start()
    // {
    //     InitializeEntries();
    // }

    void OnEnable()
    {
        if (entries.Count == 0)
        {
            InitializeEntries();
        }

    }
    private void OnDisable()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

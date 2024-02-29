using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;

public class VitalizerCountHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI vitalizerCount;

    public void UpdateCount(int amount)
    {
        vitalizerCount.text = amount.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        vitalizerCount.text = GameMetadataTracker.Instance.GetVitalizerCount().ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseKeyboardSettings : MonoBehaviour
{
    [SerializeField] private ControlsSettingsUI csUI;

    [SerializeField] private Button msMinus, msPlus;        // Add/subtract mouse sensitivity buttons

    [SerializeField] private GameObject circleDisplay;      // 0, 0 is middle   -- [-135, 135]
    [SerializeField] private Slider fill;
    [SerializeField] private GameObject invertedX, invertedY;

    public void InvertX()
    {
        if (MovementSettings.Instance.InvertX()) { invertedX.SetActive(true); AudioManager.instance.ui_checkBoxState = true; }
        else { invertedX.SetActive(false); AudioManager.instance.ui_checkBoxState = false; }
    }
    public void InvertY()
    {
        if (MovementSettings.Instance.InvertY()) { invertedY.SetActive(true); AudioManager.instance.ui_checkBoxState = true; }
        else { invertedY.SetActive(false); AudioManager.instance.ui_checkBoxState = false; }
    }
    public void SetInvertDisplays()
    {
        if (MovementSettings.Instance.invertedX == 1) { invertedX.SetActive(false); }
        else { invertedX.SetActive(true); }
        if (MovementSettings.Instance.invertedY == 1) { invertedY.SetActive(false); }
        else { invertedY.SetActive(true); }
    }
    public void IncMouseSensitivity()
    {
        MovementSettings.Instance.AddMouseLookMultiplier();
    }
    public void DecMouseSensitivity()
    {
        MovementSettings.Instance.SubtractMouseLookMultiplier();
    }

    private void OnEnable()
    {

    }
}

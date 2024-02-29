using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider MouseSlider;
    [SerializeField] private Slider ControllerSlider;
    [SerializeField] private Button ControlsFirstButton;        // To default to when swapping to controller with nothing selected

    private float[] setValues = { -0.5f, -0.4f, -0.3f, -0.2f, -0.1f, 0, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };

    public void DecMouseSlider()
    {
        if((int)MouseSlider.value < 1) { return; }
        MouseSlider.value -= 1f;
    }
    public void IncMouseSlider()
    {
        if ((int)MouseSlider.value > 9) { return; }
        MouseSlider.value += 1f;
    }

    // Referenced in the Slider
    public void OnMouseSliderValueChanged()
    {
        MovementSettings.Instance.SetMouseLookMultiplier(GetMouseSliderValueConverted());
    }
    public float GetMouseSliderValueConverted()
    {
        return setValues[(int)MouseSlider.value];
    }

    public void DecControllerSlider()
    {
        if ((int)ControllerSlider.value < 1) { return; }
        ControllerSlider.value -= 1f;
    }
    public void IncControllerSlider()
    {
        if ((int)ControllerSlider.value > 9) { return; }
        ControllerSlider.value += 1f;
    }

    // Referenced in the Slider
    public void OnControllerSliderValueChanged()
    {
        MovementSettings.Instance.SetControllerLookMultiplier(GetControllerSliderValueConverted());
    }
    public float GetControllerSliderValueConverted()
    {
        return setValues[(int)ControllerSlider.value];
    }
    private void InitSliderValues()
    {
        MouseSlider.value = System.Array.IndexOf(setValues, MovementSettings.Instance.mouseLookMultiplier);
        ControllerSlider.value = System.Array.IndexOf(setValues, MovementSettings.Instance.controllerLookMultiplier);
    }
    private void OnEnable()
    {
        InitSliderValues();
    }
    private void OnDisable()
    {
        GlobalSettings.Instance.SavePlayerPrefs();
    }
    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == null && GlobalSettings.Instance.controlScheme != "KEYBOARD")
        {
            ControlsFirstButton.Select();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using UnityEngine;

public class SwitchToSlideshowInput : MonoBehaviour
{
    private PlayerInput input;
    void Start()
    {
        input = FindObjectOfType<PlayerInput>();
        input.SwitchCurrentInputState(PlayerInput.PlayerInputState.SlideShowControls);
    }

    void OnDisable()
    {
        input.SwitchCurrentInputState(PlayerInput.PlayerInputState.UI);
    }
}

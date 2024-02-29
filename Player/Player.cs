using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

namespace __OasisBlitz.Player
{
    /// <summary>
    /// This is the top level class for the player. It is in charge of calling the update loops
    /// for key components like the state machine and the player physics.
    /// </summary>
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerStateMachine playerStateMachine;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private CameraStateMachine cameraStateMachine;

        // Update is called once per frame
        void Update()
        {
            playerInput.UpdateInputs();
            playerStateMachine.UpdateStates(playerInput.CurrentInputs);
            cameraStateMachine.UpdateStates();
        }

    }
}
using UnityEngine;

namespace __OasisBlitz.Player.Physics
{
    public class NewtonsThird : MonoBehaviour
    {
        private PlayerPhysics playerPhysics;

        private void Awake()
        {
            playerPhysics = GetComponent<PlayerPhysics>();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            playerPhysics.HandleContact(hit.normal);
        }
    }
}
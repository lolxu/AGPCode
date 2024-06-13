using System.Collections;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.__Scripts.Player.Environment.Cannon
{
    public class CannonLevelTransit : Cannon
    {
        public LevelCannonObjects Cannon;
        [SerializeField] private GameObject levelInterface;

        private bool bLockPlayerToCannon = false;
        private Sequence LaunchSequence = null;
        
        
        protected override void KillCannonSequence()
        {
            bLockPlayerToCannon = false;
            // reset cannon properties
            CannonSequence?.Kill();
            LaunchSequence?.Kill();
        }

        /*
         * Scene is selected, go to scene as normal
         */
        public void SceneSelected()
        {
            bCanEnterCannon = false;
            
            LaunchSequence = DOTween.Sequence();
            
            // rest of this sequence happens when level is selected (register to callbacks on level select
            LaunchSequence.AppendCallback(OnLaunchCannonAction);
            
            // launch animation
            CannonLaunchAnimation();

            // actual launch code
            LaunchSequence.AppendCallback(ExitCannon);

            _dash.DashEnabled = true;
        }
        
        public void KickPlayerOutOfCannon()
        {
            // have player pos match 
            bLockPlayerToCannon = false;

            // actual launch code
            PlayerPhysics phys = ctx.PlayerPhysics;
            
            // turn cam all the way around:
            float cameraTransitionTime = .25f;
            Vector3 CameraForward = -transform.up;

            CameraStateMachine.Instance.SetToLookAtTarget(transform.position + CameraForward * 100.0f);
            
            // release and launch player
            ctx.CharacterController.RequestJump();
            phys.ZeroAcceleration();
            phys.SetVelocity(transform.up * -launchVelocity);
            BounceAbility.Instance.RefreshBounce();
            // re-enable the cannon to use it again after a little
            DOVirtual.DelayedCall(.5f, () => OnPostExitCannon(), false);
        }

        protected override void CreateCannonSequence()
        {
            CannonSequence = DOTween.Sequence();

            CannonSequence.AppendCallback(InitializePlayerAndCannon);
            CannonSequence.AppendCallback(() =>
            {
                levelInterface.GetComponent<BurrowLevelSelect>().OpenLevelSelectInterface();
                bLockPlayerToCannon = true;
                StartCoroutine(LockPlayerToCannonDuringSelect());
            });
            CannonSequence.AppendCallback(SetCameraToCannon);
        }
        
        private IEnumerator LockPlayerToCannonDuringSelect()
        {
            while (bLockPlayerToCannon)
            {
                // lerp character position
                Vector3 targetPosition = Vector3.Lerp(ctx.CharacterController.transform.position,
                    exitPoint.transform.position, Time.deltaTime * 6.0f);
                ctx.CharacterController.SetPosition(targetPosition);
                yield return null;
            }
        }

        protected override void OnLaunchCannonAction()
        {
            ctx.ToggleDrill = true;
            

            CameraStateMachine.Instance.freeLookCam.gameObject.SetActive(false);

            LevelManager.Instance.LoadAnySceneAsync(Cannon.loadSceneName, false);
            
        }
    }
}